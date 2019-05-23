using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoresDatabase
{
    class ItemNameClass
    {
        int ID;
        String Name;
        public ItemNameClass(int id, String name)
        {
            ID = id;
            Name = name;
        }

        public int getID()
        {
            return ID;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
