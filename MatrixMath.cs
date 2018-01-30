// Copyright Eric Chauvin 2018.
// My blog is at:
// ericsourcecode.blogspot.com


using System;
// using System.Text;


namespace ClimateModel
{
  static class MatrixMath
  {

  public struct Matrix3
    {
    // First Row:
    public double M11;
    public double M12;
    public double M13;

    // Second Row:
    public double M21;
    public double M22;
    public double M23;

    public double M31;
    public double M32;
    public double M33;
    }


  public struct Matrix4
    {
    public double M11;
    public double M12;
    public double M13;
    public double M14;

    public double M21;
    public double M22;
    public double M23;
    public double M24;

    public double M31;
    public double M32;
    public double M33;
    public double M34;

    public double M41;
    public double M42;
    public double M43;
    public double M44;
    }



  // https://en.wikipedia.org/wiki/Matrix_multiplication

  internal static void MultiplyMatrix3( ref Matrix3 Result, ref Matrix3 A, ref Matrix3 B )
    {
    // Result = AB.
    // First Row, first Column.
    Result.M11 = (A.M11 * B.M11) +
                 (A.M12 * B.M21) +
                 (A.M13 * B.M31);

    // First Row, Second Column.
    Result.M12 = (A.M11 * B.M12) +
                 (A.M12 * B.M22) +
                 (A.M13 * B.M32);

    Result.M13 = (A.M11 * B.M13) +
                 (A.M12 * B.M23) +
                 (A.M13 * B.M33);

    // Second Row.
    Result.M21 = (A.M21 * B.M11) +
                 (A.M22 * B.M21) +
                 (A.M23 * B.M31);

    Result.M22 = (A.M21 * B.M12) +
                 (A.M22 * B.M22) +
                 (A.M23 * B.M32);

    Result.M23 = (A.M21 * B.M13) +
                 (A.M22 * B.M23) +
                 (A.M23 * B.M33);

    Result.M31 = (A.M31 * B.M11) +
                 (A.M32 * B.M21) +
                 (A.M33 * B.M31);

    Result.M32 = (A.M31 * B.M12) +
                 (A.M32 * B.M22) +
                 (A.M33 * B.M32);

    Result.M33 = (A.M31 * B.M13) +
                 (A.M32 * B.M23) +
                 (A.M33 * B.M33);

    }



  internal static void MultiplyMatrix4( ref Matrix4 Result, ref Matrix4 A, ref Matrix4 B )
    {
    // Result = AB.
    Result.M11 = (A.M11 * B.M11) +
                 (A.M12 * B.M21) +
                 (A.M13 * B.M31) +
                 (A.M14 * B.M41);

    Result.M12 = (A.M11 * B.M12) +
                 (A.M12 * B.M22) +
                 (A.M13 * B.M32) +
                 (A.M14 * B.M42);

    Result.M13 = (A.M11 * B.M13) +
                 (A.M12 * B.M23) +
                 (A.M13 * B.M33) +
                 (A.M14 * B.M43);

    Result.M14 = (A.M11 * B.M14) +
                 (A.M12 * B.M24) +
                 (A.M13 * B.M34) +
                 (A.M14 * B.M44);

    Result.M21 = (A.M21 * B.M11) +
                 (A.M22 * B.M21) +
                 (A.M23 * B.M31) +
                 (A.M24 * B.M41);

    Result.M22 = (A.M21 * B.M12) +
                 (A.M22 * B.M22) +
                 (A.M23 * B.M32) +
                 (A.M24 * B.M42);

    Result.M23 = (A.M21 * B.M13) +
                 (A.M22 * B.M23) +
                 (A.M23 * B.M33) +
                 (A.M24 * B.M43);

    Result.M24 = (A.M21 * B.M14) +
                 (A.M22 * B.M24) +
                 (A.M23 * B.M34) +
                 (A.M24 * B.M44);

    Result.M31 = (A.M31 * B.M11) +
                 (A.M32 * B.M21) +
                 (A.M33 * B.M31) +
                 (A.M34 * B.M41);

    Result.M32 = (A.M31 * B.M12) +
                 (A.M32 * B.M22) +
                 (A.M33 * B.M32) +
                 (A.M34 * B.M42);

    Result.M33 = (A.M31 * B.M13) +
                 (A.M32 * B.M23) +
                 (A.M33 * B.M33) +
                 (A.M34 * B.M43);

    Result.M34 = (A.M31 * B.M14) +
                 (A.M32 * B.M24) +
                 (A.M33 * B.M34) +
                 (A.M34 * B.M44);

    Result.M41 = (A.M41 * B.M11) +
                 (A.M42 * B.M21) +
                 (A.M43 * B.M31) +
                 (A.M44 * B.M41);

    Result.M42 = (A.M41 * B.M12) +
                 (A.M42 * B.M22) +
                 (A.M43 * B.M32) +
                 (A.M44 * B.M42);

    Result.M43 = (A.M41 * B.M13) +
                 (A.M42 * B.M23) +
                 (A.M43 * B.M33) +
                 (A.M44 * B.M43);

    Result.M44 = (A.M41 * B.M14) +
                 (A.M42 * B.M24) +
                 (A.M43 * B.M34) +
                 (A.M44 * B.M44);

    }




    // These three "elemental" matrices are like the
    // orthogonal basis vectors in Linear Algebra.
    // X:  1      0      0
    // Y:  0      1      0
    // Z:  0      0      1

    // Rotate around X:
    //  1      0      0
    //  0   CosA  -SinA
    //  0   SinA   CosA

    /*
    // X Matrix:
    MatrixX.M11 = 1;
    MatrixX.M12 = 0;
    MatrixX.M13 = 0;

    MatrixX.M21 = 0;
    MatrixX.M22 = CosX;
    MatrixX.M23 = -SinX;

    MatrixX.M31 = 0;
    MatrixX.M32 = SinX;
    MatrixX.M33 = CosX;


    // Rotate around Y:
    //  CosA   0   SinA
    //     0   1      0
    //  -SinA  0   CosA

    // Y Matrix:
    MatrixY.M11 = CosY;
    MatrixY.M12 = 0;
    MatrixY.M13 = SinY;

    MatrixY.M21 = 0;
    MatrixY.M22 = 1;
    MatrixY.M23 = 0;

    MatrixY.M31 = -SinY;
    MatrixY.M32 = 0;
    MatrixY.M33 = CosY;


    // Rotate around Z:
    //  CosA  -SinA   0
    //  SinA   CosA   0
    //     0      0   1

    // Z Matrix:
    MatrixZ.M11 = CosZ;
    MatrixZ.M12 = -SinZ;
    MatrixZ.M13 = 0;

    MatrixZ.M21 = SinZ;
    MatrixZ.M22 = CosZ;
    MatrixZ.M23 = 0;

    MatrixZ.M31 = 0;
    MatrixZ.M32 = 0;
    MatrixZ.M33 = 1;
    */



  }
}






