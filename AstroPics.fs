namespace RadioClock

open System
open System.Net
open MonoTouch.UIKit
open MonoTouch.Foundation

// rumors have it using Xamarin.Forms would make this a tad bit simpler... this is how we do it without
// RSS http://www.acme.com/jef/apod/rss.xml
type AstroPics(view: UIImageView) =

    let webClient   = new WebClient()    
    let timer       = new System.Timers.Timer(10000.0)
    let mutable nxtImage: UIImage = null
    let mutable img = 0

    let updatePicture(): Unit =
        // todo: nxtImage could be null here..
        view.InvokeOnMainThread(new NSAction(fun _ ->
            UIView.Transition(view, 3.0, UIViewAnimationOptions.TransitionCrossDissolve, new NSAction(fun _ -> view.Image <- nxtImage), null)
        ))

    let nextPicture(): Unit =
        updatePicture()
        let images = ["http://apod.nasa.gov/apod/image/1506/sh155walter_z66.jpg"; "http://apod.nasa.gov/apod/image/1506/PoseidonMW_Maragos_960.jpg"; "http://apod.nasa.gov/apod/image/1506/FlashMoon_OT_from_ORM_140km-DLopez_840mm600.jpg"]
        webClient.DownloadDataAsync(new Uri(List.nth images (img % images.Length)))
        img <- img + 1

    let pictureLoaded(bytes: byte[]): Unit =
        if (bytes.Length > 0) then
            nxtImage <- new UIImage(NSData.FromArray(bytes))


    do
        // install callback
        webClient.DownloadDataCompleted.Add(fun s -> pictureLoaded(s.Result))

        // start timer
        timer.Elapsed.Add(fun _ -> nextPicture())
        timer.Start()
