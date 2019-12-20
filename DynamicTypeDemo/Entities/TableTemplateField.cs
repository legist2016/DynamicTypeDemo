using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Entities
{
    public enum TableTemplateFieldType
    {
        Int=1,
        String,
        Boolean
    }
    //[DataContract]
    public class TableTemplateField
    {
        public TableTemplateField()
        {
        }
        [Key]
        //[DataMember]
        public int Id { get; set; }
        //[DataMember]
        public int TableTemplateId { get; set; }
        //[DataMember]
        public string Name { get; set; }
        //[DataMember]
        public string Title { get; set; }
        //[DataMember]
        public TableTemplateFieldType Type { get; set; }
        //[DataMember]
        public bool IsKey { get; set; }
        //[DataMember]
        public bool IsRequire { get; set; }
        //[DataMember]
        public int Length { get; set; }
        //[DataMember]
        public bool IsSysField { get; set; }
        /*public int DisplayLength { get; set; }
        public int IsMultiline { get; set; }*/
        //[JsonIgnore]
        //public virtual TableTemplate TableTemplate { get; set; }
    }    
}
