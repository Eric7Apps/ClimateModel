// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
using System.Text;


namespace ClimateModel
{
  class EarthSlice
  {
  private MainForm MForm;
  private ReferenceVertex[,] RefVertexArray;
  private int RefVertexArrayLast = 0;
  private LatLongPosition[] LatLonRow;
  private int LatLonRowLast = 0;

  // private double MaxZDiff = 0;
  private double MinDotProduct = 10;
  private double MaxDotProduct = 0;
  private double MinFlatNorm = 100000000000000d;
  private double MaxFlatNorm = 0;
  private double MaxXVelocity = 0;
  private double MaxAccelNorm = 0;
  private double EarthAccelMin = 1000000000;
  private double EarthAccelMax = 0;
  private double MaxSurfaceTest = 0;
  private const int VerticalVertexCount = 10;
  private const double LatitudeRadiansDelta =
                       ModelConstants.TenToMinus6 *
                       ModelConstants.TenToMinus1;

  internal const int RowLatDelta = 5;
  private const int RowsFromEquatorToPole =
            90 / RowLatDelta;
  internal const int VertexRowsLast =
    (RowsFromEquatorToPole * 2) + 1;

  internal const int VertexRowsMiddle =
     RowsFromEquatorToPole + 1;

  internal const int MaximumVertexesPerRow = 128;




  public struct ReferenceVertex
    {
    public int Index; // Mainly for 3D graphics.
    public Vector3.Vector Position;
    public Vector3.Vector Velocity;
    public Vector3.Vector Acceleration;
    public Vector3.Vector CentrifugalAccel;
    public Vector3.Vector EarthGravityAccel;
    }


  public struct LatLongPosition
    {
    public double GeodeticLatitude;
    public double Longitude;
    public Vector3.Vector SurfaceNormal;
    public double TextureX;
    public double TextureY;
    // public double Elevation;
    }



  private EarthSlice()
    {
    }



  internal EarthSlice( MainForm UseForm )
    {
    MForm = UseForm;
    }


  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  internal int GetRefVertexArrayLast()
    {
    return RefVertexArrayLast;
    }



  private void SetLatLonPositionXYZ(
                      ref ReferenceVertex RefVertex,
                      ref LatLongPosition Pos,
                      double CosLatRadians,
                      double SinLatRadians,
                      double CosLatRadiansPlusDelta,
                      double SinLatRadiansPlusDelta,
                      double ApproxLatitude,
                      double LongitudeHoursRadians )
    {
    // "IERS Reference Meridian: 102 meters east
    // of the Greenwich meridian."

    double LonRadians = NumbersEC.DegreesToRadians( Pos.Longitude );

    // Higher hours make the sun set in the west.
    LonRadians += LongitudeHoursRadians;

    double CosLonRadians = Math.Cos( LonRadians );
    double SinLonRadians = Math.Sin( LonRadians );

    // Along the equatorial axis:
    RefVertex.Position.X = ModelConstants.EarthRadiusMajor * (CosLatRadians * CosLonRadians );
    RefVertex.Position.Y = ModelConstants.EarthRadiusMajor * (CosLatRadians * SinLonRadians );

    // Along the polar axis:
    RefVertex.Position.Z = ModelConstants.EarthRadiusMinor * SinLatRadians;

    // This sets the Geodetic latitude too.
    SetSurfaceNormal( ref RefVertex,
                   ref Pos,
                   CosLatRadians,
                   SinLatRadians,
                   CosLatRadiansPlusDelta,
                   SinLatRadiansPlusDelta,
                   CosLonRadians,
                   SinLonRadians,
                   ApproxLatitude );

    SetCentrifugalAcceleration( ref RefVertex,
                      ref Pos,
                      CosLatRadians,
                      SinLatRadians,
                      CosLonRadians,
                      SinLonRadians,
                      LonRadians );

    Pos.TextureX = Pos.Longitude + 180.0;
    Pos.TextureX = Pos.TextureX * ( 1.0d / 360.0d);

    Pos.TextureY = Pos.GeodeticLatitude + 90.0;
    Pos.TextureY = Pos.TextureY * ( 1.0d / 180.0d );
    Pos.TextureY = 1 - Pos.TextureY;
    }




