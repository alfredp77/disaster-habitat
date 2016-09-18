using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Core.Services;
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

        public Task<IEnumerable<Attribute>> GetAssesmentAttributes()
        {
            var attr1 = new Attribute { Category = "1", Id = "1", Key = "Number of Shelter Kits"};
            var attr2 = new Attribute { Category = "1", Id = "2", Key = "Number of Hygiene Kits" };
            var attr3 = new Attribute { Category = "1", Id = "3", Key = "Number of Host Families" };
            var attr4 = new Attribute { Category = "1", Id = "4", Key = "Number of Evacuation Centers" };
            var attr5 = new Attribute { Category = "1", Id = "5", Key = "Hotlines" };
            var attr6 = new Attribute { Category = "1", Id = "6", Key = "Number of Hospitals" };
            return Task.FromResult(new List<Attribute> { attr1, attr2, attr3, attr4, attr5, attr6 } as IEnumerable<Attribute>);
        }

        private readonly Dictionary<string, Dictionary<string, Assesment>> _assesments = new Dictionary<string, Dictionary<string, Assesment>>();
        public Task Save(Assesment assesment)
        {
            Dictionary<string, Assesment> assesmentPerDisaster;
             _assesments.TryGetValue(assesment.DisasterId, out assesmentPerDisaster);

            if(assesmentPerDisaster== null)
            {
                _assesments[assesment.DisasterId] = new Dictionary<string, Assesment>();
            }
            assesmentPerDisaster = _assesments[assesment.DisasterId];
            assesmentPerDisaster[assesment.Id] = assesment;
            return Task.Factory.StartNew(() => { });
        }

        private async Task<IEnumerable<Attribute>>  GetRandomAssesmentAttr()
        {
            var attributes = await GetAssesmentAttributes();
            var rnd = new Random();
            foreach (var attr in attributes)
            {
                attr.Value = rnd.Next(1, 100).ToString();
            }
            return attributes;
        }

        private async void InitFakeAssesments()
        {
            var count = 1;
            foreach (var disasterId in _disasters.Keys)
            {
                var attributes = await GetRandomAssesmentAttr();
                var assesment1 = new Assesment { Id = count.ToString(), Location= new Location() { Name = "Location " + count.ToString() }, DisasterId = disasterId, Name = "Assesment " + count.ToString(), Attributes = attributes.ToList() };
                count++;
                await Save(assesment1);
                attributes = await GetRandomAssesmentAttr();
                var assesment2 = new Assesment { Id = count.ToString(), Location = new Location() { Name = "Location " + count.ToString() }, DisasterId = disasterId, Name = "Assesment " + count.ToString(), Attributes = attributes.ToList() };
                await Save(assesment2);
                count ++;
            }
        }

        public async Task<IEnumerable<Assesment>> GetAssesments()
        {
            if (_assesments.Count == 0)
            {
                InitFakeAssesments();
            }
            
            var items = new List<Assesment>();
            foreach(var perEvent in _assesments.Values)
            {
                items.AddRange(perEvent.Values);
            }
            return items;
        }

        public Task<IEnumerable<Assesment>> GetAssesments(string disasterId)
        {
            if (_assesments.Count == 0)
            {
                InitFakeAssesments();
            }

            Dictionary<string, Assesment> perDisaster;
            _assesments.TryGetValue(disasterId, out perDisaster);

            return Task.FromResult(perDisaster == null ? new List<Assesment>().AsEnumerable() : perDisaster.Values.AsEnumerable());
        }

        public Task<Assesment> GetAssesment(string disasterId, string assesmentId)
        {
            Dictionary<string, Assesment> assesmentPerDisaster;
            _assesments.TryGetValue(disasterId, out assesmentPerDisaster);

            if (assesmentPerDisaster == null)
            {
                return Task.FromResult<Assesment>(null);
            }

            Assesment result;
            assesmentPerDisaster.TryGetValue(assesmentId, out result);
            return Task.FromResult(result);
        }


        private readonly Dictionary<string, Shelter> _shelters = new Dictionary<string, Shelter>();
        public Task Save(Shelter shelter)
        {
            _shelters[shelter.Id] = shelter;
            return Task.Factory.StartNew(() => { });
        }
        public Task<IEnumerable<Shelter>> GetShelters()
        {
            var shelter = new Shelter {Id = "1", Name = "Shelter 1"};
            Save(shelter);
            return Task.FromResult(_shelters.Values.AsEnumerable());
        }

        
    }
}