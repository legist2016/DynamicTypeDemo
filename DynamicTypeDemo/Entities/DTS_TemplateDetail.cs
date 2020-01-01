using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Entities
{
    public class DTS_TemplateDetail
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int id { get; set; }
        public string name { get; set; }
        public int metadataId { get; set; }
    }
}
