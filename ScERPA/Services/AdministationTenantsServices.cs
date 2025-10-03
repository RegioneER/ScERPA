using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Services.Interfaces;
using ScERPA.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using ScERPA.Models.Dictionaries;
using System.Configuration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ScERPA.Areas.Administration.Models;
using ScERPA.Models.Exceptions;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ScERPA.Services
{
    public class AdministationTenantsServices : IAdministrationTentantsServices
    {
        private readonly ScERPAContext _repository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger<AdministationTenantsServices> _logger;
        private readonly IUtilities _utilities;

        public AdministationTenantsServices(ScERPAContext repository, ICommonServices commonServices, ILogger<AdministationTenantsServices> logger, IUtilities utilities)
        {
            _repository = repository;
            _commonServices = commonServices;
            _logger = logger;
            _utilities = utilities;
        }

        public async Task<TenantDetailsViewModel> GetTenantDetailsViewModelAsync(int id)
        {
            TenantDetailsViewModel vm = new();
            Tenant? elemento;

            if (EntityExists(id))
            {
                elemento = await _repository.ListaTenant
                    .AsNoTracking()
                    .Include(tenant => tenant.Configurazioni.Where(configurazione => configurazione.Ambiente == _commonServices.AmbienteCorrente))
                    .Where(tenant => tenant.Id == id)
                    .FirstOrDefaultAsync();

                if (elemento is not null)
                {
                    vm.Id = elemento.Id;
                    vm.Nome = elemento.Nome;
                    vm.Parent = elemento.Parent;
                    vm.Url = elemento.Url;
                    vm.Logo = elemento.Logo;
                    vm.Configurazione = elemento.Configurazioni is not null && elemento.Configurazioni.Any() ? elemento.Configurazioni.Single().Nome : "";
                }
                else
                {
                    throw new TenantNotFoundException($"Non esiste un tenant con id = {id}");
                }                                                                      

            } else
            {
                throw new TenantNotFoundException($"Non esiste un tenant con id = {id}");
            }
                              
            return vm;
        }

        public async Task<TenantsListViewModel> GetTenantsListViewModelAsync()
        {
            TenantsListViewModel vm =new();

            IQueryable<TenantsListItemViewModel> listaTenant = from t in _repository.ListaTenant
                                                               select new TenantsListItemViewModel { Id = t.Id, Nome = t.Nome };
            vm.Elenco = await listaTenant.AsNoTracking<TenantsListItemViewModel>().ToListAsync<TenantsListItemViewModel>(); 

            return vm;
        }

        public async Task<int> CreateTenantAsync(TenantEditInputModel inputModel)
        {
            int id = 0;
            int ConfigurationId;


            if (inputModel is not null)
            {
                Tenant tenant = new();
                tenant.Nome = _utilities.SanitizeAsPlainText(inputModel.Nome);
                tenant.Parent = _utilities.SanitizeAsPlainText(inputModel.Parent);
                tenant.Url = _utilities.SanitizeAsPlainText(inputModel.Url);
                tenant.Logo = _utilities.SanitizeAsPlainText(inputModel.Logo);
               
                if (int.TryParse(inputModel.ConfigurationId, out ConfigurationId) && ConfigurationId != 0)
                {
                    Configurazione? configurazione = _repository.Configurazioni.Find(ConfigurationId);

                    if (configurazione != null) tenant.Configurazioni.Add(configurazione);
                }

                _repository.Add<Tenant>(tenant);

                await _repository.SaveChangesAsync();

                id = tenant.Id;

                _logger.LogInformation("Creato Tenant id {Id}", id);

            }

            return id;
        }

        public async Task<int> UpdateTenantAsync(TenantEditInputModel inputModel)
        {
            int id = 0;
            Tenant? tenant;
            int configurationId;

            if (inputModel is not null && EntityExists(inputModel.Id))
            {
                id = inputModel.Id;
                tenant = _repository.ListaTenant.Where(x => x.Id == id).Include(x => x.Configurazioni.Where(x=>x.Ambiente== _commonServices.AmbienteCorrente)).Single(); 
                tenant.Nome = _utilities.SanitizeAsPlainText(inputModel.Nome);
                tenant.Parent = _utilities.SanitizeAsPlainText(inputModel.Parent);
                tenant.Url = _utilities.SanitizeAsPlainText(inputModel.Url);
                tenant.Logo = _utilities.SanitizeAsPlainText(inputModel.Logo);
                int.TryParse(inputModel.ConfigurationId,out configurationId);

                Configurazione? configurazioneCorrente =tenant.Configurazioni.SingleOrDefault();
                Configurazione configurazione = await _repository.Configurazioni.SingleAsync(x => x.Ambiente == _commonServices.AmbienteCorrente && x.Id == configurationId);

                if(configurazioneCorrente != configurazione)
                {
                    if(configurazioneCorrente != null)
                        tenant.Configurazioni.Remove(configurazioneCorrente);
                    
                    if(configurazione is not null)
                        tenant.Configurazioni.Add(configurazione);
                }

                try
                {
                    _repository.Update(tenant);
                    await _repository.SaveChangesAsync();
                    _logger.LogInformation("Aggiornato Tenant id {Id}", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityExists(tenant.Id))
                    {

                        throw new TenantNotFoundException($"Tentativo di aggiornare tenant con id = {id} inesistente");
                    }
                    else
                    {
                        throw new TenantConcurrencyErrorException($"Errore in aggiornamento Tenant con id {id}");
                    }
                }

            }
 
            return id;
        }

        private bool EntityExists(int id)
        {
            return (_repository.ListaTenant?.Any(e => e.Id == id )).GetValueOrDefault();
        }

        private bool EntityDeletable(int id)
        {            
            int elementi = 0;

            var elementiCollegati = from u in _repository.ListaTenant
                                from ut in u.ListaUtenti
                                where u.Id == id
                                select u.Id;

            elementi = elementiCollegati.Count();            

            return elementi == 0;
        }


        public async Task<int> DeleteTenantAsync(int id)
        {
            Tenant? elemento;

            if (id == 0)
            {
                throw new TenantNotFoundException($"Tentativo di cancellazione di Tenant con id = {id} inesistente");
            }
            else
            {
                elemento = await _repository.ListaTenant.FindAsync(id);
                if (elemento is null)
                {
                    throw new TenantNotFoundException($"Tentativo di cancellazione di Tenant con id = {id} inesistente");
                }
                else
                {
                    if(id == _commonServices.IdTenantCorrente)
                    {
                        throw new TenantUndeletableException($"Impossibile cancellare il tenant corrente");
                    }
                    else if(EntityDeletable(id))
                    {
                        _repository.ListaTenant.Remove(elemento);
                        await _repository.SaveChangesAsync();
                        _logger.LogInformation("Cancellazione di Tenant id {Id}", id);
                    } else
                    {
                        throw new TenantUndeletableException($"Impossibile cancellare il tenant {id} perchè collegato a dati");
                        
                    }

                }

            }

            return elemento.Id;
        }

        public async Task<TenantEditCreateViewModel> GetTenantEditCreateViewModelAsync(int? id, TenantEditInputModel? inputModel)
        {
            TenantEditCreateViewModel vm = new();
            Tenant tenant;
            int idConfigurazione=0;

            if (id is not null)
            {
                //sono nel caso di Edit
                if(EntityExists((int)id) )
                {
                    if(inputModel is not null && ((int)id == inputModel.Id ))
                    {
                        vm.inputModel = inputModel;

                    } else
                    { 
                        tenant =  _repository.ListaTenant.AsNoTracking().Where(x => x.Id == id).Include(x => x.Configurazioni).Single();

                        vm.inputModel.Id = tenant.Id;
                        vm.inputModel.Nome = _utilities.SanitizeAsPlainText(tenant.Nome);
                        vm.inputModel.Parent = _utilities.SanitizeAsPlainText(tenant.Parent);
                        vm.inputModel.Url = _utilities.SanitizeAsPlainText(tenant.Url);
                        vm.inputModel.Logo = _utilities.SanitizeAsPlainText(tenant.Logo);

                        if(tenant.Configurazioni is not null)
                        {
                            Configurazione? configurazione = tenant.Configurazioni.SingleOrDefault(x => x.Ambiente == _commonServices.AmbienteCorrente);
                            if (configurazione is not null) idConfigurazione = configurazione.Id;
                            vm.inputModel.ConfigurationId = idConfigurazione.ToString();
                        }
                    }
                } 
                else throw new TenantNotFoundException($"Tentativo di modificare Tenant con {id} inesistente");
                
            } else if(inputModel is not null)
            {
                //sono in Create ma il model non è valido quindi lo restituisco per consentire le modifiche
                vm.inputModel = inputModel;
            }


            //costruisco la select di Configurazoni
            vm.Configurazioni = await _repository.Configurazioni
                .AsNoTracking()
                .Where(x => x.Ambiente == _commonServices.AmbienteCorrente)
                .Select(x => new SelectListItem { Text = x.Nome, Value=x.Id.ToString() , Selected = x.Id == idConfigurazione })
                .ToListAsync();

            return vm;
        }
    }

}
