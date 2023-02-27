using Microsoft.Build.Framework;
using Newtonsoft.Json;

namespace PrsBackEnd.Models
{
    public class RequestLine
    {

        public int Id { get; set; }

        // FK
        [JsonIgnore]
        public Request Request { get; set; }
        public int RequestId { get; set; }

        // relationship
        public Product Product { get; set; }
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;


    }
}
