namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

type Digit(digit: String) as self =
    inherit UILabel()

    let font = UIFont.FromName("Helvetica", 100.0f)
    let duration = 1.0

    do
        self.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        self.Font <- font
        self.Text <- digit
        self.TextAlignment <- UITextAlignment.Center
        self.TextColor <- UIColor.Black

    member this.AnimateIn() =

        // --- scale animation
//        let scaleAnim = CABasicAnimation.FromKeyPath("transform.scale")
//        scaleAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
//        scaleAnim.Duration <- 0.5
//        scaleAnim.RepeatCount <- 8.0f
//        scaleAnim.AutoReverses <- true
//        scaleAnim.RemovedOnCompletion <- true
//        scaleAnim.To <- NSNumber.FromDouble(1.2) //new NSObject(CATransform3D.MakeScale(1.2f, 1.2f, 1.0f))
//        self.Layer.AddAnimation(scaleAnim, "scaleAnim")

        // --- rotate animation
        let rotAnim = CABasicAnimation.FromKeyPath("transform.rotation")
        rotAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        rotAnim.Duration <- 1.0
        //rotAnim.RepeatCount <- 8.0f
        rotAnim.AutoReverses <- false
        //rotAnim.RemovedOnCompletion <- true
        rotAnim.From <- NSNumber.FromDouble(Math.PI/2.0)
        rotAnim.To <- NSNumber.FromDouble(0.0)
        self.Layer.AddAnimation(rotAnim, "rotAnim")

        // --- animate along curve
        let path = new UIBezierPath()
        path.MoveTo(new PointF(this.Center.X - 300.0f, this.Center.Y + 200.0f))
        path.AddQuadCurveToPoint(this.Center, new PointF(this.Center.X, this.Center.Y + 200.0f))
        let anim = CAKeyFrameAnimation.GetFromKeyPath("position")
        anim.Duration <- 1.0
        anim.Path <- path.CGPath
        anim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut)
        //anim.RotationMode <- CAKeyFrameAnimation.RotateModeAuto.ToString()
        //anim.RemovedOnCompletion <- true
        self.Layer.AddAnimation(anim, "moveAnim")

        // --- bang animation
//        let rotAnim2 = CABasicAnimation.FromKeyPath("transform.rotation")
//        rotAnim2.TimeOffset <- 1.0
//        rotAnim2.Duration <- 0.3
//        rotAnim2.RepeatCount <- 2.0f
//        rotAnim2.AutoReverses <- true
//        rotAnim2.RemovedOnCompletion <- true
//        rotAnim2.From <- NSNumber.FromDouble(0.0)
//        rotAnim2.To <- NSNumber.FromDouble(-Math.PI/10.0)
//        self.Layer.AddAnimation(rotAnim2, "rotAnim2")

        // --- fade out
        self.Alpha <- 0.0f
        UIView.Animate(0.9, 0.0, UIViewAnimationOptions.CurveEaseIn, new NSAction(fun () -> self.Alpha <- 1.0f), null)

  
    member this.AnimateOut() =
        // --- rotate animation
        let rotAnim = CABasicAnimation.FromKeyPath("transform.rotation")
        rotAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        rotAnim.Duration <- 1.0
        //rotAnim.RepeatCount <- 8.0f
        rotAnim.AutoReverses <- false
        rotAnim.RemovedOnCompletion <- true
        rotAnim.From <- NSNumber.FromDouble(0.0)
        rotAnim.To <- NSNumber.FromDouble(Math.PI/2.0)
        self.Layer.AddAnimation(rotAnim, "rotAnim")

        // --- animate along curve
        let path = new UIBezierPath()
        path.MoveTo(this.Center)
        path.AddQuadCurveToPoint(new PointF(this.Center.X + 300.0f, this.Center.Y - 200.0f), new PointF(this.Center.X, this.Center.Y - 200.0f))
        let anim = CAKeyFrameAnimation.GetFromKeyPath("position")
        anim.Duration <- 1.0
        anim.Path <- path.CGPath
        anim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn)
        //anim.RotationMode <- CAKeyFrameAnimation.RotateModeAuto.ToString()
        anim.RemovedOnCompletion <- true
        self.Layer.AddAnimation(anim, "moveAnim")

        // --- fade out
        self.Alpha <- 1.0f
        UIView.Animate(0.9, 0.0, UIViewAnimationOptions.CurveEaseOut, new NSAction(fun () -> self.Alpha <- 0.0f), null)


