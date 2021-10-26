﻿using Microsoft.Extensions.Logging;
using System;
using System.Timers;
using static System.Console;

namespace Shopper
{
    public class Schedule
    {
        private Timer timer;
        private Random _random;
        private readonly ILogger<Schedule> _logger;
        private readonly Shopper _shopper;

        public Schedule(Random random, ILogger<Schedule> logger, Shopper shopper)
        {
            _random = random;
            _logger = logger;
            _shopper = shopper;

        }

        public void StartTimer()
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
            timer = new Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(StartShopping);
            timer.Start();
        }

        public void StartShopping(object sender, ElapsedEventArgs e)
        {
            WriteLine($"Starting to shop at {DateTime.Now}");
            timer.Stop();
            _shopper.GoShopping();
            StartTimer();
        }
    }
}
