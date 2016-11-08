using Kastil.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.ViewModels
{
    public class ShelterListViewModel : ItemListViewModel
    {
        private readonly ShelterEditContext _context;

        public ShelterListViewModel(ShelterEditContext context)
        {
            _context = context;
            Title = "Shelters";
            AllowAddCommand = true;
        }

        protected override void DoItemSelectedCommand(AttributedListItemViewModel itemVm)
        {
            var shelter = (Shelter)itemVm.Value;
            _context.Initialize(shelter, DisasterId);
            ShowViewModel<ShelterViewModel>();
        }

        protected override async Task<IEnumerable<Item>> GetItems()
        {
            var service = Resolve<ITap2HelpService>();
            return await service.GetShelters(DisasterId);
        }

        protected override string ItemType => typeof (Shelter).Name;        

        protected override Task DoAddCommand()
        {
            return Task.Run(() =>
            {
                _context.Initialize(disasterId: DisasterId);
                ShowViewModel<ShelterViewModel>();
            });
        }
    }
}
