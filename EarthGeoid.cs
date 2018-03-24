// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com

// Subtract one velocity vector from another to get
// the acceleration vector for that time period.
// The acceleration vector for a vertex on the surface
// can be used to test the equipotential
// surface.  (The ocean surface would flow toward
// or away from that point until it gets settled at
// the right geoid shape.)
// But that's for an idealized body that
// has the density the same all the way through it.


// And if I _don't_ calculate the water flow this
// way then what happens when I calculate water flow
// for tides, etc?  Include density values for
// _inside_ the earth?  Make a complex model of the
// density on the inside of the earth?



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
// using System.Collections.Generic;
using System.Text;
// using System.Threading.Tasks;
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
  internal double RadiusMajor = 6.378137; // Equator
  internal double RadiusMinor = 6.356752; // poles
  // internal double RadiusMinor = 3; // show test
  private MeshGeometry3D Surface;
  private GeometryModel3D GeoMod;
  private double DistanceScale = 0.03;
  private VertexRow[] VertexRows;
  private int VertexRowsLast = 0;
  private int LastVertexIndex = 0;
  internal double LongitudeHoursRadians = 0; // Time change.



  public struct LatLongPosition
    {
    public int Index;
    public double Latitude;
    public double Longitude;
    public double X;
    public double Y;
    public double Z;
    public double VelocityX;
    public double VelocityY;
    public double VelocityZ;
    public double TextureX;
    public double TextureY;
    // public double Radius;
    // public double Elevation;
    }



  public struct VertexRow
    {
    public LatLongPosition[] Row;
    public int RowLast;
    }




//       Array.Resize( ref TriArray, TriArray.Length + (4 * 1024) );



  private EarthGeoid()
    {
    }



  internal EarthGeoid( MainForm UseForm,
                    double UseDistanceScale )
    {
    MForm = UseForm;
    DistanceScale = UseDistanceScale;

    GeoMod = new GeometryModel3D();
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
      MForm.ShowStatus( "Exception in EarthGeoid.MakeNewGeometryModel(): " + Except.Message );
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



  internal void SetLatLonPositionXYZ(
                      ref LatLongPosition Result,
                      double CosLatRadians,
                      double SinLatRadians )
    {
    // This is using Geocentric Latitude.

    // Geocentric Latitude: The angle between
    // the equator and a line going from the center
    // of the ellipsoid.

    // Geodetic Latitude: The angle between
    // the equatorial plane and a line that
    // is normal to the surface of the ellipsoid.

    // "IERS Reference Meridian: 102 meters east
    // of the Greenwich meridian."

    double LonRadians = NumbersEC.DegreesToRadians( Result.Longitude );

    // Higher hours make the sun go west.
    LonRadians += LongitudeHoursRadians;

    double CosLonRadians = Math.Cos( LonRadians );
    double SinLonRadians = Math.Sin( LonRadians );

    // Along the equatorial axis:
    Result.X = RadiusMajor * (CosLatRadians * CosLonRadians );
    Result.Y = RadiusMajor * (CosLatRadians * SinLonRadians );

    // Along the polar axis:
    Result.Z = RadiusMinor * SinLatRadians;

    Result.X += X * DistanceScale;
    Result.Y += Y * DistanceScale;
    Result.Z += Z * DistanceScale;

    // === Result.VelocityX

 
    Result.TextureX = Result.Longitude + 180.0;
    Result.TextureX = Result.TextureX * ( 1.0d / 360.0d);

    Result.TextureY = Result.Latitude + 90.0;
    Result.TextureY = Result.TextureY * ( 1.0d / 180.0d );
    Result.TextureY = 1 - Result.TextureY;
    }



  private void AddSurfaceVertex( LatLongPosition Pos )
    {
    // Surface.Positions.Count
    // Surface.Positions.Items[Index];
    // Surface.Positions.Add() adds it to the end.
    // Surface.Positions.Clear(); Removes all values.

    Point3D VertexP = new Point3D( Pos.X, Pos.Y, Pos.Z );
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

    Vector3D SurfaceNormal = new Vector3D( Pos.X, Pos.Y, Pos.Z );
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
    Surface = new MeshGeometry3D();

    LastVertexIndex = 0;
    VertexRowsLast = 20 - 1;
    int VertexRowsMiddle = 9;

    VertexRows = new VertexRow[VertexRowsLast];

    LatLongPosition PosNorthPole = new LatLongPosition();
    PosNorthPole.Latitude = 90.0;
    PosNorthPole.Longitude = 0;
    PosNorthPole.Index = LastVertexIndex;
    LastVertexIndex++;

    double LatRadians = NumbersEC.DegreesToRadians( PosNorthPole.Latitude );
    double CosLatRadians = Math.Cos( LatRadians );
    double SinLatRadians = Math.Sin( LatRadians );

    SetLatLonPositionXYZ( ref PosNorthPole,
                              CosLatRadians,
                              SinLatRadians );

    LatLongPosition PosSouthPole = new LatLongPosition();
    PosSouthPole.Latitude = -90.0;
    PosSouthPole.Longitude = 0;
    PosSouthPole.Index = LastVertexIndex;
    LastVertexIndex++;

    LatRadians = NumbersEC.DegreesToRadians( PosSouthPole.Latitude );
    CosLatRadians = Math.Cos( LatRadians );
    SinLatRadians = Math.Sin( LatRadians );
    SetLatLonPositionXYZ( ref PosSouthPole,
                              CosLatRadians,
                              SinLatRadians );

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
    double RowLatDelta = 10;

    int MaximumVertexes = 256;
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

    FreeVertexRows();
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in EarthGeoid.MakeSphericalModel(): " + Except.Message );
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
      MForm.ShowStatus( "Exception in EarthGeoid.MakePoleTriangles(): " + Except.Message );
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
      MForm.ShowStatus( "Exception in EarthGeoid.MakeRowTriangles(): " + Except.Message );
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
      MForm.ShowStatus( "Exception in EarthGeoid.MakeDoubleRowTriangles(): " + Except.Message );
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
      MForm.ShowStatus( "Exception in EarthGeoid.MakeDoubleRowTriangles(): " + Except.Message );
      return false;
      }
    }



  private void FreeVertexRows()
    {
    for( int Count = 0; Count < VertexRowsLast; Count++ )
      {
      VertexRows[Count].Row = null;
      }
    }



  private bool MakeOneVertexRow( int RowIndex,
                                 int HowMany,
                                 double Latitude )
    {
    try
    {
    VertexRows[RowIndex] = new VertexRow();
    VertexRows[RowIndex].Row = new LatLongPosition[HowMany];
    VertexRows[RowIndex].RowLast = HowMany;

    double LatRadians = NumbersEC.DegreesToRadians( Latitude );
    double CosLatRadians = Math.Cos( LatRadians );
    double SinLatRadians = Math.Sin( LatRadians );

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
      Pos.Latitude = Latitude;

      // The sine and cosine of this longitude could
      // be saved in an array for the next row of
      // equal size.  (Like 1024 vertexes or what
      // ever.)

      Pos.Longitude = LonStart + (LonDelta * Count);
      Pos.Index = LastVertexIndex;
      LastVertexIndex++;

      SetLatLonPositionXYZ( ref Pos,
                            CosLatRadians,
                            SinLatRadians );

      VertexRows[RowIndex].Row[Count] = Pos;
      AddSurfaceVertex( Pos );
      }

    return true;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in EarthGeoid.MakeSphericalModel(): " + Except.Message );
      return false;
      }
    }



  }
}
