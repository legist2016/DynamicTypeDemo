namespace DynamicTypeDemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class T_CM_PRODUCT
    {
        public int id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(200)]
        public string description { get; set; }

        public float price { get; set; }

        public int state { get; set; }
    }
}
