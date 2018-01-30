// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
// using System.Collections.Generic;
using System.Text;


namespace ClimateModel
{
  static class NumbersEC
  {

  internal static bool IsAlmostEqual( double A, double B, double SmallNumber )
    {
    // How small can this be?
    // How close are they?

    if( A + SmallNumber < B )
      return false;

    if( A - SmallNumber > B )
      return false;

    return true;
    }



  internal static double DegreesMinutesToRadians( double Degrees, double Minutes, double Seconds )
    {
    double Deg = Degrees + (Minutes / 60.0d) + (Seconds / (60.0d * 60.0d));
    // Math.PI
    // 3.14159265358979323846
    double RadConstant = (2.0d * 3.14159265358979323846d) / 360.0d;
    return Deg * RadConstant;
    }



  internal static double DegreesToRadians( double Degrees )
    {
    double RadConstant = (2.0d * 3.14159265358979323846d) / 360.0d;
    return Degrees * RadConstant;
    }



  }
}
