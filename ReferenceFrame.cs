// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// This is a container for SpaceObjects, and also
// it sets objects in a reference frame.

// Vernal: "of, relating to, or occurring in the
// spring."  "fresh or new like the spring"

// If X is zero then it points to where the sun is
// at the Spring Equinox.
// If you are looking down the Z axis (above the
// top of the north pole) the X axis points to the
// right and the Y axis points up, just like any
// normal right-handed coordinate system.

// Pretty much everything rotates or orbits
// counterclockwise when looking down the Z axis.
// (From above the north pole.)


// Validity checks:
// Check on the center of mass of all the objects.
// Does it move?


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
  // private PlanetSphere Mercury;
  private PlanetSphere Venus;
  private PlanetSphere Earth;
  private PlanetSphere Moon;
  // private PlanetSphere Mars;
  // private PlanetSphere Jupiter;
  // private PlanetSphere Saturn;
  private PointLight PLight1;

  // Edit these all in one place:
  // https://theskylive.com/sun-info

  // There are values for RightAscension and
  // Declination and the Previous numbers for those
  // points, which means the previous day's values
  // or the previous minute's values, or whatever
  // time unit you're using.

  //                                                               Sun
  private double SunRightA = NumbersEC.RightAscensionToRadians( 23, 59, 15 );
  private double SunDecl = NumbersEC.DegreesMinutesToRadians( 0, 4, 44 );
  private double SunRightAPrev = NumbersEC.RightAscensionToRadians( 23, 55, 47 );
  private double SunDeclPrev = NumbersEC.DegreesMinutesToRadians( 0, 27, 18 );
  //                                                                     Moon
  private double MoonRightA = NumbersEC.RightAscensionToRadians( 2, 33, 15 );
  private double MoonDecl = NumbersEC.DegreesMinutesToRadians( 9, 39, 3 );
  private double MoonRightAPrev = NumbersEC.RightAscensionToRadians( 1, 43, 55 );
  private double MoonDeclPrev = NumbersEC.DegreesMinutesToRadians( 5, 30, 33 );
  //                                                                     Venus
  private double VenusRightA = NumbersEC.RightAscensionToRadians( 1, 3, 20 );
  private double VenusDecl = NumbersEC.DegreesMinutesToRadians( 5, 46, 38 );

  // Change the scale to help with testing and
  // visualizing it.
  private double DrawDistanceScale = 0.5;



  private ReferenceFrame()
    {
    }


  internal ReferenceFrame( MainForm UseForm,
                           Model3DGroup Use3DGroup )
    {
    MForm = UseForm;

    Main3DGroup = Use3DGroup;

    SpaceObjectArray = new SpaceObject[2];
    AddInitialSpaceObjects();
    }


  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  private void AddInitialSpaceObjects()
    {
    // https://theskylive.com/planets

    // Do these two first in order to set up the
    // coordinate system.
    AddSun();
    AddEarth();

    // AddMercury();
    AddVenus();
    AddMoon();
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

    // Lights are Model3D objects.
    // System.Windows.Media.Media3D.Model3D
    //   System.Windows.Media.Media3D.Light

    SetupAmbientLight( 0x3F, 0x3F, 0x3F );
    SetupPointLight();
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

    // Lights are Model3D objects.
    // System.Windows.Media.Media3D.Model3D
    //   System.Windows.Media.Media3D.Light

    SetupAmbientLight( 0x3F, 0x3F, 0x3F );
    SetupPointLight();
    }



  private void SetupPointLight()
    {
    PLight1 = new PointLight();
    PLight1.Color = System.Windows.Media.Colors.White;

    Point3D Location = new  Point3D( Sun.X, Sun.Y, Sun.Z );
    PLight1.Position = Location;
    PLight1.Range = 100000000.0;

    // Attenuation with distance D is like:
    // Attenuation = C + L*D + Q*D^2
    PLight1.ConstantAttenuation = 1;
    // PLight.LinearAttenuation = 1;
    // PLight.QuadraticAttenuation = 1;

    Main3DGroup.Children.Add( PLight1 );
    }



  private void SetupAmbientLight( byte Red,
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

    Sun = new PlanetSphere( MForm, DrawDistanceScale );

    // Radius: About 695,700 kilometers.
    Sun.Radius = 695.7;

    // Sun.Mass;

    // "March equinox 2018 will be at 10:15 AM
    // Mountain time on Tuesday, March 20".

    // Right Ascension:
    // https://en.wikipedia.org/wiki/Right_ascension

    // In Radians:
    double RightAscension = SunRightA;
    double Declination = SunDecl;

    // https://en.wikipedia.org/wiki/Astronomical_unit
    // "a maximum (aphelion) to
    //  a minimum (perihelion)"
    // One Astronomical unit is the distance from
    // the earth to the sun.  It's about
    // 149,597,870,700 meters or 149,597,870.7 km
    //                Radius of Sun: 695,700 km.

    double Distance = 148993; // 149597.8;
    double PrevDistance = 148993;

    // Notice that if the Declination and the Right
    // Ascension are both zero, which is what they
    // are at the Spring Equinox, then the Cosines
    // are 1, so this is = Distance * 1 * 1.
    // Sun.X = Distance * (Math.Cos( 0 ) * Math.Cos( 0 ));
    // In other words the X axis points along where
    // both Right Ascension and Declination are both
    // zero.
    Sun.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));

    // For Right Ascension of 90 degrees (or 6
    // hours), The Sine is 1, so the Y axis is along
    // where R.A. is at 6 hours and Decliniation
    // is zero.
    Sun.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));

    // The Z axis is along where Declination is at
    // 90 degrees, which is through the North Pole.
    Sun.Z = Distance * Math.Sin( Declination );

    Sun.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\sun.jpg";

    double XPrev = PrevDistance * (Math.Cos( SunDeclPrev ) * Math.Cos( SunRightAPrev ));
    double YPrev = PrevDistance * (Math.Cos( SunDeclPrev ) * Math.Sin( SunRightAPrev ));
    double ZPrev = PrevDistance * Math.Sin( SunDeclPrev );

    // Per one unit of time.
    Sun.VelocityX = Sun.X - XPrev;
    Sun.VelocityY = Sun.Y - YPrev;
    Sun.VelocityZ = Sun.Z - ZPrev;

    AddSpaceObject( Sun );

    // The sun has about 99.8 percent of the mass
    // of the Solar System.
    }




  private void AddEarth()
    {
    // https://en.wikipedia.org/wiki/Earth

    // Make Earth an EarthGeoid, not a PlanetSphere.
    Earth = new PlanetSphere( MForm, DrawDistanceScale );

    // Radius: About 6,371.0 kilometers.
    Earth.Radius = 6.371;


    // Shift the time of day:
    // If I make this 3 then the Earth rotates to
    // the east by 3 hours.  (The sun moves to the
    // west three hours toward sunset.)
    // On March 20th, at the Spring Equinox,
    // the sun is straight up above Greenwich
    // England at longitude zero.  (When the shift
    // hours is zero.)

    Earth.LongitudeHours = 0;

    Earth.X = 0;
    Earth.Y = 0;
    Earth.Z = 0;

    // The Earth has almost a circular orbit.
    // Going at about 30 kilometers per second.

    // Earth's orbit is about 149,597.8 thousand
    // kilometers for its radius.
    // 2 * Pi times this radius is the distance
    // the earth covers in about a year.
    // So that circumferance divided by 365 days
    // gives the velocity in kilometers per day.

    // Make it so the Earth is moving in the opposite
    // direction and the sun's velocity is zero in
    // this coordinate system.
    Earth.VelocityX = -Sun.VelocityX;
    Earth.VelocityY = -Sun.VelocityY;
    Earth.VelocityZ = -Sun.VelocityZ;

    // Normalize this velocity vector and then
    // multiply it by a scalar velocity.

    Sun.VelocityX = 0;
    Sun.VelocityY = 0;
    Sun.VelocityZ = 0;

    ShowStatus( "Earth Velocity per unit of time." );
    ShowStatus( "Earth VelocityX: " + Earth.VelocityX.ToString( "N2" ));
    ShowStatus( "Earth VelocityY: " + Earth.VelocityY.ToString( "N2" ));
    ShowStatus( "Earth VelocityZ: " + Earth.VelocityZ.ToString( "N2" ));

    Earth.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\earth.jpg";
    AddSpaceObject( Earth );
    }




  private void AddMoon()
    {
    // https://en.wikipedia.org/wiki/Moon

    // https://theskylive.com/moon-info
    Moon = new PlanetSphere( MForm, DrawDistanceScale );

    // Radius: About 1,737.1 kilometers.
    Moon.Radius = 1.7371;

    double RightAscension = MoonRightA;
    double Declination = MoonDecl;

    double Distance = 380; // 362.6;

    Moon.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Moon.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Moon.Z = Distance * Math.Sin( Declination );

    // Moon.X = 362.6;
    // Moon.Y = 0;
    // Moon.Z = 0;
    Moon.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\moon.jpg";
    // Moon.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Earth.jpg";
    AddSpaceObject( Moon );
    }



  /*
  private void AddMars()
    {
    try
    {
    ShowStatus( " " );
    ShowStatus( "Adding Mars:" );

    Mars = new PlanetSphere( MForm, DrawDistanceScale );

    // Radius in thousands of kilometers.
    Mars.Radius = 3.396 * 100;

    double RightAscension = MarsRightA;
    double Declination = MarsDecl;


    ///////////
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

    double EarthMarsDistance = GetEarthPlanetDistance(
                   SunMarsAngle, // As seen from Earth.
                   DistanceMarsToSun,
                   DistanceEarthToSun );

    double Distance = EarthMarsDistance;
    /////////////


    double Distance = 181532;

    Mars.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Mars.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Mars.Z = Distance * Math.Sin( Declination );

    Mars.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\mars.jpg";
    AddSpaceObject( Mars );

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ReferenceFrame.AddMars(): " + Except.Message );
      }
    }
    */



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



  /*
  private void AddMercury()
    {
    // https://en.wikipedia.org/wiki/Mercury_(planet)

    try
    {
    ShowStatus( " " );
    ShowStatus( "Adding Mercury:" );

    Mercury = new PlanetSphere( MForm, DrawDistanceScale );

    // Radius in thousands of kilometers.
    Mercury.Radius = 2.440 * 100;

    double RightAscension = MercuryRightA;
    double Declination = MercuryDecl;

    double Distance = 116291;

    Mercury.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Mercury.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Mercury.Z = Distance * Math.Sin( Declination );

    Mercury.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Mercury.jpg";
    AddSpaceObject( Mercury );

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ReferenceFrame.AddMercury(): " + Except.Message );
      }
    }
    */



  private void AddVenus()
    {
    Venus = new PlanetSphere( MForm, DrawDistanceScale );

    // Radius in thousands of kilometers.
    Venus.Radius = 6.051 * 100; // 6,051 km

    double RightAscension = VenusRightA;
    double Declination = VenusDecl;

    double Distance = 241481;

    Venus.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Venus.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Venus.Z = Distance * Math.Sin( Declination );

    Venus.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Venus.jpg";
    AddSpaceObject( Venus );
    }


  /*
  private void AddJupiter()
    {
    Jupiter = new PlanetSphere( MForm, DrawDistanceScale );

    // Radius in thousands of kilometers.
    Jupiter.Radius = 69.911 * 100; // 69,911 km

    double RightAscension = JupiterRightA;
    double Declination = JupiterDecl;

    double Distance = 712122;

    Jupiter.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Jupiter.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Jupiter.Z = Distance * Math.Sin( Declination );

    Jupiter.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Jupiter.jpg";
    AddSpaceObject( Jupiter );
    }
    */


  /*
  private void AddSaturn()
    {
    Saturn = new PlanetSphere( MForm, DrawDistanceScale );

    // Radius in thousands of kilometers.
    Saturn.Radius = 58.232 * 100; // 58,232 km

    double RightAscension = SaturnRightA;
    double Declination = SaturnDecl;

    double Distance = 1520373;

    Saturn.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Saturn.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Saturn.Z = Distance * Math.Sin( Declination );

    Saturn.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Saturn.jpg";
    AddSpaceObject( Saturn );
    }
    */



  internal void DoTimeStep()
    {
    // MForm.ShowStatus( "DoTimeStep() in RefFrame." );

    Earth.LongitudeHours = Earth.LongitudeHours + 0.5;
    Earth.MakeNewGeometryModel();
    ResetGeometryModels();
    // MakeNewGeometryModels();
    }



  }
}
