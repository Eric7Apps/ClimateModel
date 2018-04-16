// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// Shakespeare: "There is a tide in the affairs of
// men, Which, taken at the flood, leads on to
// fortune; Omitted, all the voyage of their life
// Is bound in shallows and in miseries. On such a
// full sea are we now afloat. And we must take the
// current when it serves. Or lose our ventures."


// WGS84: Used by the Global Positioning System.
// https://en.wikipedia.org/wiki/World_Geodetic_System
// https://en.wikipedia.org/wiki/Geoid
// https://en.wikipedia.org/wiki/Geodesy
// https://en.wikipedia.org/wiki/Spheroid
// https://en.wikipedia.org/wiki/Bathymetry
// https://en.wikipedia.org/wiki/Spherical_harmonics
// https://en.wikipedia.org/wiki/EGM96
// https://en.wikipedia.org/wiki/Effective_potential
// https://en.wikipedia.org/wiki/Reference_ellipsoid


using System;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;



namespace ClimateModel
{
  class EarthGeoid : SpaceObject
  {
  private MainForm MForm;
  internal string TextureFileName = "";
  private const double RadiusMajor = 6378137d; // Equator
  private const double RadiusMinor = 6356752d; // poles
  // Test squished ellipsoid:
  // private const double RadiusMinor = 2000000d;
  private MeshGeometry3D Surface;
  private GeometryModel3D GeoMod;
  private VertexRow[] VertexRows;

  private const int RowLatDelta = 5;
  private const int RowsFromEquatorToPole =
            90 / RowLatDelta;
  private const int VertexRowsLast =
    (RowsFromEquatorToPole * 2) + 1;

  private const int VertexRowsMiddle =
     RowsFromEquatorToPole + 1;

  private const int MaximumVertexes = 256;

  private int LastVertexIndex = 0;
  internal double LongitudeHoursRadians = 0; // Time change.
  private double MaxZDiff = 0;
  private const double LatitudeRadiansDelta = 0.0000001d;
  private double MinDotProduct = 10;
  private double MaxDotProduct = 0;
  private double MinFlatNorm = 100000000000000d;
  private double MaxFlatNorm = 0;
  private double MaxXVelocity = 0;
  private double MaxAccelNorm = 0;
  private double EarthAccelMin = 1000000000;
  private double EarthAccelMax = 0;
  private double MaxSurfaceTest = 0;


  public struct LatLongPosition
    {
    public int Index;
    public double GeodeticLatitude;
    public double Longitude;
    public Vector3.Vector Position;
    public Vector3.Vector SurfaceNormal;
    public Vector3.Vector Velocity;
    public Vector3.Vector CentrifugalAccel;
    public Vector3.Vector GravityAccel;
    // public Vector3.Vector MoonAccel;

    public double TextureX;
    public double TextureY;
    // public double Elevation;
    }



  public struct VertexRow
    {
    // A vertical column of vertexes above each
    // vertex on the surface.  Row, Column
    // public LatLongPosition[,] Row;

    public LatLongPosition[] Row;
    public int RowLast;
    }



  private EarthGeoid()
    {
    }



  internal EarthGeoid( MainForm UseForm )
    {
    MForm = UseForm;

    GeoMod = new GeometryModel3D();
    }



  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  internal override GeometryModel3D GetGeometryModel()
    {
    return GeoMod;
    }



  internal override void MakeNewGeometryModel()
    {
    try
    {
    DiffuseMaterial SolidMat = new DiffuseMaterial();
    // SolidMat.Brush = Brushes.Blue;
    SolidMat.Brush = SetTextureImageBrush();

    MakeGeoidModel();

    // if( Surface == null )

    GeoMod.Geometry = Surface;
    GeoMod.Material = SolidMat;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakeNewGeometryModel(): " + Except.Message );
      }
    }



  private ImageBrush SetTextureImageBrush()
    {
    // Imaging Namespace:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging?view=netframework-4.7.1

    // ImageDrawing:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imagedrawing?view=netframework-4.7.1

    BitmapImage BMapImage = new BitmapImage();

    // Things have to be in this Begin-end block.
    BMapImage.BeginInit();

    BMapImage.UriSource = new Uri( TextureFileName );

    // BMapImage.DecodePixelWidth = 200;

    BMapImage.EndInit();

    // ImageBrush:
    // https://msdn.microsoft.com/en-us/library/system.windows.media.imagebrush(v=vs.110).aspx
    ImageBrush ImgBrush = new ImageBrush();
    ImgBrush.ImageSource = BMapImage;
    return ImgBrush;
    }



