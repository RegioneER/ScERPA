using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScERPA.Data;
using ScERPA.Models;
using Microsoft.AspNetCore.HttpOverrides;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using Serilog;
using Serilog.Context;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.DataProtection;
using Polly;
using Microsoft.Extensions.Http.Resilience;
using System.Net;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;



//prima della creazione del builder uso bootstraplogger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Sto avviando l'applicazione");

    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddEnvironmentVariables();

    var connectionString = builder.Configuration.GetConnectionString("ScERPAConnection") ?? throw new InvalidOperationException("Connection string 'ScERPAConnection' not found.");
    String corsOrigins = builder.Configuration.GetValue<String>("corsOrigin") ?? "";
    string adminUsername =  builder.Configuration.GetSection("SuperAdminDevelop")?.GetValue<String>("Username") ?? "";
    string adminCognome = builder.Configuration.GetSection("SuperAdminDevelop")?.GetValue<String>("Cognome") ?? "";
    string adminNome = builder.Configuration.GetSection("SuperAdminDevelop")?.GetValue<String>("Nome") ?? "";
    string adminEmail = builder.Configuration.GetSection("SuperAdminDevelop")?.GetValue<String>("Email") ?? "";
    string adminPassword = builder.Configuration.GetSection("SuperAdminDevelop")?.GetValue<String>("Password") ?? "";


    builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromGlobalLogContext());


    GlobalLogContext.PushProperty("Application", "ScERPA");

    builder.Services.AddDbContext<ScERPAContext>(options => options.UseSqlServer(connectionString));

    builder.Services.AddDefaultIdentity<ApplicationUser>(options => { options.SignIn.RequireConfirmedAccount = false; options.User.RequireUniqueEmail = false; options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._\\@";})
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ScERPAContext>();

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { "it-IT" };
        options.SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
    });


    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.Name = "ScERPA";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/Home/Index";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Home/Error";
        options.SlidingExpiration = true;
        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    });

    builder.Services.Configure<SecurityStampValidatorOptions>(o =>
                   o.ValidationInterval = TimeSpan.FromMinutes(1));

    builder.Services.AddScoped<IAnprServices, AnprServices>();
    builder.Services.AddScoped<IUtilities, Utilities>();
    builder.Services.AddScoped<IAnprApi, AnprApiWSO2>();
    builder.Services.AddScoped<IAuthenticationClient, AuthenticationClientWSO2>();
    builder.Services.AddScoped<IConsumerServices, ConsumerServices>();
    builder.Services.AddScoped<IAdministrationAreasServices, AdministrationAreasServices>();
    builder.Services.AddScoped<IAdministationEServicesServices, AdministrationEServicesServices>();
    builder.Services.AddScoped<IAdministrationFinalitiesServices, AdministrationFinalitiesServices>();
    builder.Services.AddScoped<IAdministrationTentantsServices,AdministationTenantsServices>();
    builder.Services.AddScoped<IAdministrationConfigurationsServices, AdministrationConfigurationsServices>();
    builder.Services.AddScoped<IAdministrationUsersServices, AdministrationUsersServices>();
    builder.Services.AddScoped<ICommonServices, CommonServices>();
    builder.Services.AddScoped<IApiServices, ApiServices>();
    builder.Services.AddScoped<IAnalyticsServices, AnalyticsServices>();
    builder.Services.AddScoped<IMassiveRequestsServices, MassiveRequestsServices>();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ScERPAContext>();

    builder.Services.AddControllersWithViews().AddMvcOptions(options =>
        options.Filters.Add(
            new ResponseCacheAttribute
            {
                NoStore = true,
                Location = ResponseCacheLocation.None
            }));
    builder.Services.AddRazorPages();
    builder.Services.AddMemoryCache();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.HttpOnly = true;
        options.SuppressXFrameOptionsHeader = false;
    });

    builder.Services.AddCors(options => options.AddPolicy("_myAllowSpecificOrigins",
              policy =>
              {
                  policy.AllowAnyMethod().AllowAnyHeader().WithOrigins(corsOrigins.Split(";"));
              })
    );

    builder.Services.AddResiliencePipeline("default", builder =>
    {
        builder.AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>(),
            // Retry delay
            Delay = TimeSpan.FromSeconds(2),
            // Maximum retries           
            MaxRetryAttempts = 3,
            // Exponential backoff                   
            BackoffType = DelayBackoffType.Constant,
            // Adds jitter to reduce collision
            UseJitter = true
        })
        .AddCircuitBreaker(new CircuitBreakerStrategyOptions
        {
            // Customize and configure the circuit breaker logic.
            SamplingDuration = TimeSpan.FromSeconds(3),
            FailureRatio = 0.7,
            MinimumThroughput = 2,
            ShouldHandle = static args =>
            {
                return ValueTask.FromResult(args is
                {
                    Outcome.Result: HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
                });
            }
        })
       // Timeout after 15 seconds  
       .AddTimeout(TimeSpan.FromSeconds(15));
    });


    var app = builder.Build();



    app.UseForwardedHeaders();
    app.UseCookiePolicy(new CookiePolicyOptions
    {
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always,
        MinimumSameSitePolicy = SameSiteMode.Strict
    });


    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");


    }
    else
    {
        //Sono in sviluppo
        //Applico le migrations
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ScERPAContext>();
            if(dbContext is not null) dbContext.Database.Migrate();
        }
        
        //se non esiste un superadmin lo creo per test
        
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ScERPAContext>();

            string roleName = "SUPERADMIN";
      

            // Crea ruolo se non esiste
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Crea utente se non esiste
            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new ApplicationUser { UserName = adminUsername, Email = adminEmail, EmailConfirmed = true, Attivo=true, Cognome=adminCognome,Nome=adminNome};
                var tenant = dbContext.ListaTenant.Find(1);
                if (tenant is not null) user.Tenants.Add(tenant);
                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                    await userManager.AddClaimAsync(user,new Claim("CurrentTenantId", "1"));
                }
                else
                {
                    // Logga eventuali errori
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Errore: {error.Description}");
                    }
                }
            }
        }        
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors("_myAllowSpecificOrigins");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapAreaControllerRoute(
        name: "Administraion_route",
        areaName: "Administration",
        pattern: "Administration/{controller}/{action=Index}"
        );


    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}");


    app.MapRazorPages();

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
        if (context.Response.Headers.ContainsKey("X-Powered-By")) context.Response.Headers.Remove("X-Powered-By");
        if (context.Response.Headers.ContainsKey("X-AspNetMvc-Version")) context.Response.Headers.Remove("X-AspNetMvc-Version");
        context.Response.Headers.Append("Permissions-Policy", "accelerometer=*, ambient-light-sensor=*, autoplay=*, battery=*, camera=*, cross-origin-isolated=*, display-capture=*, document-domain=*, encrypted-media=*, execution-while-not-rendered=*, execution-while-out-of-viewport=*, fullscreen=*, geolocation=*, gyroscope=*, keyboard-map=*, magnetometer=*, microphone=*, midi=*, navigation-override=*, payment=*, picture-in-picture=*, publickey-credentials-get=*, screen-wake-lock=*, sync-xhr=*, usb=*, web-share=*, xr-spatial-tracking=*, clipboard-read=*, clipboard-write=*, gamepad=*, speaker-selection=*, conversion-measurement=*, focus-without-user-activation=*, hid=*, idle-detection=*, interest-cohort=*, serial=*, sync-script=*, trust-token-redemption=*, unload=*, window-placement=*, vertical-scroll=*");
        context.Response.Headers.Append("Referrer-Policy", "same-origin");
        context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode = block");
        context.Response.Headers.Append("Content-Security-Policy", "connect-src 'self' localhost; default-src 'self' localhost; style-src 'unsafe-inline' 'self' data: localhost; script-src 'unsafe-inline' 'self' ; font-src 'self' data: localhost; img-src 'self' data: localhost www.w3.org; frame-src 'self' localhost; frame-ancestors 'self' localhost;");
        context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "unsafe-none");
        context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-site");
        context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-site");
        context.Response.Headers.Append("SameSite", "Strict");
        await next.Invoke();
    });


    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "C'è stato un problema nell'avvio applicazione");
    return;
}
finally
{
    Log.Information("Applicazione arrestata normalmente");
    Log.CloseAndFlush();
}