  private void SetSurfaceNormal(
                      ref ReferenceVertex RefVertex,
                      ref LatLongPosition Result,
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
      Result.SurfaceNormal.X = RefVertex.Position.X;
      Result.SurfaceNormal.Y = RefVertex.Position.Y;
      Result.SurfaceNormal.Z = 0;
      Vector3.Normalize( ref Result.SurfaceNormal, Result.SurfaceNormal );
      Result.GeodeticLatitude = ApproxLatitude;
      return;
      }

    Vector3.Vector PositionAtDelta = new Vector3.Vector();
    PositionAtDelta.X = ModelConstants.EarthRadiusMajor * (CosLatRadiansPlusDelta * CosLonRadians );
    PositionAtDelta.Y = ModelConstants.EarthRadiusMajor * (CosLatRadiansPlusDelta * SinLonRadians );
    PositionAtDelta.Z = ModelConstants.EarthRadiusMinor * SinLatRadiansPlusDelta;

    Vector3.Vector FlatVector = new Vector3.Vector();

    Vector3.Copy( ref FlatVector, ref PositionAtDelta );
    Vector3.Subtract( ref FlatVector, ref RefVertex.Position );

    if( RefVertex.Position.Z > PositionAtDelta.Z )
      throw( new Exception( "RefVertex.Position.Z > PositionAtDelta.Z." ));

    if( FlatVector.Z < 0 )
      throw( new Exception( "FlatVector.Z < 0." ));

    double FlatNorm = Vector3.Norm( ref FlatVector );

    if( FlatNorm < MinFlatNorm )
      MinFlatNorm = FlatNorm;

    if( FlatNorm > MaxFlatNorm )
      MaxFlatNorm = FlatNorm;

    Vector3.Normalize( ref FlatVector, FlatVector );

    // Straight up through the north pole.
    Vector3.Vector StraightUp = new Vector3.Vector();
    StraightUp.X = 0;
    StraightUp.Y = 0;
    StraightUp.Z = 1;

    // The dot product of two normalized vectors.
    double Dot = Vector3.DotProduct( ref StraightUp, ref FlatVector );

    if( Dot < 0 )
      throw( new Exception( "Dot < 0." ));

    if( Dot < MinDotProduct )
      MinDotProduct = Dot;

    if( Dot > MaxDotProduct )
      MaxDotProduct = Dot;

    double StraightUpAngle = Math.Acos( Dot );
    double AngleToEquator =
               (Math.PI / 2.0) - StraightUpAngle;

    double Degrees = NumbersEC.RadiansToDegrees( StraightUpAngle );

    if( RefVertex.Position.Z >= 0 )
      Result.GeodeticLatitude = Degrees;
    else
      Result.GeodeticLatitude = -Degrees;

    // The Geodetic Latitude is what has been used
    // historically because back when Eratosthenes
    // was making his measurements in about 200 BC,
    // he had no way of knowing exactly where the
    // center of mass of the Earth was.  Back then
    // he could only measure a reference direction
    // of "straight up" which depends on the gravity
    // field at that position on the surface.  (You
    // could use a "plumb bob" to measure the
    // direction of "straight up/down".)
    // Now that we have GPS satellites, we can make
    // very good measurements of the center of mass
    // of the Earth and use Geocentric coordinates.
    // But most maps still use Geodetic Coordinates.

    Vector3.Vector PerpenVector = new Vector3.Vector();
    // Make a vector perpendicular to FlatVector and
    // toward StraightUp.
    Vector3.MakePerpendicular( ref PerpenVector, ref FlatVector, ref StraightUp );
    if( RefVertex.Position.Z < 0 )
      Vector3.Negate( ref PerpenVector );

