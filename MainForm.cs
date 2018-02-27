// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// Climate Model


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
// using System.Threading.Tasks;
using System.Windows.Forms;


namespace ClimateModel
{
  // public partial class MainForm : Form
  public partial class MainForm : Form
  {
  internal const string VersionDate = "2/27/2018";
  internal const int VersionNumber = 09; // 0.9
  private System.Threading.Mutex SingleInstanceMutex = null;
  private bool IsSingleInstance = false;
  private bool IsClosing = false;
  private bool Cancelled = false;
  internal const string MessageBoxTitle = "Climate Model";
  // private string DataDirectory = "";
  // private ConfigureFile ConfigFile;
  private ThreeDForm ThreeDF;

  // System.Windows.Forms.
  private MenuStrip menuStrip1;
  private ToolStripMenuItem fileToolStripMenuItem;
  private ToolStripMenuItem exitToolStripMenuItem;
  private ToolStripMenuItem showToolStripMenuItem;
  private ToolStripMenuItem earthSceneToolStripMenuItem;
  private TextBox MainTextBox;
  private ToolStripMenuItem testToolStripMenuItem;
  private ToolStripMenuItem testToolStripMenuItem1;
  private System.Windows.Forms.Timer SingleInstanceTimer;



  public MainForm()
    {
    // InitializeComponent();
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



  private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
    Close();
    }



  private void earthSceneToolStripMenuItem_Click(object sender, EventArgs e)
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
      MessageBox.Show( "Exception in MainForm, opening ThreeDForm: " + Except.Message, MessageBoxTitle, MessageBoxButtons.OK);
      return;
      }
    }



  private void FreeEverything()
    {
    menuStrip1.Dispose();
    fileToolStripMenuItem.Dispose();
    exitToolStripMenuItem.Dispose();
    showToolStripMenuItem.Dispose();
    earthSceneToolStripMenuItem.Dispose();
    MainTextBox.Dispose();
    testToolStripMenuItem.Dispose();
    testToolStripMenuItem1.Dispose();
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



  private void testToolStripMenuItem1_Click(object sender, EventArgs e)
    {
    try
    {
    ShowStatus( "Testing: QuaternionEC.TestMultiply()." );

    QuaternionEC.TestBasics();

    ShowStatus( "Test OK." );
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in the test: " + Except.Message );
      MessageBox.Show( "Exception in the test: " + Except.Message, MessageBoxTitle, MessageBoxButtons.OK );
      return;
      }
    }



  private void InitializeGuiComponents()
    {
    SingleInstanceTimer = new System.Windows.Forms.Timer();

    menuStrip1 = new System.Windows.Forms.MenuStrip();
    fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    earthSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    MainTextBox = new System.Windows.Forms.TextBox();
    testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
    menuStrip1.SuspendLayout();

    SuspendLayout();

    menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
    menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileToolStripMenuItem,
            showToolStripMenuItem,
            testToolStripMenuItem});
      menuStrip1.Location = new System.Drawing.Point(0, 0);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.Size = new System.Drawing.Size(715, 28);
      menuStrip1.TabIndex = 0;
      menuStrip1.Text = "menuStrip1";

      fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            exitToolStripMenuItem});
      fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
      fileToolStripMenuItem.Text = "&File";

      exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      exitToolStripMenuItem.Size = new System.Drawing.Size(108, 26);
      exitToolStripMenuItem.Text = "E&xit";
      exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);

      showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            earthSceneToolStripMenuItem});
      showToolStripMenuItem.Name = "showToolStripMenuItem";
      showToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
      showToolStripMenuItem.Text = "&Show";

      earthSceneToolStripMenuItem.Name = "earthSceneToolStripMenuItem";
      earthSceneToolStripMenuItem.Size = new System.Drawing.Size(161, 26);
      earthSceneToolStripMenuItem.Text = "&Earth Scene";
      earthSceneToolStripMenuItem.Click += new System.EventHandler(this.earthSceneToolStripMenuItem_Click);

      MainTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      MainTextBox.Location = new System.Drawing.Point(0, 28);
      MainTextBox.Multiline = true;
      MainTextBox.Name = "MainTextBox";
      MainTextBox.ReadOnly = true;
      MainTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      MainTextBox.Size = new System.Drawing.Size(715, 383);
      MainTextBox.TabIndex = 1;

      testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            testToolStripMenuItem1});
      testToolStripMenuItem.Name = "testToolStripMenuItem";
      testToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
      testToolStripMenuItem.Text = "&Test";

      testToolStripMenuItem1.Name = "testToolStripMenuItem1";
      testToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
      testToolStripMenuItem1.Text = "&Test";
      testToolStripMenuItem1.Click += new System.EventHandler(this.testToolStripMenuItem1_Click);

      SingleInstanceTimer.Tick += new System.EventHandler(this.SingleInstanceTimer_Tick);

      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.Black;
      this.ClientSize = new System.Drawing.Size(715, 411);
      this.Controls.Add(this.MainTextBox);
      this.Controls.Add(this.menuStrip1);
      // this.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
      this.ForeColor = System.Drawing.Color.White;
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Climate Model";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

      this.Font = new System.Drawing.Font( "Consolas", 34.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
      this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));

      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }


  }
}






