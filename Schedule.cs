using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Timers;
using static System.Console;

namespace shopper
{
    public class Schedule
    {
        private Random _random;
        public Schedule(Random random) => _random = random;
        public async Task StartTimer()
        {
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
            await Task.Delay(Convert.ToInt32(tickTime));
            WriteLine($"Starting to shop at {DateTime.Now}");
        }
    }
}
