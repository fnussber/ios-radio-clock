namespace RadioClock

open System
open System.Threading
open MonoTouch.AVFoundation
open MonoTouch.Foundation

type Station(name: String, url: NSUrl) =
    member this.Name = name
    member this.Url = url
    new (name: String, urlString: String) = Station(name, new NSUrl(urlString))

module Radio = 

    let mutable private stations = [
        new Station("DRS1", "http://stream.srg-ssr.ch/drs1/mp3_128.m3u")
        new Station("DRS2", "http://stream.srg-ssr.ch/drs2/mp3_128.m3u")
        new Station("DRS3", "http://stream.srg-ssr.ch/drs3/mp3_128.m3u") ]

    let mutable player: Option<AVPlayer> = None
    let mutable private station: Option<Station> = Some(stations.[0])//None

    let observeChange(u: NSObservedChange) =
        Console.WriteLine("+++ item metadata changed +++")
        player 
            |> Option.map(fun p  -> p.CurrentItem.TimedMetadata) 
            |> Option.map(fun is -> Array.ForEach(is, fun i -> Console.WriteLine(i.ToString))) 
            |> ignore

    let AddStation s = 
        stations <- s :: stations

    let RemoveStation sd = 
        stations <- List.filter (fun s -> s <> sd) stations

    let Stations() = 
        stations |> List.sortBy (fun s -> s.Name)

    let VolumeUp() u =
        player |> Option.map(fun p -> p.Volume = Math.Min(1.0f, p.Volume + u))

    let VolumeDown() d =
        player |> Option.map(fun p -> p.Volume = Math.Max(0.0f, p.Volume - d))

    let Mute() =
        player |> Option.map(fun p -> p.Volume = 0.0f)

    let IsPlaying() = player <> None

    let Play() = 
        let item = new AVPlayerItem(new NSUrl("http://stream.srg-ssr.ch/drs3/mp3_128.m3u"))
        item.AddObserver("timedMetadata", NSKeyValueObservingOptions.Prior + NSKeyValueObservingOptions.Initial, observeChange) |> ignore// observer: Action<NSObservedChange>
        let avplayer = new AVPlayer(item)
        avplayer.Play()
        player <- Some(avplayer)
//        // TODO: don't start again if already playing..
//        player <- 
//            match station with
//                | Some s ->
//                    let p = new AVPlayer(new NSUrl("http://stream.srg-ssr.ch/drs3/mp3_128.m3u"))
//                    //Thread.Sleep(2000)
//                    //p.Volume <- 1.0f
//                    //p.ReplaceCurrentItemWithPlayerItem
//                    //view.Layer.AddSublayer(AVPlayerLayer.FromPlayer(p))
//                    //AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback) |> ignore
//                    p.Play()
//                    Some(p)
//                | None ->
//                    None

    let Check() =
        Console.WriteLine("do this")

    let Stop() =
        match player with
            | Some p -> 
                p.Pause()
                p.Dispose()
                player <- None
            | None -> 
                ()
