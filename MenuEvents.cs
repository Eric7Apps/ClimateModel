// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
// using System.Collections.Generic;
using System.Text;
// using System.Threading.Tasks;
// using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;



namespace ClimateModel
{
  public class MenuEvents
  {
  private MainForm MForm;
  internal MenuStrip menuStrip1;
  private ToolStripMenuItem fileToolStripMenuItem;
  private ToolStripMenuItem exitToolStripMenuItem;
  private ToolStripMenuItem showToolStripMenuItem;
  private ToolStripMenuItem earthSceneToolStripMenuItem;
  private ToolStripMenuItem testToolStripMenuItem;
  private ToolStripMenuItem testToolStripMenuItem1;


  private MenuEvents()
    {
    }



  internal MenuEvents( MainForm UseForm )
    {
    MForm = UseForm;
    }


  internal void FreeEverything()
    {
    menuStrip1.Dispose();
    fileToolStripMenuItem.Dispose();
    exitToolStripMenuItem.Dispose();
    showToolStripMenuItem.Dispose();
    earthSceneToolStripMenuItem.Dispose();
    testToolStripMenuItem.Dispose();
    testToolStripMenuItem1.Dispose();
    }


  internal void InitializeGuiComponents()
    {
    try
    {
    menuStrip1 = new System.Windows.Forms.MenuStrip();
    fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    earthSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
    testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();


    menuStrip1.SuspendLayout();

    menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
    menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileToolStripMenuItem,
            showToolStripMenuItem,
            testToolStripMenuItem });
            // helpToolStripMenuItem

    menuStrip1.Location = new System.Drawing.Point(0, 0);
    menuStrip1.Name = "menuStrip1";
    menuStrip1.Size = new System.Drawing.Size(632, 28);
    menuStrip1.TabIndex = 0;
    menuStrip1.Text = "menuStrip1";


    fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            // OpenPageToolStripMenuItem,
            exitToolStripMenuItem});
    fileToolStripMenuItem.Name = "fileToolStripMenuItem";
    fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
    fileToolStripMenuItem.Text = "&File";

    exitToolStripMenuItem.Name = "exitToolStripMenuItem";
    exitToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
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

    testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
          testToolStripMenuItem1});
    testToolStripMenuItem.Name = "testToolStripMenuItem";
    testToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
    testToolStripMenuItem.Text = "&Test";

    testToolStripMenuItem1.Name = "testToolStripMenuItem1";
    testToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
    testToolStripMenuItem1.Text = "&Test";
    testToolStripMenuItem1.Click += new System.EventHandler(this.testToolStripMenuItem1_Click);

    menuStrip1.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));

    }
    catch( Exception Except )
      {
      string ShowS = "Exception in MenuEvents.InitializeGuiComponents(): " + Except.Message;
      MessageBox.Show( ShowS, MainForm.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop );
      }
    }



  internal void ResumeLayout()
    {
    try
    {
    menuStrip1.ResumeLayout(false);
    menuStrip1.PerformLayout();
    }
    catch( Exception Except )
      {
      string ShowS = "Exception in MenuEvents.ResumeLayout(): " + Except.Message;
      MessageBox.Show( ShowS, MainForm.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop );
      }
    }



  private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
    MForm.Close();
    }



  private void earthSceneToolStripMenuItem_Click(object sender, EventArgs e)
    {
    MForm.ShowEarthScene();
    }






/*
  private void OpenPageToolStripMenuItem_Click( object sender, EventArgs e )
    {
    MForm.ShowStatus( "Testing Page." );

//////
    Where do I want to call a function at?
    I mean I don't want a lot of unrelated code
    in here.
///////

    string FileName = "C:\\Eric\\BrowserEC\\bin\\Release\\TestDownload.txt";
    LexicalAnalysis1 Lex1 = new LexicalAnalysis1( MForm );
    Lex1.ProcessSyntax( FileName );

/////
    Page TestPage = new Page( MForm );
    TestPage.MakeNewFromFile( "Test Title",
             "https://www.google.com",
             "C:\\Eric\\BrowserEC\\bin\\Release\\TestDownload.txt" );
///////
    }
*/



  private void testToolStripMenuItem1_Click(object sender, EventArgs e)
    {
    MessageBox.Show( "Check this.", MainForm.MessageBoxTitle, MessageBoxButtons.OK );
    /*
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
    */
    }


  }
}
