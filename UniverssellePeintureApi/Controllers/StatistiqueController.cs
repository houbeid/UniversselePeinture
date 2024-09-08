﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using UniverssellePeintureApi.DTO;
using UniverssellePeintureApi.DTO.Response;
using UniverssellePeintureApi.Model;
using static UniverssellePeintureApi.DTO.Response.StatistiqueResponse;

namespace UniverssellePeintureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatistiqueController : ControllerBase
    {
        private readonly ApiDbContext _context;
        public StatistiqueController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<StatistiqueResponse> GetStatistique()
        {
            var produits = await _context.Produits.ToListAsync();
            if (produits == null)
            {
                throw new ArgumentException("Invalid statistique", nameof(produits));
            }
            var statistiqueList = new StatistiqueResponse();
            int stock_fabrique_total = 0;
            int stock_actuel_total = 0;
            foreach (var Produit in produits)
            {
                statistiqueList.statistiquProduits.Add(new StatistiquProduit()
                {
                    produit = Produit.Name,
                    stock_fabrique = Produit.stock,
                    stock_actuel = Produit.StockActuel,
                    pourcentage_produit = Produit.PourcentageVente
                });
                stock_fabrique_total += Produit.stock;
                stock_actuel_total += Produit.StockActuel;
            }
            statistiqueList.statistiquProduits.Add(new StatistiquProduit()
            {
                produit = "Total",
                stock_fabrique = stock_fabrique_total,
                stock_actuel = stock_actuel_total,
                pourcentage_vent = ((stock_fabrique_total - stock_actuel_total) / (double)stock_fabrique_total) * 100
            });

            var clients = await _context.Clients.ToListAsync();
            var initialClientCount = clients.Count;
            var zones = clients.GroupBy(c => c.Zone).ToList();

            foreach (var zoneGroup in zones)
            {
                CoverageData coverage = new CoverageData
                {
                    Address = zoneGroup.Key,
                    Coverage = (zoneGroup.Count() / (double)initialClientCount) * 100
                };
                statistiqueList.coverageDatas.Add(coverage);
            }

            return statistiqueList;
        }
    }
}