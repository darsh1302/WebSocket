using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Realtime.MetroLine.Entity
{
    public class MetrolLineScheduler
    {

        public static DateTime InitiateTimer = DateTime.Today;

        public List<ResponseEntity> getTimeofArrivalbyStopID(int stopId)
        {
            List<ResponseEntity> lstResponseEntities = new List<ResponseEntity>();
            DateTime currentTime = DateTime.Now;

            int TotalMinute = currentTime.Hour * 60 + currentTime.Minute;
            int InitiateTimerMinute = (stopId - 1) * Configurations.stopDistance;

            for (int i = 0; i < Configurations.MaxRoute; i++)
            {

                int remainder = (TotalMinute - (Configurations.stopDistance * i) + InitiateTimerMinute) % Configurations.ServiceInterval;
                int arrivalMinute = remainder > 0 ? Configurations.ServiceInterval - remainder : 0;
                ResponseEntity ability = new ResponseEntity(i + 1, arrivalMinute, arrivalMinute + Configurations.ServiceInterval);
                lstResponseEntities.Add(ability);
            }

            return lstResponseEntities;
        }
    }
}
