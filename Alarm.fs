namespace RadioClock

open System
open MonoTouch.Foundation
open MonoTouch.UIKit


module Alarm = 

    let mutable timers: list<UILocalNotification> = []
    let mutable alarms: list<UILocalNotification> = []

    let removeTimer timer = timers <- timers |> List.filter(fun t -> t <> timer)
    let removeAlarm alarm = alarms <- alarms |> List.filter(fun a -> a <> alarm)

    let StartTimer() =
        Console.WriteLine("start timer")
        Radio.Play

    let CancelTimer() =
        Console.WriteLine("cancel timer")

    let DoTimer timer =
        Console.WriteLine("do timer")
        removeTimer timer
        Radio.Stop

    let Start alarmTime =
        Console.WriteLine("start alarm")

        let notif  = new UILocalNotification()
        let fire   = DateTime.Now.AddSeconds(20.0) //:> NSDate
        let nsFire = new NSDate()
        notif.FireDate <- nsFire.AddSeconds(20.0)   // easier way to do that??
        //notif.TimeZone 
        notif.AlertAction <- "Wecki, wecki!"
        notif.AlertBody <- "Hey, an alert went off."
        //notif.SoundName <- UILocalNotification.DefaultSoundName //:> string
        //notif.ApplicationIconBadgeNumber <- 3

        UIApplication.SharedApplication.IdleTimerDisabled <- true
        UIApplication.SharedApplication.ScheduleLocalNotification(notif)


    let Cancel alarm =
        Console.WriteLine("cancel alarm")
        removeAlarm alarm
        UIApplication.SharedApplication.CancelLocalNotification(alarm)
        UIApplication.SharedApplication.IdleTimerDisabled <- false

    let Do alarm =
        Console.WriteLine("do alarm")
        removeAlarm alarm
        UIApplication.SharedApplication.IdleTimerDisabled <- false
        Radio.Play()

