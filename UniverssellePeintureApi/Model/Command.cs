using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniverssellePeintureApi.Model
{
    public class Command
    {
        [Key]
        public int Id { get; set; }
        public string? distrubitaire { get; set; }

        public string? client { get; set; }
        public string? produit { get; set; }

        public int Qte { get; set; }

        public decimal   poids { get; set; }

        public decimal TotalPoids { get; set; }

        public string? Zone { get; set; }

        public string? Code { get; set; }

        public decimal Cach { get; set; }

        public string? phone { get; set; }

        public decimal A_Payer { get; set; }

        public DateTime? Command_Date { get; set; }
    }
}
