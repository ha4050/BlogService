using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Models
{
    public class BlogLog
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string? Keyword { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public WebsiteData? WebsiteData { get; set; }
    }
}
