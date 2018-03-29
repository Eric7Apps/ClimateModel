// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// This is a container for SpaceObjects, and also
// it sets objects in a reference frame.

// Vernal: "of, relating to, or occurring in the
// spring."  "fresh or new like the spring"

// The sun has about 99.8 percent of the mass
// of the Solar System.  Most of the rest of the
// mass is in Jupiter.

// Pretty much everything rotates or orbits
// counterclockwise when looking down the Z axis.
// (From above the north pole.)


// https://en.wikipedia.org/wiki/International_Celestial_Reference_Frame
// https://en.wikipedia.org/wiki/Equatorial_coordinate_system
// https://en.wikipedia.org/wiki/Solar_System
// https://en.wikipedia.org/wiki/Equinox
// https://en.wikipedia.org/wiki/Theory_of_tides
// https://en.wikipedia.org/wiki/Celestial_mechanics
// https://en.wikipedia.org/wiki/Continuum_mechanics
// https://en.wikipedia.org/wiki/Orbital_mechanics
// https://en.wikipedia.org/wiki/Epoch_(astronomy)
// https://en.wikipedia.org/wiki/Astrometry
// https://en.wikipedia.org/wiki/Ephemeris



using System;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;


namespace ClimateModel
{
  class ReferenceFrame
  {
  private MainForm MForm;
  private Model3DGroup Main3DGroup;
  private SpaceObject[] SpaceObjectArray;
  private int SpaceObjectArrayLast = 0;
  private PlanetSphere Sun;
  private PlanetSphere Mercury;
  private PlanetSphere Venus;
  private EarthGeoid Earth;
  private PlanetSphere Moon;
  private PlanetSphere Mars;
  private PlanetSphere Jupiter;
  private PlanetSphere Saturn;
  private double SunRightA;
  private double SunDecl;
  private double SunRightAPrev;
  private double SunDeclPrev;
  private double MoonRightA;
  private double MoonDecl;
  private double MoonRightAPrev;
  private double MoonDeclPrev;
  private double MercuryRightA;
  private double MercuryDecl;
  private double VenusRightA;
  private double VenusDecl;
  private double MarsRightA;
  private double MarsDecl;
  private double JupiterRightA;
  private double JupiterDecl;
  private double SaturnRightA;
  private double SaturnDecl;



  private ReferenceFrame()
    {
    }


  internal ReferenceFrame( MainForm UseForm,
                           Model3DGroup Use3DGroup )
    {
    MForm = UseForm;

    Main3DGroup = Use3DGroup;

    SetInitialPositionValues();

    SpaceObjectArray = new SpaceObject[2];
    AddInitialSpaceObjects();
    }



  private void SetInitialPositionValues()
    {
    // Edit these all in one place:
    // https://theskylive.com/planets

    // Right Ascension:
    // https://en.wikipedia.org/wiki/Right_ascension

    SunRightA =
       NumbersEC.RightAscensionToRadians( 0, 32, 15 );
    SunDecl =
       NumbersEC.DegreesMinutesToRadians( 3, 28, 50 );
    SunRightAPrev =
       NumbersEC.RightAscensionToRadians( 0, 31, 58 );
    SunDeclPrev =
       NumbersEC.DegreesMinutesToRadians( 3, 27, 3 );

    MoonRightA =
       NumbersEC.RightAscensionToRadians( 11, 9, 57 );
    MoonDecl =
       NumbersEC.DegreesMinutesToRadians( 8, 15, 56 );
    MoonRightAPrev =
       NumbersEC.RightAscensionToRadians( 5, 25, 29 );
    MoonDeclPrev =
       NumbersEC.DegreesMinutesToRadians( 19, 9, 38 );

    MercuryRightA =
       NumbersEC.RightAscensionToRadians( 0, 46, 36 );
    MercuryDecl =
       NumbersEC.DegreesMinutesToRadians( 8, 33, 18 );

    VenusRightA =
       NumbersEC.RightAscensionToRadians( 1, 44, 30 );
    VenusDecl =
       NumbersEC.DegreesMinutesToRadians( 10, 11, 52 );

    ////////////////////////////////////////////////////
    // Look how close the Mars and Saturn Right
    // Ascension values are.
    MarsRightA =
       NumbersEC.RightAscensionToRadians( 18, 28, 29 );
    MarsDecl =
       NumbersEC.DegreesMinutesToRadians( -23, 33, 46 );

    SaturnRightA =
       NumbersEC.RightAscensionToRadians( 18, 37, 8 );
    SaturnDecl =
       NumbersEC.DegreesMinutesToRadians( -22, 16, 44 );

    /////////////////////////////


    JupiterRightA =
       NumbersEC.RightAscensionToRadians( 15, 20, 57 );
    JupiterDecl =
       NumbersEC.DegreesMinutesToRadians( -17, 8, 16 );

    }



