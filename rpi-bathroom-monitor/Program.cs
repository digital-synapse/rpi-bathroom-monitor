using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rpi_bathroom_monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            // try to get a PIN number from the command line arguments
            if (args.Length > 0)
            {
                int pin = 0;
                if (int.TryParse(args[0], out pin))
                {
                    Bathroom.DOOR_SENSOR_PIN = pin;
                }
            }

            HttpService.Start(Bathroom.GetStatus, "/bathroom/status", 9000);
            HttpService.Start(Bathroom.Enqueue, "/bathroom/enqueue", 9000);

            Console.WriteLine("Press ANY key to exit...");
            while (!Console.KeyAvailable)
            {                
                Thread.Sleep(50);
            }
        }

        
    }
}
