// Copyright Eric Chauvin 2018.
// My blog is at:
// https://scientificmodels.blogspot.com/



// For historical reasons there are complications
// involving the definition of Latitude, and the
// difference between Geocentric Latitude and
// Geodetic Latitude, and so this has an
// ApproximateLatitude value and a
// GeodeticLatitude value, which are slightly
// different.  See SetGeodeticLatitude() for how
// the Geodetic Latitude is calculated.



using System;
using System.Text;



namespace ClimateModel
{
  class ReferenceVertexArray
  {
  private MainForm MForm;
  private int ArraySize = 0;
  private double RadiusMinor = 0;
  private double RadiusMajor = 0;
  private double ApproximateLatitude = 0;
  private double GeodeticLatitude = 0;
  private double LatRadians = 0;
  private double CosLatRadians = 0;
  private double SinLatRadians = 0;
  private double CosLatRadiansPlusDelta = 0;
  private double SinLatRadiansPlusDelta = 0;
  private QuaternionEC.QuaternionRec RotationQ;
  private QuaternionEC.QuaternionRec InverseRotationQ;

  // private double[] LongitudeArray;
  private Vector3.Vector[] PositionArray;
  private Vector3.Vector[] SurfaceNormalArray;
  private Vector3.Vector[] SurfaceFlatArray;
  private const double LatitudeRadiansDelta =
                       ModelConstants.TenToMinus6 *
                       ModelConstants.TenToMinus1;



  private ReferenceVertexArray()
    {
    }



  internal ReferenceVertexArray( MainForm UseForm,
                       double UseApproximateLatitude,
                       double UseRadiusMinor,
                       double UseRadiusMajor,
                       int UseArraySize )
    {
    MForm = UseForm;

    ArraySize = UseArraySize;
    RadiusMinor = UseRadiusMinor;
    RadiusMajor = UseRadiusMajor;

    try
    {
    RotationQ = new QuaternionEC.QuaternionRec();
    InverseRotationQ = new QuaternionEC.QuaternionRec();

    ApproximateLatitude = UseApproximateLatitude;
    LatRadians = NumbersEC.DegreesToRadians( ApproximateLatitude );
    CosLatRadians = Math.Cos( LatRadians );
    SinLatRadians = Math.Sin( LatRadians );
    CosLatRadiansPlusDelta = Math.Cos( LatRadians + LatitudeRadiansDelta );
    SinLatRadiansPlusDelta = Math.Sin( LatRadians + LatitudeRadiansDelta );

    SetGeodeticLatitude();

    SetupEarthTiltRotations();
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in ReferenceVertexArray constructor: " + Except.Message );
      }
    }



  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }






  private void SetupEarthTiltRotations()
    {
    double Angle = -23.43689;
    Angle = NumbersEC.DegreesToRadians( Angle );

    QuaternionEC.QuaternionRec Axis = new
                       QuaternionEC.QuaternionRec();

    Axis.X = 1; // Rotate around the X axis.
    Axis.Y = 0;
    Axis.Z = 0;
    Axis.W = 0;

    QuaternionEC.SetAsRotation( ref RotationQ,
                                ref Axis,
                                Angle );

    QuaternionEC.Inverse( ref InverseRotationQ, ref RotationQ );
    }