  private void AddInitialSpaceObjects()
    {
    /////////////////////////////
    // Do these two first in order to set up the
    // coordinate system.
    AddSun();
    AddEarth(); // See notes in AddEarth().
    /////////////////////////////

    AddMercury();
    AddVenus();
    AddMoon();
    AddMars();
    AddJupiter();
    AddSaturn();
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

    double OuterDistance = 1.5;

    double X = Sun.Position.X;
    double Y = Sun.Position.Y;
    double Z = Sun.Position.Z;

    // This is a crude way of making the outside of
    // the sun look bright, and at the same time
    // approximate light coming from the sun.  Put
    // four lights around it and close to it.  Use
    // EmissiveMaterial instead?
    SetupPointLight( X + (Sun.Radius * OuterDistance),
                     Y,
                     Z );

    SetupPointLight( X - (Sun.Radius * OuterDistance),
                     Y,
                     Z );

    SetupPointLight( X,
                     Y + (Sun.Radius * OuterDistance),
                     Z );

    SetupPointLight( X,
                     Y - (Sun.Radius * OuterDistance),
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
    byte RGB = 0x1F;
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



  private void AddSun()
    {
    // https://theskylive.com/sun-info
    // https://en.wikipedia.org/wiki/Sun

    Sun = new PlanetSphere( MForm );

    // Radius: About 695,700 kilometers.
    Sun.Radius = 695.7;

    // Sun.Mass;

    double RightAscension = SunRightA;
    double Declination = SunDecl;

    double Distance = 148993; // 149597.8;
    double PrevDistance = 148993;

    // Notice that if the Declination and the Right
    // Ascension are both zero, which is what they
    // are at the Spring Equinox, then the Cosines
    // are 1, so this X is = Distance * 1 * 1.
    // Sun.X = Distance * (Math.Cos( 0 ) * Math.Cos( 0 ));
    // In other words the X axis points along where
    // both Right Ascension and Declination are both
    // zero.
    Sun.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));

    // For Right Ascension of 90 degrees (or 6
    // hours), The Sine is 1, so the Y axis is along
    // where R.A. is at 6 hours and Decliniation
    // is zero.
    Sun.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));

    // The Z axis is along where Declination is at
    // 90 degrees, which is through the North Pole.
    Sun.Position.Z = Distance * Math.Sin( Declination );

    ShowStatus( " " );
    ShowStatus( "Sun.Position.X: " + Sun.Position.X.ToString( "N0" ));
    ShowStatus( "Sun.Position.Y: " + Sun.Position.Y.ToString( "N0" ));
    ShowStatus( "Sun.Position.Z: " + Sun.Position.Z.ToString( "N0" ));
    ShowStatus( " " );


    Sun.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\sun.jpg";

    double XPrev = PrevDistance * (Math.Cos( SunDeclPrev ) * Math.Cos( SunRightAPrev ));
    double YPrev = PrevDistance * (Math.Cos( SunDeclPrev ) * Math.Sin( SunRightAPrev ));
    double ZPrev = PrevDistance * Math.Sin( SunDeclPrev );

