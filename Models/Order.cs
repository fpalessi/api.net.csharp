using System.ComponentModel.DataAnnotations.Schema;


namespace apinet.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }
    }
}