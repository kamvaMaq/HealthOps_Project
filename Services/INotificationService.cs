using HealthOps_Project.Models;
using System.Threading.Tasks;

namespace HealthOps_Project.Services
{
    public interface INotificationService
    {
        Task NotifyNewPrescriptionAsync(Prescription prescription);
        Task NotifyPrescriptionUpdatedAsync(Prescription prescription);
        Task NotifyPrescriptionDeletedAsync(Prescription prescription);
        Task NotifyScriptProcessedAsync(Prescription prescription);
        Task NotifyMedicationDispensedAsync(Prescription prescription);
    }
}