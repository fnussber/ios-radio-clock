namespace RadioClock

open System
open MonoTouch.UIKit
open MonoTouch.Foundation

module Toolbar =

    let private timerEvent   = new Event<option<TimeSpan>>()
    let private alarmEvent   = new Event<option<TimeSpan>>()
    let private stationEvent = new Event<Station>()
    let private usralEvent   = new Event<string>()
    let private radioEvent   = new Event<string>()

    let timerButton = timerEvent.Publish
    let alarmButton = alarmEvent.Publish
    let stationButton = stationEvent.Publish
    let radioButton = radioEvent.Publish
    let userAlarmCreate = usralEvent.Publish

    let button image handler =
        let btn = 
            new UIBarButtonItem(
                Image = new UIImage(image: string), 
                Style = UIBarButtonItemStyle.Plain
            )
        btn.Clicked.Add(handler(btn)) // note: handler needs to know button in order to display popup menu
        btn

    let menu title msg actions btn (ctrl: UIViewController) =
        let m = UIAlertController.Create(title, msg, UIAlertControllerStyle.ActionSheet)  
        m.ModalInPopover <- true
        m.PopoverPresentationController.BarButtonItem <- btn 
        List.map (fun a -> m.AddAction a) actions |> ignore
        ctrl.PresentViewController(m, true, null)
        m

    let menuEntry str span (event: Event<_>) =
        UIAlertAction.Create(str, UIAlertActionStyle.Default, (fun _ -> event.Trigger span))

    let timerMenu (ctrl: UIViewController) btn = 
        let actions = List.map (fun (l: string, t:TimeSpan) -> menuEntry l (Some t) timerEvent) Config.sleepTimes
        let m = menu "Timer" "Select Remaining Time" actions btn ctrl
        m.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, (fun _ -> timerEvent.Trigger None))) 

    let alarmMenu (ctrl: UIViewController) btn = 
        let actions = List.map (fun (t: TimeSpan) -> menuEntry (t.ToString("hh\:mm")) (Some t) alarmEvent) Config.alarmTimes
        let m = menu "Alarm" "Set Alarm Time" actions btn ctrl
        m.AddAction(UIAlertAction.Create("New",    UIAlertActionStyle.Default, (fun _ -> usralEvent.Trigger ""))) 
        m.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, (fun _ -> alarmEvent.Trigger None))) 

    let stationMenu (ctrl: UIViewController) btn = 
        let actions = List.map (fun (s: Station) -> menuEntry (s.Name) s stationEvent) Config.stations
        menu "Station" "Select Radio Station" actions btn ctrl |> ignore

    let toolbarItems (ctrl: UIViewController)  = 
        // hide toolbar when any of the buttons/menu actions has been selected (i.e. an even fired)
        timerButton.Add  (fun _ -> ctrl.NavigationController.ToolbarHidden <- true)
        alarmButton.Add  (fun _ -> ctrl.NavigationController.ToolbarHidden <- true)
        stationButton.Add(fun _ -> ctrl.NavigationController.ToolbarHidden <- true)
        radioButton.Add  (fun _ -> ctrl.NavigationController.ToolbarHidden <- true)
        userAlarmCreate.Add  (fun _ -> ctrl.NavigationController.ToolbarHidden <- true)

        // create toolbar buttons, note that the menus are created "on-the-fly", i.e. they
        // will reflect changes to the underlying data that represents the selectable items
        [|
            button Layout.SleepIcon   (fun btn _ -> timerMenu   ctrl btn) 
            button Layout.AlarmIcon   (fun btn _ -> alarmMenu   ctrl btn)
            button Layout.StationIcon (fun btn _ -> stationMenu ctrl btn)
            button Layout.RadioIcon   (fun btn _ -> radioEvent.Trigger "")
        |]
       

