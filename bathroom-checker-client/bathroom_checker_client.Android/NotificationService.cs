using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace bathroom_checker_client.Droid
{


    [Service(Name = "com.ds.bathroomchecker.NotificationService", Process = ":notificationservice_process", Exported = true)]
    class NotificationService : Service
    {
        const string TAG = "BNS";
        const int SAY_HELLO_TO_NOTIFICATION_SERVICE = 1;
        const int GET_UTC_TIMESTAMP = 2;

        private Messenger messenger; // Instance variable for the Messenger

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");
            return messenger.Binder;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            messenger = new Messenger(new RequestHandler(this));
            Log.Info(TAG, $"NotificationService is running in process id {Android.OS.Process.MyPid()}.");

            Notification.Builder notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                .SetContentTitle("Title")
                .SetContentText("This is the content");

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());
        }

        // A notification requires an id that is unique to the application.
        const int NOTIFICATION_ID = 9000;



        public class RequestHandler : Android.OS.Handler
        {
            private Service svc;
            public RequestHandler(Service svc)
            {
                this.svc = svc;
            }

            public override void HandleMessage(Message msg)
            {
                int messageType = msg.What;
                Log.Debug(TAG, $"Message type: {messageType}.");

                switch (messageType)
                {
                    case SAY_HELLO_TO_NOTIFICATION_SERVICE:
                        // The client as sent a simple Hello, say in the Android Log.
                        break;

                    case GET_UTC_TIMESTAMP:
                        // Call methods on the service to retrive a timestamp message.
                        break;
                    default:
                        Log.Warn(TAG, $"Unknown messageType, ignoring the value {messageType}.");
                        base.HandleMessage(msg);
                        break;
                }
            }
        }

        public class Connection : Java.Lang.Object, IServiceConnection
        {
            static readonly string TAG = typeof(Connection).FullName;

            MainActivity mainActivity;
            Messenger messenger;

            public Connection(MainActivity activity)
            {
                IsConnected = false;
                mainActivity = activity;
            }

            public bool IsConnected { get; private set; }
            public Messenger Messenger { get; private set; }

            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

                IsConnected = service != null;
                Messenger = new Messenger(service);

                if (IsConnected)
                {
                    // things to do when the connection is successful. perhaps notify the client? enable UI features?
                }
                else
                {
                    // things to do when the connection isn't successful. 
                }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
                IsConnected = false;
                Messenger = null;

                // Things to do when the service disconnects. perhaps notify the client? disable UI features?

            }
        }
    }    
}