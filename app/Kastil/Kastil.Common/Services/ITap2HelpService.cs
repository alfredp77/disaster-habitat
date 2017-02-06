using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public interface ITap2HelpService
    {
        Task<IEnumerable<Disaster>> GetDisasters();
        Task<IEnumerable<DisasterAid>> GetDisasterAids(string disasterId);

        Task<IEnumerable<Assessment>> GetAssessments();
        Task<IEnumerable<Assessment>> GetAssessments(string disasterId);
        Task<Assessment> GetAssessment(string disasterId, string assessmentId);
        Task<IEnumerable<AssessmentAttribute>> GetAssessmentAttributes(string assessmentId);
        Task SaveAssessmentAttribute(AssessmentAttribute attribute);
        Task DeleteAssessmentAttribute(string attributeId);

        Task<IEnumerable<Shelter>> GetShelters();
        Task<IEnumerable<Shelter>> GetShelters(string disasterId);
        Task<Shelter> GetShelter(string disasterId, string shelterId);
        Task<Shelter> GetShelter(string shelterId);
        Task<IEnumerable<ShelterAttribute>> GetShelterAttributes(string shelterId);
        Task SaveShelterAttribute(ShelterAttribute attribute);
        Task DeleteShelterAttribute(string attributeId);

        Task<IEnumerable<Attribute>> GetAllAttributes();

        Task Save(Assessment assessment);
        Task Save(Shelter shelter);
        Task Save(List<Shelter> shelters);

        Task DeleteAssessments(string disasterId);
        Task DeleteShelter(string shelterId);
        Task DeleteShelters(string removedDisasterId);
    }
}