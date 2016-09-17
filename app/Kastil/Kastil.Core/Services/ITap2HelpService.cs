using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface ITap2HelpService
    {
        Task<IEnumerable<Disaster>> GetDisasters();
        Task<IEnumerable<Assesment>> GetAssesments();
        Task<IEnumerable<Shelter>> GetShelters();
        Task<IEnumerable<Attribute>> GetShelterAttributes();
        Task<IEnumerable<Attribute>> GetAssesmentAttributes();

        Task Save(Assesment assesment);
        Task Save(Shelter shelter);
    }
}