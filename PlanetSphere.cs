// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com



using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;



namespace ClimateModel
{
  class PlanetSphere : SpaceObject
  {
  private MainForm MForm;
  private ThreeDSphere GeometrySphere;
  internal string TextureFileName = "";
  internal double Radius = 1;
  private MeshGeometry3D Surface;
  private GeometryModel3D GeoMod;
  private double DistanceScale = 0.03;
  internal double LongitudeHours = 0; // Time change.


  /*
  public struct LatLongRec
    {
    public double Latitude;
    public double Longitude;
    // public double Radius;
    // public double Elevation;
    public int TriIndex;
    }
*/



  private PlanetSphere()
    {
    }



  internal PlanetSphere( MainForm UseForm,
                    double UseDistanceScale )
    {
    MForm = UseForm;
    DistanceScale = UseDistanceScale;

    GeoMod = new GeometryModel3D();
    GeometrySphere = new ThreeDSphere( MForm, DistanceScale );
    }


  internal override GeometryModel3D GetGeometryModel()
    {
    return GeoMod;
    }


  internal override void MakeNewGeometryModel()
    {
    try
    {
    DiffuseMaterial SolidMat = new DiffuseMaterial();
    // SolidMat.Brush = Brushes.Blue;
    SolidMat.Brush = SetTextureImageBrush();

    Surface = GeometrySphere.MakeSphericalModel( Radius,
                             X,
                             Y,
                             Z,
                             LongitudeHours );

    // if( Surface == null )

    GeoMod.Geometry = Surface;
    GeoMod.Material = SolidMat;
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in PlanetSphere.MakeNewGeometryModel(): " + Except.Message );
      }
    }



  private ImageBrush SetTextureImageBrush()
    {
    // Imaging Namespace:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging?view=netframework-4.7.1

    // ImageDrawing:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imagedrawing?view=netframework-4.7.1

    BitmapImage BMapImage = new BitmapImage();

    // Things have to be in this Begin-end block.
    BMapImage.BeginInit();

    BMapImage.UriSource = new Uri( TextureFileName );

    // BMapImage.DecodePixelWidth = 200;

    BMapImage.EndInit();

    // ImageBrush:
    // https://msdn.microsoft.com/en-us/library/system.windows.media.imagebrush(v=vs.110).aspx
    ImageBrush ImgBrush = new ImageBrush();
    ImgBrush.ImageSource = BMapImage;
    return ImgBrush;
    }



  }
}
