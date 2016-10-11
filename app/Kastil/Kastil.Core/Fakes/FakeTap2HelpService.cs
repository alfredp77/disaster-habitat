using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using Attribute = Kastil.Shared.Models.Attribute;

namespace Kastil.Core.Fakes
{
    public class FakeTap2HelpService : ITap2HelpService
    {
        private readonly Dictionary<string, Disaster> _disasters = new Dictionary<string, Disaster>();
        private string _acehDisasterId = Guid.NewGuid().ToString();

        public FakeTap2HelpService()
        {
            GenerateDisasters();
        }

        #region Disaster incidents
        private void GenerateDisasters()
        {
            var disaster1 = new Disaster
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Typhoon Haiyan",
                DateWhen = new DateTimeOffset(2013, 11, 8, 0, 0, 0, TimeSpan.FromHours(8)),
                Location = new Location(14.599512, 120.984222) { Name = "Manila", Country = "Phillippines" }
            };
            var disaster2 = new Disaster
            {
                Id = _acehDisasterId,
                Name = "Aceh Tsunami",
                DateWhen = new DateTimeOffset(2004, 12, 24, 0, 0, 0, TimeSpan.FromHours(7)),
                Location = new Location(5.54829, 95.323756) { Name = "Banda Aceh", Country = "Indonesia" }
            };
            _disasters.Add(disaster1.Id, disaster1);
            _disasters.Add(disaster2.Id, disaster2);
        }

        public Task<IEnumerable<Disaster>> GetDisasters()
        {
            return Task.FromResult(_disasters.Values.AsEnumerable());
        }
        #endregion

        #region Attributes
        public Task<IEnumerable<Attribute>> GetShelterAttributes()
        {
            var attr1 = new Attribute { Category = "2", Id = "1", Key = "Hotlines" };
            var attr2 = new Attribute { Category = "2", Id = "2", Key = "Available Capacity" };
            var attr3 = new Attribute { Category = "2", Id = "3", Key = "Others" };
            return Task.FromResult(new List<Attribute> { attr1, attr2, attr3} as IEnumerable<Attribute>);
        }

        public Task<IEnumerable<Attribute>> GetAssessmentAttributes()
        {
            var attr1 = new Attribute { Category = "1", Id = "1", Key = "Number of Shelter Kits"};
            var attr2 = new Attribute { Category = "1", Id = "2", Key = "Number of Hygiene Kits" };
            var attr3 = new Attribute { Category = "1", Id = "3", Key = "Number of Host Families" };
            var attr4 = new Attribute { Category = "1", Id = "4", Key = "Number of Evacuation Centers" };
            var attr5 = new Attribute { Category = "1", Id = "5", Key = "Hotlines" };
            var attr6 = new Attribute { Category = "1", Id = "6", Key = "Number of Hospitals" };
            return Task.FromResult(new List<Attribute> { attr1, attr2, attr3, attr4, attr5, attr6 } as IEnumerable<Attribute>);
        }
        
        private async Task<IEnumerable<Attribute>> GetRandomAttributes(bool forAssessment = true)
        {
            var attributes = forAssessment ? await GetAssessmentAttributes() : await GetShelterAttributes();
            var rnd = new Random();
            foreach (var attr in attributes)
            {
                attr.Value = rnd.Next(1, 100).ToString();
            }
            return attributes;
        }
        #endregion

        #region Assessment
        private readonly Dictionary<string, Dictionary<string, Assessment>> _assessments = new Dictionary<string, Dictionary<string, Assessment>>();
        public Task Save(Assessment assessment)
        {
            Dictionary<string, Assessment> assessmentPerDisaster;
            _assessments.TryGetValue(assessment.DisasterId, out assessmentPerDisaster);

            if (assessmentPerDisaster == null)
            {
                _assessments[assessment.DisasterId] = new Dictionary<string, Assessment>();
            }
            assessmentPerDisaster = _assessments[assessment.DisasterId];
            assessmentPerDisaster[assessment.Id] = assessment;
            return Task.Factory.StartNew(() => { });
        }

        private async Task InitFakeAssessments()
        {
            var count = 1;
            foreach (var disasterId in _disasters.Keys)
            {
                var attributes = await GetRandomAttributes();
                var assessment1 = new Assessment { Id = count.ToString(), LocationName= "Location " + count, DisasterId = disasterId, Name = "Assessment " + count, Attributes = attributes.ToList() };
                count++;
                await Save(assessment1);
                attributes = await GetRandomAttributes();
                var assessment2 = new Assessment { Id = count.ToString(), LocationName = "Location " + count, DisasterId = disasterId, Name = "Assessment " + count, Attributes = attributes.ToList() };
                await Save(assessment2);
                count ++;
            }
        }

