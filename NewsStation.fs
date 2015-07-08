namespace RadioClock

open System
open System.Text
open System.Xml
open MonoTouch.UIKit

module NewsStation = 

    let timer = new System.Timers.Timer(10000.0)
    let NextHeadline    = new Event<string>()
    let NextDescription = new Event<string>()
    let mutable nitems  = list<NewsItem>.Empty

    let headFont  = UIFont.FromName("Helvetica-Bold", 30.0f)
    let storyFont = UIFont.FromName("Helvetica", 20.0f)

    let loadNewsItems(): Unit = 
        // execute load in background and add news items once they are available
        let newestItems = RssFeed.items("http://www.tagesanzeiger.ch/rss.html") // TODO: DO IN BACKGROUND!
        lock nitems (fun _ -> nitems <- nitems |> List.append(newestItems))

    let nextItem(): Unit =
        // produce the next item, if available, empty item if currently no items are available
        if nitems.Length <= 2 then loadNewsItems()
        let next = lock nitems (fun _ -> 
            if nitems.Length > 0 then 
                let n = nitems.Head
                nitems <- nitems.Tail 
                n
            else 
                {head="NONE";desc="NONE";}) // TODO: is there a headOption in F#?
        NextHeadline.Trigger(next.head)
        NextDescription.Trigger(next.desc)

    do
        timer.Elapsed.Add(fun _ -> nextItem())
        timer.Start()

//    member t.NextHeadline = nextHeadline
//    member t.NextDescription = nextDescription

    // TODO: other cleanup needed? Is this needed at all??
//    interface IDisposable with
//        member x.Dispose() = timer.Dispose()

