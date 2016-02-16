using CustomMapCalloutTestApp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CustomMapCalloutTestApp.Pages
{
  public partial class MapPage : ContentPage
  {
    private bool _runOnce = true;

    private List<CustomPin> _mapEntries;

    public MapPage()
    {
      InitializeComponent();

      SetupMap();
    }

    /*
    protected override void OnAppearing()
    {
      base.OnAppearing();

      if (_runOnce)
      {
        _runOnce = false;
        SetupMap();
      }
    }
    */

    private void SetupMap()
    {
      // RFP:: Where custom pins are setup
      SetupMapEntries();
      ZoomToArea();
    }

    private void SetupMapEntries()
    { 
      _mapEntries = new List<CustomPin>();
      _mapEntries.Add(new CustomPin()
      {
        FirstLine = "Location 1.1",
        SecondLine = "Location 1.2 what happens if one of these is really long and goes too far?",
        ThirdLine = "Location 1.3",
        FourthLine = "Location 1.4",
        Location = new Position(34.0278897, -118.301869)
      });
      _mapEntries.Add(new CustomPin()
      {
        FirstLine = "Location 2.1",
        SecondLine = "Location 2.2",
        ThirdLine = "Location 2.3",
        FourthLine = "Location 2.4",
        Location = new Position(34.047797, -118.321869)
      });
      _mapEntries.Add(new CustomPin()
      {
        FirstLine = "Location 3.1",
        SecondLine = "Location 3.2",
        ThirdLine = "Location 3.3",
        FourthLine = "Location 3.4",
        Location = new Position(34.007897, -118.300069)
      });

      foreach (var entry in _mapEntries)
      {
        var pin = new Pin()
        {
          Type = PinType.Place,
          Position = entry.Location,
          Label = "Main label Text Here",
          Address = "Detail label"
        };
        entry.Pin = pin;
        MyMap.Pins.Add(pin);
      }

      MyMap.CustomPins = _mapEntries;
    }

    private void ZoomToArea()
    {
      MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(34.0278897, -118.301869), Distance.FromMiles(1)));
    }
  }
}
