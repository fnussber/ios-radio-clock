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
        let newsTicker = new Ticker(news.Stories())
        let weatherTicker = new Ticker(weather.Weather())

        this.View.BackgroundColor <- UIColor.Black
        this.View.AddSubview (clock)
        this.View.AddSubview (weatherTicker)
        this.View.AddSubview (newsTicker)
        this.View.AddSubview (toolbar)
  

        let metrics = new NSDictionary()
        let views = new NSDictionary("toolbar", toolbar, "newsTicker", newsTicker, "weatherTicker", weatherTicker, "clock", clock)
        let ch0 = NSLayoutConstraint.FromVisualFormat("H:|[clock]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch1 = NSLayoutConstraint.FromVisualFormat("H:|[weatherTicker]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch2 = NSLayoutConstraint.FromVisualFormat("H:|[newsTicker]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch3 = NSLayoutConstraint.FromVisualFormat("H:|[toolbar]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv = NSLayoutConstraint.FromVisualFormat("V:|[clock][newsTicker(50)][weatherTicker(50)][toolbar(44)]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.View.AddConstraints(ch0)
        this.View.AddConstraints(ch1)
        this.View.AddConstraints(ch2)
        this.View.AddConstraints(ch3)
        this.View.AddConstraints(cv)

        clock.Start()

    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true



