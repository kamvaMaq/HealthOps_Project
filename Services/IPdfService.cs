using HealthOps_Project.Models;
using System.Collections.Generic;

namespace HealthOps_Project.Services
{
    public interface IPdfService
    {
        byte[] GeneratePrescriptionPdf(Prescription prescription);
        byte[] GeneratePrescriptionsReport(List<Prescription> prescriptions);
        byte[] GenerateStocktakeReport(List<Stocktake> stocktakes);
    }
}