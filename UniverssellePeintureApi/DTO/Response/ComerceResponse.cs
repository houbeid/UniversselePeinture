using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniverssellePeintureApi.Model;

namespace UniverssellePeintureApi.DTO.Response
{
    public class ComerceResponse
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Telephone { get; set; }


        public List<ClientResponse> Clients { get; set; }
    }

    public class ClientResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string? Name_Society { get; set; }
        public string Phone_Number { get; set; }

        public string Respnsible_Name { get; set; }
        public string? Gérant { get; set; }
        public string? Solvabilité { get; set; }
        public string? CoordonnéesGPS { get; set; }
        public string Zone { get; set; }
        public string? Recommandation { get; set; }
        public DateTime? Visit_Date { get; set; }

        public DateTime? Delivery_Date { get; set; }
        public string? Description { get; set; }
    }
}
