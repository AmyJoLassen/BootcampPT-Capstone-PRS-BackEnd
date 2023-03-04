using Microsoft.Build.Framework;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrsBackEnd.Models
{
    public class RequestLine
    {

        public int Id { get; set; }

        // FK
        [JsonIgnore]
        [ForeignKey(nameof(RequestId))]
        public Request Request { get; set; }
        public int RequestId { get; set; }

        // relationship
        [JsonIgnore]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;


    }
}
