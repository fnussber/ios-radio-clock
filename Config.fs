namespace RadioClock

open System

module Config =

    let sleepTimes: list<string * TimeSpan> = [
        "60 minutes", TimeSpan( 1,  0, 0)
        "30 minutes", TimeSpan( 0, 30, 0)
        "20 minutes", TimeSpan( 0, 20, 0)
        "10 minutes", TimeSpan( 0, 10, 0)
        "5 minutes",  TimeSpan( 0,  5, 0)
        "1 minute",   TimeSpan( 0,  1, 0)
    ]

    let mutable alarmTimes: list<TimeSpan> = [
        TimeSpan( 7,  0, 0) // 07:00
        TimeSpan( 7, 30, 0) // 07:30
        TimeSpan( 8,  0, 0) // 08:00
    ]

