using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniverssellePeintureApi.Model
{
    public class Historique
    {
        [Key] 
        public int Id { get; set; }

        [Required]
        public string NameProduit { get; set; }

        public int Quantite { get; set; }

        public DateTime Delivery_date { get; set; }
        public decimal Montant { get; set; }

        public string distributeur { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
