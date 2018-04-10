// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
using System.Text;


namespace ClimateModel
{

  static class ModelConstants
  {
  // Double precision format:
  // 53-bit "significand precision".
  // https://en.wikipedia.org/wiki/Double-precision_floating-point_format

  // Quad precision:
  // 113-bit significand precision.
  // https://en.wikipedia.org/wiki/Quadruple-precision_floating-point_format

  // binary256:
  // 237-bit significand precision.

  internal const double TenTo1 = 10.0d;
  internal const double TenTo3 = 10.0d * 10.0d * 10.0d;
  internal const double TenTo6 = TenTo3 * TenTo3;
  internal const double TenTo9 = TenTo6 * TenTo3;
  internal const double TenTo10 = TenTo9 * TenTo1;
  internal const double TenTo20 = TenTo10 * TenTo10;

  internal const double TenToMinus1 = 1.0d / 10.0d;
  internal const double TenToMinus3 = 1.0d / TenTo3;
  internal const double TenToMinus6 = 1.0d / TenTo6;
  internal const double TenToMinus9 = 1.0d / TenTo9;
  internal const double TenToMinus10 = 1.0d / TenTo10;
  internal const double TenToMinus20 = 1.0d / TenTo20;


  // Meters^3  Kilograms^-1  Seconds^-2
  // https://en.wikipedia.org/wiki/Gravitational_constant
  internal const double GravitationConstant =
               6.6740831d *
               (TenToMinus10 * TenToMinus1);

  // Mass in kilograms.
  internal const double MassOfEarth = 5.97237d *
                                      TenTo20 *
                                      TenTo3 *
                                      TenTo1;

  // Kilograms cancels out, so EarthG is in units
  // of Meters^3  Seconds^-2




  }
}
