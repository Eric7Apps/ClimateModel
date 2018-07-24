// Copyright Eric Chauvin 2018.
// My blog is at:
// https://scientificmodels.blogspot.com/



// This is any object in space.  A planet, space
// ship, the Sun, or whatever.


using System;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;


namespace ClimateModel
{
  abstract class SpaceObject
  {
  protected MainForm MForm;
  internal string ObjectName = "";
  internal Vector3.Vector Position;
  internal Vector3.Vector Velocity;
  internal Vector3.Vector Acceleration;
  internal double Mass;
  internal JPLHorizonsData JPLData;



  private SpaceObject()
    {

    }



  internal SpaceObject( MainForm UseForm,
                        string UseName,
                        string JPLFileName )
    {
    MForm = UseForm;
    ObjectName = UseName;
    JPLData = new JPLHorizonsData( MForm );
    JPLData.ReadFromTextFile( JPLFileName );
    }



  internal void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  abstract internal void MakeNewGeometryModel();

  abstract internal GeometryModel3D GetGeometryModel();



  internal void SetNextPositionFromVelocity(
                                  double TimeDelta )
    {
    Vector3.Vector MoveBy = Velocity;
    // It moves by this much in TimeDelta time.
    MoveBy = Vector3.MultiplyWithScalar( MoveBy, TimeDelta );
    Position = Vector3.Add( Position, MoveBy );
    }




  internal void SetToNearestJPLPosition( ulong NearestDate )
    {
    JPLHorizonsData.JPLRec Rec = JPLData.GetNearestRecByDateTime( NearestDate );
    // ShowStatus( "Rec.CalDate is:" );
    if( Rec.CalDate == 0 )
      {
      ShowStatus( "No JPL data for: " + ObjectName );
      return;
      }

    // ECTime ShowTime = new ECTime( Rec.CalDate );
    // ShowStatus( ShowTime.ToCrudeString());

    Position.X = Rec.PositionX;
    Position.Y = Rec.PositionY;
    Position.Z = Rec.PositionZ;

    Velocity.X = Rec.VelocityX;
    Velocity.Y = Rec.VelocityY;
    Velocity.Z = Rec.VelocityZ;

    ShowStatus( " " );
    ShowStatus( ObjectName );
    ShowStatus( "Position.X: " + Position.X.ToString( "N1" ));
    ShowStatus( "Position.Y: " + Position.Y.ToString( "N1" ));
    ShowStatus( "Position.Z: " + Position.Z.ToString( "N1" ));

    ShowStatus( "Velocity.X: " + Velocity.X.ToString( "N1" ));
    ShowStatus( "Velocity.Y: " + Velocity.Y.ToString( "N1" ));
    ShowStatus( "Velocity.Z: " + Velocity.Z.ToString( "N1" ));
    }



  }
}
