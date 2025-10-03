

using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Models.Exceptions;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ScERPA.Services.Interfaces;

namespace ScERPA.Services
{
    public class AdministrationUsersServices : IAdministrationUsersServices
    {
        private readonly ScERPAContext _repository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger<AdministrationUsersServices> _logger;
        private readonly IUserStore<ApplicationUser> _userStore;

        private readonly UserManager<ApplicationUser> _userManager;


        public AdministrationUsersServices(ScERPAContext repository, ICommonServices commonServices, ILogger<AdministrationUsersServices> logger, UserManager<ApplicationUser> userManager,IUserStore<ApplicationUser> userStore)
        {
            _repository = repository;
            _commonServices = commonServices;
            _logger = logger;
            _userManager = userManager;
            _userStore = userStore;

        }

        public async Task<UserAdminViewModel> GetConsumerAsync(string? userId)
        {
            UserAdminViewModel? elemento;

            var utenti = from utente in _repository.Users
                         join utente_ruolo in _repository.UserRoles on utente.Id equals utente_ruolo.UserId
                         join ruolo in _repository.Roles on utente_ruolo.RoleId equals ruolo.Id
                         where ruolo.NormalizedName == "CONSUMER"
                         select utente;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Cercato utente con id null o stringa vuota");
                throw new UserNotFoundException("Cercato utente con id null o stringa vuota");
            }

            elemento = await utenti.AsQueryable().AsNoTracking().Where(u => u.Id == userId && u.Tenants.Contains(_commonServices.TenantCorrente(_commonServices.IdTenantCorrente))).Select(u => new UserAdminViewModel { UserId = u.Id, Cognome = u.Cognome, Nome = u.Nome, Email = u.Email ?? "" }).FirstOrDefaultAsync();
            
            if (elemento is null)
            {
                throw new UserNotFoundException($"Non esiste un utente con id = {userId}");
            }


            return elemento;
        }

        public async Task<List<UserListItemViewModel>> GetUsersListForFinalitiesByAdminAsync(UsersListSearchPanel? searchPanel)
        {
            List<UserListItemViewModel> risultati;


            //prendo i soli utenti attivi e con ruolo Consumer
            var utenti = from utente in _repository.Users
                         join utente_ruolo in _repository.UserRoles on utente.Id equals utente_ruolo.UserId
                         join ruolo in _repository.Roles on utente_ruolo.RoleId equals ruolo.Id
                         where ruolo.NormalizedName == "CONSUMER" && utente.Attivo
                         select utente;

            // prendo quelli del tenant corrente
            var listaUtenti = utenti.AsQueryable().AsNoTracking().Where(u => u.Tenants.Contains(_commonServices.TenantCorrente(_commonServices.IdTenantCorrente)));

            //Imposto i filtri di ricerca
            if (searchPanel is not null)
            {
                if (!string.IsNullOrEmpty(searchPanel.Cognome))
                    listaUtenti = listaUtenti.Where(u => EF.Functions.Like(u.Cognome, $"%{searchPanel.Cognome}%"));
                if (!string.IsNullOrEmpty(searchPanel.Nome))
                    listaUtenti = listaUtenti.Where(u => EF.Functions.Like(u.Nome, $"%{searchPanel.Nome}%"));
            }


            var risultatiQuery = listaUtenti.Select(e => new UserListItemViewModel { Id = e.Id, CognomeNome = e.Cognome + " "+ e.Nome, Email = e.Email ?? "", Attivo = e.Attivo, Ruoli = new List<string>() });

            risultati = await risultatiQuery.OrderBy(u => u.CognomeNome).ToListAsync();

            return risultati;
        }