  private void SetLatLonPositionXYZ(
                      ref LatLongPosition Result,
                      double CosLatRadians,
                      double SinLatRadians,
                      double CosLatRadiansPlusDelta,
                      double SinLatRadiansPlusDelta,
                      double ApproxLatitude )
    {
    // "IERS Reference Meridian: 102 meters east
    // of the Greenwich meridian."

    // Latitude:
    // https://en.wikipedia.org/wiki/Latitude

    double LonRadians = NumbersEC.DegreesToRadians( Result.Longitude );

    // Higher hours make the sun set in the west.
    LonRadians += LongitudeHoursRadians;

    double CosLonRadians = Math.Cos( LonRadians );
    double SinLonRadians = Math.Sin( LonRadians );

    // Along the equatorial axis:
    Result.Position.X = RadiusMajor * (CosLatRadians * CosLonRadians );
    Result.Position.Y = RadiusMajor * (CosLatRadians * SinLonRadians );

    // Along the polar axis:
    Result.Position.Z = RadiusMinor * SinLatRadians;

    // This sets the Geodetic latitude too.
    SetSurfaceNormal( ref Result,
                   CosLatRadians,
                   SinLatRadians,
                   CosLatRadiansPlusDelta,
                   SinLatRadiansPlusDelta,
                   CosLonRadians,
                   SinLonRadians,
                   ApproxLatitude );

    SetCentrifugalAcceleration( ref Result,
                      CosLatRadians,
                      SinLatRadians,
                      CosLonRadians,
                      SinLonRadians,
                      LonRadians );

    SetGravityAcceleration( ref Result );


    Result.TextureX = Result.Longitude + 180.0;
    Result.TextureX = Result.TextureX * ( 1.0d / 360.0d);

    Result.TextureY = Result.GeodeticLatitude + 90.0;
    Result.TextureY = Result.TextureY * ( 1.0d / 180.0d );
    Result.TextureY = 1 - Result.TextureY;
    }



  private void SetSurfaceNormal(
                      ref LatLongPosition Result,
                      double CosLatRadians,
                      double SinLatRadians,
                      double CosLatRadiansPlusDelta,
                      double SinLatRadiansPlusDelta,
                      double CosLonRadians,
                      double SinLonRadians,
                      double ApproxLatitude )
    {
    // If it's the north pole.
    if( ApproxLatitude > 89.9999)
      {
      Result.SurfaceNormal.X = 0;
      Result.SurfaceNormal.Y = 0;
      Result.SurfaceNormal.Z = 1;
      Result.GeodeticLatitude = ApproxLatitude;
      return;
      }

    // South pole.
    if( ApproxLatitude < -89.9999)
      {
      Result.SurfaceNormal.X = 0;
      Result.SurfaceNormal.Y = 0;
      Result.SurfaceNormal.Z = -1;
      Result.GeodeticLatitude = ApproxLatitude;
      return;
      }

    // Equator
    if( (ApproxLatitude < 0.00001) &&
        (ApproxLatitude > -0.00001))
      {
      // This avoids calculating a perpendicular
      // with two vectors _very_ close to each other.
      // It avoids normalizing a very small vector.
      Result.SurfaceNormal.X = Result.Position.X;
      Result.SurfaceNormal.Y = Result.Position.Y;
      Result.SurfaceNormal.Z = 0;
      Vector3.Normalize( ref Result.SurfaceNormal, Result.SurfaceNormal );
      Result.GeodeticLatitude = ApproxLatitude;
      return;
      }

    Vector3.Vector PositionAtDelta = new Vector3.Vector();
    PositionAtDelta.X = RadiusMajor * (CosLatRadiansPlusDelta * CosLonRadians );
    PositionAtDelta.Y = RadiusMajor * (CosLatRadiansPlusDelta * SinLonRadians );
    PositionAtDelta.Z = RadiusMinor * SinLatRadiansPlusDelta;

    Vector3.Vector FlatVector = new Vector3.Vector();

    Vector3.Copy( ref FlatVector, ref PositionAtDelta );
    Vector3.Subtract( ref FlatVector, ref Result.Position );

    if( Result.Position.Z > PositionAtDelta.Z )
      throw( new Exception( "Result.Position.Z > PositionAtDelta.Z." ));

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

    if( Result.Position.Z >= 0 )
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
    if( Result.Position.Z < 0 )
      Vector3.Negate( ref PerpenVector );

    Vector3.Copy( ref Result.SurfaceNormal, ref PerpenVector );
    }




