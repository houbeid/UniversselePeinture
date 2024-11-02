namespace UniverssellePeintureApi.DTO.Response
{
    public class HistoriqueResponse
    {
        public string Produit { get; set; }
        public int Quantite { get; set; }
        public decimal Montant { get; set; }
        public DateTime Date { get; set; }
        public string Commercial { get; set; }
    }

    public class HistoriqueRecetteResponse
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string CodeClient { get; set; }
        public decimal recette { get; set; }
    }
}
