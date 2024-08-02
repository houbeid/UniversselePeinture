

namespace UniverssellePeintureApi.DTO
{
    public class AddFactureDto
    {
        public DateTime date { get; set; }

        public string CodeClient { get; set;}

        public string Facture { get; set;}

        public decimal Montant { get; set; }

        public string distribiteur { get; set; }
    }
}
