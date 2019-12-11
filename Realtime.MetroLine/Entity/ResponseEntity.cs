using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Realtime.MetroLine.Entity
{
    public class ResponseEntity
    {

        private int RouteNumber;
        private int FirstTimer;
        private int SecondTimer;
        public string Output;

        public ResponseEntity(int routeNumber, int firstTimer, int secondTimer)
        {
            RouteNumber = routeNumber;
            FirstTimer = firstTimer;
            SecondTimer = secondTimer;

            Output = $"Route {RouteNumber} in {firstTimer} mins and {secondTimer} mins";
        }

    }
}
