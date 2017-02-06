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
    public class FakeTap2HelpService : BaseService, ITap2HelpService
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
                ObjectId = Guid.NewGuid().ToString(),
				Name = "Typhoon Haiyan (F)",
                Description = "Typhoon Haiyan, known as Super Typhoon Yolanda in the Philippines, was one of the most intense tropical cyclones on record, which devastated portions of Southeast Asia, particularly the Philippines.",
                GiveUrl = "http://www.give2habitat.org/member/t/thenpdgroup",
                ImageUrl = "https://api.backendless.com/4ED00D7B-240E-B654-FF92-8E5E8C6F0100/v1/files/disasterIncidentImages/typhoon-haiyan.jpg",
                DateWhen = new DateTimeOffset(2013, 11, 8, 0, 0, 0, TimeSpan.FromHours(8)),
                Location = "Manila, Phillippines"
            };
            var disaster2 = new Disaster
            {
                ObjectId = _acehDisasterId,
				Name = "Nepal Earthquake 2015 (F)",
                Description = "Nepal was hit by a 7.8 magnitude earthquake on April 25 2015, causing massive damage and high loss of life, followed by major aftershocks on April 26 causing further destruction. As of June 8, Habitat has built 21 demonstration temporary shelters in Kavre and Sindhupalchowk districts to show affected communities how to effectively use materials from a temporary shelter kit. Habitat staff and volunteers have also distributed more than 1,600 kits in worst-hit Kavre, Gorkha, Dhading, and Sindhupalchowk.",
                GiveUrl = "http://www.give2habitat.org/singapore/nepaleq2015",
                ImageUrl = "https://api.backendless.com/4ED00D7B-240E-B654-FF92-8E5E8C6F0100/v1/files/disasterIncidentImages/nepal_quake.png",
                DateWhen = new DateTimeOffset(2015, 04, 25, 0, 0, 0, TimeSpan.FromHours(7)),
                Location = "Namche Bazar, Nepal"
            };
            _disasters.Add(disaster1.ObjectId, disaster1);
            _disasters.Add(disaster2.ObjectId, disaster2);
        }

        public Task<IEnumerable<Disaster>> GetDisasters()
        {
            return Task.FromResult(_disasters.Values.AsEnumerable());
        }

        public Task<IEnumerable<DisasterAid>> GetDisasterAids(string disasterId)
        {
            return Task.FromResult(Enumerable.Empty<DisasterAid>());
        }

        #endregion

        #region Attributes
        public Task<IEnumerable<Attribute>> GetAllAttributes()
        {
            var attr1 = new Attribute { Type = "*", ObjectId = "1", Key = "Number of Shelter Kits" };
            var attr2 = new Attribute { Type = "*", ObjectId = "2", Key = "Number of Hygiene Kits" };
            var attr3 = new Attribute { Type = "S", ObjectId = "3", Key = "Number of Host Families" };
            var attr4 = new Attribute { Type = "A", ObjectId = "4", Key = "Number of Evacuation Centers" };
            var attr5 = new Attribute { Type = "S", ObjectId = "5", Key = "Hotlines" };
            var attr6 = new Attribute { Type = "*", ObjectId = "6", Key = "Number of Hospitals" };
			var attr7 = new Attribute { Type = "S", ObjectId = "7", Key = "Available Capacity" };
			var attr8 = new Attribute { Type = "*", ObjectId = "8", Key = "Others" };
            return Task.FromResult(new List<Attribute> { attr1, attr2, attr3, attr4, attr5, attr6, attr7, attr8 } as IEnumerable<Attribute>);
        }

        
        private async Task<IEnumerable<T>> CreateRandomAttributes<T>() where T : ValuedAttribute, new()
        {
			var valuedAttribute = (await GetAllAttributes()).Select(a => a.CreateValuedAttribute<T>());
			var result = new List<T>();
            var rnd = new Random();
            foreach (var attr in valuedAttribute)
            {
				if (rnd.Next(0, 10) < 5) {

                    attr.ObjectId = Guid.NewGuid().ToString();
                    attr.Value = rnd.Next(1, 100).ToString();
					result.Add(attr);
				}
            }
            return result;
        }
        #endregion

        #region Assessment
        private readonly Dictionary<string, Assessment> _assessments = new Dictionary<string, Assessment>();
        public Task Save(Assessment assessment)
        {
            return Asyncer.Async(() => _assessments[assessment.ObjectId] = assessment);
        }

        private async Task InitFakeAssessments()
        {
            var count = 1;
            foreach (var disasterId in _disasters.Keys)
            {                
                await CreateAssessment("1", disasterId);
                await CreateAssessment("2", disasterId);
            }
        }

        private readonly Dictionary<string, AssessmentAttribute> _assessmentAttributes = new Dictionary<string, AssessmentAttribute>();
        private async Task CreateAssessment(string assessmentId, string disasterId)
        {
            var assessment1 = new Assessment
            {
                ObjectId = assessmentId,
                Location = "Location " + assessmentId,
                DisasterId = disasterId,
                Name = "Assessment " + assessmentId
            };
            var attributes = (await CreateRandomAttributes<AssessmentAttribute>()).ToList();
            foreach (var assessmentAttribute in attributes)
            {                
                assessmentAttribute.AssessmentId = assessment1.ObjectId;
                _assessmentAttributes.Add(assessmentAttribute.ObjectId, assessmentAttribute);
            }
            
            await Save(assessment1);
        }

        public async Task<IEnumerable<Assessment>> GetAssessments()
        {
            if (_assessments.Count == 0)
            {
                await InitFakeAssessments();
            }

            return _assessments.Values;
        }

        public async Task<IEnumerable<Assessment>> GetAssessments(string disasterId)
        {
            if (_assessments.Count == 0)
            {
                await InitFakeAssessments();
            }

            return _assessments.Values.Where(a => a.DisasterId == disasterId);
        }

        public async Task<Assessment> GetAssessment(string disasterId, string assessmentId)
        {
            if (_assessments.Count == 0)
            {
                await InitFakeAssessments();
            }

            Assessment assessment;
            _assessments.TryGetValue(assessmentId, out assessment);

            return assessment?.DisasterId == disasterId ? assessment : null;
        }

        public async Task DeleteAssessments(string disasterId)
        {
            var assessments = await GetAssessments(disasterId);
            foreach (var assessment in assessments)
            {
                _assessments.Remove(assessment.ObjectId);
            }
        }

        public Task<IEnumerable<AssessmentAttribute>>  GetAssessmentAttributes(string assessmentId)
        {
            return Task.FromResult(_assessmentAttributes.Values.Where(a => a.AssessmentId == assessmentId));
        }

        public Task SaveAssessmentAttribute(AssessmentAttribute attribute)
        {
            return Asyncer.Async(() => _assessmentAttributes[attribute.ObjectId] = attribute);
        }

        public Task DeleteAssessmentAttribute(string attributeId)
        {
            return Asyncer.Async(() => _assessmentAttributes.Remove(attributeId));
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
            _shelters.RemoveAll(s => shelterId == s.ObjectId);

            return Asyncer.DoNothing();
        }

        public Task Save(Shelter shelter)
        {
            var matchingShelter = _shelters.FirstOrDefault(s => s.ObjectId == shelter.ObjectId);
            if (matchingShelter != null)
                _shelters[_shelters.IndexOf(matchingShelter)] = shelter;
            else
                _shelters.Add(shelter);

            return Asyncer.DoNothing();
        }


        private readonly Dictionary<string, ShelterAttribute> _shelterAttributes = new Dictionary<string, ShelterAttribute>();
        private async Task InitFakeShelters()
        {
            var random = new Random();
            var disasters = _disasters.Values.ToList();
            _shelters.Add(await CreateShelter("1", disasters[random.Next(0, disasters.Count - 1)].ObjectId));
            _shelters.Add(await CreateShelter("2", disasters[random.Next(0, disasters.Count - 1)].ObjectId));
            _shelters.Add(await CreateShelter("3", disasters[random.Next(0, disasters.Count - 1)].ObjectId));
        }

        private async Task<Shelter> CreateShelter(string shelterId, string disasterId)
        {
            var shelter = new Shelter
            {
                ObjectId = shelterId,
                Name = $"Shelter {shelterId}",
                DisasterId = disasterId
            };
            var attributes = (await CreateRandomAttributes<ShelterAttribute>()).ToList();
            foreach (var attribute in attributes)
            {
                attribute.ShelterId = shelter.ObjectId;
                _shelterAttributes.Add(attribute.Key, attribute);
            }
            return shelter;
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

            return _shelters.FirstOrDefault(s => s.DisasterId == disasterId && s.ObjectId == shelterId);
        }

        public Task<Shelter> GetShelter(string shelterId)
        {
            return Task.FromResult(_shelters.FirstOrDefault(s => s.ObjectId == shelterId));
        }

        public async Task DeleteShelters(string disasterId)
        {
            var shelters = await GetShelters(disasterId);            
            foreach (var shelter in shelters)
            {
                _shelters.Remove(shelter);
            }
        }

        public Task<IEnumerable<ShelterAttribute>> GetShelterAttributes(string shelterId)
        {
            return Task.FromResult(_shelterAttributes.Values.Where(a => a.ShelterId == shelterId));
        }

        public Task SaveShelterAttribute(ShelterAttribute attribute)
        {
            return Asyncer.Async(() => _shelterAttributes[attribute.ObjectId] = attribute);
        }

        public Task DeleteShelterAttribute(string attributeId)
        {
            return Asyncer.Async(() => _shelterAttributes.Remove(attributeId));
        }
        #endregion

    }
}