using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class MedicationAdministrationViewModel
    {
        // Patient Info
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string RoomNumber { get; set; }

        // Active prescriptions
        public List<PrescriptionDto> ActivePrescriptions { get; set; } = new();

        // Administration history
        public List<AdministrationHistoryDto> AdministrationHistory { get; set; } = new();

        // For new administration (modal form)
        public MedicationAdministration Administration { get; set; } = new();
    }

    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string PrescribingDoctor { get; set; }
    }

    public class AdministrationHistoryDto
    {
        public string MedicationName { get; set; }
        public DateTime AdministeredAt { get; set; }
        public string AdministeredBy { get; set; }
        public string Notes { get; set; }
    }

   
}


//public class MedicationAdministrationViewModel
//{
//    // Patient info
//    public Patient Patient { get; set; } = null!;

//    // Active prescriptions for the patient
//    public List<Prescription> ActivePrescriptions { get; set; } = new List<Prescription>();

//    // Previous administrations
//    public List<MedicationAdministration> AdministrationHistory { get; set; } = new List<MedicationAdministration>();

//    // Single administration used in the modal for posting
//    public MedicationAdministration Administration { get; set; } = new MedicationAdministration();
////}