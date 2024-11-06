using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Models
{
    public class BlogInfo
    {
        public int Id { get; set; }
        public string? BlogTopics { get; set; }
        public int WebsiteId { get; set; }
        public WebsiteData? WebsiteData { get; set; }
    }
}
