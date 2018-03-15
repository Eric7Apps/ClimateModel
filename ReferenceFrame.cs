// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// This is a container for SpaceObjects, and also
// it sets objects in a reference frame.

// Vernal: "of, relating to, or occurring in the
// spring."  "fresh or new like the spring"

// International Celestial Reference Frame:
// https://en.wikipedia.org/wiki/International_Celestial_Reference_Frame

// Equatorial Coordinate System:
// https://en.wikipedia.org/wiki/Equatorial_coordinate_system

// https://en.wikipedia.org/wiki/Solar_System
// https://en.wikipedia.org/wiki/Equinox


// The X axis points along the Vernal Equinox.
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


// https://en.wikipedia.org/wiki/Theory_of_tides
// https://en.wikipedia.org/wiki/Celestial_mechanics
// https://en.wikipedia.org/wiki/Continuum_mechanics
// https://en.wikipedia.org/wiki/Orbital_mechanics
// https://en.wikipedia.org/wiki/Epoch_(astronomy)
// https://en.wikipedia.org/wiki/Celestial_coordinate_system
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
  private SpaceObject[] SpaceObjectArray;
  private int SpaceObjectArrayLast = 0;
  private PlanetSphere Earth;
  private PlanetSphere Moon;
  private PlanetSphere Sun;
  private PointLight PLight1;



  private ReferenceFrame()
    {
    }


  internal ReferenceFrame( MainForm UseForm )
    {
    MForm = UseForm;

    SpaceObjectArray = new SpaceObject[2];
    AddInitialSpaceObjects();
    }



  private void AddInitialSpaceObjects()
    {
    // https://theskylive.com/planets

    AddSun();
    AddEarth();
    AddMoon();
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



  internal void MakeGeometryModels( Model3DGroup Main3DGroup )
    {
    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      GeometryModel3D GeoMod = SpaceObjectArray[Count].MakeGeometryModel();
      if( GeoMod == null )
        continue;

      Main3DGroup.Children.Add( GeoMod );
      }

    // The Light is a Model3D.
    // System.Windows.Media.Media3D.Model3D
    //   System.Windows.Media.Media3D.Light

    // Light for Earth.
    PLight1 = new PointLight();
    PLight1.Color = System.Windows.Media.Colors.White;

    Point3D Location = new  Point3D( Sun.X, Sun.Y, Sun.Z );
    PLight1.Position = Location;
    PLight1.Range = 10000.0;

    // Attenuation with distance D is like:
    // Attenuation = C + L*D + Q*D^2
    PLight1.ConstantAttenuation = 1;
    // PLight.LinearAttenuation = 1;
    // PLight.QuadraticAttenuation = 1;

    Main3DGroup.Children.Add( PLight1 );
    }



  private void AddSun()
    {
    // https://theskylive.com/sun-info
    // https://en.wikipedia.org/wiki/Sun

    Sun = new PlanetSphere( MForm );

    // Radius: About 695,700 kilometers.
    Sun.Radius = 695.7;

    // Sun.Mass;

    // "March equinox 2018 will be at 10:15 AM
    // Mountain time on Tuesday, March 20".

    // Right Ascension:
    // https://en.wikipedia.org/wiki/Right_ascension

    double RightAscension = NumbersEC.RightAscensionToRadians( 23, 38, 20 );
    double Declination = NumbersEC.DegreesMinutesToRadians( -2, 20, 30 );

    // For testing:
    double Distance = 5000;

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
    AddSpaceObject( Sun );

    // The sun has about 99.8 percent of the mass
    // of the Solar System.
    }




  private void AddEarth()
    {
    // https://en.wikipedia.org/wiki/Earth

    // Make Earth an EarthGeoid, not a PlanetSphere.
    Earth = new PlanetSphere( MForm );

    // Radius: About 6,371.0 kilometers.
    Earth.Radius = 6.371;

    // Shift the time of day:
    Earth.LongitudeShiftHours = 0;

    Earth.X = 0;
    Earth.Y = 0;
    Earth.Z = 0;
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

    double RightAscension = NumbersEC.RightAscensionToRadians( 21, 48, 30 );
    double Declination = NumbersEC.DegreesMinutesToRadians( -14, 13, 18 );

    // For testing to visualize some things.
    double Distance = 30; // 362.6;

    Moon.X = Distance * (Math.Cos( Declination ) * Math.Cos( RightAscension ));
    Moon.Y = Distance * (Math.Cos( Declination ) * Math.Sin( RightAscension ));
    Moon.Z = Distance * Math.Sin( Declination );

    // Moon.X = 362.6;
    // Moon.Y = 0;
    // Moon.Z = 0;
    Moon.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\moon.jpg";
    AddSpaceObject( Moon );
    }



  }
}
