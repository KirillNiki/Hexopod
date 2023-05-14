namespace Formulas;
using Hexapod;

class Formulas
{
    /// <summary>
    /// double <c>TheoremCosLine</c> return cLine , cAngle - degree*anglePerDegree
    /// </summary>
    public static double TheoremCosLine(double aLine, double bLine, double cAngle)
    {
        return Math.Pow(Math.Pow(aLine, 2) + Math.Pow(bLine, 2) - 2 * aLine * bLine * Math.Cos(DegreeToRad(cAngle)), 0.5);
    }
    /// <summary>
    /// double <c>TheoremCosAngle</c> return cAngle in degree*anglePerDegree
    /// </summary>
    public static double TheoremCosAngle(double aLine, double bLine, double cLine)
    {
        return RadToDegree(Math.Acos((Math.Pow(aLine, 2) + Math.Pow(bLine, 2) - Math.Pow(cLine, 2)) / (2 * aLine * bLine)));
    }

    /// <summary>
    /// double <c>TheoremSin</c> return aAngle in degree*anglePerDegree
    /// </summary>
    public static double TheoremSin(double aLine, double bLine, double bAngle)
    {
        return RadToDegree(Math.Asin((aLine * Math.Sin(DegreeToRad(bAngle))) / bLine));
    }


    /// <summary>
    /// double <c>TheoremSinLine</c> return aLine
    /// </summary>
    public static double TheoremSinLine(double aAngle, double bLine, double bAngle)
    {
        return (bLine * Math.Sin(DegreeToRad(aAngle))) / Math.Sin(DegreeToRad(bAngle));
    }


    public static double RadToDegree(double input)
    {
        return input * HexapodControll.anglePerDegree * (180 / Math.PI);
    }

    public static double DegreeToRad(double input)
    {
        return input / HexapodControll.anglePerDegree * (Math.PI / 180);
    }
}