        public async Task<List<UserAdminAreaServiceFinalityListItem>> GetUserAreasServicesFinalitiesListByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int? finality)
        {
            IQueryable<UserAdminAreaServiceFinalityListItem> lista;

            if (string.IsNullOrEmpty(currentAdmin)) throw new UserNotFoundException("Tentativo di leggere la lista abilitazioni con un utente amministratore nullo");

            if (isAdmin is null || isAdmin == false)
            {
                //Se sono un Manager posso solo vedere le autorizzazioni sulle mie finalità
                lista = from finalita in _repository.ListaFinalita
                        join servizio in _repository.Servizi on finalita.ServizioId equals servizio.Id
                        join area in _repository.Aree on servizio.AreaId equals area.Id
                        where finalita.TenantId == _commonServices.IdTenantCorrente && servizio.Attivo
                        && finalita.Ambiente == _commonServices.AmbienteCorrente
                        && finalita.DataDal.Date <= DateTime.Now.Date && ((finalita.DataAl == null ? DateTime.Now.Date : finalita.DataAl.Value.Date) >= DateTime.Now.Date)
                        && finalita.ListaUtenti.Contains(_repository.Users.Find(userId))
                        && finalita.ListaUtenti.Contains(_repository.Users.Find(currentAdmin))
                        select new UserAdminAreaServiceFinalityListItem { Id = finalita.Id, Finalita = finalita.Descrizione, ServizioId = servizio.Id, Servizio = servizio.Descrizione, AreaId = area.Id, Area = area.Nome };
            }
            else
            {
                lista = from finalita in _repository.ListaFinalita
                        join servizio in _repository.Servizi on finalita.ServizioId equals servizio.Id
                        join area in _repository.Aree on servizio.AreaId equals area.Id
                        where finalita.TenantId == _commonServices.IdTenantCorrente && servizio.Attivo
                        && finalita.Ambiente == _commonServices.AmbienteCorrente
                        && finalita.DataDal.Date <= DateTime.Now.Date && ((finalita.DataAl == null ? DateTime.Now.Date : finalita.DataAl.Value.Date) >= DateTime.Now.Date)
                        && finalita.ListaUtenti.Contains(_repository.Users.Find(userId))
                        select new UserAdminAreaServiceFinalityListItem { Id = finalita.Id, Finalita = finalita.Descrizione, ServizioId = servizio.Id, Servizio = servizio.Descrizione, AreaId = area.Id, Area = area.Nome };
            }


            var risultati = await lista.AsQueryable().AsNoTracking().ToListAsync();

            if (finality != 0) risultati = risultati.Where(r => r.Id == finality).ToList();

            return risultati;
        }

        public async Task<int> RemoveAssociatonOfFinalityToUserByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int finality)
        {
            Finalita? elemento;
            ApplicationUser? utente;

            if (finality == 0 || string.IsNullOrEmpty(userId))
            {
                throw new UserFinalityNotFoundException("Tentativo di cancellazione finalità con id 0 o utente non specificato");
            }
            else
            {
                elemento = await _repository.ListaFinalita.Include(finalita => finalita.ListaUtenti.Where(u => u.Id == userId)).Where(f => f.Id == finality).FirstOrDefaultAsync();
                utente = await _repository.Users.FindAsync(userId);

                if (elemento is null || utente is null || elemento.ListaUtenti is null || elemento.ListaUtenti.Count == 0)
                {
                    throw new UserFinalityNotFoundException("Tentativo di cancellazione associazione utente e finalità non trovati");
                }
                else
                {
                    elemento.ListaUtenti?.Remove(utente);
                    await _repository.SaveChangesAsync();
                }
            }

            return elemento.Id;
        }

        public async Task<int> AssociateFinalityToUserByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int finality)
        {
            Finalita? elemento;
            ApplicationUser? utente;

            if (finality == 0 || string.IsNullOrEmpty(userId))
            {
                throw new UserFinalityNotFoundException("Tentativo di aggiungere finalità con id 0 o utente non specificato");
            }
            else
            {

                elemento = await _repository.ListaFinalita.FindAsync(finality);
                utente = await _repository.Users.FindAsync(userId);

                if (elemento is null || utente is null || elemento.ListaUtenti is null)
                {
                    throw new UserFinalityNotFoundException("Tentativo di creazione associazione utente e finalità non trovati");
                }
                else
                {
                    elemento.ListaUtenti?.Add(utente);
                    await _repository.SaveChangesAsync();
                }
            }

            return elemento.Id;
        }


        public async Task<List<SelectListItem>> GetUserAreasListByAdminAsync(string currentAdmin, bool? isAdmin, string userId)
        {
     
            var lista = await GetUserAdminAreaServicePurpouseAvailableAsync(currentAdmin, isAdmin, userId);
     

            var risultati = lista.DistinctBy(x => x.AreaId).Select(x => new SelectListItem { Value = x.AreaId.ToString(), Text = x.Area }).ToList();

            return risultati;
        }

        public async Task<List<SelectListItem>> GetUserServicesListByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int areaId)
        {
            var lista = await GetUserAdminAreaServicePurpouseAvailableAsync(currentAdmin, isAdmin, userId);

            var risultati = lista.Where(x => x.AreaId == areaId).DistinctBy(x=>x.ServizioId).Select(x => new SelectListItem { Value = x.ServizioId.ToString(), Text = x.Servizio }).Distinct().ToList();

            return risultati;
        }

        public async Task<List<SelectListItem>> GetUserFinalitiesListByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int areaId, int serviceId)
        {
            var lista = await GetUserAdminAreaServicePurpouseAvailableAsync(currentAdmin, isAdmin, userId);

            var risultati = lista.Where(x => x.AreaId == areaId && x.ServizioId == serviceId).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Finalita }).Distinct().ToList();

            return risultati;

        }




        private async Task<List<UserAdminAreaServiceFinalityListItem>> GetUserAdminAreaServicePurpouseAvailableAsync(string currentAdmin, bool? isAdmin, string userId)
        {
            IQueryable<UserAdminAreaServiceFinalityListItem> lista;

            if (string.IsNullOrEmpty(currentAdmin)) throw new UserNotFoundException("Tentativo di leggere la lista abilitazioni con un amministratore corrente nullo");

            if (isAdmin is null || isAdmin == false)
            {
                //Se sono un Manager posso solo vedere le autorizzazioni sulle mie finalità
                lista = from finalita in _repository.ListaFinalita
                        join servizio in _repository.Servizi on finalita.ServizioId equals servizio.Id
                        join area in _repository.Aree on servizio.AreaId equals area.Id
                        where finalita.TenantId == _commonServices.IdTenantCorrente && servizio.Attivo
                        && finalita.Ambiente == _commonServices.AmbienteCorrente
                        && finalita.DataDal.Date <= DateTime.Now.Date && ((finalita.DataAl == null ? DateTime.Now.Date : finalita.DataAl.Value.Date) >= DateTime.Now.Date)
                        && !finalita.ListaUtenti.Contains(_repository.Users.Find(userId))
                        && finalita.ListaUtenti.Contains(_repository.Users.Find(currentAdmin))
                        select new UserAdminAreaServiceFinalityListItem { Id = finalita.Id, Finalita = finalita.Nome, ServizioId = servizio.Id, Servizio = servizio.Descrizione, AreaId = area.Id, Area = area.Nome };
            }
            else
            {
                lista = from finalita in _repository.ListaFinalita
                        join servizio in _repository.Servizi on finalita.ServizioId equals servizio.Id
                        join area in _repository.Aree on servizio.AreaId equals area.Id
                        where finalita.TenantId == _commonServices.IdTenantCorrente && servizio.Attivo
                        && finalita.Ambiente == _commonServices.AmbienteCorrente
                        && finalita.DataDal.Date <= DateTime.Now.Date && ((finalita.DataAl == null ? DateTime.Now.Date : finalita.DataAl.Value.Date) >= DateTime.Now.Date)
                        && !finalita.ListaUtenti.Contains(_repository.Users.Find(userId))
                        select new UserAdminAreaServiceFinalityListItem { Id = finalita.Id, Finalita = finalita.Nome, ServizioId = servizio.Id, Servizio = servizio.Descrizione, AreaId = area.Id, Area = area.Nome };
            }

            var risultati = await lista.AsQueryable().AsNoTracking().ToListAsync();

            return risultati;
        }




        public async Task<List<string>> GetUserRolesNormalizedNameListAsync(string userId)
        {
            ApplicationUser? utente;
            List<String> listaNomiRuoli = new();
            
            if(!string.IsNullOrEmpty(userId))
            {
                utente = await _userStore.FindByIdAsync(userId, CancellationToken.None);

                if (utente is not null)
                {
                    var ruoli = await _userManager.GetRolesAsync(utente);
                    listaNomiRuoli = ruoli.Select(x => x.ToUpper()).ToList();
                }
            }

            

            return listaNomiRuoli;
        }

        public async Task<List<string>> GetUserTenantNameListAsync(string userId)
        {

            List<Tenant> listaTenants = await GetUserTenantListAsync(userId);

            var tenants = listaTenants.Select(x => x.Nome).ToList();


            return tenants;
        }

        public async Task<List<Tenant>> GetUserTenantListAsync(string userId)
        {


            var tenants = from u in _repository.Users
                          from ut in u.Tenants
                          where u.Id == userId
                          select ut;

            return await tenants.ToListAsync<Tenant>();
        }


        public async Task<string> GetUserMaxRoleAsync(string userId)
        {
            string ruoloMassimoUtenteCorrente = "";

            var ruoli =await  GetUserRolesNormalizedNameListAsync(userId);

            if (ruoli.Contains("SUPERADMIN")) ruoloMassimoUtenteCorrente = "SUPERADMIN";
            else if (ruoli.Contains("ADMIN")) ruoloMassimoUtenteCorrente = "ADMIN";
            else if (ruoli.Contains("MANAGER")) ruoloMassimoUtenteCorrente = "MANAGER";
            else if (ruoli.Contains("CONSUMER")) ruoloMassimoUtenteCorrente = "CONSUMER";

            return ruoloMassimoUtenteCorrente;
        }

        public async Task<IQueryable<ApplicationUser>> GetUserAdminAdministrableUsersAsync(string currentAdmin)
        {
            IQueryable<ApplicationUser>? listaUtenti;
            string ruoloCurrentUser = await GetUserMaxRoleAsync(currentAdmin);

            switch (ruoloCurrentUser)
            {
                case "SUPERADMIN":

                    listaUtenti = from u in _repository.Users
                                  select u;

                    break;
                case "ADMIN":
                    listaUtenti = from u in _repository.Users
                                  where !(from ur in _repository.UserRoles
                                          join r in _repository.Roles on ur.RoleId equals r.Id
                                          where r.NormalizedName.Contains("SUPERADMIN") || r.NormalizedName.Contains("ADMIN")
                                          select ur.UserId).Contains(u.Id)
                                          && (u.Tenants.Count == 0 || (from ut in u.Tenants where ut.Id == _commonServices.IdTenantCorrente select ut.Id).Count() == 1)
                                  select u;

                    break;
                case "MANAGER":
                    listaUtenti = from u in _repository.Users
                                  where !(from ur in _repository.UserRoles
                                          join r in _repository.Roles on ur.RoleId equals r.Id
                                          where r.NormalizedName.Contains("SUPERADMIN") || r.NormalizedName.Contains("ADMIN") || r.NormalizedName.Contains("MANAGER")
                                          select ur.UserId).Contains(u.Id)
                                  && (u.Tenants.Count == 0 || (from ut in u.Tenants where ut.Id == _commonServices.IdTenantCorrente select ut.Id).Count() == 1)
                                  select u;

                    break;

                default:
                    listaUtenti = from u in _repository.Users
                                  where false
                                  select u;
                    break;

            }

            return listaUtenti;
        }

        public async Task<List<UserListItemViewModel>> GetUsersListForProfileByAdminAsync(string currentAdmin, UsersListSearchPanel? searchPanel)
        {
            List<UserListItemViewModel> risultati;
            IQueryable<ApplicationUser>? listaUtenti;

            listaUtenti = await GetUserAdminAdministrableUsersAsync(currentAdmin);
            if (searchPanel is not null)
            {
                if (!string.IsNullOrEmpty(searchPanel.Cognome))
                    listaUtenti = listaUtenti.Where(u => EF.Functions.Like(u.Cognome, $"%{searchPanel.Cognome}%"));
                if (!string.IsNullOrEmpty(searchPanel.Nome))
                    listaUtenti = listaUtenti.Where(u => EF.Functions.Like(u.Nome, $"%{searchPanel.Nome}%"));
            }

            var risultatiQuery = listaUtenti.Select(e => new UserListItemViewModel { Id = e.Id, CognomeNome = e.Cognome + " " + e.Nome, Email = e.Email ?? "", Attivo = e.Attivo, Ruoli = new List<string>() }).Distinct();

            risultati = await risultatiQuery.OrderBy(u => u.CognomeNome).ToListAsync();

            return risultati;
        }

        public async Task<UserAdminUserProfileViewModel> GetUserProfileDetailByAdminAsync(string currentAdmin, string userId)
        {
            UserAdminUserProfileViewModel profiloUtente = new();
            ApplicationUser? utente;
            

            if (!string.IsNullOrEmpty(currentAdmin) && !string.IsNullOrEmpty(userId))
            {
                //verifico che sia tra gli utenti amministrabili da currentAdmin
                var listaUtenti = (await GetUserAdminAdministrableUsersAsync(currentAdmin)).Where(u => u.Id==userId).Select(u=>u.Id).Count();
                if (listaUtenti == 1)
                {
                    utente = await _repository.Users.FindAsync(userId);
                    if (utente is not null)
                    {
                        profiloUtente.UserId = utente.Id;
                        profiloUtente.UserName = utente.UserName?.ToString() + "";
                        profiloUtente.Cognome = utente.Cognome;
                        profiloUtente.Nome = utente.Nome;
                        profiloUtente.Email = utente.Email?.ToString() + "";
                        profiloUtente.Attivo = utente.Attivo;

                        profiloUtente.Ruoli = await GetUserRolesNormalizedNameListAsync(userId);

                        profiloUtente.Tenants = await GetUserTenantNameListAsync(userId);

                    }
                    else
                    {
                        throw new UserPermissionNotValidException($"Non esiste un utente con id = {userId} o non si dispone dei permessi");
                    } 
                } else
                {
                    throw new UserPermissionNotValidException($"non si dispone dei permessi su {userId}");
                }

            } else
            {
                throw new UserNotFoundException($"Utente o amminitratore non validi");

            }

            return profiloUtente;
        }

        public async Task<UserAminEditProfileViewModel> GetUserProfileForEditByAdminAsync(string currentAdmin, string userId)
        {
            UserAminEditProfileViewModel profiloUtente = new();
            ApplicationUser? utente;


            if (!string.IsNullOrEmpty(currentAdmin) && !string.IsNullOrEmpty(userId))
            {
                //verifico che sia tra gli utenti amministrabili da currentAdmin
                var listaUtenti = (await GetUserAdminAdministrableUsersAsync(currentAdmin)).Where(u => u.Id == userId).Select(u => u.Id).Count();
                if (listaUtenti == 1)
                {
                    utente =await _repository.Users.FindAsync(userId);
                    if (utente is not null)
                    {
                        profiloUtente.UserId = utente.Id;
                        profiloUtente.UserName = utente.UserName?.ToString() + "";
                        profiloUtente.Cognome = utente.Cognome;
                        profiloUtente.Nome = utente.Nome;
                        profiloUtente.Email = utente.Email?.ToString() + "";
                        profiloUtente.Input.UserId = userId;
                        profiloUtente.Input.Attivo = utente.Attivo;
                        profiloUtente.Input.TenantCorrenteAttivo = (await GetUserTenantNameListAsync(userId)).Contains(_commonServices.TenantCorrente(_commonServices.IdTenantCorrente)?.Nome.ToString()+"");
                        profiloUtente.Input.isConsumer = (await GetUserRolesNormalizedNameListAsync(userId)).Contains("CONSUMER");
                        profiloUtente.Input.isManager = (await GetUserRolesNormalizedNameListAsync(userId)).Contains("MANAGER");
                        profiloUtente.Input.isAdmin = (await GetUserRolesNormalizedNameListAsync(userId)).Contains("ADMIN");
                        profiloUtente.Input.isSuperAdmin = (await GetUserRolesNormalizedNameListAsync(userId)).Contains("SUPERADMIN");
                        profiloUtente.RuoloAmministratore = await GetUserMaxRoleAsync(currentAdmin);

                    }
                    else
                    {
                        throw new UserPermissionNotValidException($"Non esiste un utente con id = {userId} o non si dispone dei permessi");
                    }
                }
                else
                {
                    throw new UserPermissionNotValidException($"non si dispone dei permessi su {userId}");
                }

            }
            else
            {
                throw new UserNotFoundException($"Utente o amminitratore non validi");

            }

            return profiloUtente;
        }

        public async Task<int> UpdateUserProfileByAdminAsync(string currentAdmin, UserProfileEditInputModel inputModel)
        {
            int risultato=0;
            int amministrati;
            string userId="";
            string ruoloAdmin = "";
            ApplicationUser? utente;


            //controllo currentAdmin (ammministratore) non sia nullo
            if (currentAdmin is not null)
            {
                ruoloAdmin = await GetUserMaxRoleAsync(currentAdmin);
                var utentiAmministrabili = await GetUserAdminAdministrableUsersAsync(currentAdmin);
                amministrati= utentiAmministrabili.Where(u => u.Id == inputModel.UserId).Count();
                userId = inputModel.UserId+"";


                if (amministrati == 1 && !string.IsNullOrEmpty(ruoloAdmin))
                {                                                      
                    utente = await _repository.Users.FindAsync(userId);
                    if (utente is not null) {
                        utente.Attivo = inputModel.Attivo;
                        await SetUserCurrentTenantAsync(utente, _commonServices.IdTenantCorrente, inputModel.TenantCorrenteAttivo);
                        switch (ruoloAdmin)
                        {
                            case "SUPERADMIN":

                                //posso cambiare tutti i profili
                                await SetUserRoleAsync(utente, "Admin", inputModel.isAdmin);
                                await SetUserRoleAsync(utente, "Manager", inputModel.isManager);
                                await SetUserRoleAsync(utente, "Consumer", inputModel.isConsumer);
                                break;
                            case "ADMIN":
                                //posso cambiare solo i profili Manager, Consumer
                                 await SetUserRoleAsync(utente, "Manager", inputModel.isManager);
                                 await SetUserRoleAsync(utente, "Consumer", inputModel.isConsumer);
                                break;

                            case "MANAGER":
                                //posso cambiare solo il profilo Consumer
                                await SetUserRoleAsync(utente, "Consumer", inputModel.isConsumer);
                                break;
                        }
                        try
                        {
                            _repository.Update(utente);
                            risultato = await _repository.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            /* TODO verificare che sia salvato
                            if (!EntityExists(utente))
                            {

                                throw new Exception($"Errore in aggiornamento Tenant con id {inputModel.Input.Id}");
                            }
                            else
                            {
                                throw;
                            }
                            */

                        }
                    }
                                                         
                }
                else
                {
                    throw new UserPermissionNotValidException($"l'utente {currentAdmin} non può  amministrare il soggetto {userId}");
                    //non è gestibile dall'admin corrente
                }

            }


            return risultato;
        }


        private async Task<IdentityResult?> SetUserRoleAsync(ApplicationUser utente,string nomeRuolo,bool flag)
        {
            IdentityResult? risultato= null;

            if(utente is not null && !string.IsNullOrEmpty(nomeRuolo))
            {
                if (flag && !await _userManager.IsInRoleAsync(utente, nomeRuolo))
                {
                    //devo aggiungere il ruolo
                    risultato = await _userManager.AddToRoleAsync(utente, nomeRuolo);


                }
                else if (!flag && await _userManager.IsInRoleAsync(utente, nomeRuolo))
                {
                    //devo rimuovere il ruolo
                    risultato = await _userManager.RemoveFromRoleAsync(utente, nomeRuolo);

                }
            }


            return risultato;
        }

        private async Task<bool> SetUserCurrentTenantAsync(ApplicationUser utente, int tenantId, bool flag)
        {
            bool risultato = false;
            Tenant? tenant = null;

            try
            {
                if (tenantId != 0)
                {
                    tenant = _repository.ListaTenant.Find(tenantId);                   
                }

                if (utente is not null && tenant is not null)
                {
                    await _repository.Entry(utente).Collection(x => x.Tenants).LoadAsync();
                    var lista = await GetUserTenantListAsync(utente.Id);

                    if (lista != null)
                    {
                        bool abilitato = lista.Where(x => x.Id == tenantId).Select(x => x.Id).Count() == 1;
                        if (flag && !abilitato)
                        {
                            //devo aggiungere il tenant
                            utente.Tenants.Add(tenant);
                            _repository.Update(utente);
                            risultato = true;

                        }
                        else if (!flag && abilitato)
                        {
                            //devo rimuovere il tenant
                            utente.Tenants.Remove(tenant);
                            _repository.Update(utente);
                            risultato = true;
                        }

                    }

                }
            } catch
            {
                risultato = false;
            }


            return risultato;
        }
        
        
    }
}
