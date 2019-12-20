using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Entities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    //[DataContract]
    public class TableTemplate
    {
        public TableTemplate()
        {
            Fields = new HashSet<TableTemplateField>();
        }
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public bool IsDelete { get; set; }

        [JsonIgnore]
        public virtual ICollection<TableTemplateField> Fields { get; set; }
    }
}