  internal void DoEarthTiltRotations(
            ref Vector3.Vector[] InArray )
    {
    Vector3.Vector ResultPoint = new Vector3.Vector();
    QuaternionEC.QuaternionRec MiddlePoint = new QuaternionEC.QuaternionRec();

    int Last = InArray.Length;
    for( int Count = 0; Count < Last; Count++ )
      {
      Vector3.Vector Vec = InArray[Count];

      QuaternionEC.RotateVector3(
                     ref ResultPoint,
                     ref RotationQ,
                     ref InverseRotationQ,
                     ref Vec,
                     ref MiddlePoint );

      InArray[Count] = ResultPoint;
      }
    }



/*
====
  internal void MakeVertexRow(
                       double ApproxLatitude,
                       double LongitudeHoursRadians )
    {
    try
    {
    if( RefVertexArrayLast < 2 )
      {
      MakePoleRow( ApproxLatitude );
      return;
      }


    double LonStart = -180.0;

    // There is a beginning vertex at -180 longitude
    // and there is an ending vertex at 180
    // longitude, which is the same place, but they
    // are associated with different texture
    // coordinates.  One at the left end of the
    // texture and one at the right end.
    // So this is minus 1:
    double LonDelta = 360.0d / (double)(RefVertexArrayLast - 1);

    for( int Count = 0; Count < RefVertexArrayLast; Count++ )
      {
      ReferenceVertex RefVertex = new ReferenceVertex();

      double Longitude = LonStart + (LonDelta * Count);
      double LonRadians = NumbersEC.DegreesToRadians(
                                          Longitude );

      // Higher hours make the sun set in the west.
      LonRadians += LongitudeHoursRadians;

      double CosLonRadians = Math.Cos( LonRadians );
      double SinLonRadians = Math.Sin( LonRadians );

      // Along the equatorial axis:
      RefVertex.Position.X = RadiusMajor * (CosLatRadians * CosLonRadians );
      RefVertex.Position.Y = RadiusMajor * (CosLatRadians * SinLonRadians );

      // Along the polar axis:
      RefVertex.Position.Z = RadiusMinor * SinLatRadians;

      SetSurfaceNormal( ref RefVertex,
                   CosLatRadians,
                   SinLatRadians,
                   CosLatRadiansPlusDelta,
                   SinLatRadiansPlusDelta,
                   CosLonRadians,
                   SinLonRadians,
                   ApproxLatitude );

      RefVertexArray[Count] = RefVertex;
      }
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthLine.MakeVertexRow(): " + Except.Message );
      }
    }
====
*/




/*
====

  private void SetSurfaceNormal(
                      ref ReferenceVertex RefVertex,
                      ref ReferenceSurfaceVertex RefSurfaceVertex,
                      double CosLatRadians,
                      double SinLatRadians,
                      double CosLatRadiansPlusDelta,
                      double SinLatRadiansPlusDelta,
                      double CosLonRadians,
                      double SinLonRadians,
                      double ApproxLatitude )
    {
    if( ApproxLatitude < -89.999 )
      throw( new Exception( "ApproxLatitude < -89.999 in SetSurfaceNormal()." ));

    if( ApproxLatitude > 89.999 )
      throw( new Exception( "ApproxLatitude > 89.999 in SetSurfaceNormal()." ));

    // If it's at the equator.
    if( (ApproxLatitude < 0.00001) &&
        (ApproxLatitude > -0.00001))
      {
      // This avoids calculating a perpendicular
      // with two vectors _very_ close to each other.
      // It avoids normalizing a very small vector.
      RefSurfaceVertex.Normal.X = RefVertex.Position.X;
      RefSurfaceVertex.Normal.Y = RefVertex.Position.Y;
      RefSurfaceVertex.Normal.Z = 0;
      Vector3.Normalize( ref RefSurfaceVertex.Normal, RefSurfaceVertex.Normal );

      RefSurfaceVertex.FlatNoRotate.X = 0;
      RefSurfaceVertex.FlatNoRotate.Y = 0;
      RefSurfaceVertex.FlatNoRotate.Z = 1;

      return;
      }

    Vector3.Vector PositionAtDelta;
    PositionAtDelta.X = RadiusMajor * (CosLatRadiansPlusDelta * CosLonRadians );
    PositionAtDelta.Y = RadiusMajor * (CosLatRadiansPlusDelta * SinLonRadians );
    PositionAtDelta.Z = RadiusMinor * SinLatRadiansPlusDelta;

    Vector3.Copy( ref RefSurfaceVertex.FlatNoRotate, ref PositionAtDelta );
    Vector3.Subtract( ref RefVertex.SurfaceFlat, ref RefVertex.Position );

    if( RefVertex.Position.Z > PositionAtDelta.Z )
      throw( new Exception( "RefVertex.Position.Z > PositionAtDelta.Z." ));

    if( RefVertex.SurfaceFlat.Z < 0 )
      throw( new Exception( "RefVertex.SurfaceFlat.Z < 0." ));

    double FlatNorm = Vector3.Norm( ref RefVertex.SurfaceFlat );

    if( FlatNorm < MinFlatNorm )
      MinFlatNorm = FlatNorm;

    if( FlatNorm > MaxFlatNorm )
      MaxFlatNorm = FlatNorm;

    Vector3.Normalize( ref RefVertex.SurfaceFlat, RefVertex.SurfaceFlat );

    RefVertex.SurfaceFlatNoRotate = RefVertex.SurfaceFlat;

    // Straight up through the north pole.
    Vector3.Vector StraightUp = new Vector3.Vector();
    StraightUp.X = 0;
    StraightUp.Y = 0;
    StraightUp.Z = 1;

    // The dot product of two normalized vectors.
    double Dot = Vector3.DotProduct( ref StraightUp, ref RefVertex.SurfaceFlat );

    if( Dot < 0 )
      throw( new Exception( "Dot < 0." ));

    if( Dot < MinDotProduct )
      MinDotProduct = Dot;

    if( Dot > MaxDotProduct )
      MaxDotProduct = Dot;

    Vector3.Vector PerpenVector = new Vector3.Vector();
    // Make a vector perpendicular to FlatVector and
    // toward StraightUp.
    Vector3.MakePerpendicular( ref PerpenVector, ref RefVertex.SurfaceFlat, ref StraightUp );
    if( RefVertex.Position.Z < 0 )
      Vector3.Negate( ref PerpenVector );

    Vector3.Copy( ref RefVertex.SurfaceNormal, ref PerpenVector );
    }


=====
*/



