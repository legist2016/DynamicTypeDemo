using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Template
{
    public class TableTemplate
    {
        public TableTemplate()
        {
            Fields = new List<TableTemplateField>();
            Fields.Add(new TableTemplateField() {
                Name="sys_id",
                Type = TableTemplateFieldType.Int,
                Title = "ID",
                IsKey = true
            });
        }
        public string Name { get; set; }
        public virtual IList<TableTemplateField> Fields { get; set; }
    }
}
