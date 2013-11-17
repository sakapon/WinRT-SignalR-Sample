using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => StartConnection());

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
        }

        async static void StartConnection()
        {
            var connection = new HubConnection("http://localhost:8080/");
            // Specify the path explicitly.
            //var connection = new HubConnection("http://localhost:8080/signalr", false);
            connection.ConnectionSlow += () => Console.WriteLine("Connection is slow.");
            connection.Error += ex => Console.WriteLine(ex);

            var lightSensor = connection.CreateHubProxy("LightSensorHub");
            lightSensor.On<float>("NotifyIlluminanceInLux", NotifyIlluminanceInLux);
            var compass = connection.CreateHubProxy("CompassHub");
            compass.On<double>("NotifyHeadingMagneticNorth", NotifyHeadingMagneticNorth);

            await connection.Start();

            var illuminanceInLux = await lightSensor.Invoke<float>("GetIlluminanceInLux");
            NotifyIlluminanceInLux(illuminanceInLux);
            var headingMagneticNorth = await compass.Invoke<double>("GetHeadingMagneticNorth");
            NotifyHeadingMagneticNorth(headingMagneticNorth);
        }

        static void NotifyIlluminanceInLux(float illuminanceInLux)
        {
            Console.WriteLine("Light: {0} lx", illuminanceInLux);
        }

        static void NotifyHeadingMagneticNorth(double headingMagneticNorth)
        {
            Console.WriteLine("Compass: {0} °", headingMagneticNorth);
        }
    }
}
