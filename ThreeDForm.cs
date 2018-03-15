// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
// using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Forms.Integration;



namespace ClimateModel
{
  // If you make this so it's not a partial class
  // you have to change it in your project file too.
  // public partial class ThreeDForm : Form
  public class ThreeDForm : Form
  {
  private MainForm MForm;
  private MenuStrip menuStrip1;
  private ToolStripMenuItem fileToolStripMenuItem;
  private ToolStripMenuItem closeToolStripMenuItem;
  private System.Windows.Forms.Panel TopPanel;
  private System.Windows.Forms.Panel ThreeDPanel;
  private ElementHost MainElementHost;
  private System.Windows.Forms.TextBox textBox1;
  private Viewport3D ViewPort;
  private ThreeDScene Scene;



  private ThreeDForm()
    {
    // InitializeComponent();
    }



  public ThreeDForm( MainForm UseForm )
    {
    try
    {
    MForm = UseForm;

    // InitializeComponent();
    SetupGUI();

    // MForm.ShowStatus( "ThreeDForm was created." );

    }
    catch( Exception Except )
      {
      MessageBox.Show( "Exception in ThreeDForm constructor: " + Except.Message, MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      return;
      }
    }



  private void SetupGUI()
    {
    menuStrip1 = new System.Windows.Forms.MenuStrip();
    fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    TopPanel = new System.Windows.Forms.Panel();
    ThreeDPanel = new System.Windows.Forms.Panel();
    MainElementHost = new System.Windows.Forms.Integration.ElementHost();
    textBox1 = new System.Windows.Forms.TextBox();
    ViewPort = new Viewport3D();

    InitializeGuiComponents();

    // This sets up the scene.
    Scene = new ThreeDScene( MForm );

    ViewPort.Camera = Scene.GetCamera();
    ViewPort.Children.Add( Scene.GetMainModelVisual3D() );
    MainElementHost.Child = ViewPort;
    }



  private void InitializeGuiComponents()
    {
    menuStrip1.SuspendLayout();
    TopPanel.SuspendLayout();
    ThreeDPanel.SuspendLayout();
    this.SuspendLayout();

    menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
    menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
    menuStrip1.Location = new System.Drawing.Point(0, 0);
    menuStrip1.Name = "menuStrip1";
    menuStrip1.Size = new System.Drawing.Size(617, 28);
    menuStrip1.TabIndex = 1;
    menuStrip1.Text = "menuStrip1";
    menuStrip1.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));

    fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
    fileToolStripMenuItem.Name = "fileToolStripMenuItem";
    fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
    fileToolStripMenuItem.Text = "&File";

    closeToolStripMenuItem.Name = "closeToolStripMenuItem";
    closeToolStripMenuItem.Size = new System.Drawing.Size(120, 26);
    closeToolStripMenuItem.Text = "&Close";
    closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);

    TopPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
    TopPanel.Controls.Add(this.textBox1);
    TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
    TopPanel.Location = new System.Drawing.Point(0, 28);
    TopPanel.Name = "TopPanel";
    TopPanel.Size = new System.Drawing.Size(617, 72);
    TopPanel.TabIndex = 2;

    ThreeDPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
    ThreeDPanel.Controls.Add(this.MainElementHost);
    ThreeDPanel.Dock = System.Windows.Forms.DockStyle.Fill;
    ThreeDPanel.Location = new System.Drawing.Point(0, 109);
    ThreeDPanel.Name = "ThreeDPanel";
    ThreeDPanel.Size = new System.Drawing.Size(617, 290);
    ThreeDPanel.TabIndex = 3;

    MainElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
    MainElementHost.Location = new System.Drawing.Point(0, 0);
    MainElementHost.Name = "MainElementHost";
    MainElementHost.Size = new System.Drawing.Size(615, 301);
    MainElementHost.TabIndex = 0;
    MainElementHost.Text = "elementHost1";
    // MainElementHost.Child = null;

    textBox1.Location = new System.Drawing.Point(26, 14);
    textBox1.Name = "textBox1";
    textBox1.Size = new System.Drawing.Size(100, 22);
    textBox1.TabIndex = 0;

