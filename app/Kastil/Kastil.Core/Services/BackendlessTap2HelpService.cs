using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using Attribute = Kastil.Shared.Models.Attribute;

namespace Kastil.Core.Services
{
    public class BackendlessTap2HelpService : BaseService, ITap2HelpService
    {
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();

        public Task<IEnumerable<Disaster>> GetDisasters()
        {
            var context = PersistenceContextFactory.CreateFor<Disaster>();
            return Asyncer.Async(context.LoadAll);
        }

        public Task<IEnumerable<Assesment>> GetAssesments()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Shelter>> GetShelters()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Attribute>> GetShelterAttributes()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Attribute>> GetAssesmentAttributes()
        {
            throw new NotImplementedException();
        }

        public Task Save(Assesment assesment)
        {
            throw new NotImplementedException();
        }

        public Task Save(Shelter shelter)
        {
            throw new NotImplementedException();
        }
    }
}