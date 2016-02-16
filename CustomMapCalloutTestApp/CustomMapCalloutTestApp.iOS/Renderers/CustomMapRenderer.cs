using CoreGraphics;
using CustomMapCalloutTestApp.Controls;
using CustomMapCalloutTestApp.iOS.Renderers;
using CustomMapCalloutTestApp.iOS.Views;
using CustomMapCalloutTestApp.Types;
using MapKit;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CustomMapCalloutTestApp.iOS.Renderers
{
  public class CustomMapRenderer : MapRenderer
  {
    private const float CALLOUT_Y_OFFSET = -12; // shifts the text fields up to make up for unused default detail field

    List<CustomPin> customPins;

    protected override void OnElementChanged(ElementChangedEventArgs<View> e)
    {
      base.OnElementChanged(e);

      if (e.OldElement != null)
      {
        var nativeMap = Control as MKMapView;
        nativeMap.GetViewForAnnotation = null;
      }

      if (e.NewElement != null)
      {
        var formsMap = (CustomMap)e.NewElement;
        var nativeMap = Control as MKMapView;
        customPins = formsMap.CustomPins;

        nativeMap.GetViewForAnnotation = GetViewForAnnotation;
      }
    }

    MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
    {
      MKAnnotationView annotationView = null;

      if (annotation is MKUserLocation)
        return null;

      var anno = annotation as MKPointAnnotation;
      var customPin = GetCustomPin(anno);
      if (customPin == null)
      {
        throw new Exception("Custom pin not found");
      }

      annotationView = mapView.DequeueReusableAnnotation(customPin.Id);
      if (annotationView == null)
      {
        // RFP:: create the cutom annotation view setting its properties to match the CustomPin on the map
        annotationView = new CustomMKPinAnnotationView(annotation, customPin.Id);
        ((CustomMKPinAnnotationView)annotationView).Id = customPin.Id;
        ((CustomMKPinAnnotationView)annotationView).FirstLine = customPin.FirstLine;
        ((CustomMKPinAnnotationView)annotationView).SecondLine = customPin.SecondLine;
        ((CustomMKPinAnnotationView)annotationView).ThirdLine = customPin.ThirdLine;
        ((CustomMKPinAnnotationView)annotationView).FourthLine = customPin.FourthLine;

        // RFP:: create a new view to be used for annotationView.DetailCalloutAccessoryView
        var newCalloutView = new UIView();
        newCalloutView.TranslatesAutoresizingMaskIntoConstraints = false; // we are using autolayout, not struts

        // RFP:: create and add 3 new UILabels. The normal secondary line on the pin will be replaced by replacing the DetailCalloutAccessoryView, so in total there will be 4 lines
        var label1 = CreateBasicLabel();
        var label2 = CreateBasicLabel();
        var label3 = CreateBasicLabel();
        newCalloutView.Add(label1);
        newCalloutView.Add(label2);
        newCalloutView.Add(label3);

        // RFP:: measure how big a text field is and calculate the height needed for the callout
        label1.Text = "TEMP"; // used to measure height
        var heightOfLabel = label1.GetSizeRequest(double.PositiveInfinity, double.PositiveInfinity).Request.Height;
        var heightOfCallout = (float)(heightOfLabel * 3 + CALLOUT_Y_OFFSET); // the height of the callout is 3 times the height of the label subtracting height used to shift callout view up to be under main text

        // RFP:: setup the constraints for the new callout view
        newCalloutView.AddConstraints(new[]
        {
          NSLayoutConstraint.Create(newCalloutView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, heightOfCallout),
        });

        // RFP:: setup the constraints for each of the new labels
        newCalloutView.AddConstraints(new[]
        {
          NSLayoutConstraint.Create(label1, NSLayoutAttribute.Top, NSLayoutRelation.Equal, newCalloutView, NSLayoutAttribute.Top, 1, CALLOUT_Y_OFFSET), // offsetting up X pixels to be right under primary text line (otherwise there is a gap)
          NSLayoutConstraint.Create(label1, NSLayoutAttribute.Left, NSLayoutRelation.Equal, newCalloutView, NSLayoutAttribute.Left, 1, 0), // setting label to match parents left edge
          NSLayoutConstraint.Create(label1, NSLayoutAttribute.Right, NSLayoutRelation.Equal, newCalloutView, NSLayoutAttribute.Right, 1, 0), // setting label to match parents right edge
        });
        newCalloutView.AddConstraints(new[]
        {
          NSLayoutConstraint.Create(label2, NSLayoutAttribute.Top, NSLayoutRelation.Equal, label1, NSLayoutAttribute.Bottom, 1, 0), // setting top of label to be at bottom of previous label
          NSLayoutConstraint.Create(label2, NSLayoutAttribute.Left, NSLayoutRelation.Equal, newCalloutView, NSLayoutAttribute.Left, 1, 0), // setting label to match parents left edge
          NSLayoutConstraint.Create(label2, NSLayoutAttribute.Right, NSLayoutRelation.Equal, newCalloutView, NSLayoutAttribute.Right, 1, 0), // setting label to match parents right edge
        });
        newCalloutView.AddConstraints(new[]
        {
          NSLayoutConstraint.Create(label3, NSLayoutAttribute.Top, NSLayoutRelation.Equal, label2, NSLayoutAttribute.Bottom, 1, 0), // setting top of label to be at bottom of previous label
          NSLayoutConstraint.Create(label3, NSLayoutAttribute.Left, NSLayoutRelation.Equal, newCalloutView, NSLayoutAttribute.Left, 1, 0), // setting label to match parents left edge
          NSLayoutConstraint.Create(label3, NSLayoutAttribute.Right, NSLayoutRelation.Equal, newCalloutView, NSLayoutAttribute.Right, 1, 0), // setting label to match parents right edge
        });

        // RFP:: replace the DetailCalloutAccessoryView with our new view
        annotationView.DetailCalloutAccessoryView = newCalloutView;
      }

      // RFP:: Get a handle on the fields in the callout to update their text values (both if newly created or reused per DequeueReusableAnnotation)
      var textField1 = annotationView.DetailCalloutAccessoryView.Subviews[0] as UILabel;
      var textField2 = annotationView.DetailCalloutAccessoryView.Subviews[1] as UILabel;
      var textField3 = annotationView.DetailCalloutAccessoryView.Subviews[2] as UILabel;
      textField1.Text = customPin.SecondLine;
      textField2.Text = customPin.ThirdLine;
      textField3.Text = customPin.FourthLine;

      // RFP:: make sure the callout is shown
      annotationView.CanShowCallout = true;

      return annotationView;
    }

    private UILabel CreateBasicLabel()
    {
      var tempTextView = new UILabel();
      tempTextView.TranslatesAutoresizingMaskIntoConstraints = false; // we want to use autolayout, not struts
      tempTextView.LineBreakMode = UILineBreakMode.TailTruncation;
      tempTextView.Lines = 1;
      tempTextView.Font = UIFont.PreferredCaption1;     
      
      return tempTextView;
    }

    CustomPin GetCustomPin(MKPointAnnotation annotation)
    {
      var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
      foreach (var pin in customPins)
      {
        if (pin.Pin.Position == position)
        {
          return pin;
        }
      }
      return null;
    }
  }
}
