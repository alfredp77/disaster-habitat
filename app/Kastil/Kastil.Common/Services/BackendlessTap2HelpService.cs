using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Utils;
using Kastil.Common.Models;
using Attribute = Kastil.Common.Models.Attribute;

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

        public Task<IEnumerable<DisasterAid>> GetDisasterAids(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<DisasterAid>();
            return Asyncer.Async(() => context.LoadAll().Where(d => d.DisasterId == disasterId));
        }

        public Task<IEnumerable<Assessment>> GetAssessments()
        {
            var context = PersistenceContextFactory.CreateFor<Assessment>();
            return Asyncer.Async(context.LoadAll);
        }

        public Task<IEnumerable<Assessment>> GetAssessments(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<Assessment>();
			var assessments = context.LoadAll().ToList();
			return Asyncer.Async(() => assessments.Where(a => a.DisasterId == disasterId));
        }

        public Task<Assessment> GetAssessment(string disasterId, string assessmentId)
        {
            var context = PersistenceContextFactory.CreateFor<Assessment>();
            return Asyncer.Async(() => context.LoadAll().SingleOrDefault(a => a.DisasterId == disasterId && a.ObjectId == assessmentId));
        }

        public Task<IEnumerable<Shelter>> GetShelters()
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(context.LoadAll);
        }

        public Task<Shelter> GetShelter(string disasterId, string shelterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(() => context.LoadAll().SingleOrDefault(s => s.ObjectId == shelterId && s.DisasterId == disasterId));
        }

        public async Task<IEnumerable<Shelter>> GetShelters(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return await Asyncer.Async(() => context.LoadAll().Where(s => s.DisasterId == disasterId));
        }

        public Task<Shelter> GetShelter(string shelterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            return Asyncer.Async(() => context.LoadAll().SingleOrDefault(s => s.ObjectId == shelterId));
        }
		        

        public async Task<IEnumerable<Attribute>> GetAllAttributes()
        {
            var context = PersistenceContextFactory.CreateFor<Attribute>();
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
                await Asyncer.Async(() => context.MarkDeleted(assessment));
            }
        }

        public async Task DeleteShelter(string shelterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            var shelters = await GetShelters();
            foreach (var shelter in shelters.Where(s => shelterId == s.ObjectId))
            {
                await Asyncer.Async(() => context.MarkDeleted(shelter));
            }
        }        

        public async Task DeleteShelters(string disasterId)
        {
            var context = PersistenceContextFactory.CreateFor<Shelter>();
            var shelters = await GetShelters(disasterId);
            foreach (var shelter in shelters)
            {
                await Asyncer.Async(() => context.MarkDeleted(shelter));
            }
        }

        public Task<IEnumerable<AssessmentAttribute>> GetAssessmentAttributes(string assessmentId)
        {
            var context = PersistenceContextFactory.CreateFor<AssessmentAttribute>();
            return Asyncer.Async(() => context.LoadAll().Where(d => d.ItemId == assessmentId));
        }

        public Task SaveAssessmentAttribute(AssessmentAttribute attribute)
        {
            var context = PersistenceContextFactory.CreateFor<AssessmentAttribute>();
            return Asyncer.Async(() => context.Save(attribute));
        }

        public async Task DeleteAssessmentAttribute(string attributeId)
        {
            var context = PersistenceContextFactory.CreateFor<AssessmentAttribute>();
            var assessmentAttribute = context.LoadAll().SingleOrDefault(a => a.ObjectId == attributeId);
            if (assessmentAttribute != null)
                await Asyncer.Async(() => context.MarkDeleted(assessmentAttribute));
        }

        public Task<IEnumerable<ShelterAttribute>> GetShelterAttributes(string shelterId)
        {
            var context = PersistenceContextFactory.CreateFor<ShelterAttribute>();
            return Asyncer.Async(() => context.LoadAll().Where(d => d.ItemId == shelterId));
        }

        public Task SaveShelterAttribute(ShelterAttribute attribute)
        {
            var context = PersistenceContextFactory.CreateFor<ShelterAttribute>();
            return Asyncer.Async(() => context.Save(attribute));

        }

        public async Task DeleteShelterAttribute(string attributeId)
        {
            var context = PersistenceContextFactory.CreateFor<ShelterAttribute>();
            var shelterAttribute = context.LoadAll().SingleOrDefault(a => a.ObjectId == attributeId);
            if (shelterAttribute != null)
                await Asyncer.Async(() => context.MarkDeleted(shelterAttribute));
        }
    }
}