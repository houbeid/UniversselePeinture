using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.DTO.Response;
using UniverssellePeintureApi.Model;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public StockController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        public async Task AddStock([FromBody]  AddStockdto addStockDto)
        {
            // Trouver le client par code
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == addStockDto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            var comercial = await _context.Commerces.FirstOrDefaultAsync(c => c.Id == client.CommercantId);
            if (comercial == null)
            {
                throw new Exception("Comercial not found");
            }
            client.Delivery_Date = addStockDto.Delivery_date;

            var stock = await _context.Stocks.Include(s => s.StockProduits).FirstOrDefaultAsync(s => s.ClientId == client.Id);
            if (stock != null)
            {
                stock.Quantity += addStockDto.StockProduitdto.Sum(sp => sp.Quantite);
                foreach (var stockProduitdto in addStockDto.StockProduitdto)
                {
                    var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitdto.NameProduit);
                    if (produit != null)
                    {
                        produit.StockActuel += stockProduitdto.Quantite;
                        produit.stock += stockProduitdto.Quantite;
                        var stockproduit = await _context.StockProduits.FirstOrDefaultAsync(p => p.ProduitId == produit.Id);
                        if (stockproduit != null)
                        {
                            stockproduit.prix_actuell += produit.PrixActuel * stockProduitdto.Quantite;
                            stockproduit.Quantite += stockProduitdto.Quantite;
                            var historiqueProduit = new Historique
                            {
                                NameProduit = produit.Name,
                                Quantite = stockProduitdto.Quantite,
                                Montant = produit.PrixActuel * stockProduitdto.Quantite,
                                Delivery_date = addStockDto.Delivery_date,
                                distributeur = comercial.Nom,
                                ClientId = client.Id
                            };
                            _context.Historiques.Add(historiqueProduit);
                            await _context.SaveChangesAsync();
                        }

                        else
                        {
                            var stockProduit = new StockProduit
                            {
                                StockId = stock.Id,
                                ProduitId = produit.Id,
                                prix_actuell = produit.PrixActuel * stockProduitdto.Quantite,
                                Quantite = stockProduitdto.Quantite
                            };

                            _context.StockProduits.Add(stockProduit);
                            await _context.SaveChangesAsync();

                            var historiqueProduit = new Historique
                            {
                                NameProduit = produit.Name,
                                Quantite = stockProduitdto.Quantite,
                                Montant = produit.PrixActuel * stockProduitdto.Quantite,
                                Delivery_date = addStockDto.Delivery_date,
                                distributeur = comercial.Nom,
                                ClientId = client.Id
                            };
                            _context.Historiques.Add(historiqueProduit);
                            await _context.SaveChangesAsync();
                        }
                    }
   
                }
            }
            else
            {
                // Créer un nouveau stock
                var newStock = new Stock
                {
                    Quantity = addStockDto.StockProduitdto.Sum(sp => sp.Quantite),
                    PrixDeVenteTotal = 0, // Vous pouvez calculer le prix total si nécessaire
                    ClientId = client.Id
                };

                _context.Stocks.Add(newStock);
                await _context.SaveChangesAsync();

                // Ajouter les produits au stock et mettre à jour les pourcentages de vente
                foreach (var stockProduitDto in addStockDto.StockProduitdto)
                {
                    var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitDto.NameProduit);
                    if (produit != null)
                    {
                        produit.StockActuel += stockProduitDto.Quantite;
                        produit.stock += stockProduitDto.Quantite;


                        var stockProduit = new StockProduit
                        {
                            StockId = newStock.Id,
                            ProduitId = produit.Id,
                            prix_actuell = produit.PrixActuel * stockProduitDto.Quantite,
                            Quantite = stockProduitDto.Quantite
                        };

                        _context.StockProduits.Add(stockProduit);
                        await _context.SaveChangesAsync();
                        var historiqueProduit = new Historique
                        {
                            NameProduit = produit.Name,
                            Quantite = stockProduitDto.Quantite,
                            Montant = produit.PrixActuel * stockProduitDto.Quantite,
                            Delivery_date = addStockDto.Delivery_date,
                            distributeur = comercial.Nom,
                            ClientId = client.Id
                        };
                        _context.Historiques.Add(historiqueProduit);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            var portfeuilleClient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == addStockDto.CodeClient);
            if (portfeuilleClient == null)
            {
                throw new Exception("PortfeuilleClient not found");
            }
            //portfeuilleClient.PriceCompta += addStockDto.PriceCompta;
            //portfeuilleClient.currentPrice += addStockDto.PriceCompta;
            portfeuilleClient.depot = addStockDto.Delivery_date;
            portfeuilleClient.phone = client.Phone_Number;
            await _context.SaveChangesAsync();
        }

        [HttpPost("PriseCompta")]
        public async Task AddPrisecompta(PriseComptaDto priseComptadto)
        {
            var portfeuilleClient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == priseComptadto.CodeClient);
            if (portfeuilleClient == null)
            {
                throw new Exception("PortfeuilleClient not found");
            }
            portfeuilleClient.PriceCompta += priseComptadto.priseCompta;
            portfeuilleClient.currentPrice += priseComptadto.priseCompta;
            await _context.SaveChangesAsync();
        }

        [HttpPost("recette")]
        public async Task Addreceitte(PriseComptaDto priseComptadto)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == priseComptadto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }
            var stock = await _context.Stocks.Include(s => s.StockProduits).FirstOrDefaultAsync(s => s.ClientId == client.Id);
            if (stock == null)
            {
                throw new Exception("Stock not found for this client");
            }
            var portfeuilleClient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == priseComptadto.CodeClient);
            if (portfeuilleClient == null)
            {
                throw new Exception("PortfeuilleClient not found");
            }
            portfeuilleClient.PriceCompta -= priseComptadto.priseCompta;
            portfeuilleClient.PricePayer = stock.PrixDeVenteTotal - priseComptadto.priseCompta;
            portfeuilleClient.currentPrice = portfeuilleClient.PriceCompta - stock.PrixDeVenteTotal;
            await _context.SaveChangesAsync();
        }

        [HttpPost("update")]
        public async Task UpdateStockAsync(UpdateStockdto updateStockDto)
        {
            // Trouver le client par code
           
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == updateStockDto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }


            client.Visit_Date = updateStockDto.Visit_date;
            client.Description = updateStockDto.Description;

            // Trouver le stock associé à ce client
            var stock = await _context.Stocks.Include(s => s.StockProduits).FirstOrDefaultAsync(s => s.ClientId == client.Id);
            if (stock == null)
            {
                throw new Exception("Stock not found for this client");
            }
            stock.PrixDeVenteTotal = 0;

            // Mettre à jour les informations du stock
            //stock.PrixDeVenteTotal = updateStockDto.PrixDeVenteTotal;
            //stock.Description = updateStockDto.Description;



            // Supprimer les anciennes associations de produits
            // _context.StockProduits.RemoveRange(stock.StockProduits);

            // Ajouter les nouvelles associations de produits et mettre à jour les informations de stock
            var stockProduit = await _context.Produits.SumAsync(c => c.stock);
            foreach (var stockProduitDto in updateStockDto.StockProduitdto)
            {
                var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitDto.NameProduit);
                if (produit != null)
                {
                    var stockproduit = await _context.StockProduits.FirstOrDefaultAsync(p => p.ProduitId == produit.Id && p.StockId == stock.Id);
                    if (stockproduit == null)
                    {
                        throw new Exception("Stock not found for this client");
                    }
                    // Mettre à jour le stock actuel du produit
                    produit.StockActuel -= (stockproduit.Quantite - stockProduitDto.Quantite);

                    // Calculer le pourcentage de vente
                    produit.PourcentageProduit = Math.Round(((produit.stock - produit.StockActuel) / (double)produit.stock) * 100, 2);
                    produit.PourcentageVente = Math.Round(((produit.stock - produit.StockActuel) / (double)stockProduit) * 100, 2);

                    // Mettre à jour les informations du stockProduit existant
                    stockproduit.Quantite = stockProduitDto.Quantite;
                    stockproduit.prix_vent = ((stockproduit.Quantite - stockProduitDto.Quantite) * produit.PrixActuel);
                    stockproduit.prix_actuell = (stockproduit.prix_actuell - ((stockproduit.Quantite - stockProduitDto.Quantite) * produit.PrixActuel));
                                                                         
                    // Mettre à jour le prix de vente total du stock
                    stock.PrixDeVenteTotal += stockproduit.prix_vent;

                    // Marquer les entités comme modifiées
                    _context.Entry(produit).State = EntityState.Modified;
                    _context.Entry(stockproduit).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                }
            }
            stock.Quantity = updateStockDto.StockProduitdto.Sum(sp => sp.Quantite);
            var Portfeiulleclient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == updateStockDto.CodeClient);
            if (Portfeiulleclient == null)
            {
                throw new Exception("PortFeuilleClient not found");
            }
            
            Portfeiulleclient.visit = updateStockDto.Visit_date;
            Portfeiulleclient.Date_RDV = updateStockDto.Description;
            await _context.SaveChangesAsync();
        }

        
        [HttpGet("Produits")]
        public async Task<List<StockProduitDto>> getProduit()
        {
            

            var Produits = await _context.Produits
                   .Select(c => new StockProduitDto
                   {
                       Name = c.Name
                       
                   })
    .ToListAsync();
            return Produits;
        }
    }
}
