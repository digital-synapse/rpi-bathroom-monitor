using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ModernHttpClient;
using System.Net.Http;
using Newtonsoft.Json;

namespace bathroom_checker_client
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();                      
            BindingContext = this;

            btnAlert.Clicked += BtnAlert_Clicked;

            

            LoadAsync();
        }

        private void BtnAlert_Clicked(object sender, EventArgs e)
        {
                        
        }

        private async void LoadAsync()
        {
            var networkError = false;
            while (true)
            {
                if (string.IsNullOrEmpty(Settings.ServerIP) || networkError)
                {
                    Settings.ServerIP = await UI.InputBox(this.Navigation, "Settings", "Enter Server IP", Keyboard.Default);
                }

                lblStatus.Text = @"Checking Bathroom Status...";
                lblStatus2.Text = @":|";                

                await Task.Delay(1000); // just testing because service is soo fast

                var httpClient = new HttpClient(new NativeMessageHandler());
                string json = null;
                try
                {
                    json = await httpClient.GetStringAsync($"http://{Settings.ServerIP}:9000/bathroom/status");
                    networkError = false;
                }
                catch
                {
                    networkError = true;
                }
                if (!networkError)
                {
                    var restroom = JsonConvert.DeserializeObject<RestroomStatus>(json);

                    if (!restroom.Occupied && restroom.Queue.Count == 0)
                    {
                        lblStatus.Text = @"Bathroom Available!";
                        lblStatus2.Text = @":)";
                    }
                    else
                    {
                        lblStatus.Text = @"Bathroom Occupied";
                        lblStatus2.Text = @":(";
                        var duration = (DateTime.UtcNow - restroom.OccupiedOn);
                        if (duration.TotalMinutes > 60)
                            lblOccupiedTimer.Text = "over an hour!";
                        if (duration.TotalMinutes > 30)
                            lblOccupiedTimer.Text = "over 30 minutes!";
                        else if (duration.TotalSeconds < 30)
                            lblOccupiedTimer.Text = "just now";
                        else
                            lblOccupiedTimer.Text = ((int)duration.TotalMinutes) + " minutes and " + duration.Seconds + " seconds";

                        lblOccupiedTimer.IsVisible = true;
                    }
                    lblStatus2.IsVisible = true;
                    actProgress.IsVisible = false;
                    
                    await Task.Delay(1000 * 29); // refresh every 30 seconds
                    lblOccupiedTimer.IsVisible = false;
                }
                else
                {
                    actProgress.Color = Color.Red;
                    lblStatus.Text = @"Network Error";
                    lblStatus2.Text = @":(";
                    await Task.Delay(1000);
                }
            }
        }     
    }
}
