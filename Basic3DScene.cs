// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
// using System.Collections.Generic;
using System.Text;
// using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

// For testing.
// using System.Windows.Forms;


namespace ClimateModel
{
  class ThreeDScene
  {
  private MainForm MForm;
  private PerspectiveCamera PCamera = new PerspectiveCamera();
  // private PointLight SunLight = new PointLight();
  private AmbientLight SunLight = new AmbientLight();
  private Model3DGroup Main3DGroup = new Model3DGroup();
  private ModelVisual3D MainModelVisual3D = new ModelVisual3D();



  private ThreeDScene()
    {
    }



  internal ThreeDScene( MainForm UseForm )
    {
    try
    {
    MForm = UseForm;

    SetupScene();
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeScene constructor: " + Except.Message );
      return;
      }
    }



  internal PerspectiveCamera GetCamera()
    {
    return PCamera;
    }



  private void SetupScene()
    {
    try
    {
    SetupCamera();
    SetupSunlight();
    MakeSurface();

    MainModelVisual3D.Content = Main3DGroup;

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDScene.SetupScene(): " + Except.Message );
      return;
      }
    }



  internal ModelVisual3D GetMainModelVisual3D()
    {
    return MainModelVisual3D;
    }



  private void SetupCamera()
    {
    // Positive Z values go toward the viewer.
    PCamera.Position = new Point3D( 0, 0, 10 );
    PCamera.LookDirection = new Vector3D( 0, 0, -1 );
    PCamera.UpDirection = new Vector3D( 0, 1, 0 );
    PCamera.FieldOfView = 60;
    // Clipping planes:
    PCamera.FarPlaneDistance = 10000;
    PCamera.NearPlaneDistance = 1;
    }



  internal void MoveForwardBack( double HowFar )
    {
    Vector3D LookAt = PCamera.LookDirection;
    Point3D Position = PCamera.Position;
    Vector3D MoveBy = new Vector3D();
    MoveBy = Vector3D.Multiply( HowFar, LookAt );
    Point3D MoveTo = new Point3D();
    MoveTo = Point3D.Add( Position, MoveBy );
    PCamera.Position = MoveTo;
    }



  internal void MoveLeftRight( double Angle )
    {
    Vector3D LookDirection = PCamera.LookDirection;
    Vector3D UpDirection = PCamera.UpDirection;

    QuaternionEC.QuaternionRec Axis = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec StartPoint = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec MiddlePoint = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec ResultPoint = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec RotationQ = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec InverseRotationQ = new QuaternionEC.QuaternionRec();

    Axis.X = UpDirection.X;
    Axis.Y = UpDirection.Y;
    Axis.Z = UpDirection.Z;
    Axis.W = 0;

    StartPoint.X = LookDirection.X;
    StartPoint.Y = LookDirection.Y;
    StartPoint.Z = LookDirection.Z;
    StartPoint.W = 0;

    QuaternionEC.SetAsRotation( ref RotationQ,
                                ref Axis,
                                Angle );

    QuaternionEC.Inverse( ref InverseRotationQ, ref RotationQ );

    QuaternionEC.Rotate( ref ResultPoint,
                         ref RotationQ,
                         ref InverseRotationQ,
                         ref StartPoint,
                         ref MiddlePoint );

    LookDirection.X = ResultPoint.X;
    LookDirection.Y = ResultPoint.Y;
    LookDirection.Z = ResultPoint.Z;
    PCamera.LookDirection = LookDirection;
    }



  // For Yaw, Pitch and Roll, this is Roll.
  internal void RotateLeftRight( double Angle )
    {
    Vector3D LookDirection = PCamera.LookDirection;
    Vector3D UpDirection = PCamera.UpDirection;

    QuaternionEC.Vector3 Up = new QuaternionEC.Vector3();
    QuaternionEC.Vector3 ResultPoint = new QuaternionEC.Vector3();

    QuaternionEC.QuaternionRec Axis = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec MiddlePoint = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec RotationQ = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec InverseRotationQ = new QuaternionEC.QuaternionRec();

    Axis.X = LookDirection.X;
    Axis.Y = LookDirection.Y;
    Axis.Z = LookDirection.Z;
    Axis.W = 0;

    Up.X = UpDirection.X;
    Up.Y = UpDirection.Y;
    Up.Z = UpDirection.Z;

    QuaternionEC.SetAsRotation( ref RotationQ,
                                ref Axis,
                                Angle );

    QuaternionEC.Inverse( ref InverseRotationQ, ref RotationQ );

    QuaternionEC.RotateVector3( ref ResultPoint,
                         ref RotationQ,
                         ref InverseRotationQ,
                         ref Up,
                         ref MiddlePoint );

    UpDirection.X = ResultPoint.X;
    UpDirection.Y = ResultPoint.Y;
    UpDirection.Z = ResultPoint.Z;
    PCamera.UpDirection = UpDirection;
    }



