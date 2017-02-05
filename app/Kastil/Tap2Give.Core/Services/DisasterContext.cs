using Kastil.Common.Models;

namespace Tap2Give.Core.Services
{
    public interface IDisasterContext
    {
        Disaster Disaster { get; }
        void Initialize(Disaster disaster);
    }

    public class DisasterContext : IDisasterContext
    {
        public Disaster Disaster { get; private set; }

        public void Initialize(Disaster disaster)
        {
            Disaster = disaster;
        }
    }
}
