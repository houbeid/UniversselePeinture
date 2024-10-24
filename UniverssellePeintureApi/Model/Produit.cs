﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniverssellePeintureApi.Model
{
    public class Produit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Name { get; set; }

        [Required]
        public int stock { get; set; }

        [Required]
        public decimal PrixActuel { get; set; }

        
        public decimal poid { get; set; }

        [Required]
        public int StockActuel { get; set; }

        public double PourcentageVente { get; set; }

        public double PourcentageProduit { get; set; }

        // Relation avec StockProduit
        public ICollection<StockProduit> StockProduits { get; set; }
    }


}
