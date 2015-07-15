namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit
open MonoTouch.Foundation

[<Register("RadioClockViewController")>]
type RadioClockViewController() as this = 
    inherit UIViewController()

    let clock: Clock = new Clock()

    // ==== TOOLBAR
    let addButton(image: string, handler) =
        let btn = new UIBarButtonItem ()
        btn.Image <- new UIImage(image)
        btn.Style <- UIBarButtonItemStyle.Plain
        btn.Clicked.Add(handler)
        btn

    let toolbarItems() = 
        let btn0 = addButton("timer-32.png",       this.SetTimer)
        let btn1 = addButton("alarm_clock-32.png", this.SetAlarm)
        let btn2 = addButton("radio-32.png",       this.ToggleRadio)
        [|btn0; btn1; btn2|]
    // ==== TOOLBAR


    // Release any cached data, images, etc that aren't in use.
    override this.DidReceiveMemoryWarning() = base.DidReceiveMemoryWarning()

    // Perform any additional setup after loading the view, typically from a nib.
    override this.ViewDidLoad() = 
        base.ViewDidLoad()  

        let metaTicker    = new Ticker(Radio.NextMetadata, 0.5, 0.5)
        let newsTicker1   = new Ticker(NewsStation.NextHeadline, 0.5, 0.5)
        let newsTicker2   = new Ticker(NewsStation.NextDescription, 1.0, 6.0)
        let weatherTicker = new Ticker(WeatherStation.NextWeather, 0.0, 0.0)
        let placeHolder1  = new UILabel(TranslatesAutoresizingMaskIntoConstraints = false) // simpler way for place holder?
        let placeHolder2  = new UILabel(TranslatesAutoresizingMaskIntoConstraints = false)
        let background    = new UIImageView(TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Black)
        let astroPics     = new AstroPics(background)

        this.SetToolbarItems(toolbarItems(), true)

        this.View.AddSubview (background)
        this.View.AddSubview (clock)
        this.View.AddSubview (metaTicker)
        this.View.AddSubview (weatherTicker)
        this.View.AddSubview (newsTicker1)
        this.View.AddSubview (newsTicker2)
        this.View.AddSubview (placeHolder1)
        this.View.AddSubview (placeHolder2)
  

        let metrics = new NSDictionary()
        let views = new NSDictionary("weatherTicker", weatherTicker, "metaTicker", metaTicker, "newsTicker1", newsTicker1, "newsTicker2", newsTicker2, "clock", clock, "place1", placeHolder1, "place2", placeHolder2, "back", background)
        let ch0 = NSLayoutConstraint.FromVisualFormat("H:|[back]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch1 = NSLayoutConstraint.FromVisualFormat("V:|[back]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch2 = NSLayoutConstraint.FromVisualFormat("H:|[weatherTicker]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch3 = NSLayoutConstraint.FromVisualFormat("H:|-150-[clock]-150-|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch4 = NSLayoutConstraint.FromVisualFormat("H:|[metaTicker]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch5 = NSLayoutConstraint.FromVisualFormat("H:|[newsTicker1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch6 = NSLayoutConstraint.FromVisualFormat("H:|[newsTicker2]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv7 = NSLayoutConstraint.FromVisualFormat("V:|-30-[weatherTicker(50)][place1][clock(200)][place2(==place1)][metaTicker(50)][newsTicker1(50)][newsTicker2(50)]-20-|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.View.AddConstraints(ch0)
        this.View.AddConstraints(ch1)
        this.View.AddConstraints(ch2)
        this.View.AddConstraints(ch3)
        this.View.AddConstraints(ch4)
        this.View.AddConstraints(ch5)
        this.View.AddConstraints(ch6)
        this.View.AddConstraints(cv7)

        clock.Start()


//    override this.EdgesForExtendedLayout = UIRectEdge.None
//
//    override this.PreferredStatusBarStyle() = UIStatusBarStyle.BlackTranslucent

    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true

    override this.WillRotate(orientation, duration) = 
        Array.ForEach(this.View.Subviews, fun v -> v.SetNeedsLayout(); v.LayoutIfNeeded()) // TODO: how to enforce relayout??
        //this.View.LayoutIfNeeded()

    override this.TouchesBegan(touches, event) =
        this.NavigationController.ToolbarHidden <- false

    override this.ViewWillAppear(animated) =
        base.ViewWillAppear(animated)
        this.NavigationController.NavigationBarHidden <- true

    member this.SetTimer(eventArgs: EventArgs): Unit =
        Console.WriteLine("set timer")
        this.NavigationController.ToolbarHidden <- true

    member this.SetAlarm(eventArgs: EventArgs): Unit =
        let types = UIUserNotificationType.Alert
        let settings = UIUserNotificationSettings.GetSettingsForTypes(types, null)
        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings)
        if clock.IsStopped() then 
            Alarm.Start(new TimeSpan())
            clock.StopBlink()
            clock.Start() 
        else 
            clock.Stop()
            clock.Blink()
        this.NavigationController.ToolbarHidden <- true

    member this.ToggleRadio(eventArgs: EventArgs): Unit =
        if Radio.IsPlaying() then Radio.Stop() else Radio.Play()
        this.NavigationController.ToolbarHidden <- true


