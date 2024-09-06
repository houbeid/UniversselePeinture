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
    public class CommercesController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public CommercesController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommerce([FromBody] AddCommerceDto commerceDto)
        {
            var comercial = await _context.Commerces
            .Where(c => c.Telephone == commerceDto.Telephone)
                .FirstOrDefaultAsync();
            if (comercial != null)
            {
                ModelState.AddModelError("Commercial: ", "Commercial exist deja!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var commerce = new Commerce
            {
                Nom = commerceDto.Nom,
                Telephone = commerceDto.Telephone
            };

            _context.Commerces.Add(commerce);
            await _context.SaveChangesAsync();

            return Ok("Client created successfully.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommerceById(int id)
        {
            var commerce = await _context.Commerces.FirstOrDefaultAsync(c => c.Id == id);
            if (commerce == null)
            {
                return NotFound();
            }

            return Ok(commerce);
        }

        


        [HttpGet]
        public async Task<List<ComerceResponse>> GetAllCommerces()
        {
            try
            {
                var commerces = await _context.Commerces
                .Select(c => new ComerceResponse
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    Telephone = c.Telephone
                })
                .ToListAsync();

                if (commerces == null || !commerces.Any())
                {
                    throw new ArgumentException("Invalid Comerce", nameof(Commerce));
                }

                return commerces;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                throw new ArgumentException("Invalid Comerce", nameof(Commerce));
            }
        }

        [HttpGet]
        [Route("GenerateRecettePdf")]
        public async Task<IActionResult> GenerateRecapPdf(int idcomerce)
        {
            var comercial = await _context.Commerces
            .Where(c => c.Id == idcomerce)
                .FirstOrDefaultAsync();

            var reciette  = await _context.portFeuilleClients
            .Where(c => c.CommercantId == idcomerce).ToListAsync();
            if (reciette  == null || !reciette.Any())
            {
                return NotFound("No reciette found for the comercial.");
            }

            var pdfBytes = CreateStyledPdf(reciette, comercial.Nom);
            return File(pdfBytes, "application/pdf", $"CommandDetails_{DateTime.Now:yyyyMMdd}.pdf");

        }

        private byte[] CreateStyledPdf(List<PortFeuilleClient> recettes, string name)
        {
            using (var ms = new MemoryStream())
            {
                DateTime Date = DateTime.Now;
                Document document = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                // Titre du tableau
                Font titleFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                Font headerFont = FontFactory.GetFont("Arial", 10, Font.BOLD);
                Font cellFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

                Paragraph title = new Paragraph("Recette " + name, titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                Paragraph date = new Paragraph($"{Date:dd/MM/yyyy}", titleFont);
                date.Alignment = Element.ALIGN_CENTER;
                document.Add(date);

                document.Add(new Paragraph(" ")); // Espace

                // Création du tableau
                PdfPTable table = new PdfPTable(10) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f });

                // En-têtes du tableau
                table.AddCell(new PdfPCell(new Phrase("Date de visit", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Date de LVR", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("codes", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("client", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Zone", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Tel", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Balances de tiers", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Produit vendus", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Recette du jour", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Date RDV", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                //table.AddCell(new PdfPCell(new Phrase("A PAYER", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                // Remplir le tableau avec les données des commandes
                foreach (var recette in recettes)
                {
                    var facture = _context.Factures.Where(c => c.Code == recette.Code && c.date == recette.visit).FirstOrDefault();

                    table.AddCell(new PdfPCell(new Phrase(recette.visit.ToString() ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(recette.depot.ToString() ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(recette.Code ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(recette.name ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(recette.zone ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(recette.phone ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(recette.PriceCompta.ToString() ?? "", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(recette.PricePayer.ToString() ?? "", cellFont)));
                    if (facture != null)
                        table.AddCell(new PdfPCell(new Phrase(facture.Montant.ToString() ?? "", cellFont)));
                    else
                        table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(recette.Date_RDV ?? "", cellFont)));

                }

                // Ajouter la ligne de total
                table.AddCell(new PdfPCell(new Phrase("TOTAL", headerFont)) { Colspan = 6, HorizontalAlignment = Element.ALIGN_RIGHT, BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase(recettes.Sum(c => c.PriceCompta).ToString(), cellFont)));
                table.AddCell(new PdfPCell(new Phrase(recettes.Sum(c => c.PricePayer).ToString(), cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                table.AddCell(new PdfPCell(new Phrase("", cellFont)));

                document.Add(table);

                document.Close();
                return ms.ToArray();
            }
        }

        [HttpGet]
        [Route("GenerateRecapPdf")]
        public async Task<IActionResult> GenerateStockPdf(int idcomerce)
        {
            var comercial = await _context.Commerces
            .Where(c => c.Id == idcomerce)
                .FirstOrDefaultAsync();

            var clients = await _context.Clients
            .Where(c => c.CommercantId == idcomerce).ToListAsync();
            if (clients == null || !clients.Any())
            {
                return NotFound("No reciette found for the comercial.");
            }

            var pdfBytes = CreateStockdPdf(clients, comercial.Nom);
            return File(pdfBytes, "application/pdf", $"STOCK CLIENT {comercial.Nom}.pdf");

        }

        private byte[] CreateStockdPdf(List<Client> clients, string name)
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

                Paragraph title = new Paragraph("FICHE DE SUIVI MOUVEMENT QUINCAILLERIE");
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(new Paragraph(" ")); // Espace

                var produits = _context.Produits.ToList();

                foreach (var client in clients)
                {
                    // Création du tableau

                    var stock = _context.Stocks.Where(c => c.ClientId == client.Id).FirstOrDefault();
                    if (stock != null)
                    {
                        var portfeuille = _context.portFeuilleClients.Where(c => c.Code == client.Code).FirstOrDefault();
                        //if (portfeuille == null)
                        //{
                        //    return NotFound("No reciette found for the comercial.");
                        //}
                        PdfPTable table = new PdfPTable(11) { WidthPercentage = 100 };
                        table.SetWidths(new float[] { 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f, 15f });

                        // En-têtes du tableau
                        table.AddCell(new PdfPCell(new Phrase("Depot", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("Visite", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("Code Client", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("client", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("Zone", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("Produit", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("STK Actuell", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("Prix Vent", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("Valeur Actuel", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("Montant Compta", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        table.AddCell(new PdfPCell(new Phrase("Montant a payer", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                        decimal valeur_actuelle = 0;

                        var premierProduit = produits.FirstOrDefault();
                        var stcokPremiereproduit = _context.StockProduits.Where(c => c.StockId == stock.Id && c.ProduitId == premierProduit.Id).FirstOrDefault();

                        table.AddCell(new PdfPCell(new Phrase(client.Delivery_Date.ToString() ?? "", cellFont)));
                        table.AddCell(new PdfPCell(new Phrase(client.Visit_Date.ToString() ?? "", cellFont)));
                        table.AddCell(new PdfPCell(new Phrase(client.Code ?? "", cellFont)));
                        table.AddCell(new PdfPCell(new Phrase(client.Respnsible_Name ?? "", cellFont)));
                        table.AddCell(new PdfPCell(new Phrase(client.Zone ?? "", cellFont)));
                        table.AddCell(new PdfPCell(new Phrase(premierProduit.Name ?? "", cellFont)));
                        if (stcokPremiereproduit != null)
                            table.AddCell(new PdfPCell(new Phrase(stcokPremiereproduit.Quantite.ToString(), cellFont)));
                        else
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                        table.AddCell(new PdfPCell(new Phrase(premierProduit.PrixActuel.ToString(), cellFont)));
                        if (stcokPremiereproduit != null)
                            table.AddCell(new PdfPCell(new Phrase((stcokPremiereproduit.Quantite * premierProduit.PrixActuel).ToString(), cellFont)));
                        else
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                        table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                        table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                        if (stcokPremiereproduit != null)
                            valeur_actuelle += stcokPremiereproduit.Quantite * premierProduit.PrixActuel;
                        // Remplir le tableau avec les données des commandes
                        foreach (var produit in produits.Skip(1))
                        {
                            var stcokproduit = _context.StockProduits.Where(c => c.StockId == stock.Id && c.ProduitId == produit.Id).FirstOrDefault();

                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase(produit.Name ?? "", cellFont)));
                            if (stcokproduit != null)
                                table.AddCell(new PdfPCell(new Phrase(stcokproduit.Quantite.ToString(), cellFont)));
                            else
                                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase(produit.PrixActuel.ToString(), cellFont)));
                            if (stcokproduit != null)
                                table.AddCell(new PdfPCell(new Phrase((stcokproduit.Quantite * produit.PrixActuel).ToString(), cellFont)));
                            else
                                table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            if (stcokproduit != null)
                                valeur_actuelle += stcokproduit.Quantite * produit.PrixActuel;

                        }

                        // Ajouter la ligne de total
                        table.AddCell(new PdfPCell(new Phrase("TOTAL Valeur", headerFont)) { Colspan = 8, HorizontalAlignment = Element.ALIGN_RIGHT});
                        table.AddCell(new PdfPCell(new Phrase(valeur_actuelle.ToString(), cellFont)));
                        if (portfeuille != null)
                        {
                            table.AddCell(new PdfPCell(new Phrase(portfeuille.PriceCompta.ToString() ?? "", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase((portfeuille.PriceCompta - valeur_actuelle).ToString(), cellFont)));
                        }
                        else
                        {
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                            table.AddCell(new PdfPCell(new Phrase("", cellFont)));
                        }

                        document.Add(table);
                    }
                }

                document.Close();
                return ms.ToArray();
            }
        }
    }
}
