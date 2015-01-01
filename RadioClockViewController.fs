namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit
open MonoTouch.Foundation

[<Register("RadioClockViewController")>]
type RadioClockViewController() = 
    inherit UIViewController()

    member this.Radio = new Radio()

    // Release any cached data, images, etc that aren't in use.
    override this.DidReceiveMemoryWarning() = base.DidReceiveMemoryWarning()

    // Perform any additional setup after loading the view, typically from a nib.
    override this.ViewDidLoad() = 
        base.ViewDidLoad()

        let weather = new WeatherStation()
        let news = new NewsStation()

        let clock = new Clock()
        let toolbar = new Toolbar (clock)
        let newsTicker1 = new Ticker(news.Heads(), 0.5, 11.0)
        let newsTicker2 = new Ticker(news.Stories(), 6.0, 0.0)
        let weatherTicker = new Ticker(weather.Weather(), 0.5, 60.0)
        let placeHolder1 = new UIView(TranslatesAutoresizingMaskIntoConstraints = false) // simpler way for place holder?
        let placeHolder2 = new UIView(TranslatesAutoresizingMaskIntoConstraints = false)

        this.View.BackgroundColor <- UIColor.Black
        this.View.AddSubview (clock)
        this.View.AddSubview (weatherTicker)
        this.View.AddSubview (newsTicker1)
        this.View.AddSubview (newsTicker2)
        this.View.AddSubview (toolbar)
        this.View.AddSubview (placeHolder1)
        this.View.AddSubview (placeHolder2)
  

        let metrics = new NSDictionary()
        let views = new NSDictionary("toolbar", toolbar, "newsTicker1", newsTicker1, "newsTicker2", newsTicker2, "weatherTicker", weatherTicker, "clock", clock, "place1", placeHolder1, "place2", placeHolder2)
        let ch0 = NSLayoutConstraint.FromVisualFormat("H:|[clock]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch1 = NSLayoutConstraint.FromVisualFormat("H:|[weatherTicker]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch2 = NSLayoutConstraint.FromVisualFormat("H:|[newsTicker1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch3 = NSLayoutConstraint.FromVisualFormat("H:|[newsTicker2]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch4 = NSLayoutConstraint.FromVisualFormat("H:|[toolbar]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv = NSLayoutConstraint.FromVisualFormat("V:|[place1][clock(200)][place2(==place1)][newsTicker1(50)][newsTicker2(50)][weatherTicker(50)][toolbar(44)]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.View.AddConstraints(ch0)
        this.View.AddConstraints(ch1)
        this.View.AddConstraints(ch2)
        this.View.AddConstraints(ch3)
        this.View.AddConstraints(ch4)
        this.View.AddConstraints(cv)

        clock.Start()

    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true

    override this.WillRotate(orientation, duration) = 
        Array.ForEach(this.View.Subviews, fun v -> v.SetNeedsLayout(); v.LayoutIfNeeded()) // TODO: how to enforce relayout??
        //this.View.LayoutIfNeeded()



