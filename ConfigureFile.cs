// Programming by Eric Chauvin.
// Notes on this source code are at:
// ericsourcecode.blogspot.com


using System;
using System.Text;
// using System.Windows.Forms; // Application.
using System.Collections.Generic;
// using System.Threading.Tasks;
using System.IO;



// The compiler will create a default namespace if you don't.
// namespace CodeEditor
// {

namespace CodeEditor
{
  class ConfigureFile
  {
  private MainForm MForm;
  private Dictionary<string, string> CDictionary;
  private string FileName;
  // private AESEncryption AESEncrypt;


  private ConfigureFile()
    {
    }



  internal ConfigureFile( string FileToUseName, MainForm UseForm )
    {
    MForm = UseForm;

    FileName = FileToUseName;
    // AESEncrypt = new AESEncryption();
    // string ExampleKey = "Where does this key come from? 
    // AESEncrypt.SetKey( ExampleKey );

    CDictionary = new Dictionary<string, string>();
    ReadFromTextFile();
    }



  internal string GetString( string KeyWord )
    {
    KeyWord = KeyWord.ToLower().Trim();

    string Value;
    if( CDictionary.TryGetValue( KeyWord, out Value ))
      return Value;
    else
      return "";

    }



  internal void SetString( string KeyWord, string Value )
    {
    KeyWord = KeyWord.ToLower().Trim();

    if( KeyWord == "" )
      {
      MForm.ShowStatus( "Can't add an empty keyword to the dictionary in ConfigureFile.cs." );
      return;
      }

    CDictionary[KeyWord] = Value;
    WriteToTextFile();
    }



  private bool ReadFromTextFile()
    {
    // AESEncryption AESEncrypt = null;

    char TabChar = (char)9;

    CDictionary.Clear();
    if( !File.Exists( FileName ))
      return false;

    try
    {
    using( StreamReader SReader = new StreamReader( FileName, Encoding.UTF8 ))
      {
      while( SReader.Peek() >= 0 ) 
        {
        string Line = SReader.ReadLine();
        if( Line == null )
          continue;

        Line = Line.Trim();
        if( Line == "" )
          continue;

        // if( !Line.Contains( "\t" ))
          // Line = AESEncrypt.DecryptString( Line );

        if( !Line.Contains( "\t" ))
          continue;

        string[] SplitString = Line.Split( new Char[] { TabChar } );

        if( SplitString.Length < 2 )
          continue;

        string KeyWord = SplitString[0].Trim();
        string Value = SplitString[1].Trim();
        KeyWord = KeyWord.Replace( "\"", "" );
        Value = Value.Replace( "\"", "" );

        if( KeyWord == "" )
          continue;

        CDictionary[KeyWord] = Value;
        // try
        // CDictionary.Add( KeyWord, Value );

        }
      }
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Could not read the file: \r\n" + FileName );
      MForm.ShowStatus( Except.Message );
      return false;
      }

    return true;
    }



  internal bool WriteToTextFile()
    {
    try
    {
    using( StreamWriter SWriter = new StreamWriter( FileName, false, Encoding.UTF8 ))
      {
      foreach( KeyValuePair<string, string> Kvp in CDictionary )
        {
        string Line = Kvp.Key + "\t" + Kvp.Value;
        // Line = AESEncrypt.EncryptString( Line );
        SWriter.WriteLine( Line );
        }

      SWriter.WriteLine( " " );
      }
    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Could not write the configuration data to the file." );
      MForm.ShowStatus( Except.Message );
      return false;
      }

    return true;
    }



  /*
  private int GetIntegerValue( string Key )
    {
    try
    {
    return Int32.Parse( Config.GetString( Key ));
    }
    catch( Exception ) // Except )
      {
      // MForm.ShowStatus( "Exception in GetIntegerValue():" );
      // MForm.ShowStatus( Except.Message );
      return -1;
      }
    }
    */


  /*
  private void SetIntegerValue( string Key, int ToSet )
    {
    SetString( Key, ToSet.ToString() );
    WriteToTextFile();
    }
    */



  }
}