    Vector3.Copy( ref Result.SurfaceNormal, ref PerpenVector );
    }




  private void SetCentrifugalAcceleration(
                      ref ReferenceVertex RefVertex,
                      ref LatLongPosition Result,
                      double CosLatRadians,
                      double SinLatRadians,
                      double CosLonRadians,
                      double SinLonRadians,
                      double LonRadians )
    {
    // double XNoLongitude = RadiusMajor * CosLatRadians;
    // Ellipse.TestZValue( Result.Position.Z, SinLatRadians, XNoLongitude );

    Vector3.Vector CentrifugeVector = new Vector3.Vector();
    CentrifugeVector.X = RefVertex.Position.X;
    CentrifugeVector.Y = RefVertex.Position.Y;
    CentrifugeVector.Z = 0;

    double CentrifugeRadius = Vector3.Norm( ref CentrifugeVector );

    // This circumferance is the path that a vertex
    // takes as it rotates around on the surface of
    // the Earth.
    double Circumference =
             CentrifugeRadius * 2.0d * Math.PI;

    double AngleDelta =
          ModelConstants.EarthRotationAnglePerSecond;

    double CosLonRadiansMoveTo =
       Math.Cos( LonRadians + AngleDelta );
    double SinLonRadiansMoveTo =
       Math.Sin( LonRadians + AngleDelta );

    // The position one second later.
    Vector3.Vector MoveTo = new Vector3.Vector();
    MoveTo.X = ModelConstants.EarthRadiusMajor * (CosLatRadians * CosLonRadiansMoveTo );
    MoveTo.Y = ModelConstants.EarthRadiusMajor * (CosLatRadians * SinLonRadiansMoveTo );
    MoveTo.Z = ModelConstants.EarthRadiusMinor * SinLatRadians;

    double CosLonRadiansEarlier =
       Math.Cos( LonRadians - AngleDelta );
    double SinLonRadiansEarlier =
       Math.Sin( LonRadians - AngleDelta );

    // The position one second earlier.
    Vector3.Vector EarlierPosition = new Vector3.Vector();
    EarlierPosition.X = ModelConstants.EarthRadiusMajor * (CosLatRadians * CosLonRadiansEarlier );
    EarlierPosition.Y = ModelConstants.EarthRadiusMajor * (CosLatRadians * SinLonRadiansEarlier );
    EarlierPosition.Z = ModelConstants.EarthRadiusMinor * SinLatRadians;

    // This is how many meters it has moved in one
    // second.  So velocity is meters per second.
    Vector3.Vector VelocityMoveTo = new Vector3.Vector();
    Vector3.Copy( ref VelocityMoveTo, ref MoveTo );
    Vector3.Subtract( ref VelocityMoveTo, ref RefVertex.Position );

    Vector3.Copy( ref RefVertex.Velocity, ref VelocityMoveTo );

    // The velocity in the period from one second
    // earlier.
    Vector3.Vector VelocityEarlier = new Vector3.Vector();
    Vector3.Copy( ref VelocityEarlier, ref RefVertex.Position );
    Vector3.Subtract( ref VelocityEarlier, ref EarlierPosition );

    // This is the change in velocity in one second.

    // Earlier + acceleration = Later.
    // Later - Earlier = acceleration.
    Vector3.Copy( ref RefVertex.CentrifugalAccel, ref VelocityMoveTo );
    Vector3.Subtract( ref RefVertex.CentrifugalAccel, ref VelocityEarlier );

    // Make it point outward.
    Vector3.Negate( ref RefVertex.CentrifugalAccel );

    Vector3.Vector TestDirection = new Vector3.Vector();
    Vector3.Copy( ref TestDirection, ref RefVertex.CentrifugalAccel );
    Vector3.Normalize( ref TestDirection, TestDirection );
    Vector3.Normalize( ref CentrifugeVector, CentrifugeVector );
    // The cosine between them should be 1.0 since
    // they should point in exactly the same
    // direction.

    double Dot = Vector3.DotProduct(
           ref TestDirection, ref CentrifugeVector );
    if( (Dot > 1.00000001) || (Dot < 0.99999999) )
      throw( new Exception( "The vectors don't line up: " + Dot.ToString( "N10" ) ));

    double AccelNorm = Vector3.Norm( ref RefVertex.CentrifugalAccel );
    // MaxAccelNorm is pretty small: 0.03

    if( AccelNorm > MaxAccelNorm )
      MaxAccelNorm = AccelNorm;

    if( RefVertex.Velocity.X > MaxXVelocity )
      MaxXVelocity = RefVertex.Velocity.X;

    }




