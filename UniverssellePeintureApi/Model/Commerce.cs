namespace UniverssellePeintureApi.Model
{
    public class Commerce
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Telephone { get; set; }


        public ICollection<Client> Clients { get; set; }
    }
}
