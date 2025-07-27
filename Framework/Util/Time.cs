﻿/*
 * Copyright (C) 2012-2020 CypherCore <http://github.com/CypherCore>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

public static class Time
{
    public const int Minute = 60;
    public const int Hour = Minute * 60;
    public const int Day = Hour * 24;
    public const int Week = Day * 7;
    public const int Month = Day * 30;
    public const int Year = Month * 12;
    public const int InMilliseconds = 1000;
    
    public static readonly int MillisecondsInSecond = 1000;
    public static readonly int SecondsInMinute = 60;
    public static readonly int MinutesInHour = 60;
    public static readonly int HoursInDay = 24;
    public static readonly int DaysInWeek = 7;
    
    public static readonly int SecondMS = InMilliseconds;
    public static readonly int MinuteMS = (SecondMS * SecondsInMinute);
    public static readonly int HourMS = (MinuteMS * MinutesInHour);
    public static readonly int DayMS = (HourMS * HoursInDay);
    public static readonly int WeekMS = (DayMS * DaysInWeek);

    public static readonly DateTime ApplicationStartTime = DateTime.Now;
    public static readonly DateTime Zero = DateTime.MinValue;
    public static readonly DateTime Infinity = DateTime.MaxValue;
    
     /// <summary>
    /// Gets the current UTC time.
    /// </summary>
    public static DateTime Now => DateTime.UtcNow;

    /// <summary>
    /// Gets the application UpTime.
    /// </summary>
    public static TimeSpan UpTime => Now - ApplicationStartTime;

    /// <summary>
    /// Gets the application time relative to application start time in ms.
    /// </summary>
    public static RelativeTime NowRelative => (RelativeTime)UpTime.ToMilliseconds();

    /// <summary>
    /// Gets the difference to current UTC time.
    /// </summary>
    public static TimeSpan Diff(DateTime oldTime)
    {
        return Diff(oldTime, Now);
    }    

    /// <summary>
    /// Gets the difference to current UpTime.
    /// </summary>
    public static TimeSpan Diff(TimeSpan oldUpTime)
    {
        return UpTime - oldUpTime;
    }

    /// <summary>
    /// Gets the difference to current RelativeTime in milliseconds.
    /// </summary>
    public static Milliseconds Diff(RelativeTime oldMSTime)
    {
        return Diff(oldMSTime, NowRelative);
    }

    /// <summary>
    /// Gets the difference between two time points.
    /// </summary>
    public static TimeSpan Diff(DateTime oldTime, DateTime newTime)
    {
        return newTime - oldTime;
    }

    /// <summary>
    /// Gets the difference between two time spans.
    /// </summary>
    public static TimeSpan Diff(TimeSpan oldTimeSpan, TimeSpan newTimeSpan)
    {
        return newTimeSpan - oldTimeSpan;
    }

    /// <summary>
    /// Gets the difference between two relative to UpTime spans in milliseconds.
    /// </summary>
    public static Milliseconds Diff(RelativeTime oldMSTime, RelativeTime newMSTime)
    {
        if (oldMSTime > newMSTime)
            return (Milliseconds)((RelativeTime)0xFFFFFFFF - oldMSTime + newMSTime);
        else
            return (Milliseconds)(newMSTime - oldMSTime);
    }

    /// <summary>
    /// Gets the difference between relative time span and DateTime in milliseconds.
    /// </summary>
    public static Milliseconds Diff(RelativeTime oldMSTime, DateTime newTime)
    {
        RelativeTime newMSTime = (RelativeTime)(newTime - ApplicationStartTime).ToMilliseconds();
        return Diff(oldMSTime,newMSTime);
    }

    /// <summary>
    /// Gets the current Unix time.
    /// </summary>
    public static long UnixTime
    {
        get
        {
            return DateTimeToUnixTime(DateTime.Now);
        }
    }

    /// <summary>
    /// Gets the current Unix time, in milliseconds.
    /// </summary>
    public static long UnixTimeMilliseconds
    {
        get
        {
            return ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
        }
    }

    /// <summary>
    /// Converts a TimeSpan to its equivalent representation in milliseconds (Int64).
    /// </summary>
    /// <param name="span">The time span value to convert.</param>
    public static long ToMilliseconds(this TimeSpan span)
    {
        return (long)span.TotalMilliseconds;
    }

    /// <summary>
    /// Gets the system uptime.
    /// </summary>
    /// <returns>the system uptime in milliseconds</returns>
    public static uint GetSystemTime()
    {
        return (uint)Environment.TickCount;
    }

    public static uint GetMSTime()
    {
        return (uint)(DateTime.Now - ApplicationStartTime).ToMilliseconds();
    }

    public static uint GetMSTimeDiff(uint oldMSTime, uint newMSTime)
    {
        if (oldMSTime > newMSTime)
            return (0xFFFFFFFF - oldMSTime) + newMSTime;
        else
            return newMSTime - oldMSTime;
    }

    public static uint GetMSTimeDiffToNow(uint oldMSTime)
    {
        var newMSTime = GetMSTime();
        if (oldMSTime > newMSTime)
            return (0xFFFFFFFF - oldMSTime) + newMSTime;
        else
            return newMSTime - oldMSTime;
    }

    public static DateTime UnixTimeToDateTime(long unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
    }

    public static long DateTimeToUnixTime(DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }

    public static long GetNextResetUnixTime(int hours)
    {
        return DateTimeToUnixTime((DateTime.Now.Date + new TimeSpan(hours, 0, 0)));
    }
    public static long GetNextResetUnixTime(int days, int hours)
    {
        return DateTimeToUnixTime((DateTime.Now.Date + new TimeSpan(days, hours, 0, 0)));
    }
    public static long GetNextResetUnixTime(int months, int days, int hours)
    {
        return DateTimeToUnixTime((DateTime.Now.Date + new TimeSpan(months + days, hours, 0)));
    }

    public static string secsToTimeString(ulong timeInSecs, bool shortText = false, bool hoursOnly = false)
    {
        ulong secs = timeInSecs % Minute;
        ulong minutes = timeInSecs % Hour / Minute;
        ulong hours = timeInSecs % Day / Hour;
        ulong days = timeInSecs / Day;

        string ss = "";
        if (days != 0)
            ss += days + (shortText ? "d" : " Day(s) ");
        if (hours != 0 || hoursOnly)
            ss += hours + (shortText ? "h" : " Hour(s) ");
        if (!hoursOnly)
        {
            if (minutes != 0)
                ss += minutes + (shortText ? "m" : " Minute(s) ");
            if (secs != 0 || (days == 0 && hours == 0 && minutes == 0))
                ss += secs + (shortText ? "s" : " Second(s).");
        }

        return ss;
    }

    public static uint TimeStringToSecs(string timestring)
    {
        int secs = 0;
        int buffer = 0;
        int multiplier;

        foreach (var c in timestring)
        {
            if (char.IsDigit(c))
            {
                buffer *= 10;
                buffer += c - '0';
            }
            else
            {
                switch (c)
                {
                    case 'd':
                        multiplier = Day;
                        break;
                    case 'h':
                        multiplier = Hour;
                        break;
                    case 'm':
                        multiplier = Minute;
                        break;
                    case 's':
                        multiplier = 1;
                        break;
                    default:
                        return 0;                         //bad format
                }
                buffer *= multiplier;
                secs += buffer;
                buffer = 0;
            }
        }

        return (uint)secs;
    }

    public static string GetTimeString(long time)
    {
        long days = time / Day;
        long hours = (time % Day) / Hour;
        long minute = (time % Hour) / Minute;

        return $"Days: {days} Hours: {hours} Minutes: {minute}";
    }

    public static long GetUnixTimeFromPackedTime(uint packedDate)
    {
        var time = new DateTime((int)((packedDate >> 24) & 0x1F) + 2000, (int)((packedDate >> 20) & 0xF) + 1, (int)((packedDate >> 14) & 0x3F) + 1, (int)(packedDate >> 6) & 0x1F, (int)(packedDate & 0x3F), 0);
        return (uint)DateTimeToUnixTime(time);
    }

    public static uint GetPackedTimeFromUnixTime(long unixTime)
    {
        var now = UnixTimeToDateTime(unixTime);
        return Convert.ToUInt32((now.Year - 2000) << 24 | (now.Month - 1) << 20 | (now.Day - 1) << 14 | (int)now.DayOfWeek << 11 | now.Hour << 6 | now.Minute);
    }

    public static uint GetPackedTimeFromDateTime(DateTime now)
    {
        return Convert.ToUInt32((now.Year - 2000) << 24 | (now.Month - 1) << 20 | (now.Day - 1) << 14 | (int)now.DayOfWeek << 11 | now.Hour << 6 | now.Minute);
    }

    public static void Profile(string description, int iterations, Action func)
    {
        //Run at highest priority to minimize fluctuations caused by other processes/threads
        System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
        System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;

        // warm up 
        func();

        var watch = new System.Diagnostics.Stopwatch();

        // clean up
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        watch.Start();
        for (int i = 0; i < iterations; i++)
        {
            func();
        }
        watch.Stop();
        Console.Write(description);
        Console.WriteLine(" Time Elapsed {0} ms", watch.Elapsed.TotalMilliseconds);
    }
    
    public static TimeSpan Min(TimeSpan left, TimeSpan right)
    {
        if (left < right)
            return left;
        else
            return right;
    }

    public static TimeSpan Max(TimeSpan left, TimeSpan right)
    {
        if (left > right)
            return left;
        else
            return right;
    }

    public static DateTime Min(DateTime left, DateTime right)
    {
        if (left < right)
            return left;
        else
            return right;
    }

    public static DateTime Max(DateTime left, DateTime right)
    {
        if (left > right)
            return left;
        else
            return right;
    }

    public static Milliseconds Min(Milliseconds left, Milliseconds right)
    {
        if (left.Ticks < right.Ticks)
            return left;
        else
            return right;
    }

    public static Milliseconds Max(Milliseconds left, Milliseconds right)
    {
        if (left.Ticks > right.Ticks)
            return left;
        else
            return right;
    }

    public static Seconds Min(Seconds left, Seconds right)
    {
        if (left.Ticks < right.Ticks)
            return left;
        else
            return right;
    }

    public static Seconds Max(Seconds left, Seconds right)
    {
        if (left.Ticks > right.Ticks)
            return left;
        else
            return right;
    }
    
         
     // It is better to use these methods instead of native ones, which use the double as an argument.
     // This helps get rid of the inaccuracies that the double type suffers from.
     public static TimeSpan SpanFromMilliseconds(long Milliseconds)
     {
         return TimeSpan.FromTicks(Milliseconds * TimeSpan.TicksPerMillisecond);
     }

     public static TimeSpan SpanFromSeconds(long Seconds)
     {
         return TimeSpan.FromTicks(Seconds * TimeSpan.TicksPerSecond);
     }

     public static TimeSpan SpanFromMinutes(long Minutes)
     {
         return TimeSpan.FromTicks(Minutes * (long)Time.Minute * TimeSpan.TicksPerSecond);
     }

     public static TimeSpan SpanFromHours(long Hours)
     {
         return TimeSpan.FromTicks(Hours * (long)Time.Hour * TimeSpan.TicksPerSecond);
     }

     public static TimeSpan SpanFromDays(long Days)
     {
         return TimeSpan.FromTicks(Days * (long)Time.Day * TimeSpan.TicksPerSecond);
     }

     public static TimeSpan SpanFromWeeks(long Weeks)
     {
         return TimeSpan.FromTicks(Weeks * (long)Time.Week * TimeSpan.TicksPerSecond);
     }

     // Just for good manners
     public static TimeSpan SpanFromMilliseconds(double Milliseconds)
     {
         return TimeSpan.FromMilliseconds(Milliseconds);
     }

     public static TimeSpan SpanFromSeconds(double Seconds)
     {
         return TimeSpan.FromSeconds(Seconds);
     }

     public static TimeSpan SpanFromMinutes(double Minutes)
     {
         return TimeSpan.FromMinutes(Minutes);
     }

     public static TimeSpan SpanFromHours(double Hours)
     {
         return TimeSpan.FromHours(Hours);
     }

     public static TimeSpan SpanFromDays(double Days)
     {
         return TimeSpan.FromDays(Days);
     }

     public static TimeSpan SpanFromWeeks(double Weeks)
     {
         return TimeSpan.FromDays(Weeks * Time.Week);
     }
     
     public static long ToSeconds(this TimeSpan span)
     {
         return span.Ticks / TimeSpan.TicksPerSecond;
     }

     public static long ToMinutes(this TimeSpan span)
     {
         return span.Ticks / TimeSpan.TicksPerMinute;
     }

     public static long ToHours(this TimeSpan span)
     {
         return span.Ticks / (Hour * TimeSpan.TicksPerSecond);
     }

     public static long ToDays(this TimeSpan span)
     {
         return span.Ticks / (Day * TimeSpan.TicksPerSecond);
     }

     public static long ToWeeks(this TimeSpan span)
     {
         return span.Ticks / (Week * TimeSpan.TicksPerSecond);
     }
     
     
}

public class TimeTrackerSmall
{
    public TimeTrackerSmall(int expiry = 0)
    {
        i_expiryTime = expiry;
    }

    public void Update(int diff)
    {
        i_expiryTime -= diff;
    }

    public bool Passed()
    {
        return i_expiryTime <= 0;
    }

    public void Reset(int interval)
    {
        i_expiryTime = interval;
    }

    public int GetExpiry()
    {
        return i_expiryTime;
    }
    int i_expiryTime;
}

public class TimeTracker
{
    public TimeTracker(long expiry = 0)
    {
        i_expiryTime = expiry;
    }

    public void Update(long diff)
    {
        i_expiryTime -= diff;
    }

    public bool Passed()
    {
        return i_expiryTime <= 0;
    }

    public void Reset(long interval)
    {
        i_expiryTime = interval;
    }

    public long GetExpiry()
    {
        return i_expiryTime;
    }

    long i_expiryTime;
}

public class IntervalTimer
{
    public void Update(long diff)
    {
        _current += diff;
        if (_current < 0)
            _current = 0;
    }

    public bool Passed()
    {
        return _current >= _interval;
    }

    public void Reset()
    {
        if (_current >= _interval)
            _current %= _interval;
    }

    public void SetCurrent(long current)
    {
        _current = current;
    }

    public void SetInterval(long interval)
    {
        _interval = interval;
    }

    public long GetInterval()
    {
        return _interval;
    }

    public long GetCurrent()
    {
        return _current;
    }

    long _interval;
    long _current;
}

public class PeriodicTimer
{
    public PeriodicTimer(int period, int start_time)
    {
        i_period = period;
        i_expireTime = start_time;
    }

    public bool Update(int diff)
    {
        if ((i_expireTime -= diff) > 0)
            return false;

        i_expireTime += i_period > diff ? i_period : diff;
        return true;
    }

    public void SetPeriodic(int period, int start_time)
    {
        i_expireTime = start_time;
        i_period = period;
    }

    // Tracker interface
    public void TUpdate(int diff) { i_expireTime -= diff; }
    public bool TPassed() { return i_expireTime <= 0; }
    public void TReset(int diff, int period) { i_expireTime += period > diff ? period : diff; }

    int i_period;
    int i_expireTime;
}
