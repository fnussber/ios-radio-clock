namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit
open MonoTouch.Foundation

[<Register("RadioClockViewController")>]
type RadioClockViewController() = 
    inherit UIViewController()

    let background = 
        new UIImageView(
            TranslatesAutoresizingMaskIntoConstraints = false, 
            BackgroundColor = UIColor.Black, 
            ContentMode = UIViewContentMode.ScaleAspectFill)

    let metaTicker    = new Ticker(Radio.NextMetadata, 0.5)
    let newsTicker1   = new Ticker(NewsStation.NextHeadline, 0.5)
    let newsTicker2   = new Ticker(NewsStation.NextDescription, 1.0)
    let weatherTicker = new Ticker(WeatherStation.NextWeather, 0.5)
    let astroPicTitle = Layout.centeredLabel "" Layout.smallFont
    let placeHolder1  = Layout.label "" Layout.smallFont
    let placeHolder2  = Layout.label "" Layout.smallFont
    let placeHolder3  = Layout.label "" Layout.smallFont
    let placeHolder4  = Layout.label "" Layout.smallFont

    let updateAstroPic (pic: AstroPic) : Unit = 
        background.InvokeOnMainThread(fun _ ->
            UIView.Transition(background,    3.0, UIViewAnimationOptions.TransitionCrossDissolve, (fun _ -> background.Image   <- pic.image), null)
            UIView.Transition(astroPicTitle, 3.0, UIViewAnimationOptions.TransitionCrossDissolve, (fun _ -> astroPicTitle.Text <- pic.title), null)
        )

    do 
        // at startup hide the media player meta information ticker
        metaTicker.Hidden <- true

        // install handler for background updates
        AstroPics.NextPicture.Add(fun pic -> updateAstroPic pic)

        // turn meta ticker with media information on/off
        Radio.TurnOn.Publish.Add (fun pic -> metaTicker.Hidden <- false)
        Radio.TurnOff.Publish.Add(fun pic -> metaTicker.Hidden <- true)

    // Release any cached data, images, etc that aren't in use.
    override this.DidReceiveMemoryWarning() =
        System.Console.WriteLine("Received memory warning") 
        base.DidReceiveMemoryWarning()

    // Perform any additional setup after loading the view, typically from a nib.
    override this.ViewDidLoad() = 
        base.ViewDidLoad()  

        this.SetToolbarItems(Toolbar.toolbarItems(this), true)

        let views = [
            "back",             background      :> UIView
            "astroPicTitle",    astroPicTitle   :> UIView
            "clock",            Clock.face 
            "metaTicker",       metaTicker      :> UIView 
            "weatherTicker",    weatherTicker   :> UIView
            "newsTicker1",      newsTicker1     :> UIView
            "newsTicker2",      newsTicker2     :> UIView
            "place1",           placeHolder1    :> UIView 
            "place2",           placeHolder2    :> UIView 
            "place3",           placeHolder3    :> UIView 
            "place4",           placeHolder4    :> UIView 
            "alarmStatus",      Alarm.StatusBar
        ]

        let formats = [
            "H:|[back]|"
            "H:|[weatherTicker]|"
            "H:|[astroPicTitle]|"
            "H:|[place3][clock(600)][place4(==place3)]|"
            "H:|[metaTicker]|"
            "H:|[newsTicker1]|"
            "H:|[newsTicker2]|"
            "H:[alarmStatus]-30-|"
            "V:|[back]|"
            "V:|-30-[alarmStatus]"
            "V:|-30-[weatherTicker(50)]-[astroPicTitle][place1][clock(200)][place2(==place1)][metaTicker(50)][newsTicker1(50)][newsTicker2(50)]-20-|"
          ]
       
        Layout.layout this.View formats views

    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true

    override this.WillRotate(orientation, duration) = 
        Array.ForEach(this.View.Subviews, fun v -> v.SetNeedsLayout(); v.LayoutIfNeeded())

    override this.TouchesBegan(touches, event) =
        // toggle toolbar when user touches screen EXCEPT for the case where the
        // clock is stopped in order to let the user enter a new alarm time
        if (not (Clock.isStopped())) then
            this.NavigationController.ToolbarHidden <- not this.NavigationController.ToolbarHidden

    override this.ViewWillAppear(animated) =
        base.ViewWillAppear(animated)
        this.NavigationController.NavigationBarHidden <- true



