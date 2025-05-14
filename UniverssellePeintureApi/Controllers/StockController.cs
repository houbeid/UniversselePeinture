using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        //[HttpDelete("deleteport")]
        //public async Task Supprimerport(string code)
        //{

        //    // Charger tous les clients avec leurs stocks et les StockProduits associés
        //    var port = _context.portFeuilleClients.FirstOrDefault(c => c.Code == code);
        //    // Parcourir chaque client
        //    _context.portFeuilleClients.Remove(port);

        //    // Sauvegarder les changements dans la base de données
        //    await _context.SaveChangesAsync();
        //}

        //[HttpDelete("deleteportfeuille")]
        //public async Task Supprimerportfeuille()
        //{

        //    // Charger tous les clients avec leurs stocks et les StockProduits associés
        //    var ports = _context.portFeuilleClients.ToList();
        //    // Parcourir chaque client
        //    foreach(var port in ports)
        //    {
        //        port.PriceCompta = 0;
        //        port.PricePayer = 0;
        //        port.currentPrice = 0;
        //        port.LastPrise = 0;
        //    }

        //    // Sauvegarder les changements dans la base de données
        //    await _context.SaveChangesAsync();
        //}

        [HttpDelete("deletrecette")]
        public async Task SupprimerRecette()
        {

            // Charger tous les clients avec leurs stocks et les StockProduits associés
            var ports = _context.HistoriqueRecettes.ToList();
            // Parcourir chaque client
            foreach (var port in ports)
            {
                _context.HistoriqueRecettes.Remove(port);
            }

            // Sauvegarder les changements dans la base de données
            await _context.SaveChangesAsync();
        }

        //[HttpPost("updatecurrentprice")]
        //public async Task updatecurrentprice(string code)
        //{
        //    var port = _context.portFeuilleClients.First(c => c.Code == code);
        //    port.currentPrice = 18700;
        //    port.PricePayer = port.PriceCompta - port.currentPrice;
        //    await _context.SaveChangesAsync();
        //}

        [HttpDelete("delete")]
        public async Task SupprimerTousLesStocks()
        {

            // Charger tous les clients avec leurs stocks et les StockProduits associés
            var clients = _context.Clients
                .Include(c => c.Stocks)
                .ThenInclude(s => s.StockProduits)
                .ToList();

            // Parcourir chaque client
            foreach (var client in clients)
            {
                // Parcourir chaque stock du client
                foreach (var stock in client.Stocks)
                {
                    // Supprimer tous les StockProduits associés à ce stock
                    _context.StockProduits.RemoveRange(stock.StockProduits);

                    // Supprimer le stock lui-même
                    _context.Stocks.Remove(stock);
                }
            }

            // Sauvegarder les changements dans la base de données
            await _context.SaveChangesAsync();
        }


         [Authorize]
        [HttpPost("Add")]
        public async Task AddStock([FromBody] AddStockdto addStockDto)
        {
            // Trouver le client par code
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == addStockDto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            var username = User?.Identity?.Name;
            var comercial = await _context.Commerces.FirstOrDefaultAsync(c => c.Nom == username);
            if (comercial == null)
            {
                throw new Exception("Comercial not found");
            }

            client.Delivery_Date = addStockDto.Delivery_date;

            // Chercher le stock pour le client
            var stock = await _context.Stocks.Include(s => s.StockProduits).FirstOrDefaultAsync(s => s.ClientId == client.Id);

            // Créer un nouveau stock si aucun n'existe
            if (stock == null)
            {
                stock = new Stock
                {
                    Quantity = 0,
                    Produit_Vendue = 0,
                    PrixDeVenteTotal = 0,
                    ClientId = client.Id
                };
                _context.Stocks.Add(stock);
                await _context.SaveChangesAsync();
            }

            // Mettre à jour la quantité totale du stock
            stock.Quantity += addStockDto.StockProduitdto.Sum(sp => sp.Quantite);
            decimal prix_actuell = 0;

            // Traiter les produits du stock
            foreach (var stockProduitDto in addStockDto.StockProduitdto)
            {
                var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitDto.NameProduit);
                if (produit == null)
                {
                    continue; // Si le produit n'existe pas, passer au suivant
                }

                produit.StockActuel += stockProduitDto.Quantite;
                produit.stock += stockProduitDto.Quantite;
                var stockTotalProduit = await _context.Produits.SumAsync(c => c.stock);
                stockTotalProduit += stockProduitDto.Quantite;
                produit.PourcentageProduit = Math.Round(((produit.stock - produit.StockActuel) / (double)produit.stock) * 100, 2);
                produit.PourcentageVente = Math.Round(((produit.stock - produit.StockActuel) / (double)stockTotalProduit) * 100, 2);


                var stockproduit = await _context.StockProduits.FirstOrDefaultAsync(p => p.ProduitId == produit.Id && p.StockId == stock.Id);
                if (stockproduit != null)
                {
                    stockproduit.prix_actuell += produit.PrixActuel * stockProduitDto.Quantite;
                    stockproduit.Quantite += stockProduitDto.Quantite;
                }
                else
                {
                    var newStockProduit = new StockProduit
                    {
                        StockId = stock.Id,
                        ProduitId = produit.Id,
                        prix_actuell = produit.PrixActuel * stockProduitDto.Quantite,
                        Quantite = stockProduitDto.Quantite
                    };
                    _context.StockProduits.Add(newStockProduit);
                }

                // Ajouter un historique pour chaque produit
                var historiqueProduit = new Historique
                {
                    NameProduit = produit.Name,
                    Quantite = stockProduitDto.Quantite,
                    Montant = produit.PrixActuel * stockProduitDto.Quantite,
                    Delivery_date = addStockDto.Delivery_date,
                    distributeur = comercial.Nom,
                    ClientId = client.Id
                };
                prix_actuell += stockProduitDto.Quantite * produit.PrixActuel;
                _context.Historiques.Add(historiqueProduit);
            }

            // Mettre à jour le portefeuille du client
            var portfeuilleClient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == addStockDto.CodeClient);
            if (portfeuilleClient == null)
            {
                throw new Exception("PortfeuilleClient not found");
            }

            portfeuilleClient.depot = addStockDto.Delivery_date;
            portfeuilleClient.phone = client.Phone_Number;
            portfeuilleClient.currentPrice += prix_actuell;

            // Enregistrer toutes les modifications en une seule fois
            await _context.SaveChangesAsync();
        }


       [Authorize]
        [HttpPost("PriseCompta")]
        public async Task AddPrisecompta(PriseComptaDto priseComptadto)
        {
            var portfeuilleClient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == priseComptadto.CodeClient);
            if (portfeuilleClient == null)
            {
                throw new Exception("PortfeuilleClient not found");
            }
            //decimal? prise = portfeuilleClient.LastPrise - priseComptadto.priseCompta;
            portfeuilleClient.PriceCompta = priseComptadto.priseCompta;
            portfeuilleClient.PricePayer = priseComptadto.priseCompta - portfeuilleClient.currentPrice;
            await _context.SaveChangesAsync();
        }

        [Authorize]
        [HttpPost("recette")]
        public async Task Addreceitte(AddRecetteDto priseComptadto)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == priseComptadto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }
            //var stock = await _context.Stocks.Include(s => s.StockProduits).FirstOrDefaultAsync(s => s.ClientId == client.Id);
            //if (stock == null)
            //{
            //    throw new Exception("Stock not found for this client");
            //}
            var portfeuilleClient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == priseComptadto.CodeClient);
            if (portfeuilleClient == null)
            {
                throw new Exception("PortfeuilleClient not found");
            }
            portfeuilleClient.PriceCompta -= priseComptadto.priseCompta;
            //if (portfeuilleClient.PriceCompta == 0)
            //{
            //    foreach (var stockproduit in stock.StockProduits)
            //    {
            //        // Supprimer tous les StockProduits associés à ce stock
            //        _context.StockProduits.RemoveRange(stockproduit);

            //        // Supprimer le stock lui-même
            //    }
            //    _context.Stocks.Remove(stock);
            //    portfeuilleClient.currentPrice = 0;
            //    portfeuilleClient.PricePayer = 0;
            //}
            //else
            portfeuilleClient.PricePayer -= priseComptadto.priseCompta;
            var recette = new HistoriqueRecette
            {
                Date = priseComptadto.Recette_Date,
                Name = client.Respnsible_Name,
                CodeClient = priseComptadto.CodeClient,
                Recette = priseComptadto.priseCompta
            };
            _context.HistoriqueRecettes.Add(recette);
            await _context.SaveChangesAsync();
        }

        [HttpGet("RecetteHistorique")]
        public async Task<List<HistoriqueRecetteResponse>> GetHistoriqueRecette(DateTime date)
        {
            var recettes = await _context.HistoriqueRecettes
                .Where(c => c.Date.Date == date.Date)
                .Select(c => new HistoriqueRecetteResponse
                {
                    Date = c.Date,
                    Name = c.Name,
                    CodeClient = c.CodeClient,
                    recette = c.Recette
                })
                .ToListAsync();

            return recettes;
        }


        //[Authorize]
        //[HttpPost("update")]
        //public async Task UpdateStockAsync(UpdateStockdto updateStockDto)
        //{
        //    // Trouver le client par code
        //    var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == updateStockDto.CodeClient);
        //    if (client == null)
        //    {
        //        throw new Exception("Client not found");
        //    }

        //    client.Visit_Date = updateStockDto.Visit_date;
        //    client.Description = updateStockDto.Description;

        //    // Trouver le stock associé à ce client
        //    var stock = await _context.Stocks.Include(s => s.StockProduits).FirstOrDefaultAsync(s => s.ClientId == client.Id);
        //    if (stock == null)
        //    {
        //        throw new Exception("Stock not found for this client");
        //    }

        //    decimal prixDeVenteTotal = 0;
        //    int stockcount = 0;
        //    decimal prix_actuell = 0;
        //    // Parcourir les produits envoyés pour mise à jour
        //    foreach (var stockProduitDto in updateStockDto.StockProduitdto)
        //    {
        //        var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitDto.NameProduit);
        //        if (produit == null)
        //        {
        //            // Si un produit n'est pas trouvé, passer au suivant
        //            continue;
        //        }

        //        var stockproduit = await _context.StockProduits.FirstOrDefaultAsync(p => p.ProduitId == produit.Id && p.StockId == stock.Id);
        //        if (stockproduit == null)
        //        {
        //            // Créer un nouveau StockProduit si non trouvé
        //            throw new Exception("vous n'avez pas un stock pour cette produit");
        //        }
        //        else
        //        {
        //            // Mettre à jour la quantité de stock actuel du produit
        //            stockcount += stockproduit.Quantite - stockProduitDto.Quantite;
        //            produit.StockActuel -= (stockproduit.Quantite - stockProduitDto.Quantite);
        //            stockproduit.Quantite = stockProduitDto.Quantite;
        //            stockproduit.prix_actuell = produit.PrixActuel * stockProduitDto.Quantite;
        //        }

        //        // Calculer les pourcentages de vente
        //        var stockTotalProduit = await _context.Produits.SumAsync(c => c.stock);
        //        produit.PourcentageProduit = Math.Round(((produit.stock - produit.StockActuel) / (double)produit.stock) * 100, 2);
        //        produit.PourcentageVente = Math.Round(((produit.stock - produit.StockActuel) / (double)stockTotalProduit) * 100, 2);

        //        // Calculer le prix de vente
        //        stockproduit.prix_vent = produit.PrixActuel * stockProduitDto.Quantite;
        //        prixDeVenteTotal += stockproduit.prix_vent;
        //        prix_actuell += stockProduitDto.Quantite * produit.PrixActuel;

        //        // Marquer les entités comme modifiées
        //        _context.Entry(produit).State = EntityState.Modified;
        //        _context.Entry(stockproduit).State = EntityState.Modified;
        //    }

        //    // Mise à jour des informations de stock
        //    stock.Produit_Vendue += stock.Quantity - updateStockDto.StockProduitdto.Sum(sp => sp.Quantite);
        //    stock.Quantity -= stockcount;
        //    stock.PrixDeVenteTotal = prixDeVenteTotal;

        //    // Mise à jour du portefeuille client
        //    var portefeuilleClient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == updateStockDto.CodeClient);
        //    if (portefeuilleClient == null)
        //    {
        //        throw new Exception("PortFeuilleClient not found");
        //    }

        //    portefeuilleClient.visit = updateStockDto.Visit_date;
        //    portefeuilleClient.Date_RDV = updateStockDto.Description;
        //    portefeuilleClient.currentPrice -= stock.PrixDeVenteTotal;
        //    portefeuilleClient.PricePayer += stock.PrixDeVenteTotal;

        //    // Sauvegarder les modifications en une seule fois
        //    await _context.SaveChangesAsync();
        //}

       // [Authorize]   
        [HttpPost("update")]
        public async Task UpdateStockAsync(UpdateStockdto stockDto)
        {
            // Trouver le client par code
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Code == stockDto.CodeClient);
            if (client == null)
            {
                throw new Exception("Client not found");
            }

            // Mettre à jour les informations générales du client
           // client.Delivery_Date = stockDto.Delivery_date ?? client.Delivery_Date;
            client.Visit_Date = stockDto.Visit_date;
            client.Description = stockDto.Description ?? client.Description;

            // Trouver ou créer le stock associé à ce client
            var stock = await _context.Stocks.Include(s => s.StockProduits).ThenInclude(s => s.Produit).FirstOrDefaultAsync(s => s.ClientId == client.Id);
            if (stock == null)
            {
                // Si le stock n'existe pas, on le crée
                stock = new Stock
                {
                    Quantity = 0,
                    Produit_Vendue = 0,
                    PrixDeVenteTotal = 0,
                    ClientId = client.Id
                };
                _context.Stocks.Add(stock);
                await _context.SaveChangesAsync();
            }

            //decimal prixDeVenteTotal = 0;
            decimal prix_actuell = 0;
           // int stockcount = 0;

            // Parcourir les produits dans le DTO
            foreach (var stockProduitDto in stockDto.StockProduitdto)
            {
                var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitDto.NameProduit);
                if (produit == null)
                {
                    // Si un produit n'existe pas, passer au suivant
                    continue;
                }

                // Chercher ou créer le lien entre le stock et le produit
                var stockproduit = await _context.StockProduits.FirstOrDefaultAsync(p => p.ProduitId == produit.Id && p.StockId == stock.Id);
                if (stockproduit == null)
                {
                    // Ajout d'un nouveau produit au stock
                    stockproduit = new StockProduit
                    {
                        StockId = stock.Id,
                        ProduitId = produit.Id,
                        prix_actuell = produit.PrixActuel * stockProduitDto.Quantite,
                        Quantite = stockProduitDto.Quantite
                    };
                    _context.StockProduits.Add(stockproduit);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    // Mise à jour des informations du produit dans le stock
                   // stockcount += stockproduit.Quantite - stockProduitDto.Quantite;
                    produit.StockActuel -= (stockproduit.Quantite - stockProduitDto.Quantite);
                    stockproduit.Quantite = stockProduitDto.Quantite;
                    stockproduit.prix_actuell = produit.PrixActuel * stockProduitDto.Quantite;
                }

                // Mettre à jour les informations du produit
                //produit.StockActuel += stockProduitDto.Quantite;
               // produit.stock += stockProduitDto.Quantite;

                var stockTotalProduit = await _context.Produits.SumAsync(c => c.stock);
               // produit.PourcentageProduit = Math.Round(((produit.stock - produit.StockActuel) / (double)produit.stock) * 100, 2);
                //produit.PourcentageVente = Math.Round(((produit.stock - produit.StockActuel) / (double)stockTotalProduit) * 100, 2);

                // Calcul du prix de vente total
               // stockproduit.prix_vent = produit.PrixActuel * stockProduitDto.Quantite;
               // prixDeVenteTotal += stockproduit.prix_vent;
                prix_actuell += stockProduitDto.Quantite * produit.PrixActuel;

               // _context.Entry(produit).State = EntityState.Modified;
                _context.Entry(stockproduit).State = EntityState.Modified;
            }
            foreach (var stockProduits in stock.StockProduits)
            {
                var stcokproduit = stockDto.StockProduitdto.FirstOrDefault(c => c.NameProduit == stockProduits.Produit.Name);
                if (stcokproduit == null)
                {
                    _context.StockProduits.Remove(stockProduits);
                    await _context.SaveChangesAsync();
                }
            }

            // Mettre à jour les informations du stock
           // stock.Produit_Vendue += stock.Quantity - stockDto.StockProduitdto.Sum(sp => sp.Quantite);
            stock.Quantity = stockDto.StockProduitdto.Sum(sp => sp.Quantite);
           // stock.PrixDeVenteTotal = prixDeVenteTotal;

            // Mettre à jour le portefeuille client
            var portefeuilleClient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == stockDto.CodeClient);
            if (portefeuilleClient == null)
            {
                throw new Exception("PortFeuilleClient not found");
            }

           // portefeuilleClient.depot = stockDto.Delivery_date ?? portefeuilleClient.depot;
            portefeuilleClient.visit = stockDto.Visit_date;
            portefeuilleClient.Date_RDV = stockDto.Description ?? portefeuilleClient.Date_RDV;
            portefeuilleClient.currentPrice = prix_actuell;
            //portefeuilleClient.PricePayer += stock.PrixDeVenteTotal;

            // Sauvegarder toutes les modifications
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Diagnostique : afficher les entités concernées
                foreach (var entry in ex.Entries)
                {
                    Console.WriteLine($"Concurrency conflict on entity: {entry.Entity.GetType().Name}");
                }

                throw new Exception("Une erreur de concurrence est survenue. Veuillez réessayer.");
            }

        }



        [Authorize]
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

        //[HttpGet("UpdateProduits")]
        //public void ModifierPrixParNom(string nomProduit, decimal nouveauPrix)
        //{
            
        //        var produit = _context.Produits.FirstOrDefault(p => p.Name == nomProduit);
        //        if (produit != null)
        //        {
        //            produit.PrixActuel = nouveauPrix;
        //            _context.SaveChanges();
        //        }
        //        else
        //        {
        //            throw new Exception("Produit non trouvé.");
        //        }
            
        //}
    }
}
