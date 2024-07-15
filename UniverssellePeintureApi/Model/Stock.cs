namespace UniverssellePeintureApi.Model
{
    public class Stock
    {
        public int Id { get; set; }
        public int Quantite { get; set; }

        // Clé étrangère pour le client
        public int ClientId { get; set; }

        public Client Client { get; set; }
        // Clé étrangère pour le produit
        public int ProduitId { get; set; }

        // Navigation property pour le produit
        public Produit Produit { get; set; }
    }
}
