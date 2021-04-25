using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryDB_Sql_code_first
{
    public class PartsPC
    {
        
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string CPU { get; set; }
        public string Frequency { get; set; }
        public string Cache_memory { get; set; }
        public float Price { get; set; }


      
    }
}
