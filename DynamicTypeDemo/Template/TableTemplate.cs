using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Template
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    [DataContract]
    public class TableTemplate
    {
        public TableTemplate()
        {
            Fields = new List<TableTemplateField>();
            /*Fields.Add(new TableTemplateField() {
                Name="sys_id",
                Type = TableTemplateFieldType.Int,
                Title = "ID",
                IsKey = true
            });*/
        }
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [DataMember]
        public virtual List<TableTemplateField> Fields { get; set; }
    }
}