  private void SetGeodeticLatitude()
    {
    // If it's at the north or south pole.
    if( (ApproximateLatitude >  89.9999) ||
        (ApproximateLatitude < -89.9999))
      {
      GeodeticLatitude = ApproximateLatitude;
      return;
      }

    // If it's pretty much at the equator at
    // zero latitude.
    if( (ApproximateLatitude >  -0.0001) &&
        (ApproximateLatitude < 0.0001))
      {
      GeodeticLatitude = ApproximateLatitude;
      return;
      }

    // Straight up through the north pole.
    Vector3.Vector StraightUp = new Vector3.Vector();
    StraightUp.X = 0;
    StraightUp.Y = 0;
    StraightUp.Z = 1;

    double CosLonRadians = 1;
    double SinLonRadians = 0;

    Vector3.Vector Position = new Vector3.Vector();
    Position.X = RadiusMajor * (CosLatRadians * CosLonRadians );
    Position.Y = RadiusMajor * (CosLatRadians * SinLonRadians );
    Position.Z = RadiusMinor * SinLatRadians;

    Vector3.Vector PositionAtDelta = new Vector3.Vector();
    PositionAtDelta.X = RadiusMajor * (CosLatRadiansPlusDelta * CosLonRadians );
    PositionAtDelta.Y = RadiusMajor * (CosLatRadiansPlusDelta * SinLonRadians );
    PositionAtDelta.Z = RadiusMinor * SinLatRadiansPlusDelta;

    Vector3.Vector Flat = new Vector3.Vector();
    Vector3.Copy( ref Flat, ref PositionAtDelta );
    Vector3.Subtract( ref Flat, ref Position );

    if( Position.Z > PositionAtDelta.Z )
      throw( new Exception( "Position.Z > PositionAtDelta.Z." ));

    if( Flat.Z < 0 )
      throw( new Exception( "Flat.Z < 0." ));

    Vector3.Normalize( ref Flat, Flat );

    // The dot product of two normalized vectors.
    double Dot = Vector3.DotProduct(
                              ref StraightUp,
                              ref Flat );

    if( Dot < 0 )
      throw( new Exception( "Dot < 0." ));

    double StraightUpAngle = Math.Acos( Dot );
    double AngleToEquator =
               (Math.PI / 2.0) - StraightUpAngle;

    double Degrees = NumbersEC.RadiansToDegrees( StraightUpAngle );

    if( ApproximateLatitude >= 0 )
      GeodeticLatitude = Degrees;
    else
      GeodeticLatitude = -Degrees;

    }



  }
}
