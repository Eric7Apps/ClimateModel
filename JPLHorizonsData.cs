// Copyright Eric Chauvin 2018.
// My blog is at:
// https://scientificmodels.blogspot.com/



using System;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
// using System.Windows.Media;
// using System.Windows.Media.Media3D;
// using System.Windows.Media.Imaging;


namespace ClimateModel
{
  class JPLHorizonsData
  {
  private MainForm MForm;


  public struct JPLRec
    {
    // JDTDB, Julian Day Number, Barycentric Dynamical
    //           Time
    // https://en.wikipedia.org/wiki/Barycentric_Dynamical_Time
    // https://en.wikipedia.org/wiki/Barycentric_Coordinate_Time

    public string Date; // Calendar Date (TDB),
    public double PositionX; //  X, In kilometers.
    public double PositionY; //  Y,
    public double PositionZ; //  Z,
    public double VelocityX; // VX, Kilometers per second.
    public double VelocityY; // VY,
    public double VelocityZ; // VZ,
    //  LT     One-way down-leg Newtonian light-time (sec)
    public double Range;   //  RG  Range; distance from coordinate center (km).
    public double RangeRate; //  RR  Range-rate; radial velocity wrt coord. center (km/sec).
    }



  private JPLHorizonsData()
    {
    }



  internal JPLHorizonsData( MainForm UseForm )
    {
    MForm = UseForm;

    }



  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  internal void ReadFromTextFile( string FileName )
    {
    if( !File.Exists( FileName ))
      {
      ShowStatus( "The file does not exist." );
      ShowStatus( FileName );
      return;
      }

    try
    {
    bool IsInsideData = false;
    // using( StreamReader SReader = new StreamReader( FileName, Encoding.UTF8 ))
    int Lines = 0;
    using( StreamReader SReader = new StreamReader( FileName ))
      {
      while( SReader.Peek() >= 0 )
        {
        string Line = SReader.ReadLine();
        if( Line == null )
          continue;

        Line = Line.Trim();
        if( Line == "" )
          continue;

        // Start of data marker:
        if( Line.StartsWith( "$$SOE" ))
          {
          IsInsideData = true;
          continue;
          }

        // End marker:
        if( Line.StartsWith( "$$EOE" ))
          {
          IsInsideData = false;
          continue;
          }

        if( !IsInsideData )
          continue;

        Lines++;
        if( Lines > 100 )
          break;

        // ShowStatus( Line );

        JPLRec Rec = new JPLRec();

        if( !Line.Contains( "," ))
          continue;

        string[] SplitS = Line.Split( new Char[] { ',' } );

        if( SplitS.Length < 11 )
          continue;

        Rec.Date = SplitS[1].Trim();
        Rec.PositionX = GetDoubleValue( SplitS[2].Trim());
        Rec.PositionY = GetDoubleValue( SplitS[3].Trim());
        Rec.PositionZ = GetDoubleValue( SplitS[4].Trim());
        Rec.VelocityX = GetDoubleValue( SplitS[5].Trim());
        Rec.VelocityY = GetDoubleValue( SplitS[6].Trim());
        Rec.VelocityZ = GetDoubleValue( SplitS[7].Trim());
        //  LT  Light-time  SplitS[8]
        Rec.Range = GetDoubleValue( SplitS[9].Trim());
        Rec.RangeRate = GetDoubleValue( SplitS[10].Trim());

        ShowStatus( Rec.Date + " Range: " + Rec.Range.ToString( "N4" ));
        }
      }

    }
    catch( Exception Except )
      {
      ShowStatus( "Could not read the file: \r\n" + FileName );
      ShowStatus( Except.Message );
      }
    }



  private double GetDoubleValue( string ValueS )
    {
    try
    {
    // Numbers look like: -2.179945319184925E+07,

    return Double.Parse( ValueS );
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in GetDoubleValue():" );
      ShowStatus( Except.Message );
      return 0;
      }
    }



  }
}
