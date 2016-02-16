using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CustomMapCalloutTestApp.Types
{
  public class CustomPin
  {
    public Pin Pin { get; set; }
    public string Id { get { return "CustomPinID"; } } // RFP:: this ID is only used for dequeing reusable views, it should be the same for all pins
    public string FirstLine { get; set; }
    public string SecondLine { get; set; }
    public string ThirdLine { get; set; }
    public string FourthLine { get; set; }
    public Position Location { get; set; }
  }
}
