using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Models
{
    public class WebsiteData
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<BlogInfo>? BlogInfos { get; set; }
    }
}
