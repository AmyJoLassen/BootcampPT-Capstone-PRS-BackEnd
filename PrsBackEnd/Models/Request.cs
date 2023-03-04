using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrsBackEnd.Models
{
    public class Request
    {
        public static string StatusReview = "REVEIW";

        [Key]
        public int Id { get; set; }

        [StringLength(80)]
        public string Description { get; set; }

        [StringLength(80)]
        public string Justification { get; set; }

        [StringLength(80)]
        public string? RejectionReason { get; set; }

        [Required, StringLength(20)]
        public string DeliveryMode { get; set; } = "Pickup";

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public DateTime DateNeeded { get; set; }

        [Required, StringLength(10)]
        public string Status { get; set; } = "New";

        [Column(TypeName = "decimal(11,2)")]
        public decimal Total { get; set; } = 0;

        //FK
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public int UserId { get; set; }

        // relation property
        public List<RequestLine>? RequestLines { get; set; }
    }
}
