using System.ComponentModel.DataAnnotations;

namespace UniverssellePeintureApi.Model
{
    public class StockProduit
    {
        [Required]
        public int Quantite { get; set; }

        public decimal prix_vent { get; set; }

        public decimal  prix_actuell { get; set; }
        public int StockId { get; set; }
        public Stock Stock { get; set; }

        public int ProduitId { get; set; }
        public Produit Produit { get; set; }
    }

}
