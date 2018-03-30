// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


// Quaternion:
// https://en.wikipedia.org/wiki/Quaternion

// Quaternions and spatial rotation:
// https://en.wikipedia.org/wiki/Quaternions_and_spatial_rotation


using System;
using System.Text;

// For testing:
using System.Windows.Media.Media3D;



namespace ClimateModel
{

  static class QuaternionEC
  {
  // The original ijk are sometimes called xyz (the
  // vector part) and the scalar part is w.
  // a + bi + cj + dk
  // w + bx + cy + dz

  public struct QuaternionRec
    {
    public double X;
    public double Y;
    public double Z;
    public double W;
    }



  internal static void SetOne( ref QuaternionRec Result )
    {
    Result.X = 0;
    Result.Y = 0;
    Result.Z = 0;
    Result.W = 1;
    }



  internal static void SetZero( ref QuaternionRec Result )
    {
    Result.X = 0;
    Result.Y = 0;
    Result.Z = 0;
    Result.W = 0;
    }



  internal static void Negate( ref QuaternionRec Result )
    {
    Result.X = -Result.X;
    Result.Y = -Result.Y;
    Result.Z = -Result.Z;
    Result.W = -Result.W;
    }



  internal static void Copy( ref QuaternionRec Result, ref QuaternionRec In )
    {
    Result.X = In.X;
    Result.Y = In.Y;
    Result.Z = In.Z;
    Result.W = In.W;
    }



  internal static void Add( ref QuaternionRec Result, ref QuaternionRec In )
    {
    Result.X += In.X;
    Result.Y += In.Y;
    Result.Z += In.Z;
    Result.W += In.W;
    }



  internal static double NormSquared( ref QuaternionRec In )
    {
    double NS = (In.X * In.X) +
                (In.Y * In.Y) +
                (In.Z * In.Z) +
                (In.W * In.W);

    return NS;
    }



  internal static double Norm( ref QuaternionRec In )
    {
    double NSquared = NormSquared( ref In );

    double SmallNumber = 0.0000000000000001d;
    if( NSquared < SmallNumber )
      throw( new Exception( "Length was too short for Norm() in QuaternionEC." ));

    return Math.Sqrt( NSquared );
    }



  internal static void Normalize( ref QuaternionRec Result, ref QuaternionRec In )
    {
    double Length = Norm( ref In );
    double Inverse = 1.0d / Length;

    Result.X = In.X * Inverse;
    Result.Y = In.Y * Inverse;
    Result.Z = In.Z * Inverse;
    Result.W = In.W * Inverse;
    }



  internal static bool DoubleIsAlmostEqual( double A, double B, double SmallNumber )
    {
    // How small can this be?
    // How close are they?

    if( A + SmallNumber < B )
      return false;

    if( A - SmallNumber > B )
      return false;

    return true;
    }



  internal static bool IsAlmostEqual( ref QuaternionRec Left, ref QuaternionRec Right, double SmallNumber )
    {
    if( !DoubleIsAlmostEqual( Left.X, Right.X, SmallNumber ))
      return false;

    if( !DoubleIsAlmostEqual( Left.Y, Right.Y, SmallNumber ))
      return false;

    if( !DoubleIsAlmostEqual( Left.Z, Right.Z, SmallNumber ))
      return false;

    if( !DoubleIsAlmostEqual( Left.W, Right.W, SmallNumber ))
      return false;

    return true;
    }



  internal static void Conjugate( ref QuaternionRec Result, ref QuaternionRec In )
    {
    // It's like the conjugate of a complex number
    // a + ib is a - ib.

    Result.X = -In.X;
    Result.Y = -In.Y;
    Result.Z = -In.Z;

    Result.W = In.W;
    }



