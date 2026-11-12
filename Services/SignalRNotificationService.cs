using System.Threading.Tasks;
using HealthOps_Project.Models;
using System;

namespace HealthOps_Project.Services
{
    public class SignalRNotificationService : INotificationService
    {
        public SignalRNotificationService()
        {
            // No dependencies - simple service
        }

        public async Task NotifyNewPrescriptionAsync(Prescription prescription)
        {
            // Simple implementation without SignalR
            // You can add logging or other simple notifications here
            Console.WriteLine($"[NOTIFICATION] New prescription created: {prescription.MedicationName} for {prescription.Patient?.FirstName} {prescription.Patient?.LastName}");
            await Task.CompletedTask;
        }

        public async Task NotifyPrescriptionUpdatedAsync(Prescription prescription)
        {
            // Simple implementation without SignalR
            Console.WriteLine($"[NOTIFICATION] Prescription updated: {prescription.MedicationName} for {prescription.Patient?.FirstName} {prescription.Patient?.LastName}");
            await Task.CompletedTask;
        }

        public async Task NotifyPrescriptionDeletedAsync(Prescription prescription)
        {
            // Simple implementation without SignalR
            Console.WriteLine($"[NOTIFICATION] Prescription deleted: {prescription.MedicationName} for {prescription.Patient?.FirstName} {prescription.Patient?.LastName}");
            await Task.CompletedTask;
        }

        public async Task NotifyScriptProcessedAsync(Prescription prescription)
        {
            // Simple implementation without SignalR
            Console.WriteLine($"[NOTIFICATION] Script processed: {prescription.MedicationName} for {prescription.Patient?.FirstName} {prescription.Patient?.LastName}");
            await Task.CompletedTask;
        }

        public async Task NotifyMedicationDispensedAsync(Prescription prescription)
        {
            // Simple implementation without SignalR
            Console.WriteLine($"[NOTIFICATION] Medication dispensed: {prescription.MedicationName} for {prescription.Patient?.FirstName} {prescription.Patient?.LastName}");
            await Task.CompletedTask;
        }
    }
}