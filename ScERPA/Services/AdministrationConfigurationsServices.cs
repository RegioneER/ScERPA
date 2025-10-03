using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using ScERPA.Models.Dictionaries;
using iText.StyledXmlParser.Node;
using iText.Commons.Actions.Contexts;
using NuGet.Protocol.Core.Types;
using System.Configuration;
using ScERPA.Areas.Administration.Models;
using ScERPA.Models.Exceptions;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScERPA.Services.Interfaces;

namespace ScERPA.Services
{
    public class AdministrationConfigurationsServices: IAdministrationConfigurationsServices
    {
        private readonly ScERPAContext _repository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger<AdministrationConfigurationsServices> _logger;
        private readonly IUtilities _utilities;
        private readonly AmbientiEnum _ambienteCorrente;

        public AdministrationConfigurationsServices(ScERPAContext repository, ICommonServices commonServices, ILogger<AdministrationConfigurationsServices> logger, IUtilities utilities)
        {
            _repository = repository;
            _commonServices = commonServices;
            _logger = logger;
            _utilities = utilities;
            _ambienteCorrente = _commonServices.AmbienteCorrente;
        }

        public async Task<int> CreateConfigurationAsync(ConfigurationEditInputModel inputModel)
        {
            int id = 0;
            Configurazione configurazione = new();

            configurazione.Ambiente = _commonServices.AmbienteCorrente;

            if (inputModel is not null)
            {

                configurazione.Nome = _utilities.SanitizeAsPlainText(inputModel.Nome);

                configurazione.ApiManagerClientId = _utilities.Encrypt(_utilities.SanitizeAsPlainText(inputModel.ApiManagerClientId));
                configurazione.ApiManagerClientSecret = _utilities.Encrypt(_utilities.SanitizeAsPlainText(inputModel.ApiManagerClientSecret));
                configurazione.ApiManagerOauthEndpoint = inputModel.ApiManagerOauthEndpoint;

                _repository.Add<Configurazione>(configurazione);
                await _repository.SaveChangesAsync();

                id = configurazione.Id;

                _logger.LogInformation("Creata configurazione con id {Id}", id);
            }

            return id;
        }

        public async Task<int> DeleteConfigurationAsync(int id)
        {

            if (!EntityExists(id))
            {
                throw new ConfigurationNotFoundException($"Tentativo di cancellazione di configurazione  con id = {id} inesistente");
            }
            else if (EntityDeletable(id))
            {
                _repository.Configurazioni.Remove(_repository.Configurazioni.Find(id));
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Cancellazione della finalità con id {Id}", id);
            }
            else
            {
                throw new ConfigurationUndeletableException($"Impossibile cancellare la configurazione {id} perchè collegata a Tenants");

            }
            
            _logger.LogWarning("Cancellazione di configurazione id {Id}", id);
            return id;
        }

        private bool EntityDeletable(int id)
        {
            int elementi = 0;

            var elementiCollegati = from c in _repository.Configurazioni
                                    let listatenant = c.ListaTenant
                                    from t in listatenant
                                    where c.Id == id
                                    select t.Id;

            elementi = elementiCollegati.Count();

            return elementi==0;
        }

        public async Task<ConfigurationDetailsViewModel> GetConfigurationDetailsViewModelAsync(int id)
        {
            ConfigurationDetailsViewModel vm = new();
            Configurazione? elemento;

            if (EntityExists(id))
            {

                elemento = await _repository.Configurazioni.FirstOrDefaultAsync(x => x.Id == id && x.Ambiente == _commonServices.AmbienteCorrente);

                if (elemento is not null)
                {
                    vm.Id = elemento.Id;
                    vm.Nome = elemento.Nome;
                    vm.ApiManagerClientId = "*";
                    vm.ApiManagerClientSecret = "*";
                    vm.Ambiente = elemento.Ambiente;
                    vm.ApiManagerOauthEndpoint = elemento.ApiManagerOauthEndpoint;

                }
                else
                {
                    throw new ConfigurationNotFoundException($"Non esiste una configurazione con id = {id}");
                }
            }
            else
            {
                throw new ConfigurationNotFoundException($"Non esiste una configurazione con id = {id}");
            }

            return vm;
        }