    this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    this.ClientSize = new System.Drawing.Size(617, 412);
    this.Controls.Add(this.ThreeDPanel);
    this.Controls.Add(this.TopPanel);
    this.Controls.Add(this.menuStrip1);
    this.KeyPreview = true;
    this.MainMenuStrip = this.menuStrip1;
    this.Name = "ThreeDForm";
    this.Text = "ThreeDForm";
    this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
    this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ThreeDForm_KeyDown);
    this.Font = new System.Drawing.Font( "Consolas", 34.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
    // this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.MainForm_FormClosing );

    menuStrip1.ResumeLayout(false);
    menuStrip1.PerformLayout();
    TopPanel.ResumeLayout(false);
    TopPanel.PerformLayout();
    ThreeDPanel.ResumeLayout(false);
    this.ResumeLayout(false);
    this.PerformLayout();
    }



  internal void FreeEverything()
    {
    menuStrip1.Dispose();
    fileToolStripMenuItem.Dispose();
    closeToolStripMenuItem.Dispose();
    TopPanel.Dispose();
    ThreeDPanel.Dispose();
    MainElementHost.Dispose();
    textBox1.Dispose();
    // ViewPort.Dispose();
    // Scene;
    }


  private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
    Close();
    }



  private void ThreeDForm_KeyDown(object sender, KeyEventArgs e)
    {
    try
    {
    // MessageBox.Show( "Test.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );

// A  The A key.
// Add The add key.
// D0 The 0 key.
// D1 The 1 key.
// Delete The DEL key.
// End The END key.
// Enter The ENTER key.
// F1 The F1 key.
// Home The HOME key.
// Insert The INS key.
// NumPad2 The 2 key on the numeric keypad.
// Return The RETURN key.
// Space The SPACEBAR key.
// Subtract The subtract key.
// Tab The TAB key.

    double Angle = NumbersEC.DegreesToRadians( 2 );

    if( e.Control )
      {
      if( e.KeyCode == Keys.Left )
        {
        Scene.RotateLeftRight( -Angle );
        }

      if( e.KeyCode == Keys.Right )
        {
        Scene.RotateLeftRight( Angle );
        }

      if( e.KeyCode == Keys.PageUp )
        {
        Scene.MoveForwardBack( 30.0 );
        }

      if( e.KeyCode == Keys.PageDown )
        {
        Scene.MoveForwardBack( -30.0 );
        }

      return;
      }


    if( e.Alt )
      {

      return;
      }


    if( e.Shift )
      {
      if( e.KeyCode == Keys.Left )
        {
        Scene.ShiftLeftRight( -3.0 );
        }

      if( e.KeyCode == Keys.Right )
        {
        Scene.ShiftLeftRight( 3.0 );
        }

      if( e.KeyCode == Keys.Up )
        {
        Scene.ShiftUpDown( 3.0 );
        }

      if( e.KeyCode == Keys.Down )
        {
        Scene.ShiftUpDown( -3.0 );
        }

      return;
      }

    if( e.KeyCode == Keys.Escape ) //  && (e.Alt || e.Control || e.Shift))
      {
      // MessageBox.Show( "Escape.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      }

    if( e.KeyCode == Keys.PageUp )
      {
      // MessageBox.Show( "Page up.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      Scene.MoveForwardBack( 2.0 );
      }

    if( e.KeyCode == Keys.PageDown )
      {
      Scene.MoveForwardBack( -2.0 );
      }

    if( e.KeyCode == Keys.Left )
      {
      Scene.MoveLeftRight( Angle );
      }

    if( e.KeyCode == Keys.Right )
      {
      Scene.MoveLeftRight( -Angle );
      }

    if( e.KeyCode == Keys.Up )
      {
      Scene.MoveUpDown( Angle );
      }

    if( e.KeyCode == Keys.Down )
      {
      Scene.MoveUpDown( -Angle );
      }
    }
    catch( Exception Except )
      {
      MessageBox.Show( "Exception in ThreeDForm.KeyDown: " + Except.Message, MainForm.MessageBoxTitle, MessageBoxButtons.OK );
      }
    }


  }
}
