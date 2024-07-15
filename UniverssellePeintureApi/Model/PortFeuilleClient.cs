using System.ComponentModel.DataAnnotations;

namespace UniverssellePeintureApi.Model
{
    public class PortFeuilleClient
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }

        public string name { get; set; }
        public string zone { get; set; }

         public DateTime? depot { get; set; }

        public  DateTime? visit { get; set; }

        public decimal? currentPrice { get; set; }
        public decimal? PriceCompta { get; set; }

        public decimal? PricePayer { get; set; }

        // Clé étrangère pour le commerçant
        public int CommercantId { get; set; }

        // Navigation property pour le commerçant
        public Commerce Commerce { get; set; }
    }
}
