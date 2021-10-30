using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Timers;
using static System.Console;

namespace shopper
{
    public class Schedule
    {
        public async Task StartTimer()
        {
            var _random = new Random();
            DateTime nowTime = DateTime.Now;
            //off time so it doesn't look like a bot
            DateTime eveningOffTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 22, 0, 0, 0);
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, 0, 0, 0);
            if (nowTime > eveningOffTime)
            {
                scheduledTime = scheduledTime.AddHours(10);
            }
            if (nowTime > scheduledTime)
            {
                //random times so it doesn't look like a bot is checking it at the same times everyday
                scheduledTime = scheduledTime.AddMinutes(_random.Next(45, 76)).AddSeconds(_random.Next(1, 59));
            }
            WriteLine($"Timer started at {nowTime} Going shopping at {scheduledTime}");
            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            //await Task.Delay(Convert.ToInt32(tickTime));
            //for development, go faster
            await Task.Delay(Convert.ToInt32(10000));
            WriteLine($"Starting to shop at {DateTime.Now}");
        }
    }
}