type Digit2() as this =
    inherit UIView()

    let mutable d = 0
    let label0 = new Digit(d.ToString())
    let label1 = new Digit(d.ToString())

    do
        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        this.AddSubview(label0)
        this.AddSubview(label1)
        let metrics = new NSDictionary()
        let views = new NSDictionary("l0", label0, "l1", label1)
        let ch0 = NSLayoutConstraint.FromVisualFormat("H:|[l0]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch1 = NSLayoutConstraint.FromVisualFormat("H:|[l1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv0 = NSLayoutConstraint.FromVisualFormat("V:|[l0]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv1 = NSLayoutConstraint.FromVisualFormat("V:|[l1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.AddConstraints(ch0)
        this.AddConstraints(ch1)
        this.AddConstraints(cv0)
        this.AddConstraints(cv1)


    member this.Digit = d
    member this.Label0 = label0
    member this.Label1 = label1

    member this.Next(next: int) =
        label0.InvokeOnMainThread(new NSAction(fun _ ->
                label0.AnimateOut()
                label1.AnimateIn()
                label0.Text <- d.ToString()
                label1.Text <- (next.ToString())
                d <- next
                )
            )


type Clock() as this =
    inherit UIView()

    let h1 = new Digit2()
    let h0 = new Digit2()
//    let c0 = new UILabel(Text=":") TODO: add colons! as separators
    let m1 = new Digit2()
    let m0 = new Digit2()
//    let c1 = new UILabel(Text=":")
    let s1 = new Digit2()
    let s0 = new Digit2()

    do
        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        this.AddSubview(h1) // use list..
        this.AddSubview(h0)
        this.AddSubview(m1)
        this.AddSubview(m0)
        this.AddSubview(s1)
        this.AddSubview(s0)
        let metrics = new NSDictionary()
        let views = new NSDictionary("h1", h1, "h0", h0, "m1", m1, "m0", m0, "s1", s1, "s0", s0)
        let ch = NSLayoutConstraint.FromVisualFormat("H:|[h1][h0(==h1)][m1(==h0)][m0(==m1)][s1(==m0)][s0(==s1)]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv1 = NSLayoutConstraint.FromVisualFormat("V:|[h1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv2 = NSLayoutConstraint.FromVisualFormat("V:|[h0]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv3 = NSLayoutConstraint.FromVisualFormat("V:|[m1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv4 = NSLayoutConstraint.FromVisualFormat("V:|[m0]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv5 = NSLayoutConstraint.FromVisualFormat("V:|[s1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv6 = NSLayoutConstraint.FromVisualFormat("V:|[s0]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.AddConstraints(ch)
        this.AddConstraints(cv1)
        this.AddConstraints(cv2)
        this.AddConstraints(cv3)
        this.AddConstraints(cv4)
        this.AddConstraints(cv5)
        this.AddConstraints(cv6)

        this.DoIt()

    member this.DoIt() =
        let timer = new System.Timers.Timer(1000.0)
        timer.Elapsed.Add(fun _ -> this.Pulse())
        timer.Start()

    member this.Pulse() =
        let now = System.DateTime.Now
        let (nh1, nh0) = (now.Hour / 10, now.Hour % 10)
        let (nm1, nm0) = (now.Minute / 10, now.Minute % 10)
        let (ns1, ns0) = (now.Second / 10, now.Second % 10)
        if s0.Digit <> ns0 then s0.Next(ns0) 
        if s1.Digit <> ns1 then s1.Next(ns1) 
        if m0.Digit <> nm0 then m0.Next(nm0) 
        if m1.Digit <> nm1 then m1.Next(nm1) 
        if h0.Digit <> nh0 then h0.Next(nh0) 
        if h1.Digit <> nh1 then h1.Next(nh1) 



         