        public async Task<ConfigurationEditCreateViewModel> GetConfigurationEditCreateViewModelAsync(int? id, ConfigurationEditInputModel? inputModel)
        {
            ConfigurationEditCreateViewModel vm = new();
            Configurazione elemento;

            if (id is not null)
            {
                //sono nel caso di Edit
                if (EntityExists((int)id))
                {
                    if (inputModel is not null && ((int)id == inputModel.Id))
                    {
                        vm.inputModel = inputModel;

                    }
                    else
                    {
                        elemento = await _repository.Configurazioni.AsNoTracking().Where(x => x.Id == id).SingleAsync();


                        vm.inputModel.Id = elemento.Id;
                        vm.inputModel.Nome = _utilities.SanitizeAsPlainText(elemento.Nome);
                        vm.inputModel.Ambiente = elemento.Ambiente;
                        
                        vm.inputModel.ApiManagerClientId = string.IsNullOrEmpty(elemento.ApiManagerClientId) ? "" : _utilities.Decrypt(elemento.ApiManagerClientId);
                        vm.inputModel.ApiManagerClientSecret = string.IsNullOrEmpty(elemento.ApiManagerClientSecret) ? "" : _utilities.Decrypt(elemento.ApiManagerClientSecret);
                        vm.inputModel.ApiManagerOauthEndpoint = elemento.ApiManagerOauthEndpoint ?? "";

                    }
                }
                else throw new ConfigurationNotFoundException($"Tentativo di modificare una configurazione con id {id} inesistente");

            }
            else
            {
                if (inputModel is not null)
                {
                    //sono in Create ma il model non è valido quindi lo restituisco per consentire le modifiche
                    vm.inputModel = inputModel;
                }

            }

            //imposto i valori di default

            vm.inputModel.Ambiente = _commonServices.AmbienteCorrente;

            return vm;
        }


        public async Task<ConfigurationsListViewModel> GetConfigurationsListViewModelAsync()
        {
            ConfigurationsListViewModel vm = new();

            IQueryable<ConfigurationsListItemViewModel> listaElementi = from x in _repository.Configurazioni
                                                                      where x.Ambiente == _ambienteCorrente
                                                                      select new ConfigurationsListItemViewModel { Id = x.Id, Nome = x.Nome, Ambiente = x.Ambiente };

            vm.Elenco = await listaElementi.AsNoTracking().ToListAsync();

            return vm;
        }

        public async Task<int> UpdateConfigurationAsync(ConfigurationEditInputModel inputModel)
        {
            Configurazione configurazione;
            int id = 0;

            if (inputModel is not null && EntityExists(inputModel.Id))
            {
                id = inputModel.Id;
                configurazione = await _repository.Configurazioni.Where(x => x.Id == id).SingleAsync();
                configurazione.Nome = _utilities.SanitizeAsPlainText(inputModel.Nome);
                configurazione.Ambiente = _commonServices.AmbienteCorrente;
                configurazione.ApiManagerClientId = string.IsNullOrEmpty(inputModel.ApiManagerClientId) ? "" : _utilities.Encrypt(_utilities.SanitizeAsPlainText(inputModel.ApiManagerClientId));
                configurazione.ApiManagerClientSecret = string.IsNullOrEmpty(inputModel.ApiManagerClientSecret) ? "" : _utilities.Encrypt(_utilities.SanitizeAsPlainText(inputModel.ApiManagerClientSecret));
                configurazione.ApiManagerOauthEndpoint = string.IsNullOrEmpty(inputModel.ApiManagerOauthEndpoint) ? "" : _utilities.SanitizeAsPlainText(inputModel.ApiManagerOauthEndpoint);
               
                try
                {
                    _repository.Update(configurazione);
                    await _repository.SaveChangesAsync();
                    _logger.LogInformation("Aggiornata configurazione id {Id}", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityExists(configurazione.Id))
                    {

                        throw new ConfigurationNotFoundException($"Tentativo di aggiornare configurazione con id = {id} inesistente");
                    }
                    else
                    {
                        throw new ConfigurationConcurrencyErrorException($"Errore in aggiornamento configurazione con id {id}");
                    }
                }

            }

            return id;
        }

        private bool EntityExists(int id)
        {
            return (_repository.Configurazioni?.Any(e => e.Id == id && e.Ambiente==_commonServices.AmbienteCorrente)).GetValueOrDefault();
        }

    }



}
