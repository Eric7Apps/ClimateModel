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
  internal double X;
  internal double Y;
  internal double Z;
  internal double VelocityX;
  internal double VelocityY;
  internal double VelocityZ;

  // internal double Mass;



  abstract internal void MakeNewGeometryModel();

  abstract internal GeometryModel3D GetGeometryModel();



  }
}
