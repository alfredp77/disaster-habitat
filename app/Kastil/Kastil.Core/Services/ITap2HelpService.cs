using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface ITap2HelpService
    {
        Task<IEnumerable<Disaster>> GetDisasters();

        Task<IEnumerable<Assessment>> GetAssessments();
        Task<IEnumerable<Assessment>> GetAssessments(string disasterId);
        Task<Assessment> GetAssessment(string disasterId, string assessmentId);

        Task<IEnumerable<Shelter>> GetShelters();
        Task<IEnumerable<Shelter>> GetShelters(string disasterId, string assessmentId);
        Task<Shelter> GetShelter(string shelterId);

        Task<IEnumerable<Attribute>> GetShelterAttributes();
        Task<IEnumerable<Attribute>> GetAssessmentAttributes();

        Task Save(Assessment assessment);
        Task Save(Shelter shelter);
        Task Save(List<Shelter> shelters);

        Task DeleteAssessments(string disasterId);
    }
}