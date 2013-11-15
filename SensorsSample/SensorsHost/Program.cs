using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
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
        const string SelfHostUrl = "http://localhost:8080";

        static void Main(string[] args)
        {
            using (WebApp.Start(SelfHostUrl))
            {
                Console.WriteLine("Press [Enter] to exit.");
                Console.ReadLine();
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
            // Specify the path explicitly.
            //app.MapSignalR("/signalr", new HubConfiguration());

            LightSensorHub.StartBroadcast();
            CompassHub.StartBroadcast();
        }
    }

    public class LightSensorHub : Hub
    {
        static readonly IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<LightSensorHub>();
        static readonly LightSensor lightSensor = LightSensor.GetDefault();

        public static void StartBroadcast()
        {
            lightSensor.ReportInterval = 500;
            lightSensor.ReadingChanged += (o, e) =>
            {
                Console.WriteLine("Light: {0} lx", e.Reading.IlluminanceInLux);
                hubContext.Clients.All.NotifyIlluminanceInLux(e.Reading.IlluminanceInLux);
            };
        }

        public float GetIlluminanceInLux()
        {
            return lightSensor.GetCurrentReading().IlluminanceInLux;
        }
    }

    public class CompassHub : Hub
    {
        static readonly IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<CompassHub>();
        static readonly Compass compass = Compass.GetDefault();

        public static void StartBroadcast()
        {
            compass.ReadingChanged += (o, e) =>
            {
                var rounded = Math.Round(e.Reading.HeadingMagneticNorth, 3);
                Console.WriteLine("Compass: {0} °", rounded);
                hubContext.Clients.All.NotifyHeadingMagneticNorth(rounded);
            };
        }

        public double GetHeadingMagneticNorth()
        {
            return Math.Round(compass.GetCurrentReading().HeadingMagneticNorth, 3);
        }
    }
}
