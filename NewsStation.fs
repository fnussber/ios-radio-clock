namespace RadioClock

open System
open System.Text
open System.Threading
open System.Xml
open MonoTouch.UIKit

module NewsStation = 

    let NextHeadline    = new Event<UIView>()
    let NextDescription = new Event<UIView>()

    let nextSegment     = {head="****";desc="****";}
    let mutable nitems  = [nextSegment]

    let headFont = UIFont.FromName("Helvetica-Bold", 30.0f)
    let descFont = UIFont.FromName("Helvetica", 20.0f)

    let label (str: String, font: UIFont): UILabel =
        new UILabel(Text = str, Font = font, TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White)

    let nextNews (): option<NewsItem> =
        lock nitems (fun _ ->
            let next = List.tryPick Some nitems
            nitems <- nitems.Tail
            next
        )

    // TODO: loading could be started several times in case it takes too long, can we easily avoid that?
    let produceNews (): Unit = 
        // if there are no more news items left then load a new batch of news in the background
        if (nitems.Length <= 1) then Async.Start ( async {
            let newestItems = List.append (RssFeed.items("http://www.tagesanzeiger.ch/rss.html")) [nextSegment]
            lock nitems (fun _ -> nitems <- nitems |> List.append(newestItems))
        })

    let consumeNews (): Unit =
        match nextNews() with 
        | None   -> ()
        | Some n ->
            // create labels and update UI on main thread
            headFont.InvokeOnMainThread(fun _ ->
                NextHeadline.Trigger   (label(n.head, headFont))
                NextDescription.Trigger(label(n.desc, descFont))
            )

    do
        Async.Start (async {
            while true do
                produceNews()
                consumeNews()
                Thread.Sleep(10000)

        }) |> ignore
