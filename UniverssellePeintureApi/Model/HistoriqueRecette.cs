using System.ComponentModel.DataAnnotations;

namespace UniverssellePeintureApi.Model
{
    public class HistoriqueRecette
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        
        public string Name { get; set; }

        public string CodeClient { get; set; }

        
        public decimal Recette { get; set; }
    }
}
