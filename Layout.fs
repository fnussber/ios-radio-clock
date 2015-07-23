namespace RadioClock

open MonoTouch.UIKit
open MonoTouch.Foundation

module Layout =

    let bigFont   = UIFont.FromName("Helvetica-Bold", 100.0f)
    let smallFont = UIFont.FromName("Helvetica-Bold", 20.0f)
    let TinyFont  = UIFont.FromName("Helvetica",      15.0f)

    // https://icons8.com/web-app/for/ios7
    let SleepIcon   = "timer-32.png"
    let AlarmIcon   = "alarm_clock-32.png"
    let RadioIcon   = "radio-32.png"
    let StationIcon = "radio_tower-32.png"
    let MoonIcon    = "weather/moon-50.png"
    let SunIcon     = "weather/sun-50.png"

    let constraintsFromFormat str metrics views =
        NSLayoutConstraint.FromVisualFormat(str, NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views)

    let layout (view: UIView) (formats: list<string>) (views: list<string*UIView>) =
        let metricsDict = new NSDictionary()
        let viewsDict   = new NSMutableDictionary()
        List.map (fun (l, v) -> view.AddSubview(v)) views |> ignore
        List.map (fun (l:string, v) -> (viewsDict.Add(new NSString(l), v))) views |> ignore
        List.map (fun f      -> view.AddConstraints(constraintsFromFormat f metricsDict viewsDict)) formats |> ignore

    let layout2 (view: UIView) (formats: list<string>) (views: list<string*UIView>) =
        let metricsDict = new NSDictionary()
        let viewsDict   = new NSMutableDictionary()
//        List.map (fun (l, v) -> view.AddSubview(v)) views |> ignore
        List.map (fun (l:string, v) -> (viewsDict.Add(new NSString(l), v))) views |> ignore
        List.map (fun f      -> view.AddConstraints(constraintsFromFormat f metricsDict viewsDict)) formats |> ignore

    let label str font =
        new UILabel(
            Text = str,
            Font = font,
            TextColor = UIColor.White,
            BackgroundColor = UIColor.Clear,
            TranslatesAutoresizingMaskIntoConstraints = false)

    let centeredLabel str font =
        let l = label str font
        l.TextAlignment <- UITextAlignment.Center
        l

    let icon (str: string) =
        let img  = new UIImage(str)
        let ico  = img.ImageWithRenderingMode UIImageRenderingMode.AlwaysTemplate
        new UIImageView(Image = ico, TintColor = UIColor.White, TranslatesAutoresizingMaskIntoConstraints = false)

    let button str color =
        let b = UIButton.FromType(UIButtonType.RoundedRect)
        b.TranslatesAutoresizingMaskIntoConstraints <- false
        b.SetTitle(str, UIControlState.Normal)
        b.BackgroundColor <- color
        b.SetTitleColor(UIColor.White, UIControlState.Normal)
        //    setBtn.SetTitleShadowColor(UIColor.Black, UIControlState.Normal)
        b.Font <- smallFont
        b



