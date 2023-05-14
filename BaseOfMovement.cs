
using Support;
namespace Hexapod;
class BaseOfMovement
{
    /// <summary>
    /// void <c>SetLegUp</c> writing to port leg's angles {without ending (\r\n) }  while using translateVectorAngle send to newProjectionAngle and translateVector 0 (you dont need it)
    /// </summary>
    public static void SetLegUp(int groupIndex, int legInGroup, double newProjection, double newProjectionAngle, double translateVector, HexapodControll.walkState walkState, int T1, int T2, int T3, bool isThreepod, double translateVectorAngle = -1, double translateVectorAngleToAdd = -1)
    {
        int servoIndex1 = 0, servoIndex2 = 0, servoIndex3 = 0;
        if (isThreepod)
        {
            servoIndex2 = Info.ThreepodServoGroups[groupIndex][legInGroup][1];
            servoIndex3 = Info.ThreepodServoGroups[groupIndex][legInGroup][2];
            servoIndex1 = Info.ThreepodServoGroups[groupIndex][legInGroup][0];
        }
        else
        {
            servoIndex2 = Info.TwopodServoGroups[groupIndex][legInGroup][1];
            servoIndex3 = Info.TwopodServoGroups[groupIndex][legInGroup][2];
            servoIndex1 = Info.TwopodServoGroups[groupIndex][legInGroup][0];
        }

        Info.Porjections[servoIndex1] = newProjection;
        Console.WriteLine(servoIndex1 + "newProjectionAngle  " + newProjectionAngle);
        if (translateVectorAngle == -1)
            translateVectorAngle = Formulas.Formulas.TheoremSin(translateVector, newProjection, newProjectionAngle); //first angle

        (double, double) anglesDown = GetlegDownAngles(newProjection, HexapodControll.hight);
        (double, double) anglesUp = GetlegUpAngles(anglesDown.Item2, anglesDown.Item1);

        double angleToAdd = Math.Abs(Info.AllServos[servoIndex1].currentAngle - Info.AllServos[servoIndex1].minAngle);
        if (Info.AllServos[servoIndex1].currentAngle < Info.AllServos[servoIndex1].minAngle && Info.AllServos[servoIndex1].minAngle < Info.AllServos[servoIndex1].maxAngle ||
            Info.AllServos[servoIndex1].currentAngle > Info.AllServos[servoIndex1].minAngle && Info.AllServos[servoIndex1].minAngle > Info.AllServos[servoIndex1].maxAngle)
        {
            angleToAdd = -angleToAdd;
        }


        if ((walkState == HexapodControll.walkState.forward) ||
            (walkState == HexapodControll.walkState.left && (servoIndex1 == 2 || servoIndex1 == 5)) ||
            (walkState == HexapodControll.walkState.right && (servoIndex1 == 0 || servoIndex1 == 3)))
        {
            if (translateVectorAngleToAdd != -1)
                angleToAdd += translateVectorAngleToAdd;

            ServoPosWrite(servoIndex1, translateVectorAngle + angleToAdd, true);
        }
        else if ((walkState == HexapodControll.walkState.backward) ||
                (walkState == HexapodControll.walkState.left && (servoIndex1 == 0 || servoIndex1 == 3)) ||
                (walkState == HexapodControll.walkState.right && (servoIndex1 == 2 || servoIndex1 == 5)))
        {
            if (translateVectorAngleToAdd != -1)
                angleToAdd += translateVectorAngleToAdd;

            Console.WriteLine($"{servoIndex1} {-translateVectorAngle + angleToAdd}");
        }


        HexapodControll.port.Write($"T{T1}");
        ServoPosWrite(servoIndex2, anglesUp.Item1, true);
        HexapodControll.port.Write($"T{T2}");
        ServoPosWrite(servoIndex3, HexapodControll.secondUpAngle, true); //true
        HexapodControll.port.Write($"T{T3}");
    }


    /// <summary>
    /// void <c>SetLegDown</c> writing to port leg's angles {without ending(T...\r\n)}
    /// </summary>
    public static void SetLegDown(int groupIndex, int legInGroup, double newProjection, int T2, int T3, bool isThreepod, double angle = -1)
    {
        int servoIndex1 = 0, servoIndex2 = 0, servoIndex3 = 0;
        if (isThreepod)
        {
            servoIndex2 = Info.ThreepodServoGroups[groupIndex][legInGroup][1];
            servoIndex3 = Info.ThreepodServoGroups[groupIndex][legInGroup][2];
            servoIndex1 = Info.ThreepodServoGroups[groupIndex][legInGroup][0];
        }
        else
        {
            servoIndex2 = Info.TwopodServoGroups[groupIndex][legInGroup][1];
            servoIndex3 = Info.TwopodServoGroups[groupIndex][legInGroup][2];
            servoIndex1 = Info.TwopodServoGroups[groupIndex][legInGroup][0];
        }

        (double, double) anglesDown = GetlegDownAngles(newProjection, HexapodControll.hight);
        Info.Porjections[servoIndex1] = newProjection;

        ServoPosWrite(servoIndex2, anglesDown.Item1, true);
        HexapodControll.port.Write($"T{T2}");
        ServoPosWrite(servoIndex3, anglesDown.Item2, true);
        HexapodControll.port.Write($"T{T3}");
    }


    /// <summary>
    /// void <c>ServoPosWrite</c> writes to angle to port adding minAngle and loseAngle
    /// </summary>
    public static void ServoPosWrite(int servoIndex, double angle, bool addLoseAngle)
    {
        double pos;
        if (Info.AllServos[servoIndex].minAngle < Info.AllServos[servoIndex].maxAngle)
            pos = Info.AllServos[servoIndex].minAngle + angle;
        else
            pos = Info.AllServos[servoIndex].minAngle - angle;


        if (addLoseAngle && servoIndex >= 12 && servoIndex <= 17)
        {
            if (Info.AllServos[servoIndex].minAngle < Info.AllServos[servoIndex].maxAngle)
                pos = pos - HexapodControll.secondAngleToAdd;
            else
                pos = pos + HexapodControll.secondAngleToAdd;
        }

        Info.AllServos[servoIndex].currentAngle = pos;
        HexapodControll.port.Write($"#{Info.AllServos[servoIndex].pin}P{Math.Round(pos)}");
    }


    /// <summary>
    /// (double, double) <c>legUp</c> aAngle - supportLineAngle, bAngle - angle between hip and minPos; return firstAngle, secondAngle
    /// </summary>

    public static (double, double) GetlegUpAngles(double aAngle, double bAngle)
    {
        double firstAngle = HexapodControll.upAngle;
        double secondAngle = aAngle - HexapodControll.upAngle + bAngle;
        return (firstAngle, secondAngle);
    }


    /// <summary>
    /// (double, double) <c>legDown</c> return firstAngle, secondAngle
    /// </summary>
    public static (double, double) GetlegDownAngles(double newProjection, double hight)
    {
        double supportLine = Math.Pow(Math.Pow(newProjection, 2) + Math.Pow(hight, 2), 0.5);
        double supportLineAngle = Formulas.Formulas.TheoremCosAngle(HexapodControll.kneeLength, HexapodControll.hipLength, supportLine); //secondAngle
        double kneeAngle = Formulas.Formulas.TheoremSin(HexapodControll.kneeLength, supportLine, supportLineAngle);

        double newProjectionAngle = Formulas.Formulas.TheoremSin(newProjection, supportLine, 90 * HexapodControll.anglePerDegree);
        double firstAngle = kneeAngle + newProjectionAngle;

        return (firstAngle, supportLineAngle);
    }
}