namespace RadioClock

open System
open System.Net
open System.Text.RegularExpressions
open MonoTouch.UIKit
open MonoTouch.Foundation

// rumors have it using Xamarin.Forms would make this a tad bit simpler... this is how we do it without
// RSS http://www.acme.com/jef/apod/rss.xml
type AstroPics(view: UIImageView) =

    let webClient   = new WebClient()    
    let timer       = new System.Timers.Timer(10000.0)
    let mutable nxtImage: UIImage = null
    let mutable images: list<string> = List.empty
    let mutable img = 0

    let updatePicture(): Unit =
        // todo: nxtImage could be null here..
        view.InvokeOnMainThread(fun _ ->
            using (nxtImage) (fun _ ->
                UIView.Transition(view, 3.0, UIViewAnimationOptions.TransitionCrossDissolve, (fun _ -> view.Image <- nxtImage), null)
            )
        )

    let loadPicItems(): Unit = 
        // execute load in background and add news items once they are available
        let newestItems = RssFeed.items("http://www.acme.com/jef/apod/rss.xml") 
        ()
        //lock nitems (fun _ -> nitems <- nitems |> List.append(newestItems))



    let imageSource (input: string): list<string> =
        let m = Regex.Match(input, "img src=\"([^\"]*)") 
        if (m.Success) then [m.Groups.Item(1).Value] else [] // group 0 is the whole matched expression, first group is 1

    let imageSources (items: list<NewsItem>): list<string> =   
        List.map (fun i -> imageSource(i.desc)) items |> List.concat // flatten list in case regexp didn't match

    let nextPicture(): Unit =
        img <- img + 1  // TODO: need error handling in case Download fails, to be sure inc first
        updatePicture()
        webClient.DownloadDataAsync(new Uri(List.nth images (img % images.Length)))  // TODO fails if images.Length is 0

    let pictureLoaded (bytes: byte[]): Unit =
        if (bytes.Length > 0) then
            nxtImage <- new UIImage(NSData.FromArray(bytes))

    do
        // get pictures // TODO: update pics
        let items = RssFeed.items("http://www.acme.com/jef/apod/rss.xml")
        images <- imageSources(items)

        // install callback
        webClient.DownloadDataCompleted.Add(fun s -> pictureLoaded(s.Result))

        // show a first picture and start timer
        timer.Elapsed.Add(fun _ -> nextPicture())
        timer.Start()
