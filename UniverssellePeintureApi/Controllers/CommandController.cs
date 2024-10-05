using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Command = UniverssellePeintureApi.Model.Command;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public CommandController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommand([FromBody] AddCommandDto command)
        {
            var client = await _context.Clients.Include(c => c.Commerce).FirstOrDefaultAsync(c => c.Code == command.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }
            var portfeuille = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == client.Code);
            if (portfeuille == null)
            {
                throw new Exception("portfeuille not found");
            }

            var stock = await _context.Stocks.FirstOrDefaultAsync(c => c.ClientId == client.Id);
            int valeur_stock;
            int produit_vendu;
            if (stock == null)
            {
                valeur_stock = 0;
                produit_vendu = 0;
            }
            else
            {
                valeur_stock = stock.Quantity;
                produit_vendu = stock.Produit_Vendue;
            }

            int i = 0;
            try
            {
                foreach (var stockCommanddto in command.StockCommanddto)
                {
                    var produit = await _context.Produits.FirstOrDefaultAsync(c => c.Name == stockCommanddto.NameProduit);
                    if (produit == null)
                    {
                        throw new Exception("Produit not found");
                    }

                    if (i != 0)
                    {
                        var newCommand = new Command
                        {
                            produit = stockCommanddto.NameProduit,
                            Qte = stockCommanddto.Quantite,
                            poids = produit.poid,
                            TotalPoids = stockCommanddto.Quantite * stockCommanddto.poid,
                            Command_Date = command.Command_date
                        };
                        _context.Commands.Add(newCommand);

                    }
                    else
                    {
                        var newCommand = new Command
                        {
                            produit = stockCommanddto.NameProduit,
                            Qte = stockCommanddto.Quantite,
                            poids = produit.poid,
                            TotalPoids = stockCommanddto.Quantite * stockCommanddto.poid,
                            Code = command.CodeClient,
                            client = client.Respnsible_Name,
                            distrubitaire = client.Commerce.Nom,
                            A_Payer = portfeuille.PriceCompta - portfeuille.currentPrice,
                            Cach = command.cach,
                            phone = client.Phone_Number,
                            Zone = client.Zone,
                            Command_Date = command.Command_date,
                            Valeur_Stock = valeur_stock,
                            Produit_Vendue = produit_vendu
                        };
                        _context.Commands.Add(newCommand);
                    }
                    i++;
                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    // Log or return detailed information about the exception
                    return BadRequest($"Erreur lors de la sauvegarde des changements dans la base de données : {dbEx.Message} - {dbEx.InnerException?.Message}");
                }
            }
            catch (Exception ex)
            {
                // Capture de l'exception
                return BadRequest($"Erreur lors de la création de la commande : {ex.Message}");
            }


            return Ok("Command created successfully.");
        }

        [HttpGet]
        [Route("GenerateCommandPdf")]
        public async Task<IActionResult> GenerateCommandPdf(DateTime commandDate)
        {
            // Récupérer les commandes pour la date donnée
            var commands = await _context.Commands
                                         .Where(c => c.Command_Date.Value == commandDate.Date)
                                         .ToListAsync();

            if (commands == null || !commands.Any())
            {
                return NotFound("No commands found for the given date.");
            }

            // Générer le PDF
            var pdfBytes = CreateStyledPdf(commands, commandDate);

            // Retourner le PDF en tant que fichier téléchargeable
            return File(pdfBytes, "application/pdf", $"CommandDetails_{commandDate:yyyyMMdd}.pdf");
        }

        private byte[] CreateStyledPdf(List<Command> commands, DateTime commandDate)
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

                Paragraph title = new Paragraph("TABLEAU DE COMMANDES", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                Paragraph date = new Paragraph($"{commandDate:dd/MM/yyyy}", titleFont);
                date.Alignment = Element.ALIGN_CENTER;
                document.Add(date);

                document.Add(new Paragraph(" ")); // Espace

                // Création du tableau
                PdfPTable table = new PdfPTable(12) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f});

                // En-têtes du tableau
                table.AddCell(new PdfPCell(new Phrase("Distributeur", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Client", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Produits", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Qté", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Poids", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Total Poids", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Produit Vendu", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Zone", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Codes", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Cash", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Téléphone", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("A PAYER", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                // Remplir le tableau avec les données des commandes
                foreach (var command in commands)
                {
                    table.AddCell(new PdfPCell(new Phrase(command.distrubitaire ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.client ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.produit ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.Qte.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.poids.ToString("0.00"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.TotalPoids.ToString("0.00"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.Produit_Vendue.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.Zone ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.Code ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.Cach ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(command.phone ?? "", cellFont)));
                    if (command.A_Payer == 0)
                        table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                    else
                        table.AddCell(new PdfPCell(new Phrase(command.A_Payer.ToString() ?? "", cellFont)));
                }

                // Ajouter la ligne de total
                table.AddCell(new PdfPCell(new Phrase("TOTAL", headerFont)) { Colspan = 1, HorizontalAlignment = Element.ALIGN_RIGHT, BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase(commands.Sum(c => c.TotalPoids).ToString("0.00"), cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));

                document.Add(table);

                // Signatures
                document.Add(new Paragraph(" "));
                PdfPTable signatureTable = new PdfPTable(2) { WidthPercentage = 100 };
                signatureTable.SetWidths(new float[] { 50f, 50f });

                signatureTable.AddCell(new PdfPCell(new Phrase("Responsable Commercial", cellFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });
                signatureTable.AddCell(new PdfPCell(new Phrase("Directeur Général", cellFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });

                signatureTable.AddCell(new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER, FixedHeight = 50f }); // espace pour la signature
                signatureTable.AddCell(new PdfPCell(new Phrase(" ")) { Border = Rectangle.NO_BORDER, FixedHeight = 50f });

                document.Add(signatureTable);

                document.Close();
                return ms.ToArray();
            }

        }
    }
}
