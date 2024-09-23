namespace UniverssellePeintureApi.DTO
{
   
        public class AddCommandDto
        {
            public string CodeClient { get; set; }

            public DateTime Command_date { get; set; }

            public string? cach { get; set; }



            public List<StockCommandDto> StockCommanddto { get; set; }
        }

        public class StockCommandDto
        {
            public string NameProduit { get; set; }
            public int Quantite { get; set; }

            public decimal poid { get; set; }
        }

    public class PriseComptaDto
    {
        public string CodeClient { get; set; }
        public decimal priseCompta { get; set; }
    }

}
