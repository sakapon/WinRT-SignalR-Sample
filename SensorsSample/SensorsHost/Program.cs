using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace SensorsHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var lightSensor = LightSensor.GetDefault();
            lightSensor.ReportInterval = 500;
            Action<LightSensorReading> notifyLight = r => Console.WriteLine("Light: {0} lx", r.IlluminanceInLux);
            notifyLight(lightSensor.GetCurrentReading());
            lightSensor.ReadingChanged += (o, e) => notifyLight(e.Reading);

            var compass = Compass.GetDefault();
            Action<CompassReading> notifyCompass = r => Console.WriteLine("Compass: {0:N3} °", r.HeadingMagneticNorth);
            notifyCompass(compass.GetCurrentReading());
            compass.ReadingChanged += (o, e) => notifyCompass(e.Reading);

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
        }
    }
}
