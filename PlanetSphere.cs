// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com



using System;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;



namespace ClimateModel
{
  class PlanetSphere : SpaceObject
  {
  private MainForm MForm;
  internal string TextureFileName = "";
  internal double Radius = 1;
  private MeshGeometry3D Surface;
  private GeometryModel3D GeoMod;
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
    public Vector3.Vector SurfaceNormal;
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



  private PlanetSphere()
    {
    }



  internal PlanetSphere( MainForm UseForm )
    {
    MForm = UseForm;

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

    MakeSphericalModel();

    // if( Surface == null )

    GeoMod.Geometry = Surface;
    GeoMod.Material = SolidMat;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in PlanetSphere.MakeNewGeometryModel(): " + Except.Message );
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
    double LonRadians = NumbersEC.DegreesToRadians( Result.Longitude );

    // Higher hours make the sun go west.
    LonRadians += LongitudeHoursRadians;

    double CosLonRadians = Math.Cos( LonRadians );
    double SinLonRadians = Math.Sin( LonRadians );

    Result.X = Radius * (CosLatRadians * CosLonRadians );
    Result.Y = Radius * (CosLatRadians * SinLonRadians );
    Result.Z = Radius * SinLatRadians;

    Result.SurfaceNormal.X = Result.X;
    Result.SurfaceNormal.Y = Result.Y;
    Result.SurfaceNormal.Z = Result.Z;
    Vector3.Normalize( ref Result.SurfaceNormal, Result.SurfaceNormal );

    Result.X += Position.X;
    Result.Y += Position.Y;
    Result.Z += Position.Z;

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

    // Use a scale for drawing.
    double ScaledX = Pos.X * ModelConstants.ThreeDSizeScale;
    double ScaledY = Pos.Y * ModelConstants.ThreeDSizeScale;
    double ScaledZ = Pos.Z * ModelConstants.ThreeDSizeScale;
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



  private void MakeSphericalModel()
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

    int MaximumVertexes = 64;
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
      MForm.ShowStatus( "Exception in PlanetSphere.MakeSphericalModel(): " + Except.Message );
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
      MForm.ShowStatus( "Exception in PlanetSphere.MakePoleTriangles(): " + Except.Message );
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
      MForm.ShowStatus( "Exception in PlanetSphere.MakeRowTriangles(): " + Except.Message );
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
      MForm.ShowStatus( "Exception in PlanetSphere.MakeDoubleRowTriangles(): " + Except.Message );
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
      MForm.ShowStatus( "Exception in PlanetSphere.MakeDoubleRowTriangles(): " + Except.Message );
      return false;
      }
    }



  private void FreeVertexRows()
    {
    for( int Count = 0; Count < VertexRowsLast; Count++ )
      {
      VertexRows[Count].Row = null;
      }

    VertexRows = null;
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
      MForm.ShowStatus( "Exception in PlanetSphere.MakeSphericalModel(): " + Except.Message );
      return false;
      }
    }



  }
}
