using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterationSetter : MonoBehaviour {
    public static RuleSet Quaternions = new RuleSet(
        IterationSetter.toMat(1, 0, 0, 0,
                              0, -1, 0, 0,
                              0, 0, -1, 0,
                              0, 0, 0, -1),
        IterationSetter.toMat(0, 1, 0, 0,
                              1, 0, 0, 0,
                              0, 0, 0, 1,
                              0, 0, -1, 0),
        IterationSetter.toMat(0, 0, 1, 0,
                              0, 0, 0, -1,
                              1, 0, 0, 0,
                              0, 1, 0, 0),
        IterationSetter.toMat(0, 0, 0, 1,
                              0, 0, 1, 0,
                              0, -1, 0, 0,
                              1, 0, 0, 0));
    public static RuleSet Missile = new RuleSet(
        IterationSetter.toMat(1, 0, 0, 0,
                              0, -1, 0, 0,
                              0, 0, -1, 0,
                              0, 0, 0, -1),
        IterationSetter.toMat(0, 1, 0, 0,
                             -1, 0, 0, 0,
                              0, 0, 0, 1,
                              0, 0, 1, 0),
        IterationSetter.toMat(0, 0, 1, 0,
                              0, 0, 0, 1,
                             -1, 0, 0, 0,
                              0, 1, 0, 0),
        IterationSetter.toMat(0, 0, 0, 1,
                              0, 0, 1, 0,
                              0, 1, 0, 0,
                             -1, 0, 0, 0));
    public static RuleSet TopHeavyCity = new RuleSet(
        IterationSetter.toMat(1, 0, 0, 0,
                              0,-1, 0, 0,
                              0, 0,-1, 0,
                              0, 0, 0, 1),
        IterationSetter.toMat(0, 1, 0, 0,
                              1, 0, 0, 0,
                              0, 0, 0, 1,
                              0, 0, 1, 0),
        IterationSetter.toMat(0, 0, 1, 0,
                              0, 0, 0, 1,
                              1, 0, 0, 0,
                              0, 1, 0, 0),
        IterationSetter.toMat(0, 0, 0, 1,
                              0, 0,-1, 0,
                              0,-1, 0, 0,
                              1, 0, 0, 0));
    public static RuleSet HungryMound = new RuleSet(
        IterationSetter.toMat(1, 0, 0, 0,
                              0, -1, 0, 0,
                              0, 0, -1, 0,
                              0, 0, 0, 1),
        IterationSetter.toMat(0, 1, 0, 0,
                              1, 0, 0, 0,
                              0, 0, 0, 1,
                              0, 0, 1, 0),
        IterationSetter.toMat(0, 0, 1, 0,
                              0, 0, 0, 1,
                              1, 0, 0, 0,
                              0, 1, 0, 0),
        IterationSetter.toMat(0, 0, 0, 1,
                              0, 0, 1, 0,
                              0, 1, 0, 0,
                              1, 0, 0, 0));
    public static RuleSet AncientTemple = new RuleSet(
        IterationSetter.toMat(1, 0, 0, 0,
                              0, -1, 0, 0,
                              0, 0, -1, 0,
                              0, 0, 0, -1),
        IterationSetter.toMat(0, 1, 0, 0,
                              1, 0, 0, 0,
                              0, 0, 0, 1,
                              0, 0, 1, 0),
        IterationSetter.toMat(0, 0, 1, 0,
                              0, 0, 0, 1,
                              1, 0, 0, 0,
                              0, 1, 0, 0),
        IterationSetter.toMat(0, 0, 0, 1,
                              0, 0, -1, 0,
                              0, -1, 0, 0,
                              1, 0, 0, 0));
    public static RuleSet Erruption = new RuleSet(
        IterationSetter.toMat(1, 0, 0, 0,
                              0, 1, 0, 0,
                              0, 0, 1, 0,
                              0, 0, 0, 1),
        IterationSetter.toMat(0, 1, 0, 0,
                              1, 0, 0, 0,
                              0, 0, 0, 1,
                              0, 0, 1, 0),
        IterationSetter.toMat(0, 0, 1, 0,
                              0, 0, 0, 1,
                              1, 0, 0, 0,
                              0, 1, 0, 0),
        IterationSetter.toMat(0, 0, 0, 1,
                              0, 0, 1, 0,
                              0, 1, 0, 0,
                              1, 0, 0, 0));
    public static RuleSet RuinedCity = new RuleSet(
        IterationSetter.toMat(1, 0, 0, 0,
                              0, -1, 0, 0,
                              0, 0, -1, 0,
                              0, 0, 0, -1),
        IterationSetter.toMat(0, 1, 0, 0,
                              1, 0, 0, 0,
                              0, 0, 0, 1,
                              0, 0, 1, 0),
        IterationSetter.toMat(0, 0, 1, 0,
                              0, 0, 0, 1,
                              1, 0, 0, 0,
                              0, 1, 0, 0),
        IterationSetter.toMat(0, 0, 0, 1,
                              0, 0, 1, 0,
                              0, 1, 0, 0,
                              1, 0, 0, 0));

    public static Polynomial Squared = new Polynomial(new Vector4(1, 0, 0, 0),
        new Vector4(0, 0, 0, 0),
        new Vector4(1, 0, 0, 0),
        new Vector4(0, 0, 0, 0));
    public static Polynomial Cubed = new Polynomial(new Vector4(1, 0, 0, 0),
        new Vector4(0, 0, 0, 0),
        new Vector4(0, 0, 0, 0),
        new Vector4(1, 0, 0, 0));
    public static Polynomial SquaredYCubed = new Polynomial(new Vector4(1, 0, 0, 0),
        new Vector4(0, 0, 0, 0),
        new Vector4(1, 0, 0, 0),
        new Vector4(1, 0, 0, 0));

    public MultTechnique myMult = MultTechnique.Random;
    public ChosenPolynomial myPoly = ChosenPolynomial.Random;

    public enum ChosenPolynomial
    {
        Squared,
        Cubed,
        SquaredYCubed,
        Random
    }

    public enum MultTechnique
    {
        Quaternion,
        Random,
        RuinedCity,
        Erruption,
        AncientTemple,
        TopHeavyCity,
        HungryMound,
        Missile
    }

    public void setRules(CallIteration toSet)
    {
        switch (myMult)
        {
            default:
            case MultTechnique.Random:
                toSet.multRule = randomRules();
                break;
            case MultTechnique.Quaternion:
                toSet.multRule = IterationSetter.Quaternions;
                break;
            case MultTechnique.RuinedCity:
                toSet.multRule = IterationSetter.RuinedCity;
                break;
            case MultTechnique.Erruption:
                toSet.multRule = IterationSetter.Erruption;
                break;
            case MultTechnique.AncientTemple:
                toSet.multRule = IterationSetter.AncientTemple;
                break;
            case MultTechnique.TopHeavyCity:
                toSet.multRule = IterationSetter.AncientTemple;
                break;
            case MultTechnique.HungryMound:
                toSet.multRule = IterationSetter.HungryMound;
                break;
            case MultTechnique.Missile:
                toSet.multRule = IterationSetter.Missile;
                break;
        }

        switch (myPoly)
        {
            default:
            case ChosenPolynomial.Random:
                toSet.poly = randomPoly();
                break;
            case ChosenPolynomial.Squared:
                toSet.poly = Squared;
                break;
               case ChosenPolynomial.SquaredYCubed:
                toSet.poly = SquaredYCubed;
                break;
        }

        toSet.init();
    }

    public RuleSet randomRules()
    {
        return new RuleSet(randomMat(), randomMat(), randomMat(), randomMat());
    }
    public Polynomial randomPoly()
    {
        return new Polynomial(randomVect(), randomVect(), randomVect(), randomVect());
    }

    public Vector4 randomVect()
    {
        return new Vector4(rF(), rF(), rF(), rF()).normalized*rF();
    }

    private float rF()
    {
        return Random.Range(-1, 1);
    }

    public float matrixNormed (Matrix4x4 mat)
    {
        return Mathf.Sqrt(
            mat.m00 * mat.m00 + mat.m01 * mat.m01 + mat.m02 * mat.m02 + mat.m03 * mat.m03 +
            mat.m10 * mat.m10 + mat.m11 * mat.m11 + mat.m12 * mat.m12 + mat.m13 * mat.m13 +
            mat.m20 * mat.m20 + mat.m21 * mat.m21 + mat.m22 * mat.m22 + mat.m23 * mat.m23 +
            mat.m30 * mat.m30 + mat.m31 * mat.m31 + mat.m32 * mat.m32 + mat.m33 * mat.m33);
    }
    
    public Matrix4x4 randomMat()
    {
        Matrix4x4 mat =  toMat(rF(), rF(), rF(), rF(),
            rF(), rF(), rF(), rF(),
            rF(), rF(), rF(), rF(),
            rF(), rF(), rF(), rF());
        float normedValue = matrixNormed(mat);
        float scaler = 6f * rF() / normedValue;
        return toMat(mat.m00* scaler, mat.m01 * scaler, mat.m02 * scaler, mat.m03 * scaler,
            mat.m10 * scaler, mat.m11 * scaler, mat.m12 * scaler, mat.m13 * scaler,
            mat.m20 * scaler, mat.m21 * scaler, mat.m22 * scaler, mat.m23 * scaler,
            mat.m30 * scaler, mat.m31 * scaler, mat.m32 * scaler, mat.m33 * scaler);


    }

    public class Polynomial {
        public Vector4 co0;
        public Vector4 co1;
        public Vector4 co2;
        public Vector4 co3;

        public Polynomial(Vector4 co0, Vector4 co1, Vector4 co2, Vector4 co3)
        {
            this.co0 = co0;
            this.co1 = co1;
            this.co2 = co2;
            this.co3 = co3;
        }
    }

    public class RuleSet
    {
        public Matrix4x4 Hresult;
        public Matrix4x4 Iresult;
        public Matrix4x4 Jresult;
        public Matrix4x4 Kresult;

        public RuleSet(Matrix4x4 hMat, Matrix4x4 IMat, Matrix4x4 JMat, Matrix4x4 KMat)
        {
            Hresult = hMat;
            Iresult = IMat;
            Jresult = JMat;
            Kresult = KMat;
        }
    }

    public static Matrix4x4 toMat(float m00, float m01, float m02, float m03,
        float m10, float m11, float m12, float m13,
        float m20, float m21, float m22, float m23,
        float m30, float m31, float m32, float m33)
    {
        Matrix4x4 toReturn = new Matrix4x4();
        toReturn.m00 = m00; toReturn.m01 = m01;
        toReturn.m02 = m02; toReturn.m03 = m03;

        toReturn.m10 = m10; toReturn.m11 = m11;
        toReturn.m12 = m12; toReturn.m13 = m13;

        toReturn.m20 = m20; toReturn.m21 = m21;
        toReturn.m22 = m22; toReturn.m23 = m23;

        toReturn.m30 = m30; toReturn.m31 = m31;
        toReturn.m32 = m32; toReturn.m33 = m33;
        return toReturn;
    }
}
