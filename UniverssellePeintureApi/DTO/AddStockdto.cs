namespace UniverssellePeintureApi.DTO
{
    public class AddStockdto
    {
        public string CodeClient { get; set; }

        public DateTime Delivery_date { get; set; }
        public decimal PriceCompta { get; set; }

        public List<StockProduitdto> StockProduitdto { get; set; }
    }

    public class StockProduitdto
    {
        public string NameProduit { get; set; }
        public int Quantite { get; set; }
    }
}
