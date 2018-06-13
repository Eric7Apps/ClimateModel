// Copyright Eric Chauvin 2018.
// My blog is at:
// https://scientificmodels.blogspot.com/



using System;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;


namespace ClimateModel
{
  class SolarSystem
  {
  private MainForm MForm;
  private Model3DGroup Main3DGroup;
  private SpaceObject[] SpaceObjectArray;
  private int SpaceObjectArrayLast = 0;
  // private double RadiusScale = 300.0;
  private PlanetSphere Sun;
  // private PlanetSphere Mercury;
  // private PlanetSphere Venus;
  private EarthGeoid Earth;
  // private PlanetSphere Moon;
  // private PlanetSphere Mars;
  // private PlanetSphere Jupiter;
  // private PlanetSphere Saturn;
  private Vector3.Vector Barycenter;
  private ECTime SunTime; // Local time.
  private ECTime SpringTime; // Spring Equinox.



  private SolarSystem()
    {
    }



  internal SolarSystem( MainForm UseForm,
                        Model3DGroup Use3DGroup )
    {
    MForm = UseForm;
    Main3DGroup = Use3DGroup;

    // The local time for the sun.
    SunTime = new ECTime();
    SpringTime = new ECTime();
    InitializeTimes();

    SpaceObjectArray = new SpaceObject[2];
    AddInitialSpaceObjects();

    SetBarycenter();

    // ECTime RightNow = new ECTime();
    // RightNow.SetToNow();
    // SetPositionsAndTime( RightNow );
    }




  private void InitializeTimes()
    {
    SunTime.SetToNow();

    // https://en.wikipedia.org/wiki/March_equinox

    // "Spring Equinox 2018 was at 10:15 AM on
    // March 20."
    SpringTime.SetUTCTime( 2018,
                            3,
                            20,
                            10,
                            15,
                            0,
                            0 );

    }



  private void SetBarycenter()
    {
    double Distance = ModelConstants.DistanceToSun;

    double RightAscension =
       NumbersEC.RightAscensionToRadians( 4, 58, 20 );
    double Declination =
       NumbersEC.DegreesMinutesToRadians( 22, 40, 49 );

    Barycenter.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Barycenter.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Barycenter.Z = Distance * Math.Sin( Declination );

    Sun.Position.X = Barycenter.X;
    Sun.Position.Y = Barycenter.Y;
    Sun.Position.Z = Barycenter.Z;
    }



/*
  private void SetPositionsAndTime( ECTime SetTime )
    {
    SunTime.Copy( SetTime );

    // The time difference from the Spring Equinox
    // to the SetTime.
    double TimeDiffSeconds = SpringTime.GetSecondsDifference( SunTime );

    // "Earth's orbit has an eccentricity of 0.0167."

    // One sidereal year.
    // Earth orbit in days: 365.256
    double EarthOrbitInHours = 365.256d * 24.0d;
    double EarthOrbitInMinutes = EarthOrbitInHours * 60.0d;
    double EarthOrbitInSeconds = EarthOrbitInMinutes * 60.0d;
    double PartOfOrbit = TimeDiffSeconds / EarthOrbitInSeconds;
    PartOfOrbitRadians = 2 * Math.PI * PartOfOrbit;
    // If Earth was halfway around this would be
    // 2 * Math.PI * 0.5.

    // This would be in the ecliptic plane.
    // Earth.Position.X = Barycenter.X +
    //    (Distance * Math.Cos( PartOfOrbit ));


    // Barycenter.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    // Barycenter.Z = Distance * Math.Sin( Declination );


    // ShowStatus( " " );
    // ShowStatus( "Sun.Position.X: " + Sun.Position.X.ToString( "N0" ));
    // ShowStatus( "Sun.Position.Y: " + Sun.Position.Y.ToString( "N0" ));
    // ShowStatus( "Sun.Position.Z: " + Sun.Position.Z.ToString( "N0" ));
    // ShowStatus( " " );


    //
    // Earth's orbit:
    //                      b  m  t
    // Aphelion:          152100000000
    // Perihelion:        147095000000
    }
*/




  private void AddInitialSpaceObjects()
    {
    AddSun();
    AddEarth();

    // AddMercury();
    // AddVenus();
    // AddMoon();
    // AddMars();
    // AddJupiter();
    // AddSaturn();
    }



