using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{

    public class ServiceCallResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }

    public interface IDisasterService
    {
        Task<Disaster> Load(string id);
        Task<IEnumerable<Disaster>> Load(DisasterFilter filter, int page=0);
    }

    public class DisasterFilter
    {
        public DateTimeOffset? Since { get; set; }
        public string Name { get; set; }        
    }

    public class DisasterService : IDisasterService
    {
        public Task<Disaster> Load(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Disaster>> Load(DisasterFilter filter, int page = 0)
        {
            throw new NotImplementedException();
        }
        
    }

    public class FakeDisasterService : IDisasterService
    {
        private readonly Dictionary<string, Disaster> _disasters = new Dictionary<string, Disaster>();
         
        public FakeDisasterService()
        {
            var disaster1 = new Disaster {Id = Guid.NewGuid().ToString(), Name = "Typhoon Haiyan", When= new DateTimeOffset(2013,11,8,0,0,0,TimeSpan.FromHours(8))};
            var disaster2 = new Disaster { Id = Guid.NewGuid().ToString(), Name = "Aceh Tsunami", When = new DateTimeOffset(2004,12,24,0,0,0,TimeSpan.FromHours(7)) };
            _disasters.Add(disaster1.Id, disaster1);
            _disasters.Add(disaster2.Id, disaster2);
        }

        public Task<Disaster> Load(string id)
        {
            Disaster disaster;
            _disasters.TryGetValue(id, out disaster);
            return Task.FromResult(disaster);
        }

        public Task<IEnumerable<Disaster>> Load(DisasterFilter filter, int page = 0)
        {
            return Task.FromResult(_disasters.Values.OrderByDescending(d => d.When).AsEnumerable());
        }
       
    }
}
