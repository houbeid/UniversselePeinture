using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.DTO.Response;
using UniverssellePeintureApi.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactureController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public FactureController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> CreateFacture([FromBody] AddFactureDto factureDto)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == factureDto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }
            var facture = new Facture
            {
                date = factureDto.date,
                Adress = client.Zone,
                Code = factureDto.CodeClient,
                facture = factureDto.Facture,
                client = client.Respnsible_Name,
                Montant = factureDto.Montant,
                distributeur = factureDto.distribiteur,
                ClientId = client.Id
            };
            _context.Factures.Add(facture);
            await _context.SaveChangesAsync();
            return Ok("facture created successfully.");
        }

        [HttpGet]
        public async Task<List<FactureResponse>> GetFacture()
        {
            try
            {
                var Facture = await _context.Factures
                    .Where(c => c.date == DateTime.Today)
                    .Select(c => new FactureResponse
                    {
                        date = c.date,
                        Adress = c.Adress,
                        Code = c.Code,
                        client = c.client,
                        facture = c.facture,
                        Montant = c.Montant,
                        distributeur = c.distributeur
                    })
     .ToListAsync();

                if (Facture == null || !Facture.Any())
                {
                    throw new ArgumentException("Invalid facture", nameof(Commerce));
                }

                return Facture;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                throw new ArgumentException("Invalid Facture", nameof(Commerce));
            }
        }

        [HttpGet]
        [Route("GenerateFacturePdf")]
        public async Task<IActionResult> GenerateCommandPdf(DateTime FactureDate)
        {
            // Récupérer les commandes pour la date donnée
            var factures = await _context.Factures
                                         .Where(c => c.date.Day == FactureDate.Date.Day)
                                         .ToListAsync();

            if (factures == null || !factures.Any())
            {
                return NotFound("No commands found for the given date.");
            }

            // Générer le PDF
            var pdfBytes = CreateStyledPdf(factures, FactureDate);

            // Retourner le PDF en tant que fichier téléchargeable
            return File(pdfBytes, "application/pdf", $"Etats_{FactureDate:yyyyMMdd}.pdf");
        }

        private byte[] CreateStyledPdf(List<Facture> factures, DateTime FactureDate)
        {
            using (var ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                // Titre du tableau
                Font titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                Font headerFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
                Font cellFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

                Paragraph title = new Paragraph("SUIVI DES FACTURES", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                Paragraph date = new Paragraph($"{FactureDate:dd/MM/yyyy}", titleFont);
                date.Alignment = Element.ALIGN_CENTER;
                document.Add(date);

                document.Add(new Paragraph(" ")); // Espace

                // Création du tableau
                PdfPTable table = new PdfPTable(7) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 15f, 15f, 15f, 15f, 15f, 15f, 15f });

                // En-têtes du tableau
                table.AddCell(new PdfPCell(new Phrase("Date", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Client", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Adresse", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("code", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("numero de facture", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Montant", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Distrubuteur", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                // Remplir le tableau avec les données des commandes
                foreach (var facture in factures)
                {
                    table.AddCell(new PdfPCell(new Phrase(facture.date.ToString() ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(facture.client ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(facture.Adress ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(facture.Code, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(facture.facture, cellFont)));
                    if (facture.Montant == 0)
                        table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                    else
                        table.AddCell(new PdfPCell(new Phrase(facture.Montant.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(facture.distributeur ?? "", cellFont)));
                }

                document.Add(table);

                // Signatures
                document.Add(new Paragraph(" "));
                PdfPTable signatureTable = new PdfPTable(3) { WidthPercentage = 100 };
                signatureTable.SetWidths(new float[] { 50f, 50f, 50f });

                signatureTable.AddCell(new PdfPCell(new Phrase("Responsable Commercial", cellFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });
                signatureTable.AddCell(new PdfPCell(new Phrase("comptable", cellFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                signatureTable.AddCell(new PdfPCell(new Phrase("Directeur Général", cellFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });

                signatureTable.AddCell(new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER, FixedHeight = 50f }); // espace pour la signature
                signatureTable.AddCell(new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER, FixedHeight = 50f });
                signatureTable.AddCell(new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER, FixedHeight = 50f });

                document.Add(signatureTable);

                document.Close();
                return ms.ToArray();
            }
        }
    }
}
