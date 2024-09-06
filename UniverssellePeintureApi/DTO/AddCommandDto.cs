namespace UniverssellePeintureApi.DTO
{
   
        public class AddCommandDto
        {
            public string CodeClient { get; set; }

            public DateTime Command_date { get; set; }
            public decimal A_Payer { get; set; }

            public decimal cach { get; set; }

            public string distributaire { get; set; }

            public string CommercialPhone { get; set; }



            public List<StockCommandDto> StockCommanddto { get; set; }
        }

        public class StockCommandDto
        {
            public string NameProduit { get; set; }
            public int Quantite { get; set; }

            public decimal poid { get; set; }
        }

}