        public async Task<IEnumerable<Assessment>> GetAssessments()
        {
            if (_assessments.Count == 0)
            {
                await InitFakeAssessments();
            }
            
            var items = new List<Assessment>();
            foreach(var perEvent in _assessments.Values)
            {
                items.AddRange(perEvent.Values);
            }
            return items;
        }

        public async Task<IEnumerable<Assessment>> GetAssessments(string disasterId)
        {
            if (_assessments.Count == 0)
            {
                await InitFakeAssessments();
            }

            Dictionary<string, Assessment> perDisaster;
            _assessments.TryGetValue(disasterId, out perDisaster);

            return perDisaster == null ? new List<Assessment>().AsEnumerable() : perDisaster.Values.AsEnumerable();
        }

        public Task<Assessment> GetAssessment(string disasterId, string assessmentId)
        {
            Dictionary<string, Assessment> assessmentPerDisaster;
            _assessments.TryGetValue(disasterId, out assessmentPerDisaster);

            if (assessmentPerDisaster == null)
            {
                return Task.FromResult<Assessment>(null);
            }

            Assessment result;
            assessmentPerDisaster.TryGetValue(assessmentId, out result);
            return Task.FromResult(result);
        }

        public async Task DeleteAssessments(string disasterId)
        {
            var assessments = await GetAssessments(disasterId);
            foreach (var assessment in assessments)
            {
                _assessments.Remove(assessment.Id);
            }
        }
        #endregion

        #region Shelter
        private readonly List<Shelter> _shelters = new List<Shelter>();
        public Task Save(List<Shelter> shelters)
        {
            _shelters.AddRange(shelters);
            return Asyncer.DoNothing();
        }

        public Task DeleteShelter(string shelterId)
        {
            _shelters.RemoveAll(s => shelterId == s.Id);
            
            return Asyncer.DoNothing();
        }

        public Task Save(Shelter shelter)
        {
            shelter.DateVerifiedOn = DateTime.Now;
            _shelters.Add(shelter);
            return Asyncer.DoNothing();
        }

        private async Task InitFakeShelters()
        {
            var attributes = await GetRandomAttributes(false);

            _shelters.Add(new Shelter
            {
                Id = "1",
                Name = "Manila Shelter 1",
				DateVerifiedOn = new DateTimeOffset(2016, 1, 1,0,0,0, TimeSpan.Zero),
                Location = new Location(14.599512, 120.984222) { Name = "Manila", Country = "Phillippines" },
                Attributes = attributes.ToList()
            });

            _shelters.Add(new Shelter
            {
                Id = "2",
                Name = "Aceh Shelter 1",
                DateVerifiedOn = new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Location = new Location(5.54829, 95.323756) { Name = "Banda Aceh", Country = "Indonesia" },
                Attributes = attributes.ToList()
            });

            _shelters.Add(new Shelter
            {
                Id = "3",
                Name = "Aceh Shelter 2",
                DateVerifiedOn = new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Location = new Location(5.54829, 95.323756) { Name = "Banda Aceh", Country = "Indonesia" },
                DisasterId = _acehDisasterId,
                Attributes = attributes.ToList()
            });
        }
        
        public async Task<IEnumerable<Shelter>> GetShelters()
        {
            if (_shelters.Count == 0)
            {
                await InitFakeShelters();
            }

            return _shelters.AsEnumerable();
        }

        public async Task<IEnumerable<Shelter>> GetSheltersLinkedToDisaster(string disasterId, string assessmentId)
        {
            if (_shelters.Count == 0)
                await InitFakeShelters();

            if (!string.IsNullOrEmpty(assessmentId))
                return _shelters.Where(s => s.AssessmentId == assessmentId);

            if (!string.IsNullOrEmpty(disasterId))
            {
                var shelters = _shelters.Where(s => s.DisasterId == disasterId).ToList();
                if (shelters.Any())
                    return shelters;
            }

            return new List<Shelter>();
        }

        public async Task<IEnumerable<Shelter>> GetSheltersAvailableForDisaster(string disasterId)
        {
            if (_shelters.Count == 0)
                await InitFakeShelters();

            var disasters = await GetDisasters();
            var disasterLocation = disasters.First(d => d.Id == disasterId).Location;
            return _shelters.Any() ? _shelters.Where(s => string.IsNullOrEmpty(s.DisasterId) && s.Location.Country == disasterLocation.Country) : new List<Shelter>();
        }

        public Task<Shelter> GetShelter(string shelterId)
        {
            Shelter result = null;
            if (!string.IsNullOrWhiteSpace(shelterId))
                result = _shelters.FirstOrDefault(s => s.Id == shelterId);

            return result != null ? Task.FromResult(result) : Task.FromResult<Shelter>(null);
        }
        #endregion

    }
}