namespace UniverssellePeintureApi.Model
{
    public class Produit
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public decimal? PrixVente { get; set; }

        public decimal? valeur_actuel { get; set; }

        // Liste des stocks associés à ce produit
        public ICollection<Stock> Stocks { get; set; }
    }
}