  private void SetGravityAcceleration(
                      ref ReferenceVertex RefVertex,
                      ref LatLongPosition Result )
    {
    double EarthRadius =
                 Vector3.Norm( ref RefVertex.Position );

    Vector3.Copy( ref RefVertex.EarthGravityAccel, ref RefVertex.Position );
    Vector3.Normalize( ref RefVertex.EarthGravityAccel, RefVertex.EarthGravityAccel );

    // Make it point toward the center of gravity.
    Vector3.Negate( ref RefVertex.EarthGravityAccel );

    double EarthAccel =
               (ModelConstants.MassOfEarth *
               ModelConstants.GravitationConstant) /
               (EarthRadius * EarthRadius);

    if( EarthAccel < EarthAccelMin )
      EarthAccelMin = EarthAccel;

    if( EarthAccel > EarthAccelMax )
      EarthAccelMax = EarthAccel;

      // EarthAccelMax:     9.8643342185
      // EarthAccelMin:     9.7982976481

    Vector3.MultiplyWithScalar( ref RefVertex.EarthGravityAccel, EarthAccel );

    Vector3.Vector TestVector = new Vector3.Vector();
    Vector3.Copy( ref TestVector, ref RefVertex.EarthGravityAccel );
    Vector3.Add( ref TestVector, ref RefVertex.CentrifugalAccel );
    Vector3.Normalize( ref TestVector, TestVector );

    // The dot product of two normalized vectors.
    double Dot = Vector3.DotProduct( ref TestVector, ref Result.SurfaceNormal );
    // They are _almost_ pointing in exactly the
    // opposite direction.
    // Exception at: if( (Dot > -0.999999))
    // It's at: -0.9999989915
    if( (Dot > -0.99999))
      throw( new Exception( "The Dot to the surface normal is not right: " + Dot.ToString( "N10" ) ));

    // Adding two vectors that are pointing in
    // opposite directions.
    Vector3.Add( ref TestVector, ref Result.SurfaceNormal );
    double SurfaceTest = Vector3.Norm( ref TestVector );

    // MaxSurfaceTest: 0.0016429800
    // MaxAccelNorm: 0.03
    if( SurfaceTest > MaxSurfaceTest )
      MaxSurfaceTest = SurfaceTest;

    }




/*
  private void SetPlanetGravityAcceleration(
                  ref ReferenceFrame RefFrame )
    {

    RefFrame.SetPlanetGravityAcceleration(
                  ref Vector3.Vector Position )
                  ref Vector3.Vector Acceleration )


    }
*/



  internal void AllocateVertexArrays( int LatLonCount,
                             int RefVertCount )
    {
    LatLonRow = new LatLongPosition[LatLonCount];
    LatLonRowLast = LatLonCount;

    RefVertexArray = new
           ReferenceVertex[RefVertCount, VerticalVertexCount];

    RefVertexArrayLast = RefVertCount;
    }




