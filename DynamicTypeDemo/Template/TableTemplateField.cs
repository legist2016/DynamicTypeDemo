using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Template
{
    public enum TableTemplateFieldType
    {
        Int=1,
        String
    }
    public class TableTemplateField
    {
        public TableTemplateField()
        {            
        }
        public int Id { get; set; }
        public int TableTemplateId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public TableTemplateFieldType Type { get; set; }
        public bool IsKey { get; set; }
        public bool IsRequire { get; set; }
        public int Length { get; set; }
        public bool IsSysField { get; set; }
        /*public int DisplayLength { get; set; }
        public int IsMultiline { get; set; }*/
    }    
}
