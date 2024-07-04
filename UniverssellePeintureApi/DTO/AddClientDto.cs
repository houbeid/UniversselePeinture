﻿namespace UniverssellePeintureApi.DTO
{
    public class AddClientDto
    {
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
        public int CommercantId { get; set; }
    }
}
