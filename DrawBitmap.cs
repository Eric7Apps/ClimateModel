// Copyright Eric Chauvin 2018.
// My blog is at:
// https://scientificmodels.blogspot.com/



using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

// using System.Windows.Forms;
// using System.Windows;


// https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/getting-started-with-graphics-programming
// https://docs.microsoft.com/en-us/dotnet/api/system.drawing?view=netframework-4.7.2

    // Bitmap class:
    // https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=netframework-4.7.2

// PictureBox class:
// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.picturebox?view=netframework-4.7.2

// Graphics class:
// https://docs.microsoft.com/en-us/dotnet/api/system.drawing.graphics?view=netframework-4.7.2


namespace ClimateModel
{
  class DrawBitmap
  {
  private MainForm MForm;



  private DrawBitmap()
    {
    }



  internal DrawBitmap( MainForm UseForm )
    {
    MForm = UseForm;

    }



  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  internal void MakeImageFile( string FileName )
    {
    try
    {
    using( Bitmap BMap = new Bitmap( FileName, true ))
      {
      if( BMap == null )
        {
        ShowStatus( "Bitmap was null." );
        return;
        }

      // Bitmap( int width, int height,
        // System.Drawing.Imaging.PixelFormat format );
      // Pixel format: Format24bppRgb
      ShowStatus( "Loaded the bitmap." );
      ShowStatus( "Pixel format: " + BMap.PixelFormat.ToString() );

      using( Graphics Graph = Graphics.FromImage( BMap ))
        {
        if( Graph == null )
          {
          ShowStatus( "Graphics was null." );
          return;
          }

        // X goes to the right and Y points downward.

        // https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/lines-curves-and-shapes

        Pen MyPen = new Pen( Color.Black, 3 );
        Graph.DrawLine( MyPen, 0, 0, 200, 200 );

        // Save(Stream, ImageFormat)
        string ToFileName = FileName.Replace( ".jpg" , "Test.jpg" );

        BMap.Save( ToFileName, ImageFormat.Jpeg );

        }
      } // The file stays locked until it is disposed.

    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in DrawBitmap.LoadImageFile(): " + Except.Message );
      }
    }



  }
}
