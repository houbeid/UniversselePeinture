using System.ComponentModel.DataAnnotations;

namespace UniverssellePeintureApi.Model
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal PrixDeVenteTotal { get; set; }

        // Relation avec Client
        public int ClientId { get; set; }
        public Client Client { get; set; }

        // Relation avec StockProduit
        public ICollection<StockProduit> StockProduits { get; set; }
    }


}
