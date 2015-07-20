namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit
open MonoTouch.Foundation

[<Register("RadioClockViewController")>]
type RadioClockViewController() = 
    inherit UIViewController()

    // Release any cached data, images, etc that aren't in use.
    override this.DidReceiveMemoryWarning() =
        System.Console.WriteLine("Received memory warning") 
        base.DidReceiveMemoryWarning()

    // Perform any additional setup after loading the view, typically from a nib.
    override this.ViewDidLoad() = 
        base.ViewDidLoad()  

        let alarmStatus   = Alarm.StatusBar() //new UIView(TranslatesAutoresizingMaskIntoConstraints = false)
        let metaTicker    = new Ticker(Radio.NextMetadata, 0.5, 0.5)
        let newsTicker1   = new Ticker(NewsStation.NextHeadline, 0.5, 0.5)
        let newsTicker2   = new Ticker(NewsStation.NextDescription, 1.0, 6.0)
        let weatherTicker = new Ticker(WeatherStation.NextWeather, 0.0, 0.0)
        let placeHolder1  = new UILabel(TranslatesAutoresizingMaskIntoConstraints = false) // simpler way for place holder?
        let placeHolder2  = new UILabel(TranslatesAutoresizingMaskIntoConstraints = false)
        let background    = new UIImageView(TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Black)
        let astroPics     = new AstroPics(background)

        this.SetToolbarItems(Toolbar.toolbarItems(this), true)

        this.View.AddSubviews(
            background, 
            Clock.face,
            metaTicker,
            weatherTicker,
            newsTicker1,
            newsTicker2,
            placeHolder1,
            placeHolder2,
            alarmStatus
        )

        let views = [
            "alarmStatus",      alarmStatus
            "weatherTicker",    weatherTicker   :> UIView
            "metaTicker",       metaTicker      :> UIView 
            "newsTicker1",      newsTicker1     :> UIView
            "newsTicker2",      newsTicker2     :> UIView
            "clock",            Clock.face 
            "place1",           placeHolder1    :> UIView 
            "place2",           placeHolder2    :> UIView 
            "back",             background      :> UIView
        ]

        let formats = [
            "H:|[back]|"
            "H:|[weatherTicker]|"
            "H:|-150-[clock]-150-|"
            "H:|[metaTicker]|"
            "H:|[newsTicker1]|"
            "H:|[newsTicker2]|"
            "H:[alarmStatus]-30-|"
            "V:|[back]|"
            "V:|-30-[alarmStatus(50)]"
            "V:|-30-[weatherTicker(50)][place1][clock(200)][place2(==place1)][metaTicker(50)][newsTicker1(50)][newsTicker2(50)]-20-|"
          ]

       
        Layout.layout2 this.View formats views

        Clock.start()


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


