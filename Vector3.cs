// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
using System.Text;


namespace ClimateModel
{

  static class Vector3
  {


  public struct Vector
    {
    public double X;
    public double Y;
    public double Z;
    }




  internal static void SetZero( ref Vector3.Vector Result )
    {
    Result.X = 0;
    Result.Y = 0;
    Result.Z = 0;
    }



  internal static void Negate( ref Vector3.Vector Result )
    {
    Result.X = -Result.X;
    Result.Y = -Result.Y;
    Result.Z = -Result.Z;
    }



  internal static void Copy( ref Vector3.Vector Result, ref Vector3.Vector In )
    {
    Result.X = In.X;
    Result.Y = In.Y;
    Result.Z = In.Z;
    }



  internal static void Add( ref Vector3.Vector Result, ref Vector3.Vector In )
    {
    Result.X += In.X;
    Result.Y += In.Y;
    Result.Z += In.Z;
    }



  internal static void Subtract( ref Vector3.Vector Result, ref Vector3.Vector In )
    {
    Result.X -= In.X;
    Result.Y -= In.Y;
    Result.Z -= In.Z;
    }




  internal static double NormSquared( ref Vector3.Vector In )
    {
    double NS = (In.X * In.X) +
                (In.Y * In.Y) +
                (In.Z * In.Z);

    return NS;
    }



  internal static double Norm( ref Vector3.Vector In )
    {
    double NSquared = NormSquared( ref In );

    double SmallNumber = 0.0000000000000001d;
    if( NSquared < SmallNumber )
      throw( new Exception( "Length was too short for Norm() in Vector3." ));

    return Math.Sqrt( NSquared );
    }



  internal static void Normalize( ref Vector3.Vector Result, Vector3.Vector In )
    {
    double Length = (In.X * In.X) +
                    (In.Y * In.Y) +
                    (In.Z * In.Z);

    double SmallNumber = 0.00000000000000000001d;
    if( Length < SmallNumber )
      throw( new Exception( "Length was too short for NormalizeVector3() in QuaternionEC." ));

    Length = Math.Sqrt( Length );
    double Inverse = 1.0d / Length;

    Result.X = In.X * Inverse;
    Result.Y = In.Y * Inverse;
    Result.Z = In.Z * Inverse;
    }



  internal static void MultiplyWithScalar( ref Vector3.Vector Result, double Scalar )
    {
    Result.X = Result.X * Scalar;
    Result.Y = Result.Y * Scalar;
    Result.Z = Result.Z * Scalar;
    }



  /*
  internal static double DotProduct( ref QuaternionRec Left, ref QuaternionRec Right )
    {
    double Dot = (Left.X * Right.X) +
                 (Left.Y * Right.Y) +
                 (Left.Z * Right.Z);
// Is W zero?  Is it used?
                // (Left.W * Right.W);

    return Dot;
    }
    */


  }
}
