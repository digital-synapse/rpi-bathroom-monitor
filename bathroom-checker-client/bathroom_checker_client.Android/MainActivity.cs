using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace bathroom_checker_client.Droid
{
    [Activity(Label = "bathroom_checker_client", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            //startService();
         
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        private void startService()
        {
            // This is the package name of the APK, set in the Android manifest
            const string REMOTE_SERVICE_COMPONENT_NAME = "com.ds.bathroomchecker";
            // This is the name of the service, according the value of ServiceAttribute.Name
            const string REMOTE_SERVICE_PACKAGE_NAME = "com.ds.bathroomchecker.NotificationService";

            // Provide the package name and the name of the service with a ComponentName object.
            ComponentName cn = new ComponentName(REMOTE_SERVICE_PACKAGE_NAME, REMOTE_SERVICE_COMPONENT_NAME);
            Intent serviceToStart = new Intent();
            serviceToStart.SetComponent(cn);
            IServiceConnection serviceConnection = new NotificationService.Connection(this);
            var result = BindService(serviceToStart, serviceConnection, Bind.AutoCreate);

            //if (!result) throw new Exception("Binding to service failed!");
        }
    }
}

