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
using System.Windows.Media.Imaging;

// For testing.
// using System.Windows.Forms;


namespace ClimateModel
{
  class ThreeDScene
  {
  private MainForm MForm;
  private PerspectiveCamera PCamera = new PerspectiveCamera();
  private Model3DGroup Main3DGroup = new Model3DGroup();
  private ModelVisual3D MainModelVisual3D = new ModelVisual3D();
  internal ReferenceFrame RefFrame;




  private ThreeDScene()
    {
    }



  internal ThreeDScene( MainForm UseForm )
    {
    try
    {
    MForm = UseForm;

    RefFrame = new ReferenceFrame( MForm, Main3DGroup );
    SetupCamera();
    MainModelVisual3D.Content = Main3DGroup;
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



  internal ModelVisual3D GetMainModelVisual3D()
    {
    return MainModelVisual3D;
    }



  internal void SetCameraTo( double X,
                             double Y,
                             double Z,
                             double LookX,
                             double LookY,
                             double LookZ,
                             double UpX,
                             double UpY,
                             double UpZ )
    {
    PCamera.Position = new Point3D( X, Y, Z );
    PCamera.LookDirection = new Vector3D( LookX, LookY, LookZ );
    PCamera.UpDirection = new Vector3D( UpX, UpY, UpZ );
    }



  internal void SetCameraToOriginal()
    {
    PCamera.Position = new Point3D( 0, -15, 0 );
    PCamera.LookDirection = new Vector3D( 0, 1, 0 );
    PCamera.UpDirection = new Vector3D( 0, 0, 1 );
    }



  private void SetupCamera()
    {
    // Positive Z values go toward the viewer.
    PCamera.Position = new Point3D( 0, -15, 0 );
    PCamera.LookDirection = new Vector3D( 0, 1, 0 );
    PCamera.UpDirection = new Vector3D( 0, 0, 1 );
    PCamera.FieldOfView = 60;
    // Clipping planes:
    // Too much of a range for clipping will cause
    // problems with the Depth buffer.
    // Saturn is at about 1,498,032 thousand
    // kilometers from Earth.  And 1,505,795 from
    // the sun.  Twice the radius of Saturn's
    // orbit: 1,600,000 * 2.
    PCamera.FarPlaneDistance = 1600000 * 2;
    PCamera.NearPlaneDistance = 0.5;
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

    Vector3.Vector Up = new Vector3.Vector();
    Vector3.Vector ResultPoint = new Vector3.Vector();

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



  internal void ShiftLeftRight( double HowFar )
    {
    Vector3D LookDirection = PCamera.LookDirection;
    Vector3D UpDirection = PCamera.UpDirection;

    QuaternionEC.QuaternionRec Cross = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec Look = new QuaternionEC.QuaternionRec();
    QuaternionEC.QuaternionRec Up = new QuaternionEC.QuaternionRec();

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

    Vector3D CrossVect = new Vector3D();
    CrossVect.X = Cross.X;
    CrossVect.Y = Cross.Y;
    CrossVect.Z = Cross.Z;

    Point3D Position = PCamera.Position;
    Vector3D MoveBy = new Vector3D();
    Point3D MoveTo = new Point3D();

    MoveBy = Vector3D.Multiply( HowFar, CrossVect );
    MoveTo = Point3D.Add( Position, MoveBy );
    PCamera.Position = MoveTo;
    }



  internal void ShiftUpDown( double HowFar )
    {
    Vector3D UpDirection = PCamera.UpDirection;

    Point3D Position = PCamera.Position;
    Vector3D MoveBy = new Vector3D();
    Point3D MoveTo = new Point3D();

    MoveBy = Vector3D.Multiply( HowFar, UpDirection );
    MoveTo = Point3D.Add( Position, MoveBy );
    PCamera.Position = MoveTo;
    }



/*
  private void MakeSurface()
    {
    try
    {
    MeshGeometry3D TriSurface = new MeshGeometry3D();

    // Texture Coordinates:
    // https://msdn.microsoft.com/en-us/library/system.windows.media.media3d.meshgeometry3d.texturecoordinates(v=vs.110).aspx

    // For texture coordinates in 2D "Bitmap image
    // space", so to speak, Y starts at the top and
    // goes downward.
    Point TopLeft = new Point( 0, 0 );
    Point BottomRight = new Point( 1, 1 );
    Point TopRight = new Point( 1, 0 );
    Point BottomLeft = new Point( 0, 1 );

    // In this 3D space Y goes upward.
    Point3D TopLeft3D = new Point3D( 0, 1, 0 );
    Point3D BottomRight3D = new Point3D( 1, 0, 0 );
    Point3D TopRight3D = new Point3D( 1, 1, 0 );
    Point3D BottomLeft3D = new Point3D( 0, 0, 0 );

    TriSurface.Positions.Add( BottomLeft3D );
    TriSurface.Positions.Add( BottomRight3D );
    TriSurface.Positions.Add( TopLeft3D );
    TriSurface.Positions.Add( TopRight3D );

    TriSurface.TextureCoordinates.Add( BottomLeft );
    TriSurface.TextureCoordinates.Add( BottomRight );
    TriSurface.TextureCoordinates.Add( TopLeft );
    TriSurface.TextureCoordinates.Add( TopRight );

    // Counterclockwise winding goes toward the viewer.
    TriSurface.TriangleIndices.Add( 0 );
    TriSurface.TriangleIndices.Add( 1 );
    TriSurface.TriangleIndices.Add( 2 );

    TriSurface.TriangleIndices.Add( 1 );
    TriSurface.TriangleIndices.Add( 3 );
    TriSurface.TriangleIndices.Add( 2 );

    // Positive Z values go toward the viewer.
    // So the normal is toward the viewer.
    Vector3D BasicNormal = new Vector3D( 0, 0, 1 );
    TriSurface.Normals.Add( BasicNormal );
    TriSurface.Normals.Add( BasicNormal );
    TriSurface.Normals.Add( BasicNormal );
    TriSurface.Normals.Add( BasicNormal );

    DiffuseMaterial SolidMat = new DiffuseMaterial();
    // SolidMat.Brush = Brushes.Blue;
    SolidMat.Brush = SetTextureImageBrush();

    GeometryModel3D GeoMod = new GeometryModel3D();
    GeoMod.Geometry = TriSurface;
    GeoMod.Material = SolidMat;

    Main3DGroup.Children.Add( GeoMod );

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDScene.MakeSurface(): " + Except.Message );
      return;
      }
    }
*/


/*
  private ImageBrush SetTextureImageBrush()
    {
    // Imaging Overview:
    // https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/imaging-overview

    // Imaging Namespace:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging?view=netframework-4.7.1

    // ImageDrawing:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imagedrawing?view=netframework-4.7.1

    BitmapImage BMapImage = new BitmapImage();

    // Things have to be in this Begin-end block.
    BMapImage.BeginInit();

    BMapImage.UriSource = new Uri( "C:\\Eric\\ClimateModel\\bin\\Release\\earth.jpg" );
    // BMapImage.UriSource = new Uri( "C:\\Eric\\ClimateModel\\bin\\Release\\TestImage.jpg" );

    // BMapImage.DecodePixelWidth = 200;

    BMapImage.EndInit();

    // ImageBrush:
    // https://msdn.microsoft.com/en-us/library/system.windows.media.imagebrush(v=vs.110).aspx
    ImageBrush ImgBrush = new ImageBrush();
    ImgBrush.ImageSource = BMapImage;
    return ImgBrush;
    }
*/


  internal void DoTimeStep()
    {
    RefFrame.DoTimeStep();
    }


  }
}