  private int MakeSurfacePoleRow(
                             double ApproxLatitude,
                             int GraphicsIndex )
    {
    ReferenceVertex RefVertex = new ReferenceVertex();
    LatLongPosition Pos = new LatLongPosition();

    RefVertex.Index = GraphicsIndex;
    GraphicsIndex++;
    Pos.Longitude = 0;

    RefVertex.Position.X = 0;
    RefVertex.Position.Y = 0;
    Pos.SurfaceNormal.X = 0;
    Pos.SurfaceNormal.Y = 0;
    Pos.GeodeticLatitude = ApproxLatitude;

    Vector3.SetZero( ref RefVertex.CentrifugalAccel );

    if( ApproxLatitude > 0 )
      {
      RefVertex.Position.Z = ModelConstants.EarthRadiusMinor;
      Pos.SurfaceNormal.Z = 1;
      }
    else
      {
      RefVertex.Position.Z = -ModelConstants.EarthRadiusMinor;
      Pos.SurfaceNormal.Z = -1;
      }

    Pos.TextureX = Pos.Longitude + 180.0;
    Pos.TextureX = Pos.TextureX * ( 1.0d / 360.0d);

    Pos.TextureY = Pos.GeodeticLatitude + 90.0;
    Pos.TextureY = Pos.TextureY * ( 1.0d / 180.0d );
    Pos.TextureY = 1 - Pos.TextureY;

    SetGravityAcceleration( ref RefVertex, ref Pos );
    Vector3.Copy( ref RefVertex.Acceleration, ref RefVertex.EarthGravityAccel );
    // Vector3.Add( ref RefVertex.Acceleration, ref RefVertex.CentrifugalAccel );
    // Compare the acceleration with this.Acceleration.
    // Vector3.Add( ref RefVertex.Acceleration, ref this.Acceleration );

    LatLonRow[0] = Pos;
    RefVertexArray[0,0] = RefVertex;

    return GraphicsIndex;
    }



  internal int MakeSurfaceVertexRow(
                       double ApproxLatitude,
                       double LongitudeHoursRadians,
                       int GraphicsIndex )
    {
    try
    {
    if( LatLonRowLast < 2 )
      {
      return MakeSurfacePoleRow( ApproxLatitude, GraphicsIndex );
      }

    double LatRadians = NumbersEC.DegreesToRadians( ApproxLatitude );
    double CosLatRadians = Math.Cos( LatRadians );
    double SinLatRadians = Math.Sin( LatRadians );
    double CosLatRadiansPlusDelta = Math.Cos( LatRadians + LatitudeRadiansDelta );
    double SinLatRadiansPlusDelta = Math.Sin( LatRadians + LatitudeRadiansDelta );

    double LonStart = -180.0;

    // There is a beginning vertex at -180 longitude
    // and there is an ending vertex at 180
    // longitude, which is the same place, but they
    // are associated with different texture
    // coordinates.  One at the left end of the
    // texture and one at the right end.
    // So this is minus 1:
    double LonDelta = 360.0d / (double)(LatLonRowLast - 1);

    for( int Count = 0; Count < LatLonRowLast; Count++ )
      {
      ReferenceVertex RefVertex = new ReferenceVertex();
      LatLongPosition Pos = new LatLongPosition();

      RefVertex.Index = GraphicsIndex;
      GraphicsIndex++;

      Pos.Longitude = LonStart + (LonDelta * Count);
      // Pos.Index = LastVertexIndex;
      // LastVertexIndex++;
      SetLatLonPositionXYZ( ref RefVertex,
                            ref Pos,
                            CosLatRadians,
                            SinLatRadians,
                            CosLatRadiansPlusDelta,
                            SinLatRadiansPlusDelta,
                            ApproxLatitude,
                            LongitudeHoursRadians );

      SetGravityAcceleration( ref RefVertex, ref Pos );
      Vector3.Copy( ref RefVertex.Acceleration, ref RefVertex.EarthGravityAccel );
      Vector3.Add( ref RefVertex.Acceleration, ref RefVertex.CentrifugalAccel );

      // Compare the acceleration with this.Acceleration.
      // Vector3.Add( ref RefVertex.Acceleration, ref this.Acceleration );

      LatLonRow[Count] = Pos;
      RefVertexArray[Count,0] = RefVertex;
      }

    // ShowStatus( " " );


    // double MaxZDiffInMeters = MaxZDiff;
    // double MaxZDiffInMilliMeters = MaxZDiffInMeters * 1000d;
    // ShowStatus( "MaxZDiffInMilliMeters: " + MaxZDiffInMilliMeters.ToString( "N10" ));

    // MaxZDiffInMilliMeters: 0.000001
    // MaxDotProduct: 0.0033024614

    // ShowStatus( "MinDotProduct: " + MinDotProduct.ToString( "N10" ));
    // ShowStatus( "MaxDotProduct: " + MaxDotProduct.ToString( "N10" ));

    // ShowStatus( "MinFlatNorm in meters: " + MinFlatNorm.ToString( "N10" ));
    // ShowStatus( "MaxFlatNorm in meters: " + MaxFlatNorm.ToString( "N10" ));

    // ShowStatus( "MaxXVelocity in meters per second: " + MaxXVelocity.ToString( "N2" ));

    // MaxXVelocity in meters per second: 463.82
    // The speed of light in a vacuum is
    // 299,792,458 meters per second.

    // Get some idea how much the Lorentz
    // tranformation would change things at this
    // velocity.
    // double TestLorentz = (463.82d * 463.82d) /
    //           ModelConstants.SpeedOfLightSqr;

    // TestLorentz: 0.00000000000239363285
    // ShowStatus( "TestLorentz: " + TestLorentz.ToString( "N20" ));

    // ShowStatus( "MaxAccelNorm: " + MaxAccelNorm.ToString( "N2" ));
    // ShowStatus( "EarthAccelMin: " + EarthAccelMin.ToString( "N10" ));
    // ShowStatus( "EarthAccelMax: " + EarthAccelMax.ToString( "N10" ));
    // ShowStatus( "MaxSurfaceTest: " + MaxSurfaceTest.ToString( "N10" ));

    return GraphicsIndex;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthSlice.MakeVertexRow(): " + Except.Message );
      return -1;
      }
    }



