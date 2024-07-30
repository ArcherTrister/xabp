// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

using Quartz;
using Quartz.Impl.Calendar;
using Quartz.Util;

using X.Abp.Quartz.Dtos;

namespace X.Abp.Quartz.Calendars.Dtos;
public class CalendarDetailDto
{
    protected CalendarDetailDto(ICalendar calendar)
    {
        CalendarType = calendar.GetType().AssemblyQualifiedNameWithoutVersion();
        Description = calendar.Description;
        if (calendar.CalendarBase != null)
        {
            CalendarBase = Create(calendar.CalendarBase);
        }
    }

    public string CalendarType { get; }

    public string Description { get; }

    public CalendarDetailDto CalendarBase { get; }

    public static CalendarDetailDto Create(ICalendar calendar)
    {
        if (calendar is AnnualCalendar annualCalendar)
        {
            return new AnnualCalendarDto(annualCalendar);
        }

        if (calendar is CronCalendar cronCalendar)
        {
            return new CronCalendarDto(cronCalendar);
        }

        return calendar is DailyCalendar dailyCalendar
            ? (global::X.Abp.Quartz.Calendars.Dtos.CalendarDetailDto)new DailyCalendarDto(dailyCalendar)
            : calendar is HolidayCalendar holidayCalendar
            ? (global::X.Abp.Quartz.Calendars.Dtos.CalendarDetailDto)new HolidayCalendarDto(holidayCalendar)
            : calendar is MonthlyCalendar monthlyCalendar
            ? new MonthlyCalendarDto(monthlyCalendar)
            : calendar is WeeklyCalendar weeklyCalendar ? new WeeklyCalendarDto(weeklyCalendar) : new CalendarDetailDto(calendar);
    }

    public class AnnualCalendarDto : CalendarDetailDto
    {
        public AnnualCalendarDto(AnnualCalendar calendar)
            : base(calendar)
        {
            DaysExcluded = calendar.DaysExcluded;
            TimeZone = new TimeZoneDto(calendar.TimeZone);
        }

        public IReadOnlyCollection<DateTime> DaysExcluded { get; }

        public TimeZoneDto TimeZone { get; }
    }

    public class CronCalendarDto : CalendarDetailDto
    {
        public CronCalendarDto(CronCalendar calendar)
            : base(calendar)
        {
            CronExpression = calendar.CronExpression.CronExpressionString;
            TimeZone = new TimeZoneDto(calendar.TimeZone);
        }

        public string CronExpression { get; }

        public TimeZoneDto TimeZone { get; }
    }

    public class DailyCalendarDto : CalendarDetailDto
    {
        public DailyCalendarDto(DailyCalendar calendar)
            : base(calendar)
        {
            InvertTimeRange = calendar.InvertTimeRange;
            TimeZone = new TimeZoneDto(calendar.TimeZone);
        }

        public bool InvertTimeRange { get; }

        public TimeZoneDto TimeZone { get; }
    }

    public class HolidayCalendarDto : CalendarDetailDto
    {
        public HolidayCalendarDto(HolidayCalendar calendar)
            : base(calendar)
        {
            ExcludedDates = calendar.ExcludedDates.ToList();
            TimeZone = new TimeZoneDto(calendar.TimeZone);
        }

        public IReadOnlyList<DateTime> ExcludedDates { get; }

        public TimeZoneDto TimeZone { get; }
    }

    public class MonthlyCalendarDto : CalendarDetailDto
    {
        public MonthlyCalendarDto(MonthlyCalendar calendar)
            : base(calendar)
        {
            DaysExcluded = calendar.DaysExcluded.ToList();
            TimeZone = new TimeZoneDto(calendar.TimeZone);
        }

        public IReadOnlyList<bool> DaysExcluded { get; }

        public TimeZoneDto TimeZone { get; }
    }

    public class WeeklyCalendarDto : CalendarDetailDto
    {
        public WeeklyCalendarDto(WeeklyCalendar calendar)
            : base(calendar)
        {
            DaysExcluded = calendar.DaysExcluded.ToList();
            TimeZone = new TimeZoneDto(calendar.TimeZone);
        }

        public IReadOnlyList<bool> DaysExcluded { get; }

        public TimeZoneDto TimeZone { get; }
    }
}
