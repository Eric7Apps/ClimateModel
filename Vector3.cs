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
    return Math.Sqrt( NSquared );
    }



  internal static void Normalize( ref Vector3.Vector Result, Vector3.Vector In )
    {
    double Length = (In.X * In.X) +
                    (In.Y * In.Y) +
                    (In.Z * In.Z);

    Length = Math.Sqrt( Length );

    const double SmallNumber = 0.00000000000000000001d;
    if( Length < SmallNumber )
      throw( new Exception( "Length was too short for Vector3.Normalize()." ));


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



  // https://en.wikipedia.org/wiki/Dot_product
  internal static double DotProduct( ref Vector3.Vector Left, ref Vector3.Vector Right )
    {
    double Dot = (Left.X * Right.X) +
                 (Left.Y * Right.Y) +
                 (Left.Z * Right.Z);

    // This is commutative:
    // double Dot = (Right.X * Left.X) +
    //              (Right.Y * Left.Y) +
    //              (Right.Z * Left.Z);

    return Dot;
    }



  internal static void MakePerpendicular( ref Vector3.Vector Result, ref Vector3.Vector A, ref Vector3.Vector B )
    {
    // A and B are assumed to be already normalized.
    // Make a vector that is perpendicular to A,
    // pointing toward B.

    Vector3.Vector CosineLengthVec = new Vector3.Vector();

    double Dot = DotProduct( ref A, ref B );
    Copy( ref CosineLengthVec, ref A );
    MultiplyWithScalar( ref CosineLengthVec, Dot );

    // CosineLengthVec now points in the same
    // direction as A, but it's shorter.

    // CosineLengthVec + Result = B
    // Result = B - CosineLengthVec
    Copy( ref Result, ref B );
    Subtract( ref Result, ref CosineLengthVec );
    Normalize( ref Result, Result );

    // They should be orthogonal to each other.
    double TestDot = DotProduct( ref A, ref Result );
    if( Math.Abs( TestDot ) > 0.00000001 )
      throw( new Exception( "TestDot should be zero." ));

    }




  }
}
