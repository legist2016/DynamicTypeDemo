using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeDemo.Entities
{
    public interface IT_GeneralTable
    {
        [Key]
        int SYS_ID { get; set; }
    }

    public interface IT_SlaveTable 
    {
        int SYS_MASTER_ID { get; set; }
    }
}
