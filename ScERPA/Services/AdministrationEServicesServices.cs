using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Core.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScERPA.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models.Exceptions;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ScERPA.Services
{
    public class AdministrationEServicesServices : IAdministationEServicesServices
    {
        private readonly ScERPAContext _repository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger<AdministrationEServicesServices> _logger;


        public AdministrationEServicesServices(ScERPAContext repository, ICommonServices commonServices, ILogger<AdministrationEServicesServices> logger)
        {
            _repository = repository;
            _commonServices = commonServices;
            _logger = logger;

        }

        public Task<int> CreateEServiceAsync(EServiceEditInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteEServiceAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<EServiceDetailsViewModel> GetEServiceDetailsViewModelAsync(int id)
        {
            EServiceDetailsViewModel? vm = new();
            Servizio? elemento;

            if (EntityExists(id))
            {

                elemento = await _repository.Servizi.Include(x => x.Area).Include(x => x.Sezione).Include(y=>y.IndirizziChiamata.Where(i=>i.Ambiente==_commonServices.AmbienteCorrente && i.TenantId==_commonServices.IdTenantCorrente)).Where(x => x.Id == id).FirstOrDefaultAsync();
                if (elemento is not null)
                {
                    vm.Id = elemento.Id;
                    vm.Area = elemento.Area.Nome;
                    vm.Nome = elemento.Nome;
                    vm.Descrizione = elemento.Descrizione;
                    vm.Attivo = elemento.Attivo ? "Si" : "No";
                    vm.Sezione = elemento.Sezione == null ? "" : elemento.Sezione.Nome;
                    vm.Cod = elemento.Cod == null ? "" : elemento.Cod;
                    vm.Indirizzo = elemento.IndirizziChiamata?.SingleOrDefault()?.Indirizzo ?? "";
                }
                else
                {
                    throw new EServiceNotFoundException($"Non esiste un Servizio con id = {id}");
                }

            }
            else
            {
                throw new EServiceNotFoundException($"Non esiste un Servizio con id = {id}");
            }

            return vm;
        }

        public async Task<EServicesListViewModel> GetEServicesListViewModelAsync()
        {
            EServicesListViewModel vm = new(); 

            IQueryable<EServicesListItemViewModel> listaElementi = from e in _repository.Servizi
                                                                   join a in _repository.Aree on e.AreaId equals a.Id
                                                                   select new EServicesListItemViewModel { Id = e.Id, Nome = e.Nome , Area= a.Nome, AreaId = a.Id, Attivo=e.Attivo ? "Si":"No"};

            vm.Elenco = await listaElementi.AsNoTracking().ToListAsync();

            return vm;
        }

        public Task<int> UpdateEServiceAsync(EServiceEditInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        private bool EntityExists(int id)
        {
            return (_repository.Servizi?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool EntityDeletable(int id)
        {
            int elementi = 0;

            var elementiCollegati = from f in _repository.ListaFinalita
                                    where f.ServizioId == id
                                    select f.Id;

            elementi = elementiCollegati.Count();

            return elementi == 0;
        }


    }
}