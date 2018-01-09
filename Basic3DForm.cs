// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;



namespace ClimateModel
{
  public partial class ThreeDForm : Form
  {
  private MainForm MForm;
  private ThreeDScene Scene;
  private Viewport3D ViewPort = new Viewport3D();



  private ThreeDForm()
    {
    InitializeComponent();
    }



  public ThreeDForm( MainForm UseForm )
    {
    InitializeComponent();

    try
    {
    MForm = UseForm;

    this.Font = new System.Drawing.Font( "Consolas", 34.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
    this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));

    Scene = new ThreeDScene( MForm );

    ViewPort.Camera = Scene.GetCamera();
    ViewPort.Children.Add( Scene.GetMainModelVisual3D() );
    MainElementHost.Child = ViewPort;
    }
    catch( Exception Except )
      {
      MessageBox.Show( "Exception in ThreeDForm constructor: " + Except.Message, MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return;
      }
    }



  internal void FreeEverything()
    {
    }



  private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
    Close();
    }



  private void ThreeDForm_KeyDown(object sender, KeyEventArgs e)
    {
    /*
// A  The A key.
// Add The add key.
// D0 The 0 key.
// D1 The 1 key.
// Delete The DEL key.
// Down The DOWN ARROW key.
// End The END key.
// Enter The ENTER key.
// F1 The F1 key.
// Home The HOME key.
// Insert The INS key.
// Left The LEFT ARROW key.
// Next The PAGE DOWN key.
// NumPad2 The 2 key on the numeric keypad.
// Prior The PAGE UP key.
// Return The RETURN key.
// Right The RIGHT ARROW key.
// Space The SPACEBAR key.
// Subtract The subtract key.
// Tab The TAB key.
// Up The UP ARROW key.

    */

    // if( e.KeyCode == Keys.Escape ) //  && (e.Alt || e.Control || e.Shift))

    if( e.KeyCode == Keys.PageUp )
      {
// PageDown The PAGE DOWN key.
//  The PAGE UP key.


      }

    }
  }
}




