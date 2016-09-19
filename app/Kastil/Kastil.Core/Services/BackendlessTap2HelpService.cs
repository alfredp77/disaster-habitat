using System;
using System.Collections.Generic;
using System.Linq;
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
            var context = PersistenceContextFactory.CreateFor<Assesment>();
            return Asyncer.Async(context.LoadAll);
        }

        public Task<IEnumerable<Assesment>> GetAssesments(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<Assesment>();
            return Asyncer.Async(() => context.LoadAll().Where(a => a.DisasterId == disasterId));
        }

        public Task<Assesment> GetAssesment(string disasterId, string assesmentId)
        {
            var context = PersistenceContextFactory.CreateFor<Assesment>();
            return Asyncer.Async(() => context.LoadAll().SingleOrDefault(a => a.DisasterId == disasterId && a.Id == assesmentId));
        }

        public Task<IEnumerable<Shelter>> GetShelters()
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(context.LoadAll);
        }

        public async Task<IEnumerable<Attribute>> GetShelterAttributes()
        {
            var context = PersistenceContextFactory.CreateFor<ShelterAttribute>();
            return await Asyncer.Async(context.LoadAll);
        }

        public async Task<IEnumerable<Attribute>> GetAssesmentAttributes()
        {
            var context = PersistenceContextFactory.CreateFor<AssesmentAttribute>();
            return await Asyncer.Async(context.LoadAll);
        }

        public Task Save(Assesment assesment)
        {
            var context = PersistenceContextFactory.CreateFor<Assesment>();
            return Asyncer.Async(() => context.Save(assesment));
        }

        public Task Save(Shelter shelter)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(() => context.Save(shelter));
        }

        public async Task DeleteAssesments(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<Assesment>();
            var assesments = await GetAssesments(disasterId);
            foreach (var assesment in assesments)
            {
                await Asyncer.Async(() => context.Delete(assesment));
            }
        }
    }
}