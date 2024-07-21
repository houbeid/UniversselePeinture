﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverssellePeintureApi.DTO;
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
                    }
                }
            }

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
            foreach (var stockProduitDto in updateStockDto.StockProduitdto)
            {
                var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Name == stockProduitDto.NameProduit);
                if (produit != null)
                {
                    var stockproduit = await _context.StockProduits.FirstOrDefaultAsync(p => p.ProduitId == produit.Id);
                    if (stockproduit == null)
                    {
                        throw new Exception("Stock not found for this client");
                    }
                    produit.StockActuel -= (stockproduit.Quantite - stockProduitDto.Quantite);

                    // Calculer le pourcentage de vente
                    produit.PourcentageVente = ((produit.stock - produit.StockActuel) / produit.stock) * 100;


                    var stockProduit = new StockProduit
                    {
                        prix_vent = ((stockproduit.Quantite - stockProduitDto.Quantite) * produit.PrixActuel),
                        prix_actuell = (stockproduit.prix_actuell - ((stockproduit.Quantite - stockProduitDto.Quantite) * produit.PrixActuel)),
                        StockId = stock.Id,
                        ProduitId = produit.Id
                    };
                    stock.PrixDeVenteTotal += stockProduit.prix_vent;
                    _context.StockProduits.RemoveRange(stockproduit);
                    _context.StockProduits.Add(stockProduit);
  
                }
            }
            stock.Quantity = updateStockDto.StockProduitdto.Sum(sp => sp.Quantite);
            var Portfeiulleclient = await _context.portFeuilleClients.FirstOrDefaultAsync(c => c.Code == updateStockDto.CodeClient);
            if (Portfeiulleclient == null)
            {
                throw new Exception("PortFeuilleClient not found");
            }
            Portfeiulleclient.PriceCompta -= updateStockDto.recipe_day;
            Portfeiulleclient.PricePayer = stock.PrixDeVenteTotal - updateStockDto.recipe_day;
            Portfeiulleclient.currentPrice = Portfeiulleclient.PriceCompta - stock.PrixDeVenteTotal;
            Portfeiulleclient.PriceCompta -= updateStockDto.recipe_day;

            await _context.SaveChangesAsync();
        }
    }
}