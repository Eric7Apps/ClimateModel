// Copyright Eric Chauvin 2018.
// My blog is at:
// https://scientificmodels.blogspot.com/



using System;
// using System.Text;


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




  internal static Vector MakeZero()
    {
    Vector Result;

    Result.X = 0;
    Result.Y = 0;
    Result.Z = 0;

    return Result;
    }



  internal static Vector Negate( Vector In )
    {
    In.X = -In.X;
    In.Y = -In.Y;
    In.Z = -In.Z;

    return In;
    }



  internal static Vector Add( Vector Left, Vector Right )
    {
    Left.X += Right.X;
    Left.Y += Right.Y;
    Left.Z += Right.Z;
    return Left;
    }



  internal static Vector Subtract( Vector Left, Vector Right )
    {
    Left.X -= Right.X;
    Left.Y -= Right.Y;
    Left.Z -= Right.Z;
    return Left;
    }



  internal static double NormSquared( Vector In )
    {
    double NS = (In.X * In.X) +
                (In.Y * In.Y) +
                (In.Z * In.Z);

    return NS;
    }



  internal static double Norm( Vector In )
    {
    double NSquared = NormSquared( In );
    return Math.Sqrt( NSquared );
    }



  internal static Vector Normalize( Vector In )
    {
    double Length = (In.X * In.X) +
                    (In.Y * In.Y) +
                    (In.Z * In.Z);

    Length = Math.Sqrt( Length );

    const double SmallNumber = 0.00000000000000000001d;
    if( Length < SmallNumber )
      return MakeZero();

    double Inverse = 1.0d / Length;

    Vector Result;
    Result.X = In.X * Inverse;
    Result.Y = In.Y * Inverse;
    Result.Z = In.Z * Inverse;
    return Result;
    }



  internal static Vector MultiplyWithScalar( Vector In, double Scalar )
    {
    Vector Result;
    Result.X = In.X * Scalar;
    Result.Y = In.Y * Scalar;
    Result.Z = In.Z * Scalar;
    return Result;
    }



  internal static double DotProduct( Vector Left, Vector Right )
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



  internal static Vector MakePerpendicular( Vector A, Vector B )
    {
    // A and B are assumed to be already normalized.
    // Make a vector that is perpendicular to A,
    // pointing toward B.

    double Dot = DotProduct( A, B );
    Vector CosineLengthVec = A;
    CosineLengthVec = MultiplyWithScalar( CosineLengthVec, Dot );

    // CosineLengthVec now points in the same
    // direction as A, but it's shorter.

    // CosineLengthVec + Result = B
    // Result = B - CosineLengthVec
    Vector Result = B;
    Result = Subtract( Result, CosineLengthVec );
    Result = Normalize( Result );

    // They should be orthogonal to each other.
    // double TestDot = DotProduct( A, Result );
    // if( Math.Abs( TestDot ) > 0.00000001 )
      // throw( new Exception( "TestDot should be zero." ));

    return Result;
    }



  }
}
