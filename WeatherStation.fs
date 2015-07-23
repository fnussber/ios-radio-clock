namespace RadioClock

open System
open System.Collections.Generic
open System.IO
open System.Net
open System.Text
open System.Xml
open MonoTouch.CoreLocation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

// Unfortunately we can't use the tools from FSharp.Data like type providers etc. because including
// the dll inflates the binary beyond the maximum size allowed for the free Xamarin Studio version :(
// Credits for weather icons: http://icons8.com/web-app/category/ios7/Weather

/// Simple weather station that uses our current location to get weather information from openweathermap.org
module WeatherStation =

    let NextWeather    = new Event<UIView>()

    let locationManager = new CLLocationManager()

    // Defines a translation from openweather icon names to the ones we are using here
    // See http://bugs.openweathermap.org/projects/api/wiki/Weather_Condition_Codes for details
    let icons =
        [ "01d", "sun-50.png";                      // sky is clear
          "01n", "moon-50.png";
          "02d", "partly_cloudy_day-50.png";        // few clouds
          "02n", "partly_cloudy_night-50.png";
          "03d", "clouds-50.png";                   // scattered clouds
          "03n", "clouds-50.png";
          "04d", "clouds-50.png";                   // broken clouds
          "04n", "clouds-50.png";
          "09d", "rain-50.png";                     // shower rain
          "09n", "rain-50.png";
          "10d", "partly_cloudy_rain-50.png";       // rain
          "10n", "rain-50.png";
          "11d", "storm-50.png";                    // thunderstorm
          "11n", "storm-50.png";
          "13d", "snow-50.png";                     // snow    
          "13n", "snow-50.png";
          "50d", "fog_day-50.png";                  // mist
          "50n", "fog_night-50.png";
          ]
        |> Map.ofList 
        |> Map.map (fun key name -> new UIImage("weather/" + name))

    /// Provides a fallback icon in case we receive an unknown weather id
    let unknownIcon = new UIImage("weather/unknown.png")

    /// Gets the icon for the given weather id
    let iconForId name = defaultArg (icons.TryFind name) unknownIcon

    let getWeather(): option<XmlDocument> =
        try 
            let lat  = if (locationManager.Location = null) then 0.0 else locationManager.Location.Coordinate.Latitude
            let long = if (locationManager.Location = null) then 0.0 else locationManager.Location.Coordinate.Longitude
            let url  = sprintf "http://api.openweathermap.org/data/2.5/forecast/daily?lat=%f&lon=%f&mode=xml&units=metric&cnt=1" lat long
            let req  = HttpWebRequest.Create(url) :?> HttpWebRequest
            let resp = req.GetResponse() // throws 501.. etc -> ERROR HANDLING!!!
            let stream = resp.GetResponseStream()
            let reader = new StreamReader(stream)
            let xml = reader.ReadToEnd()
            let doc = new XmlDocument()
            doc.LoadXml xml
            Some(doc)
        with
            | e -> None

    let value (xml: XmlDocument, value: string, attribute: string) = (xml.SelectSingleNode("/weatherdata/forecast/time/" + value + "/@" + attribute)).InnerText

    let temperature (xml: XmlDocument) : (float * float * float) = 
      (Math.Round(float(value (xml, "temperature", "day"))), Math.Round(float(value (xml, "temperature", "min"))), Math.Round(float(value (xml, "temperature", "max"))))

//    let humidity xml = float(value (xml, "humidity", "value"))

//    let weather xml = value (xml, "weather", "value")

    let icon xml = iconForId (value (xml, "symbol", "var"))

    let location(xml: XmlDocument): string = (xml.SelectSingleNode("/weatherdata/location/name")).InnerText

    let weatherLabel city temp minTemp maxTemp = 
        new UILabel(
            Text = (city + "   " + temp + "°  ↓" + minTemp + "° ↑" + maxTemp + "°"), 
            TranslatesAutoresizingMaskIntoConstraints = false, 
            TextColor = UIColor.White)



    let weatherView xml =
        match xml with
            | Some(xml) ->
                //Console.WriteLine(xml.InnerXml)
                let v = new UIView(TranslatesAutoresizingMaskIntoConstraints = false)
                let icon = new UIImageView(icon(xml)) 
                let city = location(xml)
                let (cur, min, max) = temperature(xml)
                let label = weatherLabel city (cur.ToString()) (min.ToString()) (max.ToString())
                icon.TranslatesAutoresizingMaskIntoConstraints <- false
                label.TranslatesAutoresizingMaskIntoConstraints <- false

                let views = [
                    "icon",  icon  :> UIView
                    "label", label :> UIView
                ]
                let formats = [
                    "H:|[icon(50)]-10-[label]|"
                    "V:|[icon]|"
                    "V:|[label]|"         
                ]
                Layout.layout v formats views

                v
            | None ->
                new UILabel(Text = "Couldn't read", TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White) :> UIView

    let weatherSeq() : seq<option<XmlDocument>> = 
        Seq.unfold(fun _ -> Some(getWeather(), [])) []

    do
        // NOTE: there must be a corresponding entry for NSLocationWhenInUseUsageDescription in Info.plist 
        // otherwise iOS will never ask permission and no location updates are generated.
        locationManager.RequestWhenInUseAuthorization()
        locationManager.StartUpdatingLocation()

        Async.Start (async {
            let weather = weatherSeq().GetEnumerator()
            while true do
                if (weather.MoveNext()) then 
                    locationManager.InvokeOnMainThread(fun _ ->
                        NextWeather.Trigger (weatherView(weather.Current))
                    )
                do! Async.Sleep 10000
        }) |> ignore
                 
