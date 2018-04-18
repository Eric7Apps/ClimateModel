// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// This is any object in space.  A planet, space
// ship, the Sun, or whatever.


using System;
// using System.Collections.Generic;
using System.Text;
// using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;


namespace ClimateModel
{
  abstract class SpaceObject
  {
  internal Vector3.Vector Position;
  internal Vector3.Vector Velocity;
  internal Vector3.Vector Acceleration;
  internal double Mass;
  // The time as seen from this local system.
  // internal double LocalClock;


  abstract internal void MakeNewGeometryModel();

  abstract internal GeometryModel3D GetGeometryModel();



  internal void SetNextPositionFromVelocity(
                                  double TimeDelta )
    {
    Vector3.Vector MoveBy = new Vector3.Vector();
    Vector3.Copy( ref MoveBy, ref Velocity );
    // It moves by this much in TimeDelta time.
    Vector3.MultiplyWithScalar( ref MoveBy, TimeDelta );
    Vector3.Add( ref Position, ref MoveBy );
    }



  }
}
