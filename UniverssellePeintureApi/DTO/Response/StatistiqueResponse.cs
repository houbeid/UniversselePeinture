namespace UniverssellePeintureApi.DTO.Response
{

    public class StatistiqueResponse
    {
        public List<StatistiquProduit> statistiquProduits { get; set; }
        public List<CoverageData> coverageDatas { get; set; }

        public StatistiqueResponse()
        {
            statistiquProduits = new List<StatistiquProduit>();
            coverageDatas = new List<CoverageData>();
        }
    }
    public class StatistiquProduit
    {
        public string produit { get ; set; }

        public int stock_fabrique { get; set; }

        public int stock_actuel { get; set; }

        public double pourcentage_vent { get; set; }

        public decimal pourcentage_produit { get; set; }

        
    }

    public class CoverageData
    {
        public string Address { get; set; }
        public double Coverage { get; set; }
    }
}
