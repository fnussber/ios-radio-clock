namespace RadioClock

open System
open MonoTouch.Foundation

type Station(name: String, url: NSUrl) =
    member this.Name = name
    member this.Url = url
    new (name: String, urlString: String) = Station(name, new NSUrl(urlString))

module Config =

    // duration of fade in/out in seconds
    let fadeInDuration  = 60.0f
    let fadeOutDuration = 60.0f

    // the radio stations
    let mutable stations = [
        new Station("SRF 3", "http://stream.srg-ssr.ch/drs3/mp3_128.m3u")
        new Station("SRF 2", "http://stream.srg-ssr.ch/drs2/mp3_128.m3u")
        new Station("SRF 1", "http://stream.srg-ssr.ch/drs1/mp3_128.m3u")
    ]

    // a selection of sleep timers
    let sleepTimes: list<string * TimeSpan> = [
        "60 minutes", TimeSpan( 1,  0, 0)
        "30 minutes", TimeSpan( 0, 30, 0)
        "20 minutes", TimeSpan( 0, 20, 0)
        "10 minutes", TimeSpan( 0, 10, 0)
        "5 minutes",  TimeSpan( 0,  5, 0)
        "1 minute",   TimeSpan( 0,  1, 0)
    ]

    // a selection of alarm timers
    let mutable alarmTimes: list<TimeSpan> = [
        TimeSpan( 7,  0, 0) // 07:00
        TimeSpan( 7, 30, 0) // 07:30
        TimeSpan( 8,  0, 0) // 08:00
    ]

