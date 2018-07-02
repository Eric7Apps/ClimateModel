// Copyright Eric Chauvin 2018.
// My blog is at:
// https://scientificmodels.blogspot.com/



using System;
using System.Text;


namespace ClimateModel
{
  class EarthSlice
  {
  private MainForm MForm;
  private double GeodeticLatitude = 0;
  private LatLongPosition[] LatLonRow;
  private int LatLonRowLast = 0;
  private ReferenceVertex[] RefVertexArray;
  private const int VerticalVertexCount = 10;
  private double TextureY;

  internal const int RowLatDelta = 5;
  private const int RowsFromEquatorToPole =
            90 / RowLatDelta;
  internal const int VertexRowsLast =
    (RowsFromEquatorToPole * 2) + 1;

  internal const int VertexRowsMiddle =
     RowsFromEquatorToPole + 1;

  internal const int MaximumVertexesPerRow = 128;



  public struct LatLongPosition
    {
    public int GraphicsIndex;
    public double Longitude;
    public double TextureX;
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



  internal int GetRefVertexArraySize()
    {
    return RefVertexArray[0].GetArraySize();
    }



  private void SetLatLonValues(
                            ref LatLongPosition Pos )
    {
    Pos.TextureX = Pos.Longitude + 180.0;
    Pos.TextureX = Pos.TextureX * ( 1.0d / 360.0d);
    }



  internal void AllocateArrays( int LatLonCount,
                                int RefVertCount )
    {
    RefVertexArray = new ReferenceVertex[VerticalVertexCount];

    for( int VertColumn = 0; VertColumn < VerticalVertexCount; VertColumn++ )
      {
      // Change this for the different heights of
      // the lines.  Going from the inner core to
      // the edge of outer space.
      RefVertexArray[VertColumn] = new
              ReferenceVertex( MForm, RefVertCount );

      }

    LatLonRow = new LatLongPosition[LatLonCount];
    LatLonRowLast = LatLonCount;
    }



  private int MakeSurfacePoleRow(
                             double ApproxLatitude,
                             int GraphicsIndex )
    {
    LatLongPosition Pos = new LatLongPosition();

    Pos.GraphicsIndex = GraphicsIndex;
    GraphicsIndex++;

    Pos.Longitude = 0;

    Pos.TextureX = Pos.Longitude + 180.0;
    Pos.TextureX = Pos.TextureX * ( 1.0d / 360.0d);

    LatLonRow[0] = Pos;
    return GraphicsIndex;
    }





  private void SetupReferenceVertexes(
                       double ApproxLatitude,
                       double LongitudeHoursRadians )
    {
    for( int Count = 0; Count < VerticalVertexCount; Count++ )
      {
      // This only needs to be called the first time.
      // Not every time.
      RefVertexArray[Count].SetupLatitude(
                     ApproxLatitude,
                     // Scale these for each count.
                     ModelConstants.EarthRadiusMinor,
                     ModelConstants.EarthRadiusMajor );

      RefVertexArray[Count].MakeVertexRow(
                           ApproxLatitude,
                           LongitudeHoursRadians );

      RefVertexArray[Count].DoAllEarthTiltRotations();
      }

    GeodeticLatitude = RefVertexArray[0].GetGeodeticLatitude();

    TextureY = GeodeticLatitude + 90.0;
    TextureY = TextureY * ( 1.0d / 180.0d );
    TextureY = 1 - TextureY;
    }



  internal double GetTextureY()
    {
    return TextureY;
    }



  internal int MakeSurfaceVertexRow(
                       double ApproxLatitude,
                       double LongitudeHoursRadians,
                       int GraphicsIndex )
    {
    try
    {
    SetupReferenceVertexes( ApproxLatitude,
                     LongitudeHoursRadians );

    if( LatLonRowLast < 2 )
      {
      return MakeSurfacePoleRow( ApproxLatitude,
                                 GraphicsIndex );
      }

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
      Vector3.Vector Position = RefVertexArray[0].
                      GetPosition( Count );

      LatLongPosition Pos = new LatLongPosition();

      Pos.GraphicsIndex = GraphicsIndex;
      GraphicsIndex++;

      Pos.Longitude = LonStart + (LonDelta * Count);
      SetLatLonValues( ref Pos );
      LatLonRow[Count] = Pos;
      }

    return GraphicsIndex;
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in EarthSlice.MakeVertexRow(): " + Except.Message );
      return -1;
      }
    }




  internal LatLongPosition GetLatLongPosition( int Where )
    {
    if( Where >= LatLonRowLast )
      throw( new Exception( "Where >= LatLonRowLast." ));

    return LatLonRow[Where];
    }




  internal Vector3.Vector GetPosition( int Where, int Vertical )
    {
    // if (Where >= ArraySize )

    return RefVertexArray[Vertical].GetPosition( Where );
    }



  internal Vector3.Vector GetSurfaceNormal( int Where, int Vertical )
    {
    // if (Where >= ArraySize )

    return RefVertexArray[Vertical].GetSurfaceNormal( Where );
    }




  }
}
