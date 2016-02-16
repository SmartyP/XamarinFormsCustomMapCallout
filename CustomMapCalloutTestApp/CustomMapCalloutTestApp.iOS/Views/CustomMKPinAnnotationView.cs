using MapKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomMapCalloutTestApp.iOS.Views
{
  public class CustomMKPinAnnotationView : MKPinAnnotationView
  {
    public string Id { get; set; }

    public string Url { get; set; }

    public string FirstLine { get; set; }
    public string SecondLine { get; set; }
    public string ThirdLine { get; set; }
    public string FourthLine { get; set; }

    public CustomMKPinAnnotationView(IMKAnnotation annotation, string id)
      : base(annotation, id)
    {
    }
  }
}
