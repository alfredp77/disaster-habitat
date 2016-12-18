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
        #endregion

        #region Attributes
        public Task<IEnumerable<Attribute>> GetAllAttributes()
        {
            var attr1 = new Attribute { Category = "1", ObjectId = "1", Key = "Number of Shelter Kits" };
            var attr2 = new Attribute { Category = "1", ObjectId = "2", Key = "Number of Hygiene Kits" };
            var attr3 = new Attribute { Category = "1", ObjectId = "3", Key = "Number of Host Families" };
            var attr4 = new Attribute { Category = "1", ObjectId = "4", Key = "Number of Evacuation Centers" };
            var attr5 = new Attribute { Category = "1", ObjectId = "5", Key = "Hotlines" };
            var attr6 = new Attribute { Category = "1", ObjectId = "6", Key = "Number of Hospitals" };
			var attr7 = new Attribute { Category = "1", ObjectId = "7", Key = "Available Capacity" };
			var attr8 = new Attribute { Category = "1", ObjectId = "8", Key = "Others" };
            return Task.FromResult(new List<Attribute> { attr1, attr2, attr3, attr4, attr5, attr6, attr7, attr8 } as IEnumerable<Attribute>);
        }

        
        private async Task<IEnumerable<Attribute>> GetRandomAttributes()
        {
			var attributes = (await GetAllAttributes()).ToList();
			var result = new List<Attribute>();
            var rnd = new Random();
			var serializer = Resolve<IJsonSerializer>();
            foreach (var attr in attributes)
            {
				if (rnd.Next(0, 10) < 5) {
					var newAttr = serializer.Clone(attr);
					newAttr.ObjectId = Guid.NewGuid().ToString();
					newAttr.Value = rnd.Next(1, 100).ToString();
					result.Add(newAttr);
				}
            }
            return result;
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
            assessmentPerDisaster[assessment.ObjectId] = assessment;
            return Task.Factory.StartNew(() => { });
        }

        private async Task InitFakeAssessments()
        {
            var count = 1;
            foreach (var disasterId in _disasters.Keys)
            {
                var attributes = await GetRandomAttributes();
                var assessment1 = new Assessment { ObjectId = count.ToString(), Location = "Location " + count, DisasterId = disasterId, Name = "Assessment " + count, Attributes = attributes.ToList() };
                count++;
                await Save(assessment1);
                attributes = await GetRandomAttributes();
                var assessment2 = new Assessment { ObjectId = count.ToString(), Location = "Location " + count, DisasterId = disasterId, Name = "Assessment " + count, Attributes = attributes.ToList() };
                await Save(assessment2);
                count++;
            }
        }

        public async Task<IEnumerable<Assessment>> GetAssessments()
        {
            if (_assessments.Count == 0)
            {
                await InitFakeAssessments();
            }

            var items = new List<Assessment>();
            foreach (var perEvent in _assessments.Values)
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
                _assessments.Remove(assessment.ObjectId);
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

        private async Task InitFakeShelters()
        {
            var attributes = await GetRandomAttributes();

            _shelters.Add(new Shelter
            {
                ObjectId = "1",
                Name = "Manila Shelter 1",
                Location = "Manila, Phillippines",
                Attributes = attributes.ToList()
            });

            _shelters.Add(new Shelter
            {
                ObjectId = "2",
                Name = "Aceh Shelter 1",
                Location = "Banda Aceh, Indonesia",
                Attributes = attributes.ToList()
            });

            _shelters.Add(new Shelter
            {
                ObjectId = "3",
                Name = "Aceh Shelter 2",
                Location = "Banda Aceh, Indonesia",
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

            return _shelters.FirstOrDefault(s => s.DisasterId == disasterId && s.ObjectId == shelterId);
        }

        public Task<Shelter> GetShelter(string shelterId)
        {
            Shelter result = null;
            if (!string.IsNullOrWhiteSpace(shelterId))
                result = _shelters.FirstOrDefault(s => s.ObjectId == shelterId);

            return result != null ? Task.FromResult(result) : Task.FromResult<Shelter>(null);
        }

        public async Task DeleteShelters(string disasterId)
        {
            var shelters = await GetShelters(disasterId);            
            foreach (var shelter in shelters)
            {
                _shelters.Remove(shelter);
            }
        }
        #endregion

    }
}