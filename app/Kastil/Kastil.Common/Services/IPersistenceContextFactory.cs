using Kastil.Shared.Models;

namespace Kastil.Common.Services
{
    public interface IPersistenceContextFactory
    {
        IPersistenceContext<T> CreateFor<T>() where T : BaseModel;
    }
}