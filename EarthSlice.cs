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
  private LatLongPosition[] LatLonRow;
  private int LatLonRowLast = 0;
  private EarthLine[] EarthLineArray;
  private const int VerticalVertexCount = 10;

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
    public double GeodeticLatitude;
    public double Longitude;
    public double TextureX;
    public double TextureY;
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
    return EarthLineArray[0].GetRefVertexArrayLast();
    }



  private void SetLatLonValues(
                      EarthLine.ReferenceVertex RefVertex,
                      ref LatLongPosition Pos,
                      double ApproxLatitude )
    {
    SetGeodeticLatitude( RefVertex,
                         ref Pos,
                         ApproxLatitude );

    Pos.TextureX = Pos.Longitude + 180.0;
    Pos.TextureX = Pos.TextureX * ( 1.0d / 360.0d);

    Pos.TextureY = Pos.GeodeticLatitude + 90.0;
    Pos.TextureY = Pos.TextureY * ( 1.0d / 180.0d );
    Pos.TextureY = 1 - Pos.TextureY;
    }




  private void SetGeodeticLatitude(
                      EarthLine.ReferenceVertex RefVertex,
                      ref LatLongPosition Result,
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
      Result.GeodeticLatitude = ApproxLatitude;
      return;
      }

    // Straight up through the north pole.
    Vector3.Vector StraightUp = new Vector3.Vector();
    StraightUp.X = 0;
    StraightUp.Y = 0;
    StraightUp.Z = 1;

    // The dot product of two normalized vectors.
    double Dot = Vector3.DotProduct( ref StraightUp,
                          ref RefVertex.SurfaceFlatNoRotate );

    if( Dot < 0 )
      throw( new Exception( "Dot < 0." ));

    double StraightUpAngle = Math.Acos( Dot );
    double AngleToEquator =
               (Math.PI / 2.0) - StraightUpAngle;

    double Degrees = NumbersEC.RadiansToDegrees( StraightUpAngle );

    if( ApproxLatitude >= 0 )
      Result.GeodeticLatitude = Degrees;
    else
      Result.GeodeticLatitude = -Degrees;

    }



  internal void AllocateArrays( int LatLonCount,
                                int RefVertCount )
    {
    EarthLineArray = new EarthLine[VerticalVertexCount];

    for( int VertColumn = 0; VertColumn < VerticalVertexCount; VertColumn++ )
      {
      // Change this for the different heights of
      // the lines.  Going from the inner core to
      // the edge of outer space.
      EarthLineArray[VertColumn] = new
              EarthLine( MForm,
                         RefVertCount,
                         ModelConstants.EarthRadiusMinor,
                         ModelConstants.EarthRadiusMajor );

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
    Pos.GeodeticLatitude = ApproxLatitude;

    Pos.TextureX = Pos.Longitude + 180.0;
    Pos.TextureX = Pos.TextureX * ( 1.0d / 360.0d);

    Pos.TextureY = Pos.GeodeticLatitude + 90.0;
    Pos.TextureY = Pos.TextureY * ( 1.0d / 180.0d );
    Pos.TextureY = 1 - Pos.TextureY;

    LatLonRow[0] = Pos;
    return GraphicsIndex;
    }



  internal int MakeSurfaceVertexRow(
                       double ApproxLatitude,
                       double LongitudeHoursRadians,
                       int GraphicsIndex )
    {
    try
    {
    for( int Count = 0; Count < VerticalVertexCount; Count++ )
      {
      EarthLineArray[Count].MakeVertexRow(
                           ApproxLatitude,
                           LongitudeHoursRadians );

      EarthLineArray[Count].DoEarthTiltRotations();
      }

    if( LatLonRowLast < 2 )
      {
      return MakeSurfacePoleRow( ApproxLatitude,
                                     GraphicsIndex );
      }

    double LatRadians = NumbersEC.DegreesToRadians( ApproxLatitude );
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
    double LonDelta = 360.0d / (double)(LatLonRowLast - 1);

    for( int Count = 0; Count < LatLonRowLast; Count++ )
      {
      EarthLine.ReferenceVertex RefVertex =
           EarthLineArray[0].GetRefVertex( Count );

      LatLongPosition Pos = new LatLongPosition();

      Pos.GraphicsIndex = GraphicsIndex;
      GraphicsIndex++;

      Pos.Longitude = LonStart + (LonDelta * Count);

      SetLatLonValues( RefVertex,
                            ref Pos,
                            ApproxLatitude );

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




  internal EarthLine.ReferenceVertex GetRefVertex( int Where, int Vert )
    {
    return EarthLineArray[Vert].GetRefVertex( Where );
    }



  internal LatLongPosition GetLatLongPosition( int Where )
    {
    if( Where >= LatLonRowLast )
      throw( new Exception( "Where >= LatLonRowLast." ));

    return LatLonRow[Where];
    }




  }
}
