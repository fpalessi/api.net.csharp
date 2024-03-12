using System.ComponentModel.DataAnnotations.Schema;

namespace apinet.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Title {get;set;}
        public required string Description  {get;set;}

        [Column(TypeName = "decimal(18, 2)")]
        public required decimal Price { get; set; }
        public required double Rating { get; set; }
        public required int Stock { get; set; }
        public required string Brand { get; set; }
        public required string Category { get; set; }
        public required string Image { get; set; }
    }
}