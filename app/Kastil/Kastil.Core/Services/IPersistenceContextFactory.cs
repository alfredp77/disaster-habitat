using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface IPersistenceContextFactory
    {
        IPersistenceContext<T> CreateFor<T>(string name="") where T : BaseModel;
    }
}