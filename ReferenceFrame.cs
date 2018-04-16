// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com



// This is a container for SpaceObjects, and also
// it sets objects in a reference frame.


// The sun has about 99.8 percent of the mass
// of the Solar System.  Most of the rest of the
// mass is in Jupiter.

// Pretty much everything rotates or orbits
// counterclockwise when looking down the Z axis.
// (From above the north pole.)


// https://en.wikipedia.org/wiki/Barycenter
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
  private double RadiusScale = 400.0;
  private PlanetSphere Sun;
  private PlanetSphere Mercury;
  private PlanetSphere Venus;
  private EarthGeoid Earth;
  private PlanetSphere Moon;
  private PlanetSphere Mars;
  private PlanetSphere Jupiter;
  private PlanetSphere Saturn;

  // The values they start with:
  private double SunRightA;
  private double SunDecl;
  private double SunRightAPrev;
  private double SunDeclPrev;
  private double MoonRightA;
  private double MoonDecl;
  // private double MoonRightAPrev;
  // private double MoonDeclPrev;
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
       NumbersEC.RightAscensionToRadians( 1, 37, 46 );
    SunDecl =
       NumbersEC.DegreesMinutesToRadians( 10, 10, 11 );
    SunRightAPrev =
       NumbersEC.RightAscensionToRadians( 1, 30, 35 );
    SunDeclPrev =
       NumbersEC.DegreesMinutesToRadians( 9, 28, 39 );

    MoonRightA =
       NumbersEC.RightAscensionToRadians( 2, 13, 11 );
    MoonDecl =  // Shown as negative zero.
       NumbersEC.DegreesMinutesToRadians( 8, 6, 11 );


// 23, 45, 58
// -5, 14, 23

// 22, 58, 44
// -9, 16, 42

// 17, 55, 1
// -19, 55, 27



    MercuryRightA =
       NumbersEC.RightAscensionToRadians( 0, 18, 17 );
    MercuryDecl =
       NumbersEC.DegreesMinutesToRadians( 1, 1, 40 );

    VenusRightA =
       NumbersEC.RightAscensionToRadians( 3, 9, 47 );
    VenusDecl =
       NumbersEC.DegreesMinutesToRadians( 17, 55, 22 );

    MarsRightA =
       NumbersEC.RightAscensionToRadians( 19, 10, 26 );
    MarsDecl =
       NumbersEC.DegreesMinutesToRadians( -23, 14, 54 );

    SaturnRightA =
       NumbersEC.RightAscensionToRadians( 18, 38, 26 );
    SaturnDecl =
       NumbersEC.DegreesMinutesToRadians( -22, 15, 18 );

    JupiterRightA =
       NumbersEC.RightAscensionToRadians( 15, 14, 47 );
    JupiterDecl =
       NumbersEC.DegreesMinutesToRadians( -16, 43, 9 );

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

    double X = Sun.Position.X * PlanetSphere.ThreeDSizeScale;
    double Y = Sun.Position.Y * PlanetSphere.ThreeDSizeScale;
    double Z = Sun.Position.Z * PlanetSphere.ThreeDSizeScale;
    double RadiusScaled = Sun.Radius * PlanetSphere.ThreeDSizeScale;

    // This is a crude way of making the outside of
    // the sun look bright, and at the same time
    // approximate light coming from the sun.  Put
    // four lights around it and close to it.  Use
    // EmissiveMaterial instead?
    SetupPointLight( X + (RadiusScaled * OuterDistance),
                     Y,
                     Z );

    SetupPointLight( X - (RadiusScaled * OuterDistance),
                     Y,
                     Z );

    SetupPointLight( X,
                     Y + (RadiusScaled * OuterDistance),
                     Z );

    SetupPointLight( X,
                     Y - (RadiusScaled * OuterDistance),
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



  private void AddSun()
    {
    // https://theskylive.com/sun-info
    // https://en.wikipedia.org/wiki/Sun

    Sun = new PlanetSphere( MForm );

    Sun.Radius = 695700 * ModelConstants.TenTo3;

    // Sun.Mass;

    double RightAscension = SunRightA;
    double Declination = SunDecl;

    // One Astronomical Unit (AU) is the distance from
    // the earth to the sun.
    // One AU: 149,597,870.7 kilometers.


    // Earth's orbit:
    //                      b  m  t
    // Aphelion:          152100000000
    // Perihelion:        147095000000
    double Distance = ModelConstants.DistanceToSun;
    double PrevDistance = ModelConstants.PrevDistanceToSun;

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

    // See AddEarth().
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

    Earth.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\earth.jpg";
    AddSpaceObject( Earth );
    }




  private void AddMoon()
    {
    // https://en.wikipedia.org/wiki/Moon

    // https://theskylive.com/moon-info
    Moon = new PlanetSphere( MForm );

    // Radius: About 1,737.1 kilometers.
    Moon.Radius = 1737100;

    double RightAscension = MoonRightA;
    double Declination = MoonDecl;

    double Distance = ModelConstants.DistanceToMoon;

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
    Mars = new PlanetSphere( MForm );

    Mars.Radius = 3396000 * RadiusScale;

    double RightAscension = MarsRightA;
    double Declination = MarsDecl;

    double Distance = ModelConstants.DistanceToMars;

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
    Mercury = new PlanetSphere( MForm );

    //               m  t
    Mercury.Radius = 2440000d * RadiusScale;

    double RightAscension = MercuryRightA;
    double Declination = MercuryDecl;

    double Distance = ModelConstants.DistanceToMercury;

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
    Venus.Radius = 6051000 * RadiusScale; // 6,051 km

    double RightAscension = VenusRightA;
    double Declination = VenusDecl;

    double Distance = ModelConstants.DistanceToVenus;

    Venus.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Venus.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Venus.Position.Z = Distance * Math.Sin( Declination );

    Venus.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Venus.jpg";
    AddSpaceObject( Venus );
    }



  private void AddJupiter()
    {
    Jupiter = new PlanetSphere( MForm );

    //                m  t
    Jupiter.Radius = 69911000d * RadiusScale; // 69,911 km

    double RightAscension = JupiterRightA;
    double Declination = JupiterDecl;

    double Distance = ModelConstants.DistanceToJupiter;

    Jupiter.Position.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Jupiter.Position.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Jupiter.Position.Z = Distance * Math.Sin( Declination );

    Jupiter.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Jupiter.jpg";
    AddSpaceObject( Jupiter );
    }




  private void AddSaturn()
    {
    Saturn = new PlanetSphere( MForm );

    //               m  t
    Saturn.Radius = 58232000d * RadiusScale; // 58,232 km

    double RightAscension = SaturnRightA;
    double Declination = SaturnDecl;

    double Distance = ModelConstants.DistanceToSaturn;

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