  internal ReferenceVertex GetRefVertex( int Where, int Vert )
    {
    if( Where >= RefVertexArrayLast )
      throw( new Exception( "Where >= RefVertexArrayLast." ));

    return RefVertexArray[Where, Vert];
    }



  internal LatLongPosition GetLatLongPosition( int Where )
    {
    if( Where >= LatLonRowLast )
      throw( new Exception( "Where >= LatLonRowLast." ));

    return LatLonRow[Where];
    }




  private void MakeVerticalVertexes()
    {
/*
    for( int Row = 0; Row < RefVertexArrayLast; Row++ )
      {
      int HowMany = RefVertexArray[Row, 0].RowLast;
      for( int Count = 0; Count < HowMany; Count++ )
        {
        ReferenceVertex BottomVertex =
             RefVertexArray[Row].RefVertexArray[Count, 0];

        Vector3.Vector DirectionVector = new Vector3.Vector();
        Vector3.Copy( ref DirectionVector,
                      ref BottomVertex.Position );
        Vector3.Normalize( ref DirectionVector,
                           DirectionVector );

        // RadiusMajor = 6378137  Equator
        // 10K meters:
        double VerticalDelta = 10000d; // size /  VerticalVertexCount;
        Vector3.MultiplyWithScalar( ref DirectionVector, VerticalDelta );
       
        for( int Vertical = 1; Vertical < ModelConstants.VerticalVertexCount; Vertical++ )
          {
          ReferenceVertex NextLower = VertexRows[Row].
               RefVertexArray[Count, Vertical - 1];

          ReferenceVertex RefVertex = new ReferenceVertex();
          Vector3.Copy( ref RefVertex.Position, ref NextLower.Position );
          Vector3.Add( ref RefVertex.Position, ref DirectionVector );




======
          VertexRows[Row].RefVertexArray[Count, Vertical] =
                       new
                    ReferenceVertex[HowMany, VerticalVertexCount];
======




          }
        }
      }
    */
    }




  }
}
