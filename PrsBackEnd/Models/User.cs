using System.ComponentModel.DataAnnotations;

namespace PrsBackEnd.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string Username { get; set; }

        [StringLength(30)]
        public string Password { get; set; }

        [StringLength(30)]
        public string Firstname { get; set; }

        [StringLength(30)]
        public string Lastname { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(1)]
        public bool IsReviewer { get; set; }

        [StringLength(1)]
        public bool IsAdmin { get; set; }

    }
}
