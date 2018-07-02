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
  private const double RadiusScale = 300.0;
  private PlanetSphere Sun;
  private EarthGeoid Earth;
  private PlanetSphere Moon;

  // private ECTime SunTime; // Local time.
  // private ECTime SpringTime; // Spring Equinox.



  private SolarSystem()
    {
    }



  internal SolarSystem( MainForm UseForm,
                        Model3DGroup Use3DGroup )
    {
    MForm = UseForm;
    Main3DGroup = Use3DGroup;

    // The local time for the sun.
    // SunTime = new ECTime();
    // SpringTime = new ECTime();
    // InitializeTimes();

    SpaceObjectArray = new SpaceObject[2];
    AddInitialSpaceObjects();

    // ECTime RightNow = new ECTime();
    // RightNow.SetToNow();
    // SetPositionsAndTime( RightNow );
    }



/*
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
*/





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

    // Earth's orbit:
    //                      b  m  t
    // Aphelion:          152100000000
    // Perihelion:        147095000000
    }
*/




  private void AddInitialSpaceObjects()
    {
    try
    {
    // Sun:
    string JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLSun.txt";
    Sun = new PlanetSphere( MForm, "Sun", true, JPLFileName );
    Sun.Radius = 695700 * ModelConstants.TenTo3;
    Sun.Mass = ModelConstants.MassOfSun;
    Sun.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\sun.jpg";
    AddSpaceObject( Sun );

    // Earth:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLEarth.txt";
    Earth = new EarthGeoid( MForm, "Earth", JPLFileName );
    // Shift the time of day:
    // If I make this 3 then the Earth rotates to
    // the east by 3 hours.  (The sun moves to the
    // west three hours toward sunset.)
    Earth.LongitudeHoursRadians = 0;
    Earth.Mass = ModelConstants.MassOfEarth;
    Earth.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\earth.jpg";
    AddSpaceObject( Earth );
    // Earth orbit in days: 365.256
    // const double VelocityPerDay =
    //   ModelConstants.EarthOrbitCircumference / 365.256d;
    // const double VelocityPerHour =
    //                    VelocityPerDay / 24d;

    // Moon:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLMoon.txt";
    Moon = new PlanetSphere( MForm, "Moon", false, JPLFileName );
    // Radius: About 1,737.1 kilometers.
    Moon.Radius = 1737100;
    Moon.Mass = ModelConstants.MassOfMoon;
    Moon.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\moon.jpg";
    AddSpaceObject( Moon );


    // Mercury:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLMercury.txt";
    PlanetSphere Mercury = new PlanetSphere(
              MForm, "Mercury", false, JPLFileName );

    Mercury.Radius = 2440000d * RadiusScale;
    Mercury.Mass = ModelConstants.MassOfMercury;
    Mercury.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Mercury.jpg";
    AddSpaceObject( Mercury );

    // Venus:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLVenus.txt";
    PlanetSphere Venus = new PlanetSphere(
                MForm, "Venus", false, JPLFileName );

    Venus.Radius = 6051000 * RadiusScale; // 6,051 km
    Venus.Mass = ModelConstants.MassOfVenus;
    Venus.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Venus.jpg";
    AddSpaceObject( Venus );

    // Mars:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLMars.txt";
    PlanetSphere Mars = new PlanetSphere(
                 MForm, "Mars", false, JPLFileName );

    Mars.Radius = 3396000 * RadiusScale;
    Mars.Mass = ModelConstants.MassOfMars;
    Mars.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\mars.jpg";
    AddSpaceObject( Mars );


    // Jupiter:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLJupiter.txt";
    PlanetSphere Jupiter = new PlanetSphere(
              MForm, "Jupiter", false, JPLFileName );

    //                m  t
    Jupiter.Radius = 69911000d * RadiusScale; // 69,911 km
    Jupiter.Mass = ModelConstants.MassOfJupiter;
    Jupiter.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Jupiter.jpg";
    AddSpaceObject( Jupiter );

    // Saturn:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLSaturn.txt";
    PlanetSphere Saturn = new PlanetSphere(
                MForm, "Saturn", false, JPLFileName );

    //               m  t
    Saturn.Radius = 58232000d * RadiusScale; // 58,232 km
    Saturn.Mass = ModelConstants.MassOfSaturn;
    Saturn.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Saturn.jpg";
    AddSpaceObject( Saturn );


    ECTime RightNow = new ECTime();
    RightNow.SetToNow();
    SetToJPLTimePosition( RightNow.GetIndex() );
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in SolarSystem.AddMercury(): " + Except.Message );
      }
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



  internal void SetToJPLTimePosition( ulong TimeIndex )
    {
    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObjectArray[Count].
                 SetToNearestJPLPosition( TimeIndex );

      }
    }



  internal void MakeNewGeometryModels()
    {
    Main3DGroup.Children.Clear();

    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObjectArray[Count].MakeNewGeometryModel();
      GeometryModel3D GeoMod = SpaceObjectArray[Count].GetGeometryModel();
      if( GeoMod == null )
        continue;

      Main3DGroup.Children.Add( GeoMod );
      }

    SetupAmbientLight();
    SetupSunlight();
    }



  internal void ResetGeometryModels()
    {
    Main3DGroup.Children.Clear();

    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      GeometryModel3D GeoMod = SpaceObjectArray[Count].GetGeometryModel();
      if( GeoMod == null )
        continue;

      Main3DGroup.Children.Add( GeoMod );
      }

    SetupAmbientLight();
    SetupSunlight();
    }



  private void SetupSunlight()
    {
    // Lights are Model3D objects.
    // System.Windows.Media.Media3D.Model3D
    //   System.Windows.Media.Media3D.Light

    // double OuterDistance = 1.5;

    double X = Sun.Position.X * ModelConstants.ThreeDSizeScale;
    double Y = Sun.Position.Y * ModelConstants.ThreeDSizeScale;
    double Z = Sun.Position.Z * ModelConstants.ThreeDSizeScale;
    // double RadiusScaled = Sun.Radius * ModelConstants.ThreeDSizeScale;

    SetupPointLight( X,
                     Y,
                     Z );

    }



  private void SetupPointLight( double X,
                                double Y,
                                double Z )
    {
    PointLight PLight1 = new PointLight();
    PLight1.Color = System.Windows.Media.Colors.White;

    Point3D Location = new  Point3D( X, Y, Z );
    PLight1.Position = Location;
    PLight1.Range = 100000000.0;

    // Attenuation with distance D is like:
    // Attenuation = C + L*D + Q*D^2
    PLight1.ConstantAttenuation = 1;
    // PLight.LinearAttenuation = 1;
    // PLight.QuadraticAttenuation = 1;

    Main3DGroup.Children.Add( PLight1 );
    }



  private void SetupAmbientLight()
    {
    byte RGB = 0x0F;
    SetupAmbientLightColors( RGB, RGB, RGB );
    }



  private void SetupAmbientLightColors( byte Red,
                                        byte Green,
                                        byte Blue )
    {
    try
    {
    AmbientLight AmbiLight = new AmbientLight();
    // AmbiLight.Color = System.Windows.Media.Colors.Gray; // AliceBlue

    Color AmbiColor = new Color();
    AmbiColor.R = Red;
    AmbiColor.G = Green;
    AmbiColor.B = Blue;

    AmbiLight.Color = AmbiColor;

    Main3DGroup.Children.Add( AmbiLight );
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in ThreeDScene.SetupAmbientLight(): " + Except.Message );
      }
    }




  internal void RotateView()
    {
    double AddHours = NumbersEC.DegreesToRadians( 0.5 * (360.0d / 24.0d) );
    Earth.LongitudeHoursRadians = Earth.LongitudeHoursRadians + AddHours;
    Earth.MakeNewGeometryModel();
    ResetGeometryModels();
    }




  internal void DoTimeStep()
    {
    const double TimeDelta = 60 * 10; // seconds.
    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObject SpaceObj = SpaceObjectArray[Count];
      SpaceObj.SetNextPositionFromVelocity(
                                    TimeDelta );
      }

    Vector3.Vector AccelVector = new Vector3.Vector();
    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObject SpaceObj = SpaceObjectArray[Count];
      Vector3.SetZero( ref SpaceObj.Acceleration );

      for( int Count2 = 0; Count2 < SpaceObjectArrayLast; Count2++ )
        {
        SpaceObject FarAwaySpaceObj = SpaceObjectArray[Count2];
        if( FarAwaySpaceObj.Mass < 1 )
          throw( new Exception( "The space object has no mass." ));

        Vector3.Copy( ref AccelVector, ref FarAwaySpaceObj.Position );
        Vector3.Subtract( ref AccelVector, ref SpaceObj.Position );

        double Distance = Vector3.Norm( ref AccelVector );

        // Check if it's the same planet at zero
        // distance.
        if( Distance < 1.0 )
          continue;

        double Acceleration =
             (ModelConstants.GravitationConstant *
             FarAwaySpaceObj.Mass) /
             (Distance * Distance);

        Vector3.Normalize( ref AccelVector, AccelVector );
        Vector3.MultiplyWithScalar( ref AccelVector, Acceleration );
        Vector3.Add( ref SpaceObj.Acceleration, ref AccelVector );
        }

      // Add the new Acceleration vector to the
      // velocity vector.
      Vector3.Add( ref SpaceObj.Velocity, ref SpaceObj.Acceleration );
      }

    ShowStatus( " " );
    ShowStatus( "Velocity.X: " + Earth.Velocity.X.ToString( "N2" ));
    ShowStatus( "Velocity.Y: " + Earth.Velocity.Y.ToString( "N2" ));
    ShowStatus( "Velocity.Z: " + Earth.Velocity.Z.ToString( "N2" ));
    ShowStatus( " " );


    Earth.AddTimeStepRotateAngle();

    // Earth.SetPlanetGravityAcceleration( this );

    // Move Earth only:
    // Earth.MakeNewGeometryModel();
    // ResetGeometryModels();

    // Move all of the planets:
    MakeNewGeometryModels();
    }



  internal Vector3.Vector GetEarthScaledPosition()
    {
    Vector3.Vector ScaledPos;

    ScaledPos.X = Earth.Position.X * ModelConstants.ThreeDSizeScale;
    ScaledPos.Y = Earth.Position.Y * ModelConstants.ThreeDSizeScale;
    ScaledPos.Z = Earth.Position.Z * ModelConstants.ThreeDSizeScale;

    return ScaledPos;
    }



  internal void SetEarthPositionToZero()
    {
    Earth.Position.X = 0;
    Earth.Position.Y = 0;
    Earth.Position.Z = 0;

    // Make a new Earth geometry model before
    // calling this:
    // ResetGeometryModels();

    MakeNewGeometryModels();
    }




  }
}
