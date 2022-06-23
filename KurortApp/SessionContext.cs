using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurortApp
{
    static class SessionContext
    {
        public static int Attempts { get; set; } = 0;
    }
}