  internal static void Inverse( ref QuaternionRec Result, ref QuaternionRec In )
    {
    // QX = 1, so X is the multiplicative inverse
    // of Q.  So X = 1 / Q, or Q^(-1).

    ///////////////////////
    // Test:
    QuaternionRec TestQ = new QuaternionRec();
    Copy( ref TestQ, ref In );
    ///////////////////

    double NSquared = NormSquared( ref In );
    double InverseNS = 1.0d / NSquared;

    // The negative parts are to make it the
    // conjugate.  So the Result is the conjugate
    // divided by the norm squared.
    // Since multiplication isn't commutative,
    // neither is division.  I'm pretty sure
    // that it's a theorem of Group Theory that
    // there is only one multiplicative inverse.
    // But here the value InverseNS is just a real
    // number and it commutes with the quaternion.

    Result.X = -In.X * InverseNS;
    Result.Y = -In.Y * InverseNS;
    Result.Z = -In.Z * InverseNS;

    Result.W = In.W * InverseNS;

    ///////////////
    // Test:
    double SmallNumber = 0.00000000000001;
    QuaternionRec Test1 = new QuaternionRec();
    SetOne( ref Test1 );

    QuaternionRec TestResult = new QuaternionRec();
    QuaternionRec TestX = new QuaternionRec();

    Copy( ref TestX, ref Result );

     // From the definition of "inverse", it should
    // equal 1.
    Multiply( ref TestResult, ref TestX, ref TestQ );
    if( !IsAlmostEqual(  ref Test1, ref TestResult, SmallNumber ))
      throw( new Exception( "Invert doesn't match its definition." ));

   // Is the left inverse the same as the right
   // inverse?  (A theorem from Group theory.)
   Multiply( ref TestResult, ref TestQ, ref TestX );
    if( !IsAlmostEqual(  ref Test1, ref TestResult, SmallNumber ))
      throw( new Exception( "Second test: Invert doesn't match its definition." ));

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



  // https://en.wikipedia.org/wiki/Cross_product

  internal static void CrossProduct( ref QuaternionRec Result,
                                     ref QuaternionRec Left,
                                     ref QuaternionRec Right )
    {
    // i x j = k
    // j x k = i
    // k x i = j

    // W is not used.
    Result.W = 0;

    Result.X = (Left.Y * Right.Z) -
               (Left.Z * Right.Y);

    Result.Y = (Left.Z * Right.X) -
               (Left.X * Right.Z);

    Result.Z = (Left.X * Right.Y) -
               (Left.Y * Right.X);

    }



  ///////////////////////////
  // Notes on Multiplication:

  // It is a right-handed coordinate system.  Positive
  // Z values go toward the viewer.  X goes to the
  // right, Y goes up.

  // ij = k    ji = -k
  // jk = i    kj = -i
  // ki = j    ik = -j

  // xy = z    yx = -z
  // yz = x    zy = -x
  // zx = y    xz = -y

  //   i^2 = j^2 = k^2 = ijk = -1

  // With two regular complex numbers you do:
  //      (a + bi)(c + di) =
  //      ac + adi + bic + bidi
  // Notice that c and di stay on the right side.
  // But the real part of bidi can apparently
  // commute to bdii and the ii multiplied together
  // equals -1.  bd(ii) = bd(-1) = -bd.
  // But complex numbers are Commutative and
  // Quaternions are not.

  // a1 is like a with subscript 1.

  // Multiply two quaternions:
  // Left times Right.
  // (a1x + b1y + c1z + w1)(a2x + b2y + c2z + w2)

  // Distributive Property:
  // a1x(a2x + b2y + c2z + w2) +
  // b1y(a2x + b2y + c2z + w2) +
  // c1z(a2x + b2y + c2z + w2) +
  // w1(a2x + b2y + c2z + w2)

  // Distributive Property again:
  // a1xa2x + a1xb2y + a1xc2z + a1xw2 +
  // b1ya2x + b1yb2y + b1yc2z + b1yw2 +
  // c1za2x + c1zb2y + c1zc2z + c1zw2 +
  // w1a2x + w1b2y + w1c2z + w1w2

  // Do the real parts commute within each of these
  // terms?
  // a1a2xx + a1b2xy + a1c2xz + a1w2x +
  // b1a2yx + b1b2yy + b1c2yz + b1w2y +
  // c1a2zx + c1b2zy + c1c2zz + c1w2z +
  // w1a2x + w1b2y + w1c2z + w1w2

  // xy = z    yx = -z
  // yz = x    zy = -x
  // zx = y    xz = -y
  // a1a2-1 + a1b2z + a1c2-y + a1w2x +
  // b1a2-z + b1b2-1 + b1c2x + b1w2y +
  // c1a2y + c1b2-x + c1c2-1 + c1w2z +
  // w1a2x + w1b2y + w1c2z + w1w2

  // Rearrange it so the components are together:
  // a1w2x + w1a2x + b1c2x + c1b2-x +
  // a1c2-y + b1w2y + c1a2y + w1b2y +
  // a1b2z + b1a2-z + c1w2z + w1c2z +
  // -a1a2 + -b1b2 + -c1c2 + w1w2

  // a1w2x + w1a2x + b1c2x + -c1b2x +
  // -a1c2y + b1w2y + c1a2y + w1b2y +
  // a1b2z + -b1a2z + c1w2z + w1c2z +
  // -a1a2 + -b1b2 + -c1c2 + w1w2

  // x(a1w2 + w1a2 + b1c2 + -c1b2) +
  // y(-a1c2 + b1w2 + c1a2 + w1b2) +
  // z(a1b2 + -b1a2 + c1w2 + w1c2) +
  // The W parts:
  // -a1a2 + -b1b2 + -c1c2 + w1w2
  ///////////////////////////




  internal static void Multiply( ref QuaternionRec Result, ref QuaternionRec L, ref QuaternionRec R )
    {
    //////////////////////////////////////
    // Make sure Result is not the same object
    // as L or R.
    /////////////////////////////////////

    // Result.X = a1w2 + w1a2 + b1c2 + -c1b2;
    // Result.Y = -a1c2 + b1w2 + c1a2 + w1b2;
    // Result.Z = a1b2 + -b1a2 + c1w2 + w1c2;
    // Result.W = -a1a2 + -b1b2 + -c1c2 + w1w2;

    Result.X =  (L.X * R.W) +  (L.W * R.X) +  (L.Y * R.Z) + (-L.Z * R.Y);
    Result.Y = (-L.X * R.Z) +  (L.Y * R.W) +  (L.Z * R.X) +  (L.W * R.Y);
    Result.Z =  (L.X * R.Y) + (-L.Y * R.X) +  (L.Z * R.W) +  (L.W * R.Z);
    Result.W = (-L.X * R.X) + (-L.Y * R.Y) + (-L.Z * R.Z) +  (L.W * R.W);

    ///////////////////////
    // Test:
    Quaternion ResultTest = new Quaternion();
    Quaternion LeftTest = new Quaternion();
    Quaternion RightTest = new Quaternion();

    LeftTest.X = L.X;
    LeftTest.Y = L.Y;
    LeftTest.Z = L.Z;
    LeftTest.W = L.W;

    RightTest.X = R.X;
    RightTest.Y = R.Y;
    RightTest.Z = R.Z;
    RightTest.W = R.W;

    ResultTest = Quaternion.Multiply( LeftTest, RightTest );

    // How small can this be?
    double SmallNumber = 0.00000000001d;

    if( !DoubleIsAlmostEqual( ResultTest.X, Result.X, SmallNumber ))
      throw( new Exception( "Multiply: ResultTest.X != Result.X." ));

    if( !DoubleIsAlmostEqual( ResultTest.Y, Result.Y, SmallNumber ))
      throw( new Exception( "Multiply: ResultTest.Y != Result.Y." ));

    if( !DoubleIsAlmostEqual( ResultTest.Z, Result.Z, SmallNumber ))
      throw( new Exception( "Multiply: ResultTest.Z != Result.Z." ));

    if( !DoubleIsAlmostEqual( ResultTest.W, Result.W, SmallNumber ))
      throw( new Exception( "Multiply: ResultTest.W != Result.W." ));

    /////////////////////////
    }



  internal static void MultiplyWithLeftVector3( ref QuaternionRec Result, ref Vector3.Vector L, ref QuaternionRec R )
    {
    //////////////////////////////////////
    // Make sure Result is not the same object
    // as L or R.
    /////////////////////////////////////

    // Result.X =  (L.X * R.W) +  (0 * R.X) +  (L.Y * R.Z) + (-L.Z * R.Y);
    // Result.Y = (-L.X * R.Z) +  (L.Y * R.W) +  (L.Z * R.X) +  (0 * R.Y);
    // Result.Z =  (L.X * R.Y) + (-L.Y * R.X) +  (L.Z * R.W) +  (0 * R.Z);
    // Result.W = (-L.X * R.X) + (-L.Y * R.Y) + (-L.Z * R.Z) +  (0 * R.W);

    Result.X =  (L.X * R.W) +  (L.Y * R.Z) + (-L.Z * R.Y);
    Result.Y = (-L.X * R.Z) +  (L.Y * R.W) +  (L.Z * R.X);
    Result.Z =  (L.X * R.Y) + (-L.Y * R.X) +  (L.Z * R.W);
    Result.W = (-L.X * R.X) + (-L.Y * R.Y) + (-L.Z * R.Z);

    ///////////////////////
    // Test:
    Quaternion ResultTest = new Quaternion();
    Quaternion LeftTest = new Quaternion();
    Quaternion RightTest = new Quaternion();

    LeftTest.X = L.X;
    LeftTest.Y = L.Y;
    LeftTest.Z = L.Z;
    LeftTest.W = 0; // L.W;

    RightTest.X = R.X;
    RightTest.Y = R.Y;
    RightTest.Z = R.Z;
    RightTest.W = R.W;

    ResultTest = Quaternion.Multiply( LeftTest, RightTest );

    // How small can this be?
    double SmallNumber = 0.001d;

    if( !DoubleIsAlmostEqual( ResultTest.X, Result.X, SmallNumber ))
      throw( new Exception( "MultiplyWithLeftVector3: ResultTest.X != Result.X." ));

    if( !DoubleIsAlmostEqual( ResultTest.Y, Result.Y, SmallNumber ))
      throw( new Exception( "MultiplyWithLeftVector3: ResultTest.Y != Result.Y." ));

    if( !DoubleIsAlmostEqual( ResultTest.Z, Result.Z, SmallNumber ))
      throw( new Exception( "MultiplyWithLeftVector3: ResultTest.Z != Result.Z." ));

    if( !DoubleIsAlmostEqual( ResultTest.W, Result.W, SmallNumber ))
      throw( new Exception( "MultiplyWithLeftVector3: ResultTest.W != Result.W." ));

    /////////////////////////
    }



  internal static void MultiplyWithResultVector3( ref Vector3.Vector Result, ref QuaternionRec L, ref QuaternionRec R )
    {
    //////////////////////////////////////
    // Make sure Result is not the same object
    // as L or R.  (It can't be here, since
    // it's a Vector3.)
    /////////////////////////////////////

    Result.X =  (L.X * R.W) +  (L.W * R.X) +  (L.Y * R.Z) + (-L.Z * R.Y);
    Result.Y = (-L.X * R.Z) +  (L.Y * R.W) +  (L.Z * R.X) +  (L.W * R.Y);
    Result.Z =  (L.X * R.Y) + (-L.Y * R.X) +  (L.Z * R.W) +  (L.W * R.Z);

    // It doesn't need this calculation:
    // Result.W = (-L.X * R.X) + (-L.Y * R.Y) + (-L.Z * R.Z) +  (L.W * R.W);

    ///////////////////////
    // Test:
    Quaternion ResultTest = new Quaternion();
    Quaternion LeftTest = new Quaternion();
    Quaternion RightTest = new Quaternion();

    LeftTest.X = L.X;
    LeftTest.Y = L.Y;
    LeftTest.Z = L.Z;
    LeftTest.W = L.W;

    RightTest.X = R.X;
    RightTest.Y = R.Y;
    RightTest.Z = R.Z;
    RightTest.W = R.W;

    ResultTest = Quaternion.Multiply( LeftTest, RightTest );

    // How small can this be?
    double SmallNumber = 0.00000000001d;

    if( !DoubleIsAlmostEqual( ResultTest.X, Result.X, SmallNumber ))
      throw( new Exception( "Multiply: ResultTest.X != Result.X." ));

    if( !DoubleIsAlmostEqual( ResultTest.Y, Result.Y, SmallNumber ))
      throw( new Exception( "Multiply: ResultTest.Y != Result.Y." ));

    if( !DoubleIsAlmostEqual( ResultTest.Z, Result.Z, SmallNumber ))
      throw( new Exception( "Multiply: ResultTest.Z != Result.Z." ));

    // if( !DoubleIsAlmostEqual( ResultTest.W, Result.W, SmallNumber ))
      // throw( new Exception( "Multiply: ResultTest.W != Result.W." ));

    /////////////////////////
    }



  internal static void SetAsRotation( ref QuaternionRec Result,
                                      ref QuaternionRec Axis,
                                      double Angle )
    {
    // Make sure it's a unit quaternion.
    Axis.W = 0;
    Normalize( ref Axis, ref Axis );

    double HalfAngle = Angle * 0.5d;
    double SineHalfAngle = Math.Sin( HalfAngle );
    double CosineHalfAngle = Math.Cos( HalfAngle );

    Result.X = Axis.X * SineHalfAngle;
    Result.Y = Axis.Y * SineHalfAngle;
    Result.Z = Axis.Z * SineHalfAngle;
    Result.W = CosineHalfAngle;
    }



  internal static void Rotate( ref QuaternionRec ResultPoint,
                               ref QuaternionRec RotationQ,
                               ref QuaternionRec InverseRotationQ,
                               ref QuaternionRec StartPoint,
                               ref QuaternionRec MiddlePoint )
    {
    // This function might be called millions of
    // times in a loop, so InverseRotationQ is
    // already set.

    // Make sure the Result of Multiply is not the
    // same object as Left or Right.

    Multiply( ref MiddlePoint, ref StartPoint, ref InverseRotationQ );
    Multiply( ref ResultPoint, ref RotationQ, ref MiddlePoint );
    }



  internal static void RotateVector3(
                     ref Vector3.Vector ResultPoint,
                     ref QuaternionRec RotationQ,
                     ref QuaternionRec InverseRotationQ,
                     ref Vector3.Vector StartPoint,
                     ref QuaternionRec MiddlePoint )
    {
    // This function might be called millions of
    // times in a loop, so InverseRotationQ is
    // already set.

    // Make sure the Result of Multiply is not the
    // same object as Left or Right.

    MultiplyWithLeftVector3( ref MiddlePoint, ref StartPoint, ref InverseRotationQ );
    MultiplyWithResultVector3( ref ResultPoint, ref RotationQ, ref MiddlePoint );
    }



  internal static void TestBasics()
    {
    QuaternionRec Result = new QuaternionRec();
    QuaternionRec L = new QuaternionRec();
    QuaternionRec R = new QuaternionRec();

    // Round off?
    L.X = 11.123456789;
    L.Y = 2.9876543;
    L.Z = 3.123987654321;
    L.W = 4.314159;

    R.X = 5.271;
    R.Y = 6.9702591234;
    R.Z = 7.1234587654321;
    R.W = 8.1726364567;

    Normalize( ref Result, ref R );
    Multiply( ref Result, ref L, ref R );
    Conjugate( ref Result, ref R );
    Inverse( ref Result, ref R );
    }



  }
}
