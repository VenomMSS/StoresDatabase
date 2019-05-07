using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoresDatabase
{
    class item
    {
        int ID { get; set; }
        String Name { get; set; }
        String Description { get; set; }
        String Unit { get; set; }
        int Amount { get; set; }
        String Price { get; set; }
        String Currency { get; set; }
        bool Status { get; set; }
        int LocFK { get; set; }
        int SupFK { get; set; }
        int TypeFK { get; set; }

        bool Clear()
        { 
            Amount= 0;
            return false;
        }
     }

    
}
