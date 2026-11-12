using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HealthOps_Project.Models;
using System.Collections.Generic;
using System.Linq;

namespace HealthOps_Project.Services
{
    public class QuestPdfService : IPdfService
    {
        public QuestPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GeneratePrescriptionPdf(Prescription prescription)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .AlignCenter()
                        .Text("HealthOps Hospital Management System")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(10);

                            // Fixed property names to match your Prescription model
                            x.Item().Text($"Prescription #{prescription.PrescriptionId}").SemiBold().FontSize(16);
                            x.Item().Text($"Patient: {prescription.Patient?.FirstName} {prescription.Patient?.LastName}");
                            x.Item().Text($"Patient ID: {prescription.Patient?.SouthAfricanID}");
                            x.Item().Text($"Medication: {prescription.MedicationName}");
                            x.Item().Text($"Dosage: {prescription.Dosage}");
                            x.Item().Text($"Frequency: {prescription.Frequency}");
                            x.Item().Text($"Instructions: {prescription.Instructions ?? "No specific instructions provided"}");
                            x.Item().Text($"Quantity: {prescription.Quantity}");
                            x.Item().Text($"Start Date: {prescription.StartDate:yyyy-MM-dd}");
                            x.Item().Text($"End Date: {prescription.EndDate?.ToString("yyyy-MM-dd") ?? "Not specified"}");
                            x.Item().Text($"Status: {prescription.Status}");
                            x.Item().Text($"Priority: {prescription.Priority}");
                            x.Item().Text($"Prescribing Doctor: {prescription.PrescribingDoctor}");
                            x.Item().Text($"Created: {prescription.CreatedAt:yyyy-MM-dd HH:mm}");

                            if (!string.IsNullOrEmpty(prescription.Notes))
                            {
                                x.Item().Text($"Notes: {prescription.Notes}");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GeneratePrescriptionsReport(List<Prescription> prescriptions)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .AlignCenter()
                        .Text("Prescriptions Report - HealthOps HMS")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Summary Section
                            column.Item().Background(Colors.Grey.Lighten2).Padding(10).Column(summaryColumn =>
                            {
                                summaryColumn.Item().Text("Summary").SemiBold().FontSize(12);
                                summaryColumn.Item().Text($"Total Prescriptions: {prescriptions.Count}");
                                summaryColumn.Item().Text($"Active: {prescriptions.Count(p => p.Status == "Active")}");
                                summaryColumn.Item().Text($"Pending: {prescriptions.Count(p => p.Status == "Pending")}");
                                summaryColumn.Item().Text($"Processed: {prescriptions.Count(p => p.Status == "Processed")}");
                                summaryColumn.Item().Text($"Dispensed: {prescriptions.Count(p => p.Status == "Dispensed")}");
                                summaryColumn.Item().Text($"New: {prescriptions.Count(p => p.Status == "New")}");
                            });

                            // Table Section
                            column.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1); // ID
                                    columns.RelativeColumn(2); // Patient
                                    columns.RelativeColumn(2); // Medication
                                    columns.RelativeColumn(1.5f); // Dosage
                                    columns.RelativeColumn(1.5f); // Status
                                    columns.RelativeColumn(1.5f); // Priority
                                    columns.RelativeColumn(1.5f); // Date
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("ID").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Patient").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Medication").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Dosage").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Status").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Priority").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Created").FontColor(Colors.White).SemiBold();
                                });

                                foreach (var p in prescriptions)
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.PrescriptionId.ToString());
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{p.Patient?.FirstName} {p.Patient?.LastName}");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.MedicationName);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.Dosage);

                                    var statusColor = p.Status?.ToLower() switch
                                    {
                                        "active" => Colors.Green.Darken1,
                                        "pending" => Colors.Orange.Darken1,
                                        "processed" => Colors.Blue.Medium,
                                        "dispensed" => Colors.Purple.Medium,
                                        "new" => Colors.Teal.Medium,
                                        _ => Colors.Grey.Medium
                                    };

                                    var backgroundColor = p.Status?.ToLower() switch
                                    {
                                        "active" => Colors.Green.Lighten4,
                                        "pending" => Colors.Orange.Lighten4,
                                        "processed" => Colors.Blue.Lighten4,
                                        "dispensed" => Colors.Purple.Lighten4,
                                        "new" => Colors.Teal.Lighten4,
                                        _ => Colors.Grey.Lighten3
                                    };

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                         .Background(backgroundColor)
                                         .Text(p.Status).FontColor(statusColor).SemiBold();

                                    var priorityColor = p.Priority?.ToLower() switch
                                    {
                                        "urgent" or "high" => Colors.Red.Darken1,
                                        "medium" => Colors.Orange.Darken1,
                                        "routine" or "low" => Colors.Green.Darken1,
                                        _ => Colors.Grey.Medium
                                    };

                                    var priorityBackground = p.Priority?.ToLower() switch
                                    {
                                        "urgent" or "high" => Colors.Red.Lighten4,
                                        "medium" => Colors.Orange.Lighten4,
                                        "routine" or "low" => Colors.Green.Lighten4,
                                        _ => Colors.Grey.Lighten3
                                    };

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                         .Background(priorityBackground)
                                         .Text(p.Priority).FontColor(priorityColor).SemiBold();

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(p.CreatedAt.ToString("yyyy-MM-dd"));
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {System.DateTime.Now:yyyy-MM-dd HH:mm} | Total: {prescriptions.Count} prescriptions");
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateStocktakeReport(List<Stocktake> stocktakes)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .AlignCenter()
                        .Text("Stocktake Report - HealthOps HMS")
                        .SemiBold().FontSize(16).FontColor(Colors.Green.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Summary Section
                            column.Item().Background(Colors.Grey.Lighten2).Padding(10).Column(summaryColumn =>
                            {
                                summaryColumn.Item().Text("Stocktake Summary").SemiBold().FontSize(12);
                                summaryColumn.Item().Text($"Total Records: {stocktakes.Count}");
                                summaryColumn.Item().Text($"Total Quantity Counted: {stocktakes.Sum(s => s.QuantityCounted)}");
                                summaryColumn.Item().Text($"Unique Wards: {stocktakes.Select(s => s.WardName).Distinct().Count()}");
                                summaryColumn.Item().Text($"Unique Consumables: {stocktakes.Select(s => s.ConsumableId).Distinct().Count()}");
                            });

                            // Table Section
                            column.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1); // ID
                                    columns.RelativeColumn(2); // Consumable
                                    columns.RelativeColumn(1.5f); // Ward
                                    columns.RelativeColumn(1); // Quantity
                                    columns.RelativeColumn(1.5f); // Date
                                    columns.RelativeColumn(2); // Notes
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Green.Medium).Padding(5).Text("ID").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Consumable").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Ward").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Quantity").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Date").FontColor(Colors.White).SemiBold();
                                    header.Cell().Background(Colors.Green.Medium).Padding(5).Text("Notes").FontColor(Colors.White).SemiBold();
                                });

                                foreach (var s in stocktakes)
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(s.Id.ToString());
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(s.Consumable?.Name ?? "N/A");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(s.WardName);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(s.QuantityCounted.ToString());
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(s.DateTaken.ToString("yyyy-MM-dd"));
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(s.Notes ?? "No notes");
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {System.DateTime.Now:yyyy-MM-dd HH:mm} | Total: {stocktakes.Count} stocktakes");
                });
            });

            return document.GeneratePdf();
        }
    }
}