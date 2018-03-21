// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// Climate Model


using System;
// using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace ClimateModel
{
  // public partial class MainForm : Form
  public partial class MainForm : Form
  {
  internal const string VersionDate = "3/21/2018";
  internal const int VersionNumber = 09; // 0.9
  private System.Threading.Mutex SingleInstanceMutex = null;
  private bool IsSingleInstance = false;
  private bool IsClosing = false;
  private bool Cancelled = false;
  internal const string MessageBoxTitle = "Climate Model";
  // private string DataDirectory = "";
  // private ConfigureFile ConfigFile;
  private ThreeDForm ThreeDF;
  private MenuEvents MEvents;


  // System.Windows.Forms.
  private TextBox MainTextBox;
  private System.Windows.Forms.Timer SingleInstanceTimer;



  public MainForm()
    {
    MEvents = new MenuEvents( this );
    InitializeGuiComponents();

    if( !CheckSingleInstance())
      return;

    IsSingleInstance = true;

    ShowStatus( "Version Date: " + VersionDate );
    }



  internal bool CheckEvents()
    {
    if( IsClosing )
      return false;

    Application.DoEvents();

    if( Cancelled )
      return false;

    return true;
    }


  // This has to be added in the Program.cs file.
  //   Application.ThreadException += new ThreadExceptionEventHandler( MainForm.UIThreadException );
  //   Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );
    // What about this part?
    // AppDomain.CurrentDomain.UnhandledException +=
       //  new UnhandledExceptionEventHandler( CurrentDomain_UnhandledException );
  internal static void UIThreadException( object sender, ThreadExceptionEventArgs t )
    {
    string ErrorString = t.Exception.Message;

    try
      {
      string ShowString = "There was an unexpected error:\r\n\r\n" +
             "The program will close now.\r\n\r\n" +
             ErrorString;

      MessageBox.Show( ShowString, "Program Error", MessageBoxButtons.OK, MessageBoxIcon.Stop );
      }

    finally
      {
      Application.Exit();
      }
    }



  private void SingleInstanceTimer_Tick(object sender, EventArgs e)
    {
    SingleInstanceTimer.Stop();
    Application.Exit();
    }



  private bool CheckSingleInstance()
    {
    bool InitialOwner = false; // Owner for single instance check.
    string ShowS = "Another instance of the Climate Model is already running." +
      " This instance will close.";

    try
    {
    SingleInstanceMutex = new System.Threading.Mutex( true, "Eric's Climate Model Single Instance", out InitialOwner );
    }
    catch
      {
      MessageBox.Show( ShowS, MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop );
      // mutex.Close();
      // mutex = null;

      // Can't do this here:
      // Application.Exit();
      SingleInstanceTimer.Interval = 50;
      SingleInstanceTimer.Start();
      return false;
      }

    if( !InitialOwner )
      {
      MessageBox.Show( ShowS, MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop );
      // Application.Exit();
      SingleInstanceTimer.Interval = 50;
      SingleInstanceTimer.Start();
      return false;
      }

    return true;
    }



  internal void ShowEarthScene()
    {
    try
    {
    if( ThreeDF == null )
      ThreeDF = new ThreeDForm( this );

    if( ThreeDF.IsDisposed )
      ThreeDF = new ThreeDForm( this );

    ThreeDF.Show();
    ThreeDF.WindowState = FormWindowState.Maximized;
    ThreeDF.BringToFront();
    }
    catch( Exception Except )
      {
      MessageBox.Show( "Exception in MainForm.ShowEarthScene(): " + Except.Message, MessageBoxTitle, MessageBoxButtons.OK);
      return;
      }
    }



  private void FreeEverything()
    {
    MEvents.FreeEverything();

    MainTextBox.Dispose();
    SingleInstanceTimer.Dispose();
    }



  private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
    if( IsSingleInstance )
      {
      if( DialogResult.Yes != MessageBox.Show( "Close the program?", MessageBoxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question ))
        {
        e.Cancel = true;
        return;
        }
      }

    IsClosing = true;

    // if( IsSingleInstance )
      // {
      // SaveAllFiles();
      // DisposeOfEverything();
      // }


    if( ThreeDF != null )
      {
      if( !ThreeDF.IsDisposed )
        {
        ThreeDF.Hide();
        ThreeDF.FreeEverything();
        ThreeDF.Dispose();
        }

      ThreeDF = null;
      }

    FreeEverything();
    }



  internal void ShowStatus( string Status )
    {
    if( IsClosing )
      return;

    MainTextBox.AppendText( Status + "\r\n" );
    }



  private void InitializeGuiComponents()
    {
    MEvents.InitializeGuiComponents();

    SingleInstanceTimer = new System.Windows.Forms.Timer();

    MainTextBox = new System.Windows.Forms.TextBox();

    SuspendLayout();

    MainTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
    MainTextBox.Location = new System.Drawing.Point(0, 28);
    MainTextBox.Multiline = true;
    MainTextBox.Name = "MainTextBox";
    MainTextBox.ReadOnly = true;
    MainTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
    MainTextBox.Size = new System.Drawing.Size(715, 383);
    MainTextBox.TabIndex = 1;


    SingleInstanceTimer.Tick += new System.EventHandler(this.SingleInstanceTimer_Tick);

    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
    this.BackColor = System.Drawing.Color.Black;
    this.ClientSize = new System.Drawing.Size(715, 411);
    this.Controls.Add(this.MainTextBox);
    this.Controls.Add( MEvents.menuStrip1 );
    // this.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
    this.ForeColor = System.Drawing.Color.White;
    this.MainMenuStrip = MEvents.menuStrip1;
    this.Name = "MainForm";
    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
    this.Text = "Climate Model";
    this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

    this.Font = new System.Drawing.Font( "Consolas", 34.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));

    MEvents.ResumeLayout();
    this.ResumeLayout(false);
    this.PerformLayout();
    }


  }
}
