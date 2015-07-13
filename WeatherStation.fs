namespace RadioClock

open System
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

/// Simple weather station that uses our current location to get weather information from openeweathermap.org
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

    let getWeather() =
        let lat = if (locationManager.Location = null) then 0.0 else locationManager.Location.Coordinate.Latitude
        let long = if (locationManager.Location = null) then 0.0 else locationManager.Location.Coordinate.Longitude
        let url = sprintf "http://api.openweathermap.org/data/2.5/weather?lat=%f&lon=%f&mode=xml" lat long
        let req = HttpWebRequest.Create(url) :?> HttpWebRequest
        let resp = req.GetResponse() // throws 501.. etc -> ERROR HANDLING!!!
        let stream = resp.GetResponseStream()
        let reader = new StreamReader(stream)
        let xml = reader.ReadToEnd()

        let doc = new XmlDocument()
        doc.LoadXml xml
        doc

    let value (xml: XmlDocument, value: string, attribute: string) = (xml.SelectSingleNode("/current/" + value + "/@" + attribute)).InnerText

    let temperature xml = float(value (xml, "temperature", "value"))

    let humidity xml = float(value (xml, "humidity", "value"))

    let weather xml = value (xml, "weather", "value")

    let icon xml = iconForId (value (xml, "weather", "icon"))

    let printWeather xml = 
//        let coord = locationManager.Location.Coordinate
//        Console.WriteLine("lat         = " + coord.Latitude.ToString())
//        Console.WriteLine("long        = " + coord.Longitude.ToString())
        Console.WriteLine("temperature = " + temperature(xml).ToString())
        Console.WriteLine("humidity    = " + humidity(xml).ToString())
        Console.WriteLine("weather     = " + weather(xml).ToString())
        Console.WriteLine("icon        = " + value(xml, "weather", "icon").ToString())
        ()


    let weatherView() =
        let xml = getWeather()
//        printWeather xml
        let v = new UIView(TranslatesAutoresizingMaskIntoConstraints = false)
        let icon = new UIImageView(icon(xml)) 
        icon.BackgroundColor <- new UIColor(1.0f, 1.0f, 1.0f, 0.5f)
        let label = new UILabel(Text = weather(xml), TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White)
        icon.TranslatesAutoresizingMaskIntoConstraints <- false
        v.AddSubview(icon)
        v.AddSubview(label)

        // layout
        let metrics = new NSDictionary()
        let views = new NSDictionary("icon", icon, "label", label)
        let ch0 = NSLayoutConstraint.FromVisualFormat("H:|[icon(50)][label]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv0 = NSLayoutConstraint.FromVisualFormat("V:|[icon]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv1 = NSLayoutConstraint.FromVisualFormat("V:|[label]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        v.AddConstraints(ch0)
        v.AddConstraints(cv0)
        v.AddConstraints(cv1)
        v

//    let weather(): seq<UIView> =
//        Seq.unfold(fun weather -> Some(weather, weatherView())) (weatherView()) // produce a weather view over and over again, this will in fact reload everytime latest weather info

    do
        // need to ask for iOS 8
        //if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0)) then locationManager.RequestWhenInUseAuthorization()

        // NOTE: there must be a corresponding entry for NSLocationWhenInUseUsageDescription in Info.plist 
        // otherwise iOS will never ask permission and no location updates are generated.
        locationManager.RequestWhenInUseAuthorization()
//        locationManager.UpdatedLocation |> Event.add(fun evArgs -> Console.WriteLine("NEW POSITION"))
        locationManager.StartUpdatingLocation()

        let timer = new System.Timers.Timer(10000.0)
        timer.Elapsed.Add(fun _ -> locationManager.InvokeOnMainThread(fun _ -> NextWeather.Trigger(weatherView())))
        timer.Start()
         
