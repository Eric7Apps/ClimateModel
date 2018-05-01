// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;


namespace ClimateModel
{
  class Ellipse
  {
  private MainForm MForm;

// RotatedValue( Theta + Phi );



  /*
  private void TestZValue( double ResultZ,
                           double SinLatRadians,
                           double XNoLongitude )
    {
    // https://en.wikipedia.org/wiki/Ellipse
    // https://en.wikipedia.org/wiki/Eccentric_anomaly

    if( SinLatRadians <= 0 )
      return; // ResultZ;

    // RadiusMajor = 6.378137; // Equator
    // RadiusMinor = 6.356752; // poles
    // A is the semimajor axis
    // B is the semiminor axis
    double A = RadiusMajor;
    double B = RadiusMinor;

    // The definition of an ellipse:
    // x^2/A^2 + z^2/B^2 = 1

    // Solve for Z:
    // (B^2*x^2 + A^2*z^2)/(B^2*A^2) = 1
    // B^2*x^2 + A^2*z^2 = B^2*A^2
    // A^2*z^2 = B^2*A^2 - B^2*x^2
    // A^2*z^2 = B^2(A^2 - x^2)
    // z^2 = B^2/A^2*(A^2 - x^2)
    // z = Sqrt( B^2/A^2*(A^2 - x^2))
    // z = B/A * Sqrt( (A^2 - x^2))

    double ZTest = (B / A) * Math.Sqrt( (A * A) - (XNoLongitude * XNoLongitude));

    double Difference = Math.Abs( ZTest - ResultZ );
    if( Difference > MaxZDiff )
      MaxZDiff = Difference;

    // MaxZDiffInMilliMeters: 0.00000310862446895044

    // A millimeter is a thousandth of a meter.
    // So that's 3 nanometers or something?
    }
    */


  }
}