  private void SetCentrifugalAcceleration(
                      ref LatLongPosition Result,
                      double CosLatRadians,
                      double SinLatRadians,
                      double CosLonRadians,
                      double SinLonRadians,
                      double LonRadians )
    {
    double XNoLongitude = RadiusMajor * CosLatRadians;
    TestZValue( Result.Position.Z, SinLatRadians, XNoLongitude );
    ///////////


    Vector3.Vector CentrifugeVector = new Vector3.Vector();
    CentrifugeVector.X = Result.Position.X;
    CentrifugeVector.Y = Result.Position.Y;
    CentrifugeVector.Z = 0;

    double CentrifugeRadius = Vector3.Norm( ref CentrifugeVector );

    // This circumferance is the path that a vertex
    // takes as it rotates around on the surface of
    // the Earth.
    double Circumference =
             CentrifugeRadius * 2.0d * Math.PI;

    double LongitudeRadiansDelta = 2.0d  * Math.PI;
    // One hour:
    LongitudeRadiansDelta =
                      LongitudeRadiansDelta / 24.0d;
    // One Minute:
    LongitudeRadiansDelta =
                      LongitudeRadiansDelta / 60.0d;
    // One Second:
    LongitudeRadiansDelta =
                      LongitudeRadiansDelta / 60.0d;

    // Radians per second: LongitudeRadiansDelta


    double CosLonRadiansMoveTo =
       Math.Cos( LonRadians + LongitudeRadiansDelta );
    double SinLonRadiansMoveTo =
       Math.Sin( LonRadians + LongitudeRadiansDelta );

    // The position one second later.
    Vector3.Vector MoveTo = new Vector3.Vector();
    MoveTo.X = RadiusMajor * (CosLatRadians * CosLonRadiansMoveTo );
    MoveTo.Y = RadiusMajor * (CosLatRadians * SinLonRadiansMoveTo );
    MoveTo.Z = RadiusMinor * SinLatRadians;

    double CosLonRadiansEarlier =
       Math.Cos( LonRadians - LongitudeRadiansDelta );
    double SinLonRadiansEarlier =
       Math.Sin( LonRadians - LongitudeRadiansDelta );

    // The position one second earlier.
    Vector3.Vector EarlierPosition = new Vector3.Vector();
    EarlierPosition.X = RadiusMajor * (CosLatRadians * CosLonRadiansEarlier );
    EarlierPosition.Y = RadiusMajor * (CosLatRadians * SinLonRadiansEarlier );
    EarlierPosition.Z = RadiusMinor * SinLatRadians;

    // This is how many meters it has moved in one
    // second.  So velocity is meters per second.
    Vector3.Vector VelocityMoveTo = new Vector3.Vector();
    Vector3.Copy( ref VelocityMoveTo, ref MoveTo );
    Vector3.Subtract( ref VelocityMoveTo, ref Result.Position );

    Vector3.Copy( ref Result.Velocity, ref VelocityMoveTo );

    // The velocity in the period from one second
    // earlier.
    Vector3.Vector VelocityEarlier = new Vector3.Vector();
    Vector3.Copy( ref VelocityEarlier, ref Result.Position );
    Vector3.Subtract( ref VelocityEarlier, ref EarlierPosition );

    // This is the change in velocity in one second.

    // Earlier + acceleration = Later.
    // Later - Earlier = acceleration.
    Vector3.Copy( ref Result.CentrifugalAccel, ref VelocityMoveTo );
    Vector3.Subtract( ref Result.CentrifugalAccel, ref VelocityEarlier );

    // Make it point outward.
    Vector3.Negate( ref Result.CentrifugalAccel );

    Vector3.Vector TestDirection = new Vector3.Vector();
    Vector3.Copy( ref TestDirection, ref Result.CentrifugalAccel );
    Vector3.Normalize( ref TestDirection, TestDirection );
    Vector3.Normalize( ref CentrifugeVector, CentrifugeVector );
    // The cosine between them should be 1.0 since
    // they should point in exactly the same
    // direction.
    
    double Dot = Vector3.DotProduct( 
           ref TestDirection, ref CentrifugeVector );
    if( (Dot > 1.00000001) || (Dot < 0.99999999) )
      throw( new Exception( "The vectors don't line up: " + Dot.ToString( "N10" ) ));

    double AccelNorm = Vector3.Norm( ref Result.CentrifugalAccel );
    // MaxAccelNorm is pretty small: 0.03

    if( AccelNorm > MaxAccelNorm )
      MaxAccelNorm = AccelNorm;

    if( Result.Velocity.X > MaxXVelocity )
      MaxXVelocity = Result.Velocity.X;

    }




