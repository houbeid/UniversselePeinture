using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniverssellePeintureApi.Model
{
    public class Facture
    {
        [Key] 
        public int Id { get; set; }

        public DateTime date { get; set; }

        public string client { get; set; }

        public string Adress { get; set; }

        

        public string Code { get; set; }

        public string facture { get; set; }

        public decimal Montant { get; set; }

        public string distributeur { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
