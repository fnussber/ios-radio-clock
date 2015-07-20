namespace RadioClock

open System
open System.Threading
open MonoTouch.AVFoundation
open MonoTouch.Foundation
open MonoTouch.UIKit

type Observer(event: Event<UIView>) =
    inherit NSObject()
    override this.ObserveValue(key: NSString, obj: NSObject, change: NSDictionary, context: IntPtr) = 
        Console.WriteLine("+++ item metadata changed +++")
        match obj with
        | :? AVPlayerItem as i ->
            if i.TimedMetadata <> null then
                let s = i.TimedMetadata.[0].ValueForKey(new NSString("value")).ToString()
                let label = new UILabel(Text = s, Font = UIFont.FromName("Helvetica", 20.0f), TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White, BackgroundColor = UIColor.Clear)
                this.InvokeOnMainThread(fun _ -> event.Trigger(label))
        | _ -> ()

module Radio = 

    let NextMetadata = new Event<UIView>()

    let private observer = new Observer(NextMetadata) // simpler way to do this using delegates?

    let mutable player: Option<AVPlayer> = None
    let mutable private station: Option<Station> = Some(Config.stations.[0])//None

    let AddStation s = 
        Config.stations <- s :: Config.stations

    let RemoveStation sd = 
        Config.stations <- List.filter (fun s -> s <> sd) Config.stations

    let Stations() = 
        Config.stations |> List.sortBy (fun s -> s.Name)

    let Mute() =
        player |> Option.map(fun p -> p.Volume <- 0.0f) |> ignore


    let IsPlaying() = player <> None

    let Play (station: Station) = 
        let item = new AVPlayerItem(station.Url)
        item.AddObserver(observer, "timedMetadata", NSKeyValueObservingOptions.New + NSKeyValueObservingOptions.Initial, IntPtr(0))
        let avplayer = new AVPlayer(item)
        avplayer.Play()
        player <- Some(avplayer)

    let Stop() =
        match player with
            | Some p -> 
                player <- None
                p.Pause()
                p.CurrentItem.RemoveObserver(observer, "timedMetadata")
                p.Dispose()
            | None -> 
                ()

    let fadeVolume (t: Timers.Timer) d =
        match player with
            | Some p ->
                if      (p.Volume + d) < 0.0f then t.Stop(); Stop()
                else if (p.Volume + d) > 1.0f then p.Volume <- 1.0f; t.Stop()
                else                               p.Volume <- p.Volume + d
            | None   ->
                t.Stop()

    let fade d =
        let t = new System.Timers.Timer(1000.0)
        t.Elapsed.Add(fun _ -> 
            System.Console.WriteLine("fade")
            fadeVolume t d
        )
        t.Start()

    let fadeIn() =
        Play(Config.stations.Head)
        Mute()
        fade(1.0f/Config.fadeInDuration)

    let fadeOut() =
        fade(-1.0f/Config.fadeOutDuration)

    do
        Toolbar.radioButton.Add (fun _ ->
            if IsPlaying() then Stop() else Play(Config.stations.Head)
        )

        Toolbar.stationButton.Add (fun s ->
            Stop()
            Play(s)
        )

