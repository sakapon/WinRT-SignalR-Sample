using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Sensors;

namespace SensorsHost
{
    class Program
    {
        const string SelfHostUrl = "http://localhost:8080";
        const string AppName = "Sensors Host";
        static IDisposable webApp;
        static NotifyIcon notifyIcon;

        static void Main(string[] args)
        {
            webApp = WebApp.Start(SelfHostUrl);
            ShowNotifyIcon();
            Application.Run();
        }

        static void ShowNotifyIcon()
        {
            var exitMenu = new ToolStripMenuItem("終了(&X)");
            exitMenu.Click += (o, e) => ExitApp();
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add(exitMenu);

            notifyIcon = new NotifyIcon
            {
                ContextMenuStrip = contextMenuStrip,
                Icon = Properties.Resources.ServiceIcon,
                Text = AppName,
                Visible = true,
            };

            notifyIcon.ShowBalloonTip(3000, AppName, "サービスを開始しました。", ToolTipIcon.Info);
        }

        static void ExitApp()
        {
            webApp.Dispose();
            notifyIcon.Dispose();
            Application.Exit();
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
                //Console.WriteLine("Light: {0} lx", e.Reading.IlluminanceInLux);
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
                //Console.WriteLine("Compass: {0} °", rounded);
                hubContext.Clients.All.NotifyHeadingMagneticNorth(rounded);
            };
        }

        public double GetHeadingMagneticNorth()
        {
            return Math.Round(compass.GetCurrentReading().HeadingMagneticNorth, 3);
        }
    }
}
