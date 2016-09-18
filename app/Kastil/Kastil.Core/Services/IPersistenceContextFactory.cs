using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface IPersistenceContextFactory
    {
        IPersistenceContext<T> CreateFor<T>() where T : BaseModel;
    }
}