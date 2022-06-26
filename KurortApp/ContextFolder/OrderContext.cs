using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurortApp
{
    class OrderContext
    {
        static public Users SelectedUser { get; set; } = null;
        static public List<Services> ServicesSet { get; set; } = null;
        static public void SetDefaults()
        {
            SelectedUser = null;
            ServicesSet = new List<Services>();
        }
    }
}
