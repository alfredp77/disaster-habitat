using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Attribute = Kastil.Common.Models.Attribute;

namespace Kastil.Common.Fakes
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
                Description = "Disaster 1 happened in the interior areas of the country1 and every help is appreciated.",
                GiveUrl = "",
                ImageUrl = "",
                DateWhen = new DateTimeOffset(2013, 11, 8, 0, 0, 0, TimeSpan.FromHours(8)),
                Location = "Manila, Phillippines"
            };
            var disaster2 = new Disaster
            {
                Id = _acehDisasterId,
                Name = "Aceh Tsunami",
                Description = "Disaster happened in the coastal areas of the country2 and every help is appreciated.",
                GiveUrl = "",
                ImageUrl = "",
                DateWhen = new DateTimeOffset(2004, 12, 24, 0, 0, 0, TimeSpan.FromHours(7)),
                Location = "Banda Aceh, Indonesia"
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

        public Task<IEnumerable<Attribute>> GetAttributes<T>(T item) where T : Item
        {
            return item.GetType() == typeof(Assessment) ? GetAssessmentAttributes() : GetShelterAttributes();
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
            var matchingShelter = _shelters.FirstOrDefault(s => s.Id == shelter.Id);
            if (matchingShelter != null) 
                _shelters[_shelters.IndexOf(matchingShelter)] = shelter;
            else
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
                LocationName = "Manila, Phillippines",
                Attributes = attributes.ToList()
            });

            _shelters.Add(new Shelter
            {
                Id = "2",
                Name = "Aceh Shelter 1",
                LocationName = "Banda Aceh, Indonesia",
                Attributes = attributes.ToList()
            });

            _shelters.Add(new Shelter
            {
                Id = "3",
                Name = "Aceh Shelter 2",
                LocationName = "Banda Aceh, Indonesia",
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

        public async Task<IEnumerable<Shelter>> GetShelters(string disasterId)
        {
            if (_shelters.Count == 0)
            {
                await InitFakeShelters();
            }
            
            return _shelters.Where(s => s.DisasterId == disasterId).AsEnumerable();
        }

        public async Task<Shelter> GetShelter(string disasterId, string shelterId)
        {
            if (_shelters.Count == 0)
            {
                await InitFakeShelters();
            }

            return _shelters.FirstOrDefault(s => s.DisasterId == disasterId && s.Id == shelterId);
        }

        public Task<Shelter> GetShelter(string shelterId)
        {
            Shelter result = null;
            if (!string.IsNullOrWhiteSpace(shelterId))
                result = _shelters.FirstOrDefault(s => s.Id == shelterId);

            return result != null ? Task.FromResult(result) : Task.FromResult<Shelter>(null);
        }
        #endregion

        #region Disaster Aids
        private readonly List<DisasterIncidentAid> _disasterIncidentAids = new List<DisasterIncidentAid>();
        private void GenerateAidForDisasterIncident()
        {
            if (!_disasters.Any())
                GenerateDisasters();

            foreach (var incident in _disasters)
            {
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$10", DisplayText = "Weekly ration for 2 people." });
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$10", DisplayText = "100 bricks." });
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$10", DisplayText = "Weekly Provisions for a person." });
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$20", DisplayText = "Weekly ration for 4 people." });
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$20", DisplayText = "1000 bricks." });
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$20", DisplayText = "Weekly Provisions for a person." });
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$50", DisplayText = "Monthly ration for 2 people." });
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$50", DisplayText = "Weekly Provisions for 2 people." });
                _disasterIncidentAids.Add(new DisasterIncidentAid { DisasterId = incident.Key, DollarValue = "$100", DisplayText = "Monthly ration for 4 people." });
            }
        }

        public Task<IEnumerable<DisasterIncidentAid>> GetAidsForDisaster(string disasterId)
        {
            if (!_disasterIncidentAids.Any())
                GenerateAidForDisasterIncident();

            return Task.FromResult(_disasterIncidentAids.Where(d => d.DisasterId == disasterId));
        }
        #endregion

    }
}