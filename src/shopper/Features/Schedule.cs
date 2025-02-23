﻿using System;
using System.Threading.Tasks;
using static System.Console;

namespace shopper.Features
{
    public class Schedule
    {
        public async Task StartTimer()
        {
            var _random = new Random();
            DateTime nowTime = DateTime.Now;
            DateTime eveningOffTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 22, 0, 0, 0);
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, 0, 0, 0);
            if (nowTime > eveningOffTime)
            {
                scheduledTime = scheduledTime.AddHours(10).AddMinutes(_random.Next(1, 20)).AddSeconds(_random.Next(1, 59));
            }
            else if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddMinutes(_random.Next(45, 76)).AddSeconds(_random.Next(1, 59));
            }
            else if (scheduledTime < nowTime)
            {
                scheduledTime = scheduledTime.AddHours(1);
            }
            WriteLine($"Timer started at {nowTime} Going shopping at {scheduledTime}");
            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            await Task.Delay(Convert.ToInt32(tickTime));
            //for development, to go faster
            //await Task.Delay(Convert.ToInt32(10000));
        }
    }
}
