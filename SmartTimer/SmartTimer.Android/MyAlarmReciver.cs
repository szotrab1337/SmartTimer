using Android.Content;
using Xamarin.Forms;

namespace SmartTimer.Droid
{
    [BroadcastReceiver]
    public class MyAlarmReciver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            int alarmType = intent.GetIntExtra("ALARM_TYPE", -1);

            if (alarmType == 1)
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "MainAlarmTriggered");
            else
                MessagingCenter.Send(Xamarin.Forms.Application.Current, "SecondaryAlarmTriggered");

        }
    }
}