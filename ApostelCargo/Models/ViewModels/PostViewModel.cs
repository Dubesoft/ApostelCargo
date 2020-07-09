using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostelCargo.Models.ViewModels
{
    public class PostViewModel
    {
        public string PostContent { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Video { get; set; }
        public DateTime DateCreated { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}
