// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
// using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;



namespace ClimateModel
{
  class ThreeDSphere
  {
  private MainForm MForm;
  private VertexRow[] VertexRows;
  private int VertexRowsLast = 0;
  private int LastVertexIndex = 0;
  private double Radius = 1;
  private double TranslateX = 1;
  private double TranslateY = 1;
  private double TranslateZ = 1;
  private double LongitudeHours = 0; // Time change.
  private double DistanceScale = 0.03;


  public struct LatLongPosition
    {
    public int Index;
    public double Latitude;
    public double Longitude;
    public double X;
    public double Y;
    public double Z;
    public double TextureX;
    public double TextureY;
    }



  public struct VertexRow
    {
    public LatLongPosition[] Row;
    public int RowLast;
    }


/*
  public struct TriangleIndexes
    {
    public int Vertex1;
    public int Vertex2;
    public int Vertex3;
    }
*/


  private ThreeDSphere()
    {
    }


  internal ThreeDSphere( MainForm UseForm,
                   double UseDistanceScale )
    {
    MForm = UseForm;
    DistanceScale = UseDistanceScale;
    }




/*
  internal void PushTriangle( TriangleIndex Tri )
    {
    try
    {
    TriArray[TriArrayLast] = Tri;
    TriArrayLast++;

    if( TriArrayLast >= TriArray.Length )
      Array.Resize( ref TriArray, TriArray.Length + (4 * 1024) );

    }
    catch( Exception Except )
      {
      MessageBox.Show( "Exception in TriangleStack.PushTriangle(). " + Except.Message, MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      }
    }
*/



  internal void SetLatLonPositionXYZ( ref LatLongPosition Result )
    {
    double LatRadians = NumbersEC.DegreesToRadians( Result.Latitude );
    double LonRadians = NumbersEC.DegreesToRadians( Result.Longitude );

    // Higher hours make the sun go west.
    // The Sun goes down in the west.
    LonRadians += NumbersEC.DegreesToRadians( LongitudeHours * (360.0d / 24.0d) );

    Result.X = Radius * (Math.Cos( LatRadians ) * Math.Cos( LonRadians ));
    Result.Y = Radius * (Math.Cos( LatRadians ) * Math.Sin( LonRadians ));
    Result.Z = Radius * (Math.Sin( LatRadians ));
    Result.X += TranslateX * DistanceScale;
    Result.Y += TranslateY * DistanceScale;
    Result.Z += TranslateZ * DistanceScale;

    Result.TextureX = Result.Longitude + 180.0;
    Result.TextureX = Result.TextureX / 360.0d;

    Result.TextureY = Result.Latitude + 90.0;
    Result.TextureY = Result.TextureY / 180.0d;
    Result.TextureY = 1 - Result.TextureY;
    }



  private void AddSurfaceVertex( ref MeshGeometry3D Surface, LatLongPosition Pos )
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



  private void AddSurfaceTriangleIndex( ref MeshGeometry3D Surface,
                    int Index1,
                    int Index2,
                    int Index3 )
    {
    Surface.TriangleIndices.Add( Index1 );
    Surface.TriangleIndices.Add( Index2 );
    Surface.TriangleIndices.Add( Index3 );
    }