  private void SetGravityAcceleration(
                      ref LatLongPosition Result )
    {
    double EarthRadius =
                 Vector3.Norm( ref Result.Position );

    Vector3.Copy( ref Result.GravityAccel, ref Result.Position );
    Vector3.Normalize( ref Result.GravityAccel, Result.GravityAccel );

    // Make it point toward the center of gravity.
    Vector3.Negate( ref Result.GravityAccel );

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

    Vector3.MultiplyWithScalar( ref Result.GravityAccel, EarthAccel );

    Vector3.Vector TestVector = new Vector3.Vector();
    Vector3.Copy( ref TestVector, ref Result.GravityAccel );
    Vector3.Add( ref TestVector, ref Result.CentrifugalAccel );
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



  private void AddSurfaceVertex( LatLongPosition Pos )
    {
    // Surface.Positions.Count
    // Surface.Positions.Items[Index];
    // Surface.Positions.Add() adds it to the end.
    // Surface.Positions.Clear(); Removes all values.

    // Use a scale for drawing.

    double ScaledX = (Position.X + Pos.Position.X) * PlanetSphere.ThreeDSizeScale;
    double ScaledY = (Position.Y + Pos.Position.Y) * PlanetSphere.ThreeDSizeScale;
    double ScaledZ = (Position.Z + Pos.Position.Z) * PlanetSphere.ThreeDSizeScale;
    Point3D VertexP = new Point3D( ScaledX, ScaledY, ScaledZ );
    Surface.Positions.Add( VertexP );

    // Texture coordinates are "scaled by their
    // bounding box".  You have to create the right
    // "bounding box."  You have to give it bounds
    // by setting vertexes out on the edges.  In
    // the example above for latitude/longitude,
    // you have to set both the North Pole and
    // the South Pole vertexes in order to give
    // the north and south latitudes a "bounding box"
    // so that the texture can be scaled all the way
    // from north to south.  And you have to set
    // vertexes at 180 longitude and -180 longitude
    // (out on the edges) to give it the right
    // bounding box for longitude.  Otherwise it will
    // scale the texture image in ways you don't want.

    Point TexturePoint = new Point( Pos.TextureX, Pos.TextureY );
    Surface.TextureCoordinates.Add( TexturePoint );

    Vector3D SurfaceNormal = new Vector3D( Pos.SurfaceNormal.X, Pos.SurfaceNormal.Y, Pos.SurfaceNormal.Z );
    Surface.Normals.Add( SurfaceNormal );
    }



  private void AddSurfaceTriangleIndex( int Index1,
                                        int Index2,
                                        int Index3 )
    {
    Surface.TriangleIndices.Add( Index1 );
    Surface.TriangleIndices.Add( Index2 );
    Surface.TriangleIndices.Add( Index3 );
    }



  private void MakeGeoidModel()
    {
    try
    {
    MaxZDiff = 0;
    LastVertexIndex = 0;

    // This is an initial approximation to what will
    // be geodetic latitude.  It is a starting point
    // for something that gets more refined and exact.
    double ApproxLatitude = 0;

    Surface = new MeshGeometry3D();

    VertexRows = new VertexRow[VertexRowsLast];

    LatLongPosition PosNorthPole = new LatLongPosition();
    ApproxLatitude = 90.0;
    PosNorthPole.Longitude = 0;
    PosNorthPole.Index = LastVertexIndex;
    LastVertexIndex++;

    double LatRadians = NumbersEC.DegreesToRadians( ApproxLatitude );
    double CosLatRadians = Math.Cos( LatRadians );
    double SinLatRadians = Math.Sin( LatRadians );
    double CosLatRadiansPlusDelta = Math.Cos( LatRadians + LatitudeRadiansDelta );
    double SinLatRadiansPlusDelta = Math.Sin( LatRadians + LatitudeRadiansDelta );

    SetLatLonPositionXYZ( ref PosNorthPole,
                              CosLatRadians,
                              SinLatRadians,
                              CosLatRadiansPlusDelta,
                              SinLatRadiansPlusDelta,
                              ApproxLatitude );

    LatLongPosition PosSouthPole = new LatLongPosition();
    ApproxLatitude = -90.0;
    PosSouthPole.Longitude = 0;
    PosSouthPole.Index = LastVertexIndex;
    LastVertexIndex++;

    LatRadians = NumbersEC.DegreesToRadians( ApproxLatitude );
    CosLatRadians = Math.Cos( LatRadians );
    SinLatRadians = Math.Sin( LatRadians );
    CosLatRadiansPlusDelta = Math.Cos( LatRadians + LatitudeRadiansDelta );
    SinLatRadiansPlusDelta = Math.Sin( LatRadians + LatitudeRadiansDelta );

    SetLatLonPositionXYZ( ref PosSouthPole,
                              CosLatRadians,
                              SinLatRadians,
                              CosLatRadiansPlusDelta,
                              SinLatRadiansPlusDelta,
                              ApproxLatitude );

    VertexRows[0] = new VertexRow();
    VertexRows[0].Row = new LatLongPosition[1];
    VertexRows[0].RowLast = 1;
    VertexRows[0].Row[0] = PosNorthPole;
    AddSurfaceVertex( PosNorthPole );

    VertexRows[VertexRowsLast - 1] = new VertexRow();
    VertexRows[VertexRowsLast - 1].Row = new LatLongPosition[1];
    VertexRows[VertexRowsLast - 1].RowLast = 1;
    VertexRows[VertexRowsLast - 1].Row[0] = PosSouthPole;
    AddSurfaceVertex( PosSouthPole );

    double RowLatitude = 90;
    int HowMany = 4;
    for( int Index = 1; Index <= VertexRowsMiddle; Index++ )
      {
      RowLatitude -= RowLatDelta;
      MakeOneVertexRow( Index, HowMany, RowLatitude );
      if( HowMany < MaximumVertexes )
        HowMany = HowMany * 2;

      }


    RowLatitude = -90;
    HowMany = 4;
    for( int Index = VertexRowsLast - 2; Index > VertexRowsMiddle; Index-- )
      {
      RowLatitude += RowLatDelta;
      MakeOneVertexRow( Index, HowMany, RowLatitude );
      if( HowMany < MaximumVertexes )
        HowMany = HowMany * 2;

      }

    MakePoleTriangles();

    for( int Index = 0; Index < VertexRowsLast - 2; Index++ )
      {
      if( VertexRows[Index].RowLast == VertexRows[Index + 1].RowLast )
        {
        MakeRowTriangles( Index, Index + 1 );
        }
      else
        {
        if( VertexRows[Index].RowLast < VertexRows[Index + 1].RowLast )
          {
          MakeDoubleRowTriangles( Index, Index + 1 );
          }
        else
          {
          MakeDoubleReverseRowTriangles( Index + 1, Index );
          }
        }
      }

    double MaxZDiffInMeters = MaxZDiff;
    double MaxZDiffInMilliMeters = MaxZDiffInMeters * 1000d;
    ShowStatus( "MaxZDiffInMilliMeters: " + MaxZDiffInMilliMeters.ToString( "N10" ));

    // MaxZDiffInMilliMeters: 0.000001
    // MaxDotProduct: 0.0033024614

    ShowStatus( "MinDotProduct: " + MinDotProduct.ToString( "N10" ));
    ShowStatus( "MaxDotProduct: " + MaxDotProduct.ToString( "N10" ));

    ShowStatus( "MinFlatNorm in meters: " + MinFlatNorm.ToString( "N10" ));
    ShowStatus( "MaxFlatNorm in meters: " + MaxFlatNorm.ToString( "N10" ));

    ShowStatus( "MaxXVelocity in meters per second: " + MaxXVelocity.ToString( "N2" ));

    // MaxXVelocity in meters per second: 463.82
    // The speed of light in a vacuum is
    // 299,792,458 meters per second.

    // Get some idea how much the Lorentz
    // tranformation would change things at this
    // velocity.
    double TestLorentz = (463.82d * 463.82d) /
               ModelConstants.SpeedOfLightSqr;

    // TestLorentz: 0.00000000000239363285
    ShowStatus( "TestLorentz: " + TestLorentz.ToString( "N20" ));

    ShowStatus( "MaxAccelNorm: " + MaxAccelNorm.ToString( "N2" ));

    ShowStatus( "EarthAccelMin: " + EarthAccelMin.ToString( "N10" ));
    ShowStatus( "EarthAccelMax: " + EarthAccelMax.ToString( "N10" ));

    ShowStatus( "MaxSurfaceTest: " + MaxSurfaceTest.ToString( "N10" ));

    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakeSphericalModel(): " + Except.Message );
      }
    }



  private void MakePoleTriangles()
    {
    try
    {
    LatLongPosition PosNorthPole = VertexRows[0].Row[0];
    LatLongPosition PosSouthPole = VertexRows[VertexRowsLast - 1].Row[0];

    // This assumes there are at least 4 in this row.
    LatLongPosition Pos1 = VertexRows[1].Row[0];
    LatLongPosition Pos2 = VertexRows[1].Row[1];
    LatLongPosition Pos3 = VertexRows[1].Row[2];
    LatLongPosition Pos4 = VertexRows[1].Row[3];

    // Counterclockwise winding goes toward the
    // viewer.

    AddSurfaceTriangleIndex( PosNorthPole.Index,
                                     Pos1.Index,
                                     Pos2.Index );

    AddSurfaceTriangleIndex( PosNorthPole.Index,
                                     Pos2.Index,
                                     Pos3.Index );

    AddSurfaceTriangleIndex( PosNorthPole.Index,
                                     Pos3.Index,
                                     Pos4.Index );


    // South pole:
    Pos1 = VertexRows[VertexRowsLast - 2].Row[0];
    Pos2 = VertexRows[VertexRowsLast - 2].Row[1];
    Pos3 = VertexRows[VertexRowsLast - 2].Row[2];
    Pos4 = VertexRows[VertexRowsLast - 2].Row[3];

    // Counterclockwise winding as seen from south
    // of the south pole:
    AddSurfaceTriangleIndex( PosSouthPole.Index,
                                     Pos4.Index,
                                     Pos3.Index );

    AddSurfaceTriangleIndex( PosSouthPole.Index,
                                     Pos3.Index,
                                     Pos2.Index );

    AddSurfaceTriangleIndex( PosSouthPole.Index,
                                     Pos2.Index,
                                     Pos1.Index );

    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakePoleTriangles(): " + Except.Message );
      }
    }



  private bool MakeRowTriangles( int FirstRow, int SecondRow )
    {
    try
    {
    int RowLength = VertexRows[FirstRow].RowLast;
    int SecondRowLength = VertexRows[SecondRow].RowLast;
    if( RowLength != SecondRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "RowLength != SecondRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }


    for( int RowIndex = 0; (RowIndex + 1) < RowLength; RowIndex++ )
      {
      LatLongPosition Pos1 = VertexRows[FirstRow].Row[RowIndex];
      LatLongPosition Pos2 = VertexRows[SecondRow].Row[RowIndex];
      LatLongPosition Pos3 = VertexRows[SecondRow].Row[RowIndex + 1];

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      Pos1 = VertexRows[SecondRow].Row[RowIndex + 1];
      Pos2 = VertexRows[FirstRow].Row[RowIndex + 1];
      Pos3 = VertexRows[FirstRow].Row[RowIndex];

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      }

    return true;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakeRowTriangles(): " + Except.Message );
      return false;
      }
    }



  private bool MakeDoubleRowTriangles( int FirstRow, int DoubleRow )
    {
    try
    {
    int RowLength = VertexRows[FirstRow].RowLast;
    int DoubleRowLength = VertexRows[DoubleRow].RowLast;
    if( (RowLength * 2) > DoubleRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "(RowLength * 2) > DoubleRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }

    LatLongPosition Pos1 = VertexRows[FirstRow].Row[0];
    LatLongPosition Pos2 = VertexRows[DoubleRow].Row[0];
    LatLongPosition Pos3 = VertexRows[DoubleRow].Row[1];

    AddSurfaceTriangleIndex( Pos1.Index,
                             Pos2.Index,
                             Pos3.Index );

    for( int RowIndex = 1; RowIndex < RowLength; RowIndex++ )
      {
      int DoubleRowIndex = RowIndex * 2;
      Pos1 = VertexRows[FirstRow].Row[RowIndex + 0];
      Pos2 = VertexRows[DoubleRow].Row[DoubleRowIndex + 0];
      Pos3 = VertexRows[DoubleRow].Row[DoubleRowIndex + 1];

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45

      Pos1 = VertexRows[FirstRow].Row[RowIndex + 0];
      Pos2 = VertexRows[DoubleRow].Row[DoubleRowIndex - 1];
      Pos3 = VertexRows[DoubleRow].Row[DoubleRowIndex];
      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45
      Pos1 = VertexRows[DoubleRow].Row[DoubleRowIndex - 1];
      Pos2 = VertexRows[FirstRow].Row[RowIndex];
      Pos3 = VertexRows[FirstRow].Row[RowIndex - 1];
      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      }

    return true;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakeDoubleRowTriangles(): " + Except.Message );
      return false;
      }
    }



  private bool MakeDoubleReverseRowTriangles( int BottomRow, int DoubleRow )
    {
    try
    {
    int RowLength = VertexRows[BottomRow].RowLast;
    int DoubleRowLength = VertexRows[DoubleRow].RowLast;
    if( (RowLength * 2) > DoubleRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "DoubleReverse: (RowLength * 2) > DoubleRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }

    LatLongPosition Pos1 = VertexRows[BottomRow].Row[0];
    LatLongPosition Pos2 = VertexRows[DoubleRow].Row[1];
    LatLongPosition Pos3 = VertexRows[DoubleRow].Row[0];

    AddSurfaceTriangleIndex( Pos1.Index,
                             Pos2.Index,
                             Pos3.Index );

    for( int RowIndex = 1; RowIndex < RowLength; RowIndex++ )
      {
      int DoubleRowIndex = RowIndex * 2;
      Pos1 = VertexRows[BottomRow].Row[RowIndex];
      Pos2 = VertexRows[DoubleRow].Row[DoubleRowIndex + 1];
      Pos3 = VertexRows[DoubleRow].Row[DoubleRowIndex];

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );


      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45

      Pos1 = VertexRows[BottomRow].Row[RowIndex + 0];
      Pos2 = VertexRows[DoubleRow].Row[DoubleRowIndex];
      Pos3 = VertexRows[DoubleRow].Row[DoubleRowIndex - 1];
      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45
      Pos1 = VertexRows[DoubleRow].Row[DoubleRowIndex - 1];
      Pos2 = VertexRows[BottomRow].Row[RowIndex - 1];
      Pos3 = VertexRows[BottomRow].Row[RowIndex];
      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      }

    return true;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakeDoubleRowTriangles(): " + Except.Message );
      return false;
      }
    }




  private bool MakeOneVertexRow( int RowIndex,
                                 int HowMany,
                                 double ApproxLatitude )
    {
    try
    {
    VertexRows[RowIndex] = new VertexRow();
    VertexRows[RowIndex].Row = new LatLongPosition[HowMany];
    VertexRows[RowIndex].RowLast = HowMany;

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
    double LonDelta = 360.0d / (double)(HowMany - 1);

    for( int Count = 0; Count < HowMany; Count++ )
      {
      LatLongPosition Pos = new LatLongPosition();

      Pos.Longitude = LonStart + (LonDelta * Count);
      Pos.Index = LastVertexIndex;
      LastVertexIndex++;

      SetLatLonPositionXYZ( ref Pos,
                            CosLatRadians,
                            SinLatRadians,
                            CosLatRadiansPlusDelta,
                            SinLatRadiansPlusDelta,
                            ApproxLatitude );

      VertexRows[RowIndex].Row[Count] = Pos;
      AddSurfaceVertex( Pos );
      }

    return true;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakeSphericalModel(): " + Except.Message );
      return false;
      }
    }



  }
}
