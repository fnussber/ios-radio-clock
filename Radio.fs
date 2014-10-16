namespace RadioClock

open System
open MonoTouch.AVFoundation
open MonoTouch.Foundation


type Station(name: String, url: NSUrl) =
    member this.Name = name
    member this.Url = url
    new (name: String, urlString: String) = Station(name, new NSUrl(urlString))


type Radio() = 
    let mutable stations = [
        new Station("DRS1", "http://stream.srg-ssr.ch/drs1/mp3_128.m3u")
        new Station("DRS2", "http://stream.srg-ssr.ch/drs2/mp3_128.m3u")
        new Station("DRS3", "http://stream.srg-ssr.ch/drs3/mp3_128.m3u") ]
    let mutable player: Option<AVPlayer> = None
    let mutable station: Option<Station> = None
    //let mutable stations: List<Station> = new ArrayList<Station>()

    member this.Station 
        with get() = station
        and set(s) = station <- s

    member this.AddStation s = 
        stations <- s :: stations

    member this.RemoveStation sd = 
        stations <- List.filter (fun s -> not(s.Equals(sd))) stations

    member this.Stations
        with get(): List<Station> = List.sortBy (fun s -> s.Name) stations

    member this.VolumeUp u =
        player |> Option.map(fun p -> p.Volume = Math.Min(1.0f, p.Volume + u))

    member this.VolumeDown d =
        player |> Option.map(fun p -> p.Volume = Math.Max(0.0f, p.Volume - d))

    member this.Mute() =
        player |> Option.map(fun p -> p.Volume = 0.0f)

    member this.Play() = 
        player = 
            match station with
                | Some s ->
                    let p = new AVPlayer(s.Url)
                    p.Play()
                    Some(p)
                | None ->
                    None

    member this.Stop() =
        match player with
            | Some p -> 
                p.Pause()
                p.Dispose()
            | None -> 
                ()