namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit
open MonoTouch.Foundation

[<Register("RadioClockViewController")>]
type RadioClockViewController() = 
    inherit UIViewController()

    let background = new UIImageView(TranslatesAutoresizingMaskIntoConstraints = false, BackgroundColor = UIColor.Black)

    let updateBackground (img: UIImage) : Unit = 
        background.InvokeOnMainThread(fun _ ->
            UIView.Transition(background, 3.0, UIViewAnimationOptions.TransitionCrossDissolve, (fun _ -> background.Image <- img), null)
        )

    do 
        // install handler for background updates
        AstroPics.NextPicture.Add(fun pic -> updateBackground pic.image)

    // Release any cached data, images, etc that aren't in use.
    override this.DidReceiveMemoryWarning() =
        System.Console.WriteLine("Received memory warning") 
        base.DidReceiveMemoryWarning()

    // Perform any additional setup after loading the view, typically from a nib.
    override this.ViewDidLoad() = 
        base.ViewDidLoad()  

        let metaTicker    = new Ticker(Radio.NextMetadata, 0.5, 0.5)
        let newsTicker1   = new Ticker(NewsStation.NextHeadline, 0.5, 0.5)
        let newsTicker2   = new Ticker(NewsStation.NextDescription, 1.0, 6.0)
        let weatherTicker = new Ticker(WeatherStation.NextWeather, 0.0, 0.0)
        let placeHolder1  = new UILabel(TranslatesAutoresizingMaskIntoConstraints = false)
        let placeHolder2  = new UILabel(TranslatesAutoresizingMaskIntoConstraints = false)

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
            Alarm.StatusBar
        )

        let views = [
            "alarmStatus",      Alarm.StatusBar
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
       
        Layout.layout2 this.View formats views // TOOD: figure out why layout doesn't work (doesn't add views??)

    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true

    override this.WillRotate(orientation, duration) = 
        Array.ForEach(this.View.Subviews, fun v -> v.SetNeedsLayout(); v.LayoutIfNeeded()) // TODO: how to enforce relayout??
        //this.View.LayoutIfNeeded()

    override this.TouchesBegan(touches, event) =
        // toggle toolbar when user touches screen EXCEPT for the case where the
        //  clock is stopped in order to let the user enter a new alarm time
        if (not (Clock.isStopped())) then
            this.NavigationController.ToolbarHidden <- not this.NavigationController.ToolbarHidden

    override this.ViewWillAppear(animated) =
        base.ViewWillAppear(animated)
        this.NavigationController.NavigationBarHidden <- true



