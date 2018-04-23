// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// Shakespeare: "There is a tide in the affairs of
// men, Which, taken at the flood, leads on to
// fortune; Omitted, all the voyage of their life
// Is bound in shallows and in miseries. On such a
// full sea are we now afloat. And we must take the
// current when it serves. Or lose our ventures."



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
  private MeshGeometry3D Surface;
  private GeometryModel3D GeoMod;
  private EarthSlice[] EarthSliceArray;
  private int LastVertexIndex = 0;
  internal double LongitudeHoursRadians = 0; // Time change.



  private EarthGeoid()
    {
    }



  internal EarthGeoid( MainForm UseForm )
    {
    MForm = UseForm;

    AllocateEarthSliceArrays();

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



/*
  private void SetPlanetGravityAcceleration(
                  ref ReferenceFrame RefFrame )
    {

    RefFrame.SetPlanetGravityAcceleration(
                  ref Vector3.Vector Position )
                  ref Vector3.Vector Acceleration )


    }
*/




  private void AddSurfaceVertex(
                      EarthSlice.ReferenceVertex RefVertex,
                      EarthSlice.LatLongPosition Pos )
    {
    // Surface.Positions.Count
    // Surface.Positions.Items[Index];
    // Surface.Positions.Add() adds it to the end.
    // Surface.Positions.Clear(); Removes all values.

    // Use a scale for drawing.
    double ScaledX = (Position.X + RefVertex.Position.X) * ModelConstants.ThreeDSizeScale;
    double ScaledY = (Position.Y + RefVertex.Position.Y) * ModelConstants.ThreeDSizeScale;
    double ScaledZ = (Position.Z + RefVertex.Position.Z) * ModelConstants.ThreeDSizeScale;
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



  private void AllocateEarthSliceArrays()
    {
    try
    {
    EarthSliceArray = new EarthSlice[EarthSlice.VertexRowsLast];
    for( int Count = 0; Count < EarthSlice.VertexRowsLast; Count++ )
      {
      EarthSliceArray[Count] = new EarthSlice( MForm );
      }

    // Allocate the two poles:
    EarthSliceArray[0].
               AllocateVertexArrays( 1, 1 );

    EarthSliceArray[EarthSlice.VertexRowsLast - 1].
               AllocateVertexArrays( 1, 1 );

    // Start with the 4 vertexes right next to the
    // north pole.
    int HowMany = 4;
    for( int Index = 1; Index <= EarthSlice.VertexRowsMiddle; Index++ )
      {
      EarthSliceArray[Index].
               AllocateVertexArrays( HowMany, HowMany );

      if( HowMany < EarthSlice.MaximumVertexesPerRow )
        HowMany = HowMany * 2;

      }

    // From the south pole up to the equator.
    HowMany = 4;
    for( int Index = EarthSlice.VertexRowsLast - 2; Index > EarthSlice.VertexRowsMiddle; Index-- )
      {
      EarthSliceArray[Index].
               AllocateVertexArrays( HowMany, HowMany );

      if( HowMany < EarthSlice.MaximumVertexesPerRow )
        HowMany = HowMany * 2;

      }
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.AllocateEarthSliceArrays(): " + Except.Message );
      }
    }




  private void MakeGeoidModel()
    {
    try
    {
    // MaxZDiff = 0;
    LastVertexIndex = 0;

    Surface = new MeshGeometry3D();

    double ApproxLatitude = 90.0;

    LastVertexIndex = EarthSliceArray[0].
                           MakeSurfaceVertexRow(
                           ApproxLatitude,
                           LongitudeHoursRadians,
                           LastVertexIndex );

    EarthSlice.LatLongPosition PosNorthPole =
                           EarthSliceArray[0].
                           GetLatLongPosition( 0 );

    EarthSlice.ReferenceVertex RefVertexNorthPole =
       EarthSliceArray[0].GetRefVertex( 0, 0 );


    ApproxLatitude = -90.0;
    LastVertexIndex = EarthSliceArray[EarthSlice.VertexRowsLast - 1].
                             MakeSurfaceVertexRow(
                             ApproxLatitude,
                             LongitudeHoursRadians,
                             LastVertexIndex );

    EarthSlice.LatLongPosition PosSouthPole =
               EarthSliceArray[EarthSlice.VertexRowsLast - 1].
               GetLatLongPosition( 0 );

    EarthSlice.ReferenceVertex RefVertexSouthPole =
               EarthSliceArray[EarthSlice.VertexRowsLast - 1].
               GetRefVertex( 0, 0 );

    AddSurfaceVertex( RefVertexNorthPole, PosNorthPole );
    AddSurfaceVertex( RefVertexSouthPole, PosSouthPole );

    double RowLatitude = 90;
    int HowMany = 4;
    for( int Index = 1; Index <= EarthSlice.VertexRowsMiddle; Index++ )
      {
      RowLatitude -= EarthSlice.RowLatDelta;
      MakeOneVertexRow( Index, HowMany, RowLatitude );
      if( HowMany < EarthSlice.MaximumVertexesPerRow )
        HowMany = HowMany * 2;

      }


    RowLatitude = -90;
    HowMany = 4;
    for( int Index = EarthSlice.VertexRowsLast - 2; Index > EarthSlice.VertexRowsMiddle; Index-- )
      {
      RowLatitude += EarthSlice.RowLatDelta;
      MakeOneVertexRow( Index, HowMany, RowLatitude );
      if( HowMany < EarthSlice.MaximumVertexesPerRow )
        HowMany = HowMany * 2;

      }

    MakePoleTriangles();

    for( int Index = 0; Index < EarthSlice.VertexRowsLast - 2; Index++ )
      {
      if( EarthSliceArray[Index].
             GetRefVertexArrayLast() ==
             EarthSliceArray[Index + 1].
             GetRefVertexArrayLast())
        {
        MakeRowTriangles( Index, Index + 1 );
        }
      else
        {
        if( EarthSliceArray[Index].
             GetRefVertexArrayLast() <
             EarthSliceArray[Index + 1].
             GetRefVertexArrayLast())
          {
          MakeDoubleRowTriangles( Index, Index + 1 );
          }
        else
          {
          MakeDoubleReverseRowTriangles( Index + 1, Index );
          }
        }
      }
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
    EarthSlice.ReferenceVertex NorthPole =
         EarthSliceArray[0].GetRefVertex( 0, 0 );

    EarthSlice.ReferenceVertex SouthPole =
               EarthSliceArray[EarthSlice.VertexRowsLast - 1].
               GetRefVertex( 0, 0 );


    EarthSlice.ReferenceVertex Pos1 =
               EarthSliceArray[1].
               GetRefVertex( 0, 0 );

    EarthSlice.ReferenceVertex Pos2 =
               EarthSliceArray[1].
               GetRefVertex( 1, 0 );

    EarthSlice.ReferenceVertex Pos3 =
               EarthSliceArray[1].
               GetRefVertex( 2, 0 );

    EarthSlice.ReferenceVertex Pos4 =
               EarthSliceArray[1].
               GetRefVertex( 3, 0 );

    // Counterclockwise winding goes toward the
    // viewer.

    AddSurfaceTriangleIndex( NorthPole.Index,
                                  Pos1.Index,
                                  Pos2.Index );

    AddSurfaceTriangleIndex( NorthPole.Index,
                                  Pos2.Index,
                                  Pos3.Index );

    AddSurfaceTriangleIndex( NorthPole.Index,
                                  Pos3.Index,
                                  Pos4.Index );


    // South pole:
    Pos1 = EarthSliceArray[EarthSlice.VertexRowsLast - 2].
               GetRefVertex( 0, 0 );

    Pos2 = EarthSliceArray[EarthSlice.VertexRowsLast - 2].
               GetRefVertex( 1, 0 );

    Pos3 = EarthSliceArray[EarthSlice.VertexRowsLast - 2].
               GetRefVertex( 2, 0 );

    Pos4 = EarthSliceArray[EarthSlice.VertexRowsLast - 2].
               GetRefVertex( 3, 0 );


    // Counterclockwise winding as seen from south
    // of the south pole:
    AddSurfaceTriangleIndex( SouthPole.Index,
                                  Pos4.Index,
                                  Pos3.Index );

    AddSurfaceTriangleIndex( SouthPole.Index,
                                  Pos3.Index,
                                  Pos2.Index );

    AddSurfaceTriangleIndex( SouthPole.Index,
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
    int RowLength = EarthSliceArray[FirstRow].
                        GetRefVertexArrayLast();

    int SecondRowLength = EarthSliceArray[SecondRow].
                        GetRefVertexArrayLast();

    if( RowLength != SecondRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "RowLength != SecondRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }

    for( int RowIndex = 0; (RowIndex + 1) < RowLength; RowIndex++ )
      {
      EarthSlice.ReferenceVertex Pos1 =
           EarthSliceArray[FirstRow].
               GetRefVertex( RowIndex, 0 );

      EarthSlice.ReferenceVertex Pos2 =
           EarthSliceArray[SecondRow].
               GetRefVertex( RowIndex, 0 );

      EarthSlice.ReferenceVertex Pos3 =
           EarthSliceArray[SecondRow].
               GetRefVertex( RowIndex + 1, 0 );

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      Pos1 = EarthSliceArray[SecondRow].
               GetRefVertex( RowIndex + 1, 0 );

      Pos2 = EarthSliceArray[FirstRow].
               GetRefVertex( RowIndex + 1, 0 );

      Pos3 = EarthSliceArray[FirstRow].
               GetRefVertex( RowIndex, 0 );

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
    int RowLength = EarthSliceArray[FirstRow].
                        GetRefVertexArrayLast();

    int DoubleRowLength = EarthSliceArray[DoubleRow].
                        GetRefVertexArrayLast();


    if( (RowLength * 2) > DoubleRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "(RowLength * 2) > DoubleRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }

    EarthSlice.ReferenceVertex Pos1 =
          EarthSliceArray[FirstRow].
               GetRefVertex( 0, 0 );

    EarthSlice.ReferenceVertex Pos2 =
          EarthSliceArray[DoubleRow].
               GetRefVertex( 0, 0 );

    EarthSlice.ReferenceVertex Pos3 =
          EarthSliceArray[DoubleRow].
               GetRefVertex( 1, 0 );

    AddSurfaceTriangleIndex( Pos1.Index,
                             Pos2.Index,
                             Pos3.Index );

    for( int RowIndex = 1; RowIndex < RowLength; RowIndex++ )
      {
      int DoubleRowIndex = RowIndex * 2;

      Pos1 = EarthSliceArray[FirstRow].
               GetRefVertex( RowIndex + 0, 0 );

      Pos2 = EarthSliceArray[DoubleRow].
               GetRefVertex( DoubleRowIndex + 0, 0 );

      Pos3 = EarthSliceArray[DoubleRow].
               GetRefVertex( DoubleRowIndex + 1, 0 );

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45

      Pos1 = EarthSliceArray[FirstRow].
               GetRefVertex( RowIndex + 0, 0 );

      Pos2 = EarthSliceArray[DoubleRow].
               GetRefVertex( DoubleRowIndex - 1, 0 );

      Pos3 = EarthSliceArray[DoubleRow].
               GetRefVertex( DoubleRowIndex, 0 );

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45
      Pos1 = EarthSliceArray[DoubleRow].
               GetRefVertex( DoubleRowIndex - 1, 0 );

      Pos2 = EarthSliceArray[FirstRow].
               GetRefVertex( RowIndex, 0 );

      Pos3 = EarthSliceArray[FirstRow].
               GetRefVertex( RowIndex - 1, 0 );

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
    int RowLength = EarthSliceArray[BottomRow].
                        GetRefVertexArrayLast();

    int DoubleRowLength = EarthSliceArray[DoubleRow].
                        GetRefVertexArrayLast();

    if( (RowLength * 2) > DoubleRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "DoubleReverse: (RowLength * 2) > DoubleRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }

    EarthSlice.ReferenceVertex Pos1 =
               EarthSliceArray[BottomRow].
               GetRefVertex( 0, 0 );

    EarthSlice.ReferenceVertex Pos2 =
               EarthSliceArray[DoubleRow].
               GetRefVertex( 1, 0 );

    EarthSlice.ReferenceVertex Pos3 =
               EarthSliceArray[DoubleRow].
               GetRefVertex( 0, 0 );

    AddSurfaceTriangleIndex( Pos1.Index,
                             Pos2.Index,
                             Pos3.Index );

    for( int RowIndex = 1; RowIndex < RowLength; RowIndex++ )
      {
      int DoubleRowIndex = RowIndex * 2;

      Pos1 = EarthSliceArray[BottomRow].
             GetRefVertex( RowIndex, 0 );

      Pos2 = EarthSliceArray[DoubleRow].
             GetRefVertex( DoubleRowIndex + 1, 0 );

      Pos3 = EarthSliceArray[DoubleRow].
             GetRefVertex( DoubleRowIndex, 0 );

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );


      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45

      Pos1 = EarthSliceArray[BottomRow].
             GetRefVertex( RowIndex + 0, 0 );

      Pos2 = EarthSliceArray[DoubleRow].
             GetRefVertex( DoubleRowIndex, 0 );

      Pos3 = EarthSliceArray[DoubleRow].
             GetRefVertex( DoubleRowIndex - 1, 0 );

      AddSurfaceTriangleIndex( Pos1.Index,
                               Pos2.Index,
                               Pos3.Index );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45
      Pos1 = EarthSliceArray[DoubleRow].
             GetRefVertex( DoubleRowIndex - 1, 0 );

      Pos2 = EarthSliceArray[BottomRow].
             GetRefVertex( RowIndex - 1, 0 );

      Pos3 = EarthSliceArray[BottomRow].
             GetRefVertex( RowIndex, 0 );

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
    LastVertexIndex =
    EarthSliceArray[RowIndex].MakeSurfaceVertexRow(
                              ApproxLatitude,
                              LongitudeHoursRadians,
                              LastVertexIndex );

    for( int Count = 0; Count < HowMany; Count++ )
      {
      EarthSlice.LatLongPosition Pos =
                           EarthSliceArray[RowIndex].
                           GetLatLongPosition( Count );

      EarthSlice.ReferenceVertex RefVertex =
                        EarthSliceArray[RowIndex].
                        GetRefVertex( Count, 0 );

      AddSurfaceVertex( RefVertex, Pos );
      }

    return true;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakeSphericalModel(): " + Except.Message );
      return false;
      }
    }




  internal void AddTimeStepRotateAngle()
    {
    double AngleDelta =
          ModelConstants.EarthRotationAnglePerSecond;

    LongitudeHoursRadians =
             LongitudeHoursRadians +
             (1000 * AngleDelta);

    }



  }
}
