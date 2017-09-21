using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using RaspberryPiDotNet;

namespace rpi_bathroom_monitor
{    
    public class Bathroom
    {
        public static int DOOR_SENSOR_PIN =  15;        

        static Bathroom()
        {
            Status = new RestroomStatus();
        }
        public static RestroomStatus Status { get; set; } 


        public static string Enqueue(HttpListenerRequest request)
        {
            var url = request.Url.ToString();
            var i= url.LastIndexOf('/');
            var user = url.Substring(i + 1);

            // prevent user from enquing twice
            if (!Status.Queue.Contains(user))
                Status.Queue.Enqueue(user);

            return JsonConvert.SerializeObject(Status);
        }

        public static string GetStatus(HttpListenerRequest request)
        {
            bool occupied =false;
            GPIOMem gpio = null;

            try
            {
                gpio = new GPIOMem((GPIOPins)DOOR_SENSOR_PIN, GPIODirection.In, true);
                occupied = gpio.Read() == PinState.Low;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                gpio.Dispose();
            }

            // if the restroom is now occupied where it previously was not
            // update the status object by moving a user out of the queue
            if (occupied && !Status.Occupied)
            {
                if (Status.Queue.Count > 0)
                {                    
                    Status.OccupiedBy = Status.Queue.Dequeue();
                }
                else
                {
                    Status.OccupiedBy = "?"; // unknown user (user not using queue?)
                }
                Status.Occupied = true;
                Status.OccupiedOn = DateTime.UtcNow; // this isn't meant to be exact
            }

            // otherwise if the restroom is no longer occupied where previously it was
            // update the status object to reflect this
            else if (!occupied && Status.Occupied)
            {
                Status.Occupied = false;
                Status.OccupiedBy = null; // not occupied
            }

            return JsonConvert.SerializeObject(Status);
        }

        public class RestroomStatus
        {
            public bool Occupied { get; set; }
            public DateTime OccupiedOn { get; set; } = DateTime.MinValue;
            public string OccupiedBy { get; set; }
            public Queue<string> Queue { get; set; } = new Queue<string>();
        }
    }
}
