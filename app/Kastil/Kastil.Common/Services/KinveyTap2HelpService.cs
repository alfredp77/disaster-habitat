using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;
using Kinvey;
using Attribute = Kastil.Common.Models.Attribute;
using User = Kinvey.User;

namespace Kastil.Common.Services
{
    public class KinveyTap2HelpService : BaseService, ITap2HelpService
    {


        private async Task<IEnumerable<T>> GetObjects<T>(Func<DataStore<T>, IQueryable<T>> buildQuery = null) where  T : BaseModel
        {
            await KinveyHelpers.EnsureLogin();
            var datastore = KinveyHelpers.GetDataStore<T>();
            if (buildQuery == null)
                buildQuery = d => null;
            return await datastore.FindAsync(buildQuery(datastore));
        }

        private async Task<T> GetById<T>(string id) where T : BaseModel
        {
            var datastore = KinveyHelpers.GetDataStore<T>();
            return (await datastore.FindByIDAsync(id)).FirstOrDefault();

        }

        public Task<IEnumerable<Disaster>> GetDisasters()
        {
            return GetObjects<Disaster>(datastore => datastore.Where(d => d.IsActive));
        }
        

        public Task<IEnumerable<Assessment>> GetAssessments()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Assessment>> GetAssessments(string disasterId)
        {
            return await GetObjects<Assessment>(datastore => datastore.Where(d => d.DisasterId == disasterId));
        }

        public async Task<Assessment> GetAssessment(string disasterId, string assessmentId)
        {
            var assessment =  await GetById<Assessment>(assessmentId);
            return assessment.DisasterId != disasterId ? null : assessment;
        }

        public Task<IEnumerable<Shelter>> GetShelters()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Shelter>> GetShelters(string disasterId)
        {
            throw new NotImplementedException();
        }

        public Task<Shelter> GetShelter(string disasterId, string shelterId)
        {
            throw new NotImplementedException();
        }

        public Task<Shelter> GetShelter(string shelterId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Attribute>> GetAllAttributes()
        {
            return GetObjects<Attribute>();
        }

        public Task Save(Assessment assessment)
        {
            var datastore = KinveyHelpers.GetDataStore<Assessment>();
            return datastore.SaveAsync(assessment);
        }

        public Task Save(Shelter shelter)
        {
            throw new NotImplementedException();
        }

        public Task Save(List<Shelter> shelters)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAssessments(string disasterId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteShelter(string shelterId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteShelters(string removedDisasterId)
        {
            throw new NotImplementedException();
        }
    }
}