  internal MeshGeometry3D MakeSphericalModel( double UseRadius,
                         double X,
                         double Y,
                         double Z,
                         double LonShift )
    {
    try
    {
    // MForm.ShowStatus( "MakeSphericalModel() was called." );

    Radius = UseRadius;
    TranslateX = X;
    TranslateY = Y;
    TranslateZ = Z;
    LongitudeHours = LonShift;

    MeshGeometry3D Surface = new MeshGeometry3D();

    LastVertexIndex = 0;
    VertexRowsLast = 19;
    VertexRows = new VertexRow[VertexRowsLast];

    LatLongPosition PosNorthPole = new LatLongPosition();
    PosNorthPole.Latitude = 90.0;
    PosNorthPole.Longitude = 0;
    PosNorthPole.Index = LastVertexIndex;
    LastVertexIndex++;
    SetLatLonPositionXYZ( ref PosNorthPole );

    LatLongPosition PosSouthPole = new LatLongPosition();
    PosSouthPole.Latitude = -90.0;
    PosSouthPole.Longitude = 0;
    PosSouthPole.Index = LastVertexIndex;
    LastVertexIndex++;
    SetLatLonPositionXYZ( ref PosSouthPole );

    VertexRows[0] = new VertexRow();
    VertexRows[0].Row = new LatLongPosition[1];
    VertexRows[0].RowLast = 1;
    VertexRows[0].Row[0] = PosNorthPole;
    AddSurfaceVertex( ref Surface, PosNorthPole );

    VertexRows[VertexRowsLast - 1] = new VertexRow();
    VertexRows[VertexRowsLast - 1].Row = new LatLongPosition[1];
    VertexRows[VertexRowsLast - 1].RowLast = 1;
    VertexRows[VertexRowsLast - 1].Row[0] = PosSouthPole;
    AddSurfaceVertex( ref Surface, PosSouthPole );

    // This is for the MakePoleVertexes for the
    // north pole:
    MakeOneVertexRow( ref Surface, 1, 4, 80 );

    MakeOneVertexRow( ref Surface, 2, 8, 70 );
    MakeOneVertexRow( ref Surface, 3, 16, 60 );
    MakeOneVertexRow( ref Surface, 4, 32, 50 );
    MakeOneVertexRow( ref Surface, 5, 64, 40 );
    MakeOneVertexRow( ref Surface, 6, 128, 30 );
    MakeOneVertexRow( ref Surface, 7, 256, 20 );
    MakeOneVertexRow( ref Surface, 8, 256, 10 );
    MakeOneVertexRow( ref Surface, 9, 256, 0 );
    MakeOneVertexRow( ref Surface, 10, 256, -10 );
    MakeOneVertexRow( ref Surface, 11, 256, -20 );
    MakeOneVertexRow( ref Surface, 12, 128, -30 );
    MakeOneVertexRow( ref Surface, 13, 64, -40 );
    MakeOneVertexRow( ref Surface, 14, 32, -50 );
    MakeOneVertexRow( ref Surface, 15, 16, -60 );
    MakeOneVertexRow( ref Surface, 16, 8, -70 );

    // South pole four vertexes:
    MakeOneVertexRow( ref Surface, 17, 4, -80 );

    MakePoleTriangles( ref Surface );

    MakeDoubleRowTriangles( ref Surface, 1, 2 );
    MakeDoubleRowTriangles( ref Surface, 2, 3 );
    MakeDoubleRowTriangles( ref Surface, 3, 4 );
    MakeDoubleRowTriangles( ref Surface, 4, 5 );
    MakeDoubleRowTriangles( ref Surface, 5, 6 );
    MakeDoubleRowTriangles( ref Surface, 6, 7 );

    MakeRowTriangles( ref Surface, 7, 8 );
    MakeRowTriangles( ref Surface, 8, 9 );
    MakeRowTriangles( ref Surface, 9, 10 );
    MakeRowTriangles( ref Surface, 10, 11 );

    MakeDoubleReverseRowTriangles( ref Surface, 12, 11 );
    MakeDoubleReverseRowTriangles( ref Surface, 13, 12 );
    MakeDoubleReverseRowTriangles( ref Surface, 14, 13 );
    MakeDoubleReverseRowTriangles( ref Surface, 15, 14 );
    MakeDoubleReverseRowTriangles( ref Surface, 16, 15 );
    MakeDoubleReverseRowTriangles( ref Surface, 17, 16 );

    return Surface;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDSphere.MakeSphericalModel(): " + Except.Message );
      return null;
      }
    }



  private void MakePoleTriangles( ref MeshGeometry3D Surface )
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

    AddSurfaceTriangleIndex( ref Surface,
                    PosNorthPole.Index,
                    Pos1.Index,
                    Pos2.Index );

    AddSurfaceTriangleIndex( ref Surface,
                    PosNorthPole.Index,
                    Pos2.Index,
                    Pos3.Index );

    AddSurfaceTriangleIndex( ref Surface,
                    PosNorthPole.Index,
                    Pos3.Index,
                    Pos4.Index );


    // South pole:
    Pos1 = VertexRows[VertexRowsLast - 2].Row[0];
    Pos2 = VertexRows[VertexRowsLast - 2].Row[1];
    Pos3 = VertexRows[VertexRowsLast - 2].Row[2];
    Pos4 = VertexRows[VertexRowsLast - 2].Row[3];

    // Counterclockwise winding as seen from south
    // of the south pole:
    AddSurfaceTriangleIndex( ref Surface,
                    PosSouthPole.Index,
                    Pos4.Index,
                    Pos3.Index );

