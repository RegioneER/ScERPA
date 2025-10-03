using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Models.Exceptions;
using ScERPA.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace ScERPA.Services
{
    public class AdministrationAreasServices : IAdministrationAreasServices
    {
        private readonly ScERPAContext _repository;



        public AdministrationAreasServices(ScERPAContext repository)
        {
            _repository = repository;


        }

        public async Task<AreasListViewModel> GetAreasListViewModelAsync()
        {
            AreasListViewModel vm = new();

            IQueryable<AreasListItemViewModel> listaElementi = from e in _repository.Aree
                                                               orderby e.Ordinale
                                                               select new AreasListItemViewModel { Id = e.Id, Nome = e.Nome };

            vm.Elenco = await listaElementi.AsNoTracking().ToListAsync();

            return vm;
        }

        public async Task<AreaDetailsViewModel> GetAreaViewModelAsync(int id)
        {
            AreaDetailsViewModel vm= new();
            Area? elemento;


            if (EntityExists(id))
            {

                elemento = await _repository.Aree.Include(x => x.Sezioni).Where(x => x.Id == id).FirstOrDefaultAsync();

                if (elemento is not null)
                {
                    vm.Id= elemento.Id;
                    vm.Nome = elemento.Nome;
                    vm.Ordinale = elemento.Ordinale;
                    vm.Sezioni = elemento.Sezioni == null ? "" : string.Join(",", elemento.Sezioni.OrderBy(y => y.Ordinale).Select(x => x.Nome).ToArray());
                }
                else
                {
                    throw new AreaNotFoundException($"Non esiste un'Area con id = {id}");
                }

            }
            else
            {
                throw new AreaNotFoundException($"Non esiste un'Area con id = {id}");
            }

            return vm;

        }


        private bool EntityExists(int id)
        {
            return (_repository.Aree?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool EntityDeletable(int id)
        {
            int elementi = 0;

            var elementiCollegati = from s in _repository.Servizi                                  
                                    where s.AreaId == id
                                    select s.Id;

            elementi = elementiCollegati.Count();

            return elementi == 0;
        }

        public Task<int> CreateAreaAsync(AreaEditInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAreaAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAreaAsync(AreaEditInputModel inputModel)
        {
            throw new NotImplementedException();
        }
    }
}
