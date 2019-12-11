using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Realtime.MetroLine.Entity
{
    public class Stop
    {
        public string StopName { get; set; }
        public int StopID { get; set; }
        

        public Stop(string stopName, int stopID)
        {
            StopName = stopName;
            StopID = stopID;
        }
    }
}
