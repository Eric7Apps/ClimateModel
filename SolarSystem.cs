// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com



// https://en.wikipedia.org/wiki/International_Celestial_Reference_Frame
// https://en.wikipedia.org/wiki/Celestial_coordinate_system
// https://en.wikipedia.org/wiki/Equatorial_coordinate_system
// https://en.wikipedia.org/wiki/Ecliptic_coordinate_system

// This will replace FeferenceFrame.cs, but keep
// them both going for a while and see them both
// at the same time.  Are the planets close?  
// Overlapping each other?

// Versor:
// Quaternion rotation.
// Rotate the planets around the X axis by about 23
// degrees to put them in the ecliptic plane.

// A planet sweeps out equal areas in equal times.
// The area (triangle) is approximately half the
// cross product between the two vectors.  So it's
// approximately "sweeps out equal cross products
// in equal times."


// Start with just basic circles.
// It's no accident that 365 is close to 360 (degrees).
// Period of orbit is 365.25 days.
// AnglePerDay = 365.25 / 360;
// AnglePerHour = AnglePerDay / 24;

// Get the positions of planets as a function of time.
// Vector3.Vector SoAndSo =
             GetEarthPositionVector( ECTime TheTime );


// Find the barycenter of the solar
// system.  Make ellipses. 

// Earth's Orbit:
// https://en.wikipedia.org/wiki/Earth%27s_orbit

// Moon orbit:
// https://en.wikipedia.org/wiki/Orbit_of_the_Moon

// https://en.wikipedia.org/wiki/Jupiter#Orbit_and_rotation

// Calculate it as if the Moon and Earth are one
// point at the center of their mass:
// ModelConstants.MassOfEarthPlusMoon


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
  private double RadiusScale = 300.0;
  private PlanetSphere Sun;
  private PlanetSphere Mercury;
  private PlanetSphere Venus;
  private EarthGeoid Earth;
  private PlanetSphere Moon;
  private PlanetSphere Mars;
  private PlanetSphere Jupiter;
  private PlanetSphere Saturn;



  }
}

