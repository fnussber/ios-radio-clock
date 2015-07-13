namespace RadioClock

open System
open System.Text
open System.Xml
open MonoTouch.UIKit

module NewsStation = 

    let timer = new System.Timers.Timer(10000.0)
    let NextHeadline    = new Event<UIView>()
    let NextDescription = new Event<UIView>()
    let mutable nitems  = list<NewsItem>.Empty

    let headFont = UIFont.FromName("Helvetica-Bold", 30.0f)
    let descFont = UIFont.FromName("Helvetica", 20.0f)

    let label (str: String, font: UIFont): UILabel =
        new UILabel(Text = str, Font = font, TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White)

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
        headFont.InvokeOnMainThread(fun _ ->
            NextHeadline.Trigger(label(next.head, headFont))
            NextDescription.Trigger(label(next.desc, descFont))
        )


    do
        timer.Elapsed.Add(fun _ -> nextItem())
        timer.Start()
