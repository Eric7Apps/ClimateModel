// Copyright Eric Chauvin 2018.
// My blog is at:
// https://scientificmodels.blogspot.com/


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
  internal string TextureFileName = "";
  private MeshGeometry3D Surface;
  private GeometryModel3D GeometryMod;
  private EarthSlice[] EarthSliceArray;
  private int LastGraphicsIndex = 0;
  internal double LongitudeHoursRadians = 0; // Time change.




  internal EarthGeoid( MainForm UseForm,
                       string UseName,
                       string JPLFileName
                       ): base( UseForm,
                                UseName,
                                JPLFileName )
    {
    AllocateEarthSliceArrays();

    GeometryMod = new GeometryModel3D();
    }



  internal override GeometryModel3D GetGeometryModel()
    {
    return GeometryMod;
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

    GeometryMod.Geometry = Surface;
    GeometryMod.Material = SolidMat;
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

    BitmapImage BMapImage = new BitmapImage();

    // Things have to be in this Begin-end block.
    BMapImage.BeginInit();

    // StreamSource:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.bitmapimage.streamsource?view=netframework-4.7.2#System_Windows_Media_Imaging_BitmapImage_StreamSource

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
                      Vector3.Vector RefPosition,
                      Vector3.Vector RefNormal,
                      EarthSlice.LatLongPosition Pos )
    {
    // Surface.Positions.Count
    // Surface.Positions.Items[Index];
    // Surface.Positions.Add() adds it to the end.
    // Surface.Positions.Clear(); Removes all values.

    // Use a scale for drawing.
    double ScaledX = (Position.X + RefPosition.X) * ModelConstants.ThreeDSizeScale;
    double ScaledY = (Position.Y + RefPosition.Y) * ModelConstants.ThreeDSizeScale;
    double ScaledZ = (Position.Z + RefPosition.Z) * ModelConstants.ThreeDSizeScale;
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

    Vector3D SurfaceNormal = new Vector3D( RefNormal.X, RefNormal.Y, RefNormal.Z );
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
      EarthSliceArray[Count] = new
                  EarthSlice( MForm );
      }

    // Allocate the two poles:
    EarthSliceArray[0].
               AllocateArrays( 1, 1 );

    EarthSliceArray[EarthSlice.VertexRowsLast - 1].
               AllocateArrays( 1, 1 );

    // Start with the 4 vertexes right next to the
    // north pole.
    int HowMany = 4;
    for( int Index = 1; Index <= EarthSlice.VertexRowsMiddle; Index++ )
      {
      EarthSliceArray[Index].
               AllocateArrays( HowMany, HowMany );

      if( HowMany < EarthSlice.MaximumVertexesPerRow )
        HowMany = HowMany * 2;

      }

    // From the south pole up to the equator.
    HowMany = 4;
    for( int Index = EarthSlice.VertexRowsLast - 2; Index > EarthSlice.VertexRowsMiddle; Index-- )
      {
      EarthSliceArray[Index].
               AllocateArrays( HowMany, HowMany );

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
    LastGraphicsIndex = 0;

    Surface = new MeshGeometry3D();

    double ApproxLatitude = 90.0;

    LastGraphicsIndex =
    EarthSliceArray[0].MakeSurfaceVertexRow(
                           ApproxLatitude,
                           LongitudeHoursRadians,
                           LastGraphicsIndex );

    EarthSlice.LatLongPosition PosNorthPole =
                           EarthSliceArray[0].
                           GetLatLongPosition( 0 );

    Vector3.Vector PositionNorthPole =
              EarthSliceArray[0].GetPosition( 0, 0 );

    Vector3.Vector NormalNorthPole =
              EarthSliceArray[0].GetSurfaceNormal( 0, 0 );


    ApproxLatitude = -90.0;
    LastGraphicsIndex =
    EarthSliceArray[EarthSlice.VertexRowsLast - 1].
                             MakeSurfaceVertexRow(
                             ApproxLatitude,
                             LongitudeHoursRadians,
                             LastGraphicsIndex );

    EarthSlice.LatLongPosition PosSouthPole =
               EarthSliceArray[EarthSlice.VertexRowsLast - 1].
               GetLatLongPosition( 0 );

    Vector3.Vector PositionSouthPole =
              EarthSliceArray[EarthSlice.VertexRowsLast - 1].GetPosition( 0, 0 );

    Vector3.Vector NormalSouthPole =
              EarthSliceArray[EarthSlice.VertexRowsLast - 1].GetSurfaceNormal( 0, 0 );

    AddSurfaceVertex( PositionNorthPole,
                      NormalNorthPole,
                      PosNorthPole );

    AddSurfaceVertex( PositionSouthPole,
                      NormalSouthPole,
                      PosSouthPole );

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
               GetRefVertexArraySize() ==
             EarthSliceArray[Index + 1].
               GetRefVertexArraySize())
        {
        MakeRowTriangles( Index, Index + 1 );
        }
      else
        {
        if( EarthSliceArray[Index].
              GetRefVertexArraySize() <
             EarthSliceArray[Index + 1].
              GetRefVertexArraySize())
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
      ShowStatus( "Exception in EarthGeoid.MakeGeoidModel(): " + Except.Message );
      }
    }




  private void MakePoleTriangles()
    {
    try
    {
    EarthSlice.LatLongPosition NorthPole =
         EarthSliceArray[0].GetLatLongPosition( 0 );

    EarthSlice.LatLongPosition SouthPole =
      EarthSliceArray[EarthSlice.VertexRowsLast - 1].
        GetLatLongPosition( 0 );


    EarthSlice.LatLongPosition Pos1 =
               EarthSliceArray[1].
               GetLatLongPosition( 0 );

    EarthSlice.LatLongPosition Pos2 =
               EarthSliceArray[1].
               GetLatLongPosition( 1 );

    EarthSlice.LatLongPosition Pos3 =
               EarthSliceArray[1].
               GetLatLongPosition( 2 );

    EarthSlice.LatLongPosition Pos4 =
               EarthSliceArray[1].
               GetLatLongPosition( 3 );

    // Counterclockwise winding goes toward the
    // viewer.

    AddSurfaceTriangleIndex( NorthPole.GraphicsIndex,
                                  Pos1.GraphicsIndex,
                                  Pos2.GraphicsIndex );

    AddSurfaceTriangleIndex( NorthPole.GraphicsIndex,
                                  Pos2.GraphicsIndex,
                                  Pos3.GraphicsIndex );

    AddSurfaceTriangleIndex( NorthPole.GraphicsIndex,
                                  Pos3.GraphicsIndex,
                                  Pos4.GraphicsIndex );


    // South pole:
    Pos1 = EarthSliceArray[EarthSlice.VertexRowsLast - 2].
               GetLatLongPosition( 0 );

    Pos2 = EarthSliceArray[EarthSlice.VertexRowsLast - 2].
               GetLatLongPosition( 1 );

    Pos3 = EarthSliceArray[EarthSlice.VertexRowsLast - 2].
               GetLatLongPosition( 2 );

    Pos4 = EarthSliceArray[EarthSlice.VertexRowsLast - 2].
               GetLatLongPosition( 3 );


    // Counterclockwise winding as seen from south
    // of the south pole:
    AddSurfaceTriangleIndex( SouthPole.GraphicsIndex,
                                  Pos4.GraphicsIndex,
                                  Pos3.GraphicsIndex );

    AddSurfaceTriangleIndex( SouthPole.GraphicsIndex,
                                  Pos3.GraphicsIndex,
                                  Pos2.GraphicsIndex );

    AddSurfaceTriangleIndex( SouthPole.GraphicsIndex,
                                  Pos2.GraphicsIndex,
                                  Pos1.GraphicsIndex );

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
                        GetRefVertexArraySize();

    int SecondRowLength = EarthSliceArray[SecondRow].
                        GetRefVertexArraySize();

    if( RowLength != SecondRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "RowLength != SecondRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }

    for( int RowIndex = 0; (RowIndex + 1) < RowLength; RowIndex++ )
      {
      EarthSlice.LatLongPosition Pos1 =
           EarthSliceArray[FirstRow].
               GetLatLongPosition( RowIndex );

      EarthSlice.LatLongPosition Pos2 =
           EarthSliceArray[SecondRow].
               GetLatLongPosition( RowIndex );

      EarthSlice.LatLongPosition Pos3 =
           EarthSliceArray[SecondRow].
               GetLatLongPosition( RowIndex + 1 );

      AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                               Pos2.GraphicsIndex,
                               Pos3.GraphicsIndex );

      Pos1 = EarthSliceArray[SecondRow].
               GetLatLongPosition( RowIndex + 1 );

      Pos2 = EarthSliceArray[FirstRow].
               GetLatLongPosition( RowIndex + 1 );

      Pos3 = EarthSliceArray[FirstRow].
               GetLatLongPosition( RowIndex );

      AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                               Pos2.GraphicsIndex,
                               Pos3.GraphicsIndex );

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
                        GetRefVertexArraySize();

    int DoubleRowLength = EarthSliceArray[DoubleRow].
                        GetRefVertexArraySize();


    if( (RowLength * 2) > DoubleRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "(RowLength * 2) > DoubleRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }

    EarthSlice.LatLongPosition Pos1 =
          EarthSliceArray[FirstRow].
               GetLatLongPosition( 0 );

    EarthSlice.LatLongPosition Pos2 =
          EarthSliceArray[DoubleRow].
               GetLatLongPosition( 0 );

    EarthSlice.LatLongPosition Pos3 =
          EarthSliceArray[DoubleRow].
               GetLatLongPosition( 1 );

    AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                             Pos2.GraphicsIndex,
                             Pos3.GraphicsIndex );

    for( int RowIndex = 1; RowIndex < RowLength; RowIndex++ )
      {
      int DoubleRowIndex = RowIndex * 2;

      Pos1 = EarthSliceArray[FirstRow].
               GetLatLongPosition( RowIndex + 0 );

      Pos2 = EarthSliceArray[DoubleRow].
              GetLatLongPosition( DoubleRowIndex + 0 );

      Pos3 = EarthSliceArray[DoubleRow].
              GetLatLongPosition( DoubleRowIndex + 1 );

      AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                               Pos2.GraphicsIndex,
                               Pos3.GraphicsIndex );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45

      Pos1 = EarthSliceArray[FirstRow].
               GetLatLongPosition( RowIndex + 0 );

      Pos2 = EarthSliceArray[DoubleRow].
              GetLatLongPosition( DoubleRowIndex - 1 );

      Pos3 = EarthSliceArray[DoubleRow].
              GetLatLongPosition( DoubleRowIndex );

      AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                               Pos2.GraphicsIndex,
                               Pos3.GraphicsIndex );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45
      Pos1 = EarthSliceArray[DoubleRow].
             GetLatLongPosition( DoubleRowIndex - 1 );

      Pos2 = EarthSliceArray[FirstRow].
             GetLatLongPosition( RowIndex );

      Pos3 = EarthSliceArray[FirstRow].
             GetLatLongPosition( RowIndex - 1 );

      AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                               Pos2.GraphicsIndex,
                               Pos3.GraphicsIndex );

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
                        GetRefVertexArraySize();

    int DoubleRowLength = EarthSliceArray[DoubleRow].
                        GetRefVertexArraySize();

    if( (RowLength * 2) > DoubleRowLength )
      {
      System.Windows.Forms.MessageBox.Show( "DoubleReverse: (RowLength * 2) > DoubleRowLength.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return false;
      }

    EarthSlice.LatLongPosition Pos1 =
               EarthSliceArray[BottomRow].
               GetLatLongPosition( 0 );

    EarthSlice.LatLongPosition Pos2 =
               EarthSliceArray[DoubleRow].
               GetLatLongPosition( 1 );

    EarthSlice.LatLongPosition Pos3 =
               EarthSliceArray[DoubleRow].
               GetLatLongPosition( 0 );

    AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                             Pos2.GraphicsIndex,
                             Pos3.GraphicsIndex );

    for( int RowIndex = 1; RowIndex < RowLength; RowIndex++ )
      {
      int DoubleRowIndex = RowIndex * 2;

      Pos1 = EarthSliceArray[BottomRow].
             GetLatLongPosition( RowIndex );

      Pos2 = EarthSliceArray[DoubleRow].
             GetLatLongPosition( DoubleRowIndex + 1 );

      Pos3 = EarthSliceArray[DoubleRow].
             GetLatLongPosition( DoubleRowIndex );

      AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                               Pos2.GraphicsIndex,
                               Pos3.GraphicsIndex );


      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45

      Pos1 = EarthSliceArray[BottomRow].
             GetLatLongPosition( RowIndex + 0 );

      Pos2 = EarthSliceArray[DoubleRow].
             GetLatLongPosition( DoubleRowIndex );

      Pos3 = EarthSliceArray[DoubleRow].
             GetLatLongPosition( DoubleRowIndex - 1 );

      AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                               Pos2.GraphicsIndex,
                               Pos3.GraphicsIndex );

      // 0  1  2  3  4  5  6  7
      // 01 23 45 67 89 01 23 45
      Pos1 = EarthSliceArray[DoubleRow].
             GetLatLongPosition( DoubleRowIndex - 1 );

      Pos2 = EarthSliceArray[BottomRow].
             GetLatLongPosition( RowIndex - 1 );

      Pos3 = EarthSliceArray[BottomRow].
             GetLatLongPosition( RowIndex );

      AddSurfaceTriangleIndex( Pos1.GraphicsIndex,
                               Pos2.GraphicsIndex,
                               Pos3.GraphicsIndex );

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
    LastGraphicsIndex = EarthSliceArray[RowIndex].
                  MakeSurfaceVertexRow(
                              ApproxLatitude,
                              LongitudeHoursRadians,
                              LastGraphicsIndex );

    for( int Count = 0; Count < HowMany; Count++ )
      {
      EarthSlice.LatLongPosition Pos =
                           EarthSliceArray[RowIndex].
                           GetLatLongPosition( Count );

      Vector3.Vector RefPosition =
              EarthSliceArray[RowIndex].GetPosition( Count, 0 );

      Vector3.Vector RefNormal =
              EarthSliceArray[RowIndex].GetSurfaceNormal( Count, 0 );

      AddSurfaceVertex( RefPosition,
                        RefNormal,
                        Pos );

      }

    return true;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthGeoid.MakeOneVertexRow(): " + Except.Message );
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