    // Per one unit of time.  What ever time the
    // previous positions were taken at.
    Sun.Velocity.X = Sun.Position.X - XPrev;
    Sun.Velocity.Y = Sun.Position.Y - YPrev;
    Sun.Velocity.Z = Sun.Position.Z - ZPrev;

    AddSpaceObject( Sun );
    }



  private void AddEarth()
    {
    // https://en.wikipedia.org/wiki/Earth

    // https://en.wikipedia.org/wiki/Astronomical_unit
    // Notice the "helio" part of the words here.
    // Maximum: Aphelion
    // Minimum: Perihelion
    // Peri: near.
    // Apo: away from.
    // 'Perigee' and 'Apogee' are used for orbits
    // around the Earth.

    // Earth's orbit:
    // Aphelion:   152,100,000 km
    // Perihelion: 147,095,000 km

    // Earth is an EarthGeoid, not a PlanetSphere.
    Earth = new EarthGeoid( MForm );

    // Shift the time of day:
    // If I make this 3 then the Earth rotates to
    // the east by 3 hours.  (The sun moves to the
    // west three hours toward sunset.)

    Earth.LongitudeHoursRadians = 0;

    // Earth starts out at the center of the
    // non-rotating coordinate system.
    Earth.Position.X = 0;
    Earth.Position.Y = 0;
    Earth.Position.Z = 0;

    // One Astronomical unit is the distance from
    // the earth to the sun.
    // One AU: 149,597,870.7 kilometers.

    // Distance in kilometers.
    double Circumference = 149597870.7d * 2.0d * Math.PI;
    // Earth orbit in days: 365.256
    double VelocityPerDay = Circumference / 365.256d;
    double VelocityPerHour = VelocityPerDay / 24d;
    double VelocityPerSecond = VelocityPerHour / (60.0d * 60.0d);

    ShowStatus( " " );
    ShowStatus( "Earth Velocity kilometers per hour: " + VelocityPerHour.ToString( "N2" ));
    ShowStatus( "Earth Velocity kilometers per second: " + VelocityPerSecond.ToString( "N2" ));
    ShowStatus( " " );
    // Earth Velocity kilometers per hour: 107,225.15
    // Earth Velocity kilometers per second: 29.78

    // Make it so the Earth is moving in the opposite
    // direction and the sun's velocity is zero in
    // this coordinate system.
    Earth.Velocity.X = -Sun.Velocity.X;
    Earth.Velocity.Y = -Sun.Velocity.Y;
    Earth.Velocity.Z = -Sun.Velocity.Z;
    Sun.Velocity.X = 0;
    Sun.Velocity.Y = 0;
    Sun.Velocity.Z = 0;

    // Make a better estimate of velocity.
    QuaternionEC.NormalizeVector3( ref Earth.Velocity, Earth.Velocity );
    QuaternionEC.MultiplyVector3WithScalar( ref Earth.Velocity, VelocityPerSecond );

    ShowStatus( "Earth Velocity kilometers per second:" );
    ShowStatus( "Earth Velocity.X: " + Earth.Velocity.X.ToString( "N2" ));
    ShowStatus( "Earth Velocity.Y: " + Earth.Velocity.Y.ToString( "N2" ));
    ShowStatus( "Earth Velocity.Z: " + Earth.Velocity.Z.ToString( "N2" ));
    ShowStatus( " " );

    // The Earth is moving counterclockwise around
    // the sun.  Looking down from above the north
    // pole that means that after spring equinox
    // the Earth is moving in the positive X
    // direction, the negative Y direction, and the
    // negative Z direction.  It's the opposite of
    // the way the sun _appears_ to be moving.

    // This position for the sun was not long after
    // the spring equinox, on March 29th.
    // Velocity.X should be zero at the spring
    // equinox.  At the spring equinox, the earth
    // is at the origin and the sun is straight
    // down the X axis.
    // March 29th position of sun:
    // Sun.Position.X: 147,248
    // Sun.Position.Y: 20,858
    // Sun.Position.Z: 9,045

    // Earth Velocity kilometers per second:
    // Earth Velocity.X: 4.53
    // Earth Velocity.Y: -27.09
    // Earth Velocity.Z: -11.52

    Earth.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\earth.jpg";
    AddSpaceObject( Earth );
    }




  private void AddMoon()
    {
    // https://en.wikipedia.org/wiki/Moon

    // https://theskylive.com/moon-info
    Moon = new PlanetSphere( MForm );

    // Radius: About 1,737.1 kilometers.
    Moon.Radius = 1.7371;

    double RightAscension = MoonRightA;
    double Declination = MoonDecl;

    double Distance = 380; // 362.6;

    Moon.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Moon.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Moon.Position.Z = Distance * Math.Sin( Declination );

    // Moon.Velocity.X with respect to the Earth's
    // velocity.


    Moon.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\moon.jpg";
    // Moon.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Earth.jpg";
    AddSpaceObject( Moon );
    }




  private void AddMars()
    {
    try
    {
    // ShowStatus( "Adding Mars:" );

    Mars = new PlanetSphere( MForm );

    // Radius in thousands of kilometers.
    // Times 1000 to make it visible.
    Mars.Radius = 3.396 * 1000;

    double RightAscension = MarsRightA;
    double Declination = MarsDecl;


    /*
    double SunMarsAngle = Math.Abs( SunRightA - MarsRightA );
    // ShowStatus( "SunMarsAngle: " + SunMarsAngle.ToString( "N2" ));

    // Mars is about 227,939,200 km from the Sun.
    // Earth is about 149,597,871 km from the Sun.
    // Earth to Sun is one Astronomical unit.

    double DistanceMarsToSun = 227939.2;
    double DistanceEarthToSun = 149597.9;

    // What is its distance from Mars to the Earth?
    // Minimum:  54,600,000 km
    // Maximum: 401,000,000 km

    // double EarthMarsDistance = GetEarthPlanetDistance(
    //               SunMarsAngle, // As seen from Earth.
    //               DistanceMarsToSun,
    //               DistanceEarthToSun );

    double Distance = EarthMarsDistance;
    */

    // The earth is moving closer to Mars now.
    double Distance = 168947;

    Mars.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Mars.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Mars.Position.Z = Distance * Math.Sin( Declination );

    Mars.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\mars.jpg";
    AddSpaceObject( Mars );

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ReferenceFrame.AddMars(): " + Except.Message );
      }
    }




  /* A rough approximation:
  private double GetEarthPlanetDistance(
                   double SunPlanetAngle,
                   double DistancePlanetToSun,
                   double DistanceEarthToSun )
    {
    try
    {
    // ShowStatus( " " );
    ShowStatus( "SunPlanetAngle: " + SunPlanetAngle.ToString( "N2" ));
    ShowStatus( "DistancePlanetToSun: " + DistancePlanetToSun.ToString( "N2" ));
    ShowStatus( "DistanceEarthToSun: " + DistanceEarthToSun.ToString( "N2" ));

    // SunPlanetAngle as seen from Earth, between the
    // Sun and that other planet.

    // The Law of Cosines is like the generalized
    // Pythagorean theorem.
    // c^2 = a^2 + x^2 - 2ax Cos( Gamma )
    // https://en.wikipedia.org/wiki/Law_of_cosines
    // https://en.wikipedia.org/wiki/Law_of_sines

    // Quadratic Formula:
    // x = (-b +/- (b^2 - 4ac)^0.5) / 2a

    // c^2 = a^2 + x^2 - 2adx
    // c^2 - a^2 = x^2 - 2adx
    // f = c^2 - a^2
    // f = x^2 - 2adx
    // x^2 - 2adx - f = 0
    // g = 2ad
    // x^2 - gx - f = 0

    // x = (g +- (g^2 + 4f)^0.5) / 2
    double A = DistanceEarthToSun;
    double C = DistancePlanetToSun;
    double D = Math.Cos( SunPlanetAngle );
    double F = (C * C) - (A * A);

    // if( F < 0 )
      // ShowStatus( "F is negative: " + F.ToString( "N2" ));

    double G = 2 * A * D;

    // x = (g +- (g^2 + 4f)^0.5) / 2
    double H = (G * G) + (4 * F);
    if( H < 0 )
      {
      ShowStatus( " " );
      ShowStatus( "H is negative: " + H.ToString( "N2" ));
      ShowStatus( " " );
      return -1;
      }

    H = Math.Sqrt( H );

    // There are two choices for the distance.
    double Answer1 = (G + H) * 0.5;
    double Answer2 = (G - H) * 0.5;

    ShowStatus( "Answer1: " + Answer1.ToString( "N2" ));
    ShowStatus( "Answer2: " + Answer2.ToString( "N2" ));

    double EarthPlanetDistance = Answer2;

    if( Answer2 < 0 )
      {
      EarthPlanetDistance = Answer1;
      }
    else
      {
      // Then figure out which one it is.  If the
      // velocity vector is going in the opposite
      // direction to the earth's velocity vector,
      // then it's on the other side of the sun.
      }

    return EarthPlanetDistance;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ReferenceFrame.GetEarthPlanetDistance(): " + Except.Message );
      return -1;
      }
    }
*/



  private void AddMercury()
    {
    // https://en.wikipedia.org/wiki/Mercury_(planet)

    try
    {
    // ShowStatus( "Adding Mercury:" );

    Mercury = new PlanetSphere( MForm );

    // Radius in thousands of kilometers.
    Mercury.Radius = 2.440 * 100;

    double RightAscension = MercuryRightA;
    double Declination = MercuryDecl;

    double Distance = 92770;

    Mercury.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Mercury.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Mercury.Position.Z = Distance * Math.Sin( Declination );

    Mercury.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Mercury.jpg";
    AddSpaceObject( Mercury );

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ReferenceFrame.AddMercury(): " + Except.Message );
      }
    }




  private void AddVenus()
    {
    Venus = new PlanetSphere( MForm );

    // Radius in thousands of kilometers.
    Venus.Radius = 6.051 * 100; // 6,051 km

    double RightAscension = VenusRightA;
    double Declination = VenusDecl;

    double Distance = 237239;

    Venus.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Venus.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Venus.Position.Z = Distance * Math.Sin( Declination );

    Venus.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Venus.jpg";
    AddSpaceObject( Venus );
    }



  private void AddJupiter()
    {
    Jupiter = new PlanetSphere( MForm );

    // Radius in thousands of kilometers.
    Jupiter.Radius = 69.911 * 1000; // 69,911 km

    double RightAscension = JupiterRightA;
    double Declination = JupiterDecl;

    double Distance = 695695;

    Jupiter.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Jupiter.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Jupiter.Position.Z = Distance * Math.Sin( Declination );

    Jupiter.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Jupiter.jpg";
    AddSpaceObject( Jupiter );
    }




  private void AddSaturn()
    {
    Saturn = new PlanetSphere( MForm );

    // Radius in thousands of kilometers.
    Saturn.Radius = 58.232 * 1000; // 58,232 km

    double RightAscension = SaturnRightA;
    double Declination = SaturnDecl;

    double Distance = 1498032;

    Saturn.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Saturn.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Saturn.Position.Z = Distance * Math.Sin( Declination );

    Saturn.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Saturn.jpg";
    AddSpaceObject( Saturn );
    }




  internal void DoTimeStep()
    {
    // MForm.ShowStatus( "DoTimeStep() in RefFrame." );

    double AddHours = NumbersEC.DegreesToRadians( 0.5 * (360.0d / 24.0d) );
    Earth.LongitudeHoursRadians = Earth.LongitudeHoursRadians + AddHours;
    Earth.MakeNewGeometryModel();
    ResetGeometryModels();
    // MakeNewGeometryModels();
    }



  }
}