    AddSurfaceTriangleIndex( ref Surface,
                    PosSouthPole.Index,
                    Pos3.Index,
                    Pos2.Index );

    AddSurfaceTriangleIndex( ref Surface,
                    PosSouthPole.Index,
                    Pos2.Index,
                    Pos1.Index );

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDSphere.MakePoleTriangles(): " + Except.Message );
      }
    }



  private bool MakeRowTriangles( ref MeshGeometry3D Surface, int FirstRow, int SecondRow )
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

      AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

      Pos1 = VertexRows[SecondRow].Row[RowIndex + 1];
      Pos2 = VertexRows[FirstRow].Row[RowIndex + 1];
      Pos3 = VertexRows[FirstRow].Row[RowIndex];

      AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

      }

    return true;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDSphere.MakeRowTriangles(): " + Except.Message );
      return false;
      }
    }


  // What is the area of these triangles?
  // Each of the three types?
  // The area would be bigger with the bigger
  // circumferance of the lower latitudes.
  // I mean on that lower-latitude edge of the
  // triangles.

  private bool MakeDoubleRowTriangles( ref MeshGeometry3D Surface, int FirstRow, int DoubleRow )
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

    AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

    for( int RowIndex = 1; RowIndex < RowLength; RowIndex++ )
      {
      int DoubleRowIndex = RowIndex * 2;
      Pos1 = VertexRows[FirstRow].Row[RowIndex + 0];
      Pos2 = VertexRows[DoubleRow].Row[DoubleRowIndex + 0];
      Pos3 = VertexRows[DoubleRow].Row[DoubleRowIndex + 1];

      AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45

      Pos1 = VertexRows[FirstRow].Row[RowIndex + 0];
      Pos2 = VertexRows[DoubleRow].Row[DoubleRowIndex - 1];
      Pos3 = VertexRows[DoubleRow].Row[DoubleRowIndex];
      AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45
      Pos1 = VertexRows[DoubleRow].Row[DoubleRowIndex - 1];
      Pos2 = VertexRows[FirstRow].Row[RowIndex];
      Pos3 = VertexRows[FirstRow].Row[RowIndex - 1];
      AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

      }

    return true;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDSphere.MakeDoubleRowTriangles(): " + Except.Message );
      return false;
      }
    }



  private bool MakeDoubleReverseRowTriangles( ref MeshGeometry3D Surface, int BottomRow, int DoubleRow )
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

    AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

    for( int RowIndex = 1; RowIndex < RowLength; RowIndex++ )
      {
      int DoubleRowIndex = RowIndex * 2;
      Pos1 = VertexRows[BottomRow].Row[RowIndex];
      Pos2 = VertexRows[DoubleRow].Row[DoubleRowIndex + 1];
      Pos3 = VertexRows[DoubleRow].Row[DoubleRowIndex];

      AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );


      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45

      Pos1 = VertexRows[BottomRow].Row[RowIndex + 0];
      Pos2 = VertexRows[DoubleRow].Row[DoubleRowIndex];
      Pos3 = VertexRows[DoubleRow].Row[DoubleRowIndex - 1];
      AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45
      Pos1 = VertexRows[DoubleRow].Row[DoubleRowIndex - 1];
      Pos2 = VertexRows[BottomRow].Row[RowIndex - 1];
      Pos3 = VertexRows[BottomRow].Row[RowIndex];
      AddSurfaceTriangleIndex( ref Surface,
                      Pos1.Index,
                      Pos2.Index,
                      Pos3.Index );

      }

    return true;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDSphere.MakeDoubleRowTriangles(): " + Except.Message );
      return false;
      }
    }



  private bool MakeOneVertexRow( ref MeshGeometry3D Surface,
                  int RowIndex,
                  int HowMany,
                  double Latitude )
    {
    try
    {
    VertexRows[RowIndex] = new VertexRow();
    VertexRows[RowIndex].Row = new LatLongPosition[HowMany];
    VertexRows[RowIndex].RowLast = HowMany;

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
      Pos.Longitude = LonStart + (LonDelta * Count);
      Pos.Index = LastVertexIndex;
      LastVertexIndex++;
      SetLatLonPositionXYZ( ref Pos );
      VertexRows[RowIndex].Row[Count] = Pos;
      AddSurfaceVertex( ref Surface, Pos );
      }

    return true;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDSphere.MakeSphericalModel(): " + Except.Message );
      return false;
      }
    }



  }
}