  internal void AddSpaceObject( SpaceObject ToAdd )
    {
    SpaceObjectArray[SpaceObjectArrayLast] = ToAdd;
    SpaceObjectArrayLast++;
    if( SpaceObjectArrayLast >= SpaceObjectArray.Length )
      {
      Array.Resize( ref SpaceObjectArray, SpaceObjectArray.Length + 16 );
      }
    }



  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  private void AddSun()
    {
    // https://theskylive.com/sun-info
    // https://en.wikipedia.org/wiki/Sun

    Sun = new PlanetSphere( MForm, true );

    Sun.Radius = 695700 * ModelConstants.TenTo3;
    Sun.Mass = ModelConstants.MassOfSun;

    Sun.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\sun.jpg";

    // Sun.Velocity.X = 0;
    // Sun.Velocity.Y = 0;
    // Sun.Velocity.Z = 0;

    AddSpaceObject( Sun );
    }



  private void AddEarth()
    {
    // https://en.wikipedia.org/wiki/Earth

    // Earth is an EarthGeoid, not a PlanetSphere.
    Earth = new EarthGeoid( MForm );

    // Shift the time of day:
    // If I make this 3 then the Earth rotates to
    // the east by 3 hours.  (The sun moves to the
    // west three hours toward sunset.)

    Earth.LongitudeHoursRadians = 0;

    // Earth.Position.X = 0;
    // Earth.Position.Y = 0;
    // Earth.Position.Z = 0;

    Earth.Mass = ModelConstants.MassOfEarth;

    /*
    // Distance in Meters.
    // Earth orbit in days: 365.256
    const double VelocityPerDay =
       ModelConstants.EarthOrbitCircumference / 365.256d;
    const double VelocityPerHour =
                        VelocityPerDay / 24d;
    const double VelocityPerSecond =
              VelocityPerHour / (60.0d * 60.0d);

    ShowStatus( " " );
    ShowStatus( "Earth Velocity meters per hour: " + VelocityPerHour.ToString( "N0" ));
    ShowStatus( "Earth Velocity meters per second: " + VelocityPerSecond.ToString( "N0" ));
    ShowStatus( " " );

    // Make it so the Earth is moving in the opposite
    // direction and the sun's velocity is zero in
    // this coordinate system.  (To start with.)
    Earth.Velocity.X = -Sun.Velocity.X;
    Earth.Velocity.Y = -Sun.Velocity.Y;
    Earth.Velocity.Z = -Sun.Velocity.Z;
    Sun.Velocity.X = 0;
    Sun.Velocity.Y = 0;
    Sun.Velocity.Z = 0;

    // Make a better estimate of velocity.
    Vector3.Normalize( ref Earth.Velocity, Earth.Velocity );
    Vector3.MultiplyWithScalar( ref Earth.Velocity, VelocityPerSecond );

    ShowStatus( "Earth Velocity meters per second:" );
    ShowStatus( "Earth Velocity.X: " + Earth.Velocity.X.ToString( "N0" ));
    ShowStatus( "Earth Velocity.Y: " + Earth.Velocity.Y.ToString( "N0" ));
    ShowStatus( "Earth Velocity.Z: " + Earth.Velocity.Z.ToString( "N0" ));
    ShowStatus( " " );

    // Velocity.Z is at a maximum at the spring
    // equinox.
    */

    Earth.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\earth.jpg";
    AddSpaceObject( Earth );
    }



  internal void MakeNewGeometryModels()
    {
    // Main3DGroup.Children.Clear();

    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObjectArray[Count].MakeNewGeometryModel();
      GeometryModel3D GeoMod = SpaceObjectArray[Count].GetGeometryModel();
      if( GeoMod == null )
        continue;

      Main3DGroup.Children.Add( GeoMod );
      }

    // SetupAmbientLight();
    // SetupSunlight();
    }



  internal void ResetGeometryModels()
    {
    // Main3DGroup.Children.Clear();

    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      GeometryModel3D GeoMod = SpaceObjectArray[Count].GetGeometryModel();
      if( GeoMod == null )
        continue;

      Main3DGroup.Children.Add( GeoMod );
      }

    // SetupAmbientLight();
    // SetupSunlight();
    }



  }
}
