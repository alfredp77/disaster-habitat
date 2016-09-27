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

        public FakeTap2HelpService()
        {
            GenerateDisasters();
        }

        private void GenerateDisasters()
        {
            var disaster1 = new Disaster { Id = Guid.NewGuid().ToString(), Name = "Typhoon Haiyan", DateWhen = new DateTimeOffset(2013, 11, 8, 0, 0, 0, TimeSpan.FromHours(8)) };
            var disaster2 = new Disaster { Id = Guid.NewGuid().ToString(), Name = "Aceh Tsunami", DateWhen = new DateTimeOffset(2004, 12, 24, 0, 0, 0, TimeSpan.FromHours(7)) };
            _disasters.Add(disaster1.Id, disaster1);
            _disasters.Add(disaster2.Id, disaster2);
        }

        public Task<IEnumerable<Disaster>> GetDisasters()
        {
            return Task.FromResult(_disasters.Values.AsEnumerable());
        }

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

        private readonly Dictionary<string, Dictionary<string, Assessment>> _assessments = new Dictionary<string, Dictionary<string, Assessment>>();
        public Task Save(Assessment assessment)
        {
            Dictionary<string, Assessment> assessmentPerDisaster;
             _assessments.TryGetValue(assessment.DisasterId, out assessmentPerDisaster);

            if(assessmentPerDisaster== null)
            {
                _assessments[assessment.DisasterId] = new Dictionary<string, Assessment>();
            }
            assessmentPerDisaster = _assessments[assessment.DisasterId];
            assessmentPerDisaster[assessment.Id] = assessment;
            return Task.Factory.StartNew(() => { });
        }

        private async Task<IEnumerable<Attribute>>  GetRandomAssessmentAttr()
        {
            var attributes = await GetAssessmentAttributes();
            var rnd = new Random();
            foreach (var attr in attributes)
            {
                attr.Value = rnd.Next(1, 100).ToString();
            }
            return attributes;
        }

        private async Task InitFakeAssessments()
        {
            var count = 1;
            foreach (var disasterId in _disasters.Keys)
            {
                var attributes = await GetRandomAssessmentAttr();
                var assessment1 = new Assessment { Id = count.ToString(), Location= "Location " + count, DisasterId = disasterId, Name = "Assessment " + count, Attributes = attributes.ToList() };
                count++;
                await Save(assessment1);
                attributes = await GetRandomAssessmentAttr();
                var assessment2 = new Assessment { Id = count.ToString(), Location = "Location " + count, DisasterId = disasterId, Name = "Assessment " + count, Attributes = attributes.ToList() };
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

        public Task<IEnumerable<Assessment>> GetAssessments(string disasterId)
        {
            if (_assessments.Count == 0)
            {
                InitFakeAssessments();
            }

            Dictionary<string, Assessment> perDisaster;
            _assessments.TryGetValue(disasterId, out perDisaster);

            return Task.FromResult(perDisaster == null ? new List<Assessment>().AsEnumerable() : perDisaster.Values.AsEnumerable());
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


        private readonly Dictionary<string, Shelter> _shelters = new Dictionary<string, Shelter>();
        public Task Save(Shelter shelter)
        {
            _shelters[shelter.Id] = shelter;
            return Asyncer.DoNothing();
        }
        public Task<IEnumerable<Shelter>> GetShelters()
        {
            var shelter = new Shelter {Id = "1", Name = "Shelter 1"};
            Save(shelter);
            return Task.FromResult(_shelters.Values.AsEnumerable());
        }

        public async Task DeleteAssessments(string disasterId)
        {
            var assessments = await GetAssessments(disasterId);
            foreach (var assessment in assessments)
            {
                _assessments.Remove(assessment.Id);
            }
        }
    }
}