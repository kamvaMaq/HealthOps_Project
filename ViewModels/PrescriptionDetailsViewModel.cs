using HealthOps_Project.Models;

namespace HealthOps_Project.ViewModels
{
    public class PrescriptionDetailsViewModel
    {
        public Prescription Prescription { get; set; }
        public Patient Patient { get; set; }
        public List<MedicationDelivery> DeliveryHistory { get; set; }
        public List<MedicationAdministration> AdministrationHistory { get; set; }
        public int TotalDelivered { get; set; }
        public DateTime? LastAdministered { get; set; }
        public bool IsActive { get; set; }
        public int? DaysRemaining { get; set; }


        public IEnumerable<MedicationAdministration> AdministrationRecords { get; set; } = new List<MedicationAdministration>();
        public IEnumerable<MedicationDelivery> MedicationDeliveries { get; set; } = new List<MedicationDelivery>();


    }
}
