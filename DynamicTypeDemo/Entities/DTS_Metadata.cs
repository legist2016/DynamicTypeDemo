using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Entities
{
    public class DTS_Metadata
    {
        public int id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public PrimitiveTypeKind type { get; set; }
        public int length { get; set; }

    }
}
