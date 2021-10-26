using System;
using System.Timers;
using static System.Console;

namespace Shopper
{
    public class Schedule
    {
        static Timer timer;

        public static void StartTimer()
        {
            DateTime nowTime = DateTime.Now;
            //off time so it doesn't look like a bot
            DateTime eveningOffTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 22, 0, 0, 0);
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, 0, 0, 0);
            var rando = new Random();
            if (nowTime > eveningOffTime)
            {
                scheduledTime = scheduledTime.AddHours(10);
            }
            if (nowTime > scheduledTime)
            {
                //random times so it doesn't look like a bot is checking it at the same times everyday
                scheduledTime = scheduledTime.AddMinutes(rando.Next(45, 76)).AddSeconds(rando.Next(1, 59));
            }
            WriteLine($"Timer started at {nowTime} Going shopping at {scheduledTime}");
            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            timer = new Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(StartShopping);
            timer.Start();
        }

        public static void StartShopping(object sender, ElapsedEventArgs e)
        {
            WriteLine($"Starting to shop at {DateTime.Now}");
            timer.Stop();

            StartTimer();
        }
    }
}
