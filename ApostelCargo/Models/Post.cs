using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApostelCargo.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Video { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
