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
    PCamera.FarPlaneDistance = 1000;
    PCamera.NearPlaneDistance = 1;
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


