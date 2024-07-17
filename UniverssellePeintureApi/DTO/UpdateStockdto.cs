namespace UniverssellePeintureApi.DTO
{
    public class UpdateStockdto
    {
        public string CodeClient { get; set; }

        public decimal recipe_day { get; set; }

        public DateTime Visit_date { get; set; }

        public string Description { get; set; }

        public List<StockProduitdto> StockProduitdto { get; set; }
    }
}
