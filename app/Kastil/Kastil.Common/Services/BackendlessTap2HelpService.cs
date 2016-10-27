using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Utils;
using Kastil.Shared.Models;
using Attribute = Kastil.Shared.Models.Attribute;

namespace Kastil.Common.Services
{
    public class BackendlessTap2HelpService : BaseService, ITap2HelpService
    {
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();

        public Task<IEnumerable<Disaster>> GetDisasters()
        {
            var context = PersistenceContextFactory.CreateFor<Disaster>();
            return Asyncer.Async(context.LoadAll);
        }

        public Task<IEnumerable<Assessment>> GetAssessments()
        {
            var context = PersistenceContextFactory.CreateFor<Assessment>();
            return Asyncer.Async(context.LoadAll);
        }

        public Task<IEnumerable<Assessment>> GetAssessments(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<Assessment>();
            return Asyncer.Async(() => context.LoadAll().Where(a => a.DisasterId == disasterId));
        }

        public Task<Assessment> GetAssessment(string disasterId, string assessmentId)
        {
            var context = PersistenceContextFactory.CreateFor<Assessment>();
            return Asyncer.Async(() => context.LoadAll().SingleOrDefault(a => a.DisasterId == disasterId && a.Id == assessmentId));
        }

        public Task<IEnumerable<Shelter>> GetShelters()
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(context.LoadAll);
        }

        public Task<Shelter> GetShelter(string disasterId, string shelterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(() => context.LoadAll().SingleOrDefault(s => s.Id == shelterId && s.DisasterId == disasterId));
        }

        public async Task<IEnumerable<Shelter>> GetShelters(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return await Asyncer.Async(() => context.LoadAll().Where(s => s.DisasterId == disasterId));
        }

        public Task<Shelter> GetShelter(string shelterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(() => context.LoadAll().SingleOrDefault(s => s.Id == shelterId));
        }

        public async Task<IEnumerable<Attribute>> GetShelterAttributes()
        {
            var context = PersistenceContextFactory.CreateFor<ShelterAttribute>();
            return await Asyncer.Async(context.LoadAll);
        }

        public async Task<IEnumerable<Attribute>> GetAssessmentAttributes()
        {
            var context = PersistenceContextFactory.CreateFor<AssessmentAttribute>();
            return await Asyncer.Async(context.LoadAll);
        }

        public Task Save(Assessment assessment)
        {
            var context = PersistenceContextFactory.CreateFor<Assessment>();
            return Asyncer.Async(() => context.Save(assessment));
        }

        public Task Save(List<Shelter> shelters)
        {
            throw new NotImplementedException();
        }

        public Task Save(Shelter shelter)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(() => context.Save(shelter));
        }

        public async Task DeleteAssessments(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<Assessment>();
            var assessments = await GetAssessments(disasterId);
            foreach (var assessment in assessments)
            {
                await Asyncer.Async(() => context.Delete(assessment));
            }
        }

        public async Task DeleteShelter(string shelterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            var shelters = await GetShelters();
            foreach (var shelter in shelters.Where(s => shelterId == s.Id))
            {
                await Asyncer.Async(() => context.Delete(shelter));
            }
        }

        public async Task DeleteShelters(List<string> shelterIds)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            var shelters = await GetShelters();
            foreach (var shelter in shelters.Where(s => shelterIds.Contains(s.Id)))
            {
                await Asyncer.Async(() => context.Delete(shelter));
            }
        }
    }
}