using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostelCargo.Models.ViewModels
{
    public class BlogViewModel
    {
        public IEnumerable<Post> Post { get; set; }
        public IEnumerable<Comments> Comments { get; set; }
        public IEnumerable<CommentCount> CommentCount { get; set; }
    }
}
