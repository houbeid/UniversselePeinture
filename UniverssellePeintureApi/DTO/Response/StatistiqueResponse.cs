namespace UniverssellePeintureApi.DTO.Response
{
    public class StatistiqueResponse
    {
        public string produit { get ; set; }

        public int stock_fabrique { get; set; }

        public int stock_actuel { get; set; }

        public decimal pourcentage_vent { get; set; }

        public decimal pourcentage_produit { get; set; }
    }
}
