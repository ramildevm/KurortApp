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
        public static Users CurrentUser { get; set; } = null;
        public static TimeSpan CurrentTime { get; set; }
        public static TimeSpan TimerInterval { get; set; } = TimeSpan.FromSeconds(0);
        public static void SetDefaults()
        {
            Attempts = 0;
        }
        public static void SetTimer(int time = 0)
        {
            if (time == 15)
                TimerInterval = TimeSpan.FromMinutes(15);
            else if (time == 10)
                TimerInterval = TimeSpan.FromSeconds(10);
            else
                TimerInterval = TimeSpan.FromSeconds(0);
        }
    }
}