  internal void MoveUpDown( double Angle )
    {
    Vector3D LookDirection = PCamera.LookDirection;
    Vector3D UpDirection = PCamera.UpDirection;

    QuaternionEC.QuaternionRec Cross = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec Look = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec Up = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec StartPoint = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec MiddlePoint = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec ResultPoint = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec RotationQ = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec InverseRotationQ = new QuaternionEC.QuaternionRec();

    Look.X = LookDirection.X;
    Look.Y = LookDirection.Y;
    Look.Z = LookDirection.Z;
    Look.W = 0;

    Up.X = UpDirection.X;
    Up.Y = UpDirection.Y;
    Up.Z = UpDirection.Z;
    Up.W = 0;

    // X Cross Y = Z.  The Right-hand rule.

    QuaternionEC.CrossProduct( ref Cross, 
                               ref Look,
                               ref Up );

    QuaternionEC.SetAsRotation( ref RotationQ,
                                ref Cross,
                                Angle );

    QuaternionEC.Inverse( ref InverseRotationQ, ref RotationQ );

    /////////////////
    // Rotate Up around Cross.
    StartPoint.X = Up.X;
    StartPoint.Y = Up.Y;
    StartPoint.Z = Up.Z;
    StartPoint.W = 0;

    QuaternionEC.Rotate( ref ResultPoint,
                         ref RotationQ,
                         ref InverseRotationQ,
                         ref StartPoint,
                         ref MiddlePoint );

    UpDirection.X = ResultPoint.X;
    UpDirection.Y = ResultPoint.Y;
    UpDirection.Z = ResultPoint.Z;
    PCamera.UpDirection = UpDirection;

    /////////////////
    // Rotate Look around Cross.
    StartPoint.X = Look.X;
    StartPoint.Y = Look.Y;
    StartPoint.Z = Look.Z;
    StartPoint.W = 0;

    QuaternionEC.Rotate( ref ResultPoint,
                         ref RotationQ,
                         ref InverseRotationQ,
                         ref StartPoint,
                         ref MiddlePoint );

    LookDirection.X = ResultPoint.X;
    LookDirection.Y = ResultPoint.Y;
    LookDirection.Z = ResultPoint.Z;
    PCamera.LookDirection = LookDirection;
    }



  private void MakeSurface()
    {
    try
    {
    // Make a triangle surface.
    MeshGeometry3D TriSurface = new MeshGeometry3D();

    TriSurface.Positions.Add( new Point3D( 0, 0, 0 ));
    TriSurface.Positions.Add( new Point3D( 1.0, 0, 0 ));
    TriSurface.Positions.Add( new Point3D( 0, 1.0, 0 ));

    // Counterclockwise winding goes toward the viewer.
    TriSurface.TriangleIndices.Add( 0 );
    TriSurface.TriangleIndices.Add( 1 );
    TriSurface.TriangleIndices.Add( 2 );

    // Vector3D.CrossProduct()

    // Positive Z values go toward the viewer.
    // So the normal is toward the viewer.
    TriSurface.Normals.Add( new Vector3D( 0, 0, 1 ));
    TriSurface.Normals.Add( new Vector3D( 0, 0, 1 ));
    TriSurface.Normals.Add( new Vector3D( 0, 0, 1 ));

    TriSurface.TextureCoordinates.Add( new Point( 1, 0 ));
    TriSurface.TextureCoordinates.Add( new Point( 1, 1 ));
    TriSurface.TextureCoordinates.Add( new Point( 0, 1 ));

    DiffuseMaterial SolidMat = new DiffuseMaterial();
    SolidMat.Brush = Brushes.Blue;

    GeometryModel3D GeoMod = new GeometryModel3D();
    GeoMod.Geometry = TriSurface;
    GeoMod.Material = SolidMat;

    Main3DGroup.Children.Add( GeoMod );

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDScene.MakeCube(): " + Except.Message );
      return;
      }
    }



  private void SetupSunlight()
    {
    // Positive Z values go toward the viewer.


    // SunLight.Position = new Point3D( 0, 0, 20 );
    SunLight.Color = System.Windows.Media.Brushes.White.Color;
    // SunLight.Range = 15.0;
    // SunLight.ConstantAttenuation = 3.0;
    // SunLight.LinearAttenuation = 3.0;
    // SunLight.QuadraticAttenuation = 2.0;

    Main3DGroup.Children.Add( SunLight );
    }




  }
}


