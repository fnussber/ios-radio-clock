namespace RadioClock

open System
open MonoTouch.UIKit
open MonoTouch.Foundation

module Toolbar =

    let timerEvent = new Event<option<TimeSpan>>()
    let alarmEvent = new Event<option<TimeSpan>>()
    let usralEvent = new Event<string>()
    let radioEvent = new Event<string>()

    let timerButton = timerEvent.Publish
    let alarmButton = alarmEvent.Publish
    let radioButton = radioEvent.Publish

    let button image handler =
        let btn = new UIBarButtonItem()
        btn.Image <- new UIImage(image: string)
        btn.Style <- UIBarButtonItemStyle.Plain
        btn.Clicked.Add(handler(btn)) // note: handler needs to know button in order to display popup menu
        btn

    let timerMenuEntry str span =
        UIAlertAction.Create(str, UIAlertActionStyle.Default, (fun _ -> timerEvent.Trigger span))

    let timerMenu ctrl btn = 
        let m = UIAlertController.Create("Timer", "Select Remaining Time", UIAlertControllerStyle.ActionSheet)  
        m.PopoverPresentationController.BarButtonItem <- btn 
        List.map (fun (l, t) ->
            m.AddAction(timerMenuEntry l (Some(t))) 
        ) Config.sleepTimes |> ignore
        m.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, (fun _ -> timerEvent.Trigger None))) 
        (ctrl: UIViewController).PresentViewController(m, true, null)

    let alarmMenuEntry str span =
        UIAlertAction.Create(str, UIAlertActionStyle.Default, (fun _ -> alarmEvent.Trigger span))

    let alarmMenu ctrl btn = 
        let m = UIAlertController.Create("Alarm", "Select Alarm Time", UIAlertControllerStyle.ActionSheet)  
        m.PopoverPresentationController.BarButtonItem <- btn 
        List.sort Config.alarmTimes 
        |> List.map (fun t ->
                m.AddAction(alarmMenuEntry (t.ToString("hh\:mm")) (Some(t))) 
            ) 
        |> ignore
        m.AddAction(UIAlertAction.Create("New",   UIAlertActionStyle.Default, (fun _ -> usralEvent.Trigger ""))) 
        m.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, (fun _ -> alarmEvent.Trigger None))) 
        (ctrl: UIViewController).PresentViewController(m, true, null)

    let toolbarItems (ctrl)  = [|
            button Layout.SleepIcon (fun btn _ -> timerMenu ctrl btn) 
            button Layout.AlarmIcon (fun btn _ -> alarmMenu ctrl btn)
            button Layout.RadioIcon (fun btn _ -> 
                radioEvent.Trigger ""
                ctrl.NavigationController.ToolbarHidden <- true
            )
        |]


