using System.IO.Ports;
using Gamepad;
using Support;
using System.Timers;

namespace Hexapod;

class HexapodControll
{
    const int startVal = 0;
    static double lastAngle = -1;
    public static double angle = 270;
    static int x = startVal;
    static int y = startVal;
    public enum turnRobot { left, right, none }
    public static turnRobot turn = turnRobot.none;
    public static turnRobot lastTurn = turnRobot.none;


    public const double hipLength = 8.5;
    public const double kneeLength = 12;
    public static readonly double hight = InitHight();
    public const double legRadius = 4;
    public const double minLegRadius = 1;

    public const double anglePerDegree = 9.4;
    public const double upAngle = 1400;
    public const double secondUpAngle = 600;
    public const double secondAngleToAdd = 20 * anglePerDegree;
    public const double rotationAngle = 20;
    public const double hexapodRadius = 18;

    public const double centerDistanceToAngular = 8.5; // distance Center To Angular Servo
    public const double centerDistanceToCentral = 6.5; // distance Center To Central Servo
    public const double centerAngleToAngular = 30; // angle Between Center To Angular Servo And BotLength


    public static double startProjection;


    public enum walkState { forward, backward, left, right };
    public enum changeLegPosState { justDown, upAndDown };

    public static CPort port = new CPort(CPort.emulate);
    private static int walkIteration = 0;
    private static int rotateIteration = 0;
    private static System.Timers.Timer aTimer;
    static bool moveInProgress;

    static GamepadController gamepad = new GamepadController(CPort.emulate ? "" : "/dev/input/js0");
    private static void Main()
    {
        Console.CancelKeyPress += new ConsoleCancelEventHandler(MyCancelEventHandler);
        // Test();
        StandUp();
        moveInProgress = false;

        aTimer = new System.Timers.Timer();
        aTimer.Interval = 700;
        aTimer.Elapsed += MovementHandler;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;

        Thread thread = new Thread(() => ReadGamepadKeys());
        thread.Start();
    }

    private static void MyCancelEventHandler(object sender, ConsoleCancelEventArgs args) => port.Close();

    private static double InitHight()
    {
        double projectionStartAngle = Info.AllServos[Info.AllServos.Length - 1].startAngle;
        double projectionMinAngle = Info.AllServos[Info.AllServos.Length - 1].minAngle;
        double supportLineAngle = Math.Abs(projectionStartAngle - projectionMinAngle) + secondAngleToAdd;
        double supportLine = Formulas.Formulas.TheoremCosLine(kneeLength, hipLength, supportLineAngle);

        double kneeAngle = Formulas.Formulas.TheoremSin(kneeLength, supportLine, supportLineAngle);
        double projectionAngle = Math.Abs(Info.AllServos[11].minAngle - Info.AllServos[11].startAngle) - kneeAngle;
        double hightAngle = 90 * anglePerDegree - projectionAngle;

        return Formulas.Formulas.TheoremSinLine(hightAngle, supportLine, 90 * anglePerDegree);
    }


    static void MovementHandler(Object source, System.Timers.ElapsedEventArgs e)
    {
        if (moveInProgress == true) return;
        moveInProgress = true;

        try
        {
            // Console.WriteLine("move event {0} {1}", lastTurn, turn);

            if (turn != lastTurn && lastTurn != turnRobot.none)
            {
                // Console.WriteLine("SetStartPos>>>>>>>>>>>>>>>>>>>");
                SetStartPos(true);
                rotateIteration = 0;
                lastTurn = turn;
            }
            if (angle != -1)
            {
                Console.WriteLine("angle   " + angle);
                if (Math.Abs(angle - lastAngle) >= 10 && lastAngle != -1) // angle != lastAngle
                {
                    // Console.WriteLine("SetStartPos>>>>>>>>>>>>>>>>>>>");
                    SetStartPos(true);
                    walkIteration = 0;
                }
                lastAngle = angle;

                // Console.WriteLine("Walk>>>>>>>>>>>>>>>>>>>");
                if ((angle > 45 && angle < 135) || (angle > 225 && angle < 315))
                {
                    // Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>1");
                    TwopodWalk(walkIteration, angle, turn);
                }
                else
                {
                    // Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>2");
                    ThreepodWalk(walkIteration, angle, turn);
                }
                walkIteration++;
            }
            else if (turn != turnRobot.none)
            {
                if (turn != lastTurn)
                    rotateIteration = 0;

                lastTurn = turn;
                // Console.WriteLine("Rotate>>>>>>>>>>>>>>>>>>> {0} {1}", rotateIteration, turn);

                Rotate(rotateIteration, turn);
                rotateIteration++;
            }
            else if (angle != lastAngle && angle == -1)
            {
                // Console.WriteLine("SetStartPos>>>>>>>>>>>>>>>>>>>");
                SetStartPos(true);
                walkIteration = 0;
                lastAngle = angle;
            }
        }
        finally
        {
            moveInProgress = false;
        }
    }


    static void Test()
    {
        for (int i = 6; i < 12; i++)
            port.Write($"#{Info.AllServos[i].pin}P{Info.AllServos[i].maxAngle}");

        for (int i = 12; i < 18; i++)
            port.Write($"#{Info.AllServos[i].pin}P{Info.AllServos[i].minAngle}");

        port.Write($"T200\r\n");
        Thread.Sleep(300);

        // for (int i = 6; i < 12; i++)
        //     port.Write($"#{Info.AllServos[i].pin}P{Info.AllServos[i].startAngle}");

        // port.Write($"T2000\r\n");
        // Thread.Sleep(3000);

        // foreach (Info.ServoInfo servo in Info.AllServos)
        //     port.Write($"#{servo.pin}P{servo.startAngle}");
        // port.Write("T300\r\n");
        // Thread.Sleep(300);
    }


    static void ReadGamepadKeys()
    {
        using (gamepad)
        {
            gamepad.ButtonChanged += (object sender, ButtonEventArgs e) => { };
            gamepad.AxisChanged += (object sender, AxisEventArgs e) =>
            {
                Console.WriteLine($"Axis {e.Axis} Changed: {e.Value}");
                if (e.Axis == 0)
                    x = e.Value;
                else if (e.Axis == 1)
                    y = e.Value;
                else if (e.Axis == 2)
                {
                    if (e.Value > startVal + 1000)
                        turn = turnRobot.right;
                    else if (e.Value < startVal - 1000)
                        turn = turnRobot.left;
                    else
                        turn = turnRobot.none;
                }
                Console.WriteLine("turn  " + turn);
                Console.WriteLine("x     " + x);
                Console.WriteLine("y     " + y);

                if (x != startVal || y != startVal)
                {
                    if ((x >= startVal && y >= startVal) || (x <= startVal && y <= startVal))
                    {
                        if (x == startVal)
                            angle = 0;
                        else
                            angle = Math.Atan(Math.Abs(y - startVal) / Math.Abs(x - startVal)) * 180 / Math.PI;
                    }
                    else
                        angle = Math.Atan(Math.Abs(x - startVal) / Math.Abs(y - startVal)) * 180 / Math.PI;

                    if (x >= startVal && y < startVal)
                        angle += 90;
                    else if (x > startVal && y >= startVal)
                        angle += 180;
                    else if (x <= startVal && y > startVal)
                        angle += 270;
                }
                else
                    angle = -1;

            };

            Console.ReadLine();
        }
    }


    private static void SetStartPos(bool isThreepod)
    {
        for (int i = 0; i < Info.ThreepodServoGroups.Length; i++)
        {
            for (int j = 0; j < Info.ThreepodServoGroups[i].Length; j++)
            {
                int servoIndex1 = Info.ThreepodServoGroups[i][j][0];
                double translateVectorAngle = Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].currentAngle;
                if (Info.AllServos[servoIndex1].minAngle > Info.AllServos[servoIndex1].maxAngle)
                    translateVectorAngle = -translateVectorAngle;

                BaseOfMovement.SetLegUp(i, j, startProjection, 0, 0, walkState.forward, 200, 100, 200, isThreepod, translateVectorAngle);
            }
            port.Write("\r\n");
            Thread.Sleep(300);

            for (int j = 0; j < Info.ThreepodServoGroups[i].Length; j++)
            {
                int servoIndex1 = Info.ThreepodServoGroups[i][j][0];
                BaseOfMovement.SetLegDown(i, j, Info.Porjections[servoIndex1], 200, 200, isThreepod);
            }
            port.Write("\r\n");
            Thread.Sleep(300);
        }
    }


    private static void StandUp()
    {
        for (int i = 0; i < Info.AllServos.Length; i++)
        {
            Info.AllServos[i].currentAngle = Info.AllServos[i].startAngle;
        }

        for (int i = 0; i < 6; i++)
            port.Write($"#{Info.AllServos[i].pin}P{Info.AllServos[i].startAngle}");
        port.Write("T300\r\n");
        Thread.Sleep(2000);

        double projectionStartAngle = Info.AllServos[Info.AllServos.Length - 1].startAngle;
        double projectionMinAngle = Info.AllServos[Info.AllServos.Length - 1].minAngle;
        double supportLine = Formulas.Formulas.TheoremCosLine(kneeLength, hipLength, Math.Abs(projectionStartAngle - projectionMinAngle) + secondAngleToAdd);
        double projection = Math.Pow(Math.Pow(supportLine, 2) - Math.Pow(hight, 2), 0.5);

        startProjection = projection;
        for (int i = 0; i < Info.Porjections.Length; i++)
            Info.Porjections[i] = projection;

        (double, double) anglesDown = BaseOfMovement.GetlegDownAngles(projection, 0);
        (double, double) anglesUp = BaseOfMovement.GetlegUpAngles(anglesDown.Item2, anglesDown.Item1);


        for (int i = 12; i < 18; i++)
            BaseOfMovement.ServoPosWrite(i, secondUpAngle, true);
        port.Write("T200");

        for (int i = 6; i < 12; i++)
            BaseOfMovement.ServoPosWrite(i, anglesUp.Item1, false);
        port.Write("T200\r\n");
        Thread.Sleep(500);


        int addAngle = 60;
        for (int i = 6; i < 12; i++)
            BaseOfMovement.ServoPosWrite(i, anglesDown.Item1 - addAngle, false);
        port.Write("T200\r\n");
        Thread.Sleep(500);

        for (int i = 12; i < 18; i++)
            BaseOfMovement.ServoPosWrite(i, anglesDown.Item2, true);
        port.Write("T200\r\n");
        Thread.Sleep(1000);


        for (int i = 6; i < 18; i++)
        {
            double curAngle = Math.Abs(Info.AllServos[i].startAngle - Info.AllServos[i].minAngle);
            if (i >= 6 && i < 12)
                BaseOfMovement.ServoPosWrite(i, curAngle, false);
            else
                BaseOfMovement.ServoPosWrite(i, curAngle - 300, false);
        }
        port.Write("T500\r\n");
        Thread.Sleep(700);


        for (int i = 0; i < Info.AllServos.Length; i++)
        {
            double curAngle = Math.Abs(Info.AllServos[i].startAngle - Info.AllServos[i].minAngle);
            if (i >= 12 && i <= 17)
                BaseOfMovement.ServoPosWrite(i, curAngle, false);
            else
                BaseOfMovement.ServoPosWrite(i, curAngle, true);
        }
        port.Write("T1000\r\n");
        Thread.Sleep(1000);


        Console.WriteLine(hight);
    }



    /// <summary>
    /// void <c>Walk</c> mainAngle - angle between hexopod direction vector perpendicular and current direction
    /// </summary>
    private static void ThreepodWalk(int iteration, double mainAngle, turnRobot turnRobot)
    {
        for (int i = 0; i < Info.ThreepodServoGroups.Length; i++)
        {
            WalkParts.SetLegGroupForwardUp(i, mainAngle, true, turnRobot);

            int groupIndex = i == 0 ? 1 : 0;
            WalkParts.SetLegGroupBack(groupIndex, mainAngle, true, turnRobot);

            port.Write($"T200\r\n");
            Thread.Sleep(300);

            WalkParts.SetLegGroupForwardDown(i, true);
            port.Write($"\r\n");
            Thread.Sleep(300);
        }
    }




    /// <summary>
    /// void <c>Walk</c> mainAngle - angle between hexopod direction vector perpendicular and current direction
    /// </summary>
    private const int walkingTime = 100;
    private static void TwopodWalk(int iteration, double mainAngle, turnRobot turnRobot)
    {
        for (int i = 0; i < Info.TwopodServoGroups.Length; i++)
        {
            if (iteration == 0 && i == 0)
            {
                WalkParts.SetLegGroupForwardUp(i, mainAngle, false, turnRobot);
                port.Write($"T{walkingTime}\r\n");
                Thread.Sleep(walkingTime * 2);

                WalkParts.SetLegGroupForwardDown(i, false);
                port.Write($"T{walkingTime}\r\n");
                Thread.Sleep(walkingTime * 2);

                WalkParts.SetLegGroupForwardUp(i + 1, mainAngle, false, turnRobot);
                port.Write($"T{walkingTime}\r\n");
                Thread.Sleep(walkingTime * 2);

                WalkParts.SetLegGroupStart(i, false);
                WalkParts.SetLegGroupBack(i + 2, mainAngle, false, turnRobot);
                Info.WalkLegPairsStates[i] = Info.walkLegStates.start;
                Info.WalkLegPairsStates[i + 2] = Info.walkLegStates.back;

                port.Write($"T{walkingTime}\r\n");
                Thread.Sleep(walkingTime * 2);

                WalkParts.SetLegGroupForwardDown(i + 1, false);
                Info.WalkLegPairsStates[i + 1] = Info.walkLegStates.forwad;

                port.Write($"T{walkingTime}\r\n");
                Thread.Sleep(walkingTime * 2);
            }
            else
            {
                int backIndex = Array.FindIndex(Info.WalkLegPairsStates, 0, Info.WalkLegPairsStates.Length, (element) => element == Info.walkLegStates.back);
                int forwardIndex = Array.FindIndex(Info.WalkLegPairsStates, 0, Info.WalkLegPairsStates.Length, (element) => element == Info.walkLegStates.forwad);
                int startIndex = Array.FindIndex(Info.WalkLegPairsStates, 0, Info.WalkLegPairsStates.Length, (element) => element == Info.walkLegStates.start);


                WalkParts.SetLegGroupForwardUp(backIndex, mainAngle, false, turnRobot);

                port.Write($"T{walkingTime}\r\n");
                Thread.Sleep(walkingTime * 2);

                WalkParts.SetLegGroupStart(forwardIndex, false);
                WalkParts.SetLegGroupBack(startIndex, mainAngle, false, turnRobot);
                Info.WalkLegPairsStates[forwardIndex] = Info.walkLegStates.start;
                Info.WalkLegPairsStates[startIndex] = Info.walkLegStates.back;

                port.Write($"T{walkingTime}\r\n");
                Thread.Sleep(walkingTime * 2);

                WalkParts.SetLegGroupForwardDown(backIndex, false);
                Info.WalkLegPairsStates[backIndex] = Info.walkLegStates.forwad;

                port.Write($"T{walkingTime}\r\n");
                Thread.Sleep(walkingTime * 2);
            }
        }
    }



    static void Rotate(int iteration, turnRobot turnRobot)
    {
        if (iteration == 0)
        {
            for (int i = 0; i < Info.ThreepodServoGroups.Length; i++)
                PutLegGroupOnCircle(i, changeLegPosState.upAndDown);
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>");
        }

        double hexapodRadiusAngle = (180 - rotationAngle) / 2 * anglePerDegree;
        double translateVector = Formulas.Formulas.TheoremCosLine(hexapodRadius, hexapodRadius, rotationAngle * anglePerDegree);

        for (int i = 0; i < Info.ThreepodServoGroups.Length; i++)
        {
            for (int j = 0; j < Info.ThreepodServoGroups[i].Length; j++)
            {
                int servoIndex1 = Info.ThreepodServoGroups[i][j][0];
                double newProjectionAngle = 0;
                double centerToAngularAngle = -1;
                if (servoIndex1 != 1 && servoIndex1 != 4)
                {
                    centerToAngularAngle = Formulas.Formulas.TheoremCosAngle(Info.Porjections[servoIndex1], hexapodRadius, centerDistanceToAngular);
                    if (turnRobot == turnRobot.left)
                    {
                        if (servoIndex1 == 2 || servoIndex1 == 5)
                            newProjectionAngle = hexapodRadiusAngle - centerToAngularAngle;
                        else if (servoIndex1 == 0 || servoIndex1 == 3)
                            newProjectionAngle = hexapodRadiusAngle + centerToAngularAngle;
                    }
                    else if (turnRobot == turnRobot.right)
                    {
                        if (servoIndex1 == 2 || servoIndex1 == 5)
                            newProjectionAngle = hexapodRadiusAngle + centerToAngularAngle;
                        else if (servoIndex1 == 0 || servoIndex1 == 3)
                            newProjectionAngle = hexapodRadiusAngle - centerToAngularAngle;
                    }
                }
                else
                    newProjectionAngle = hexapodRadiusAngle;

                double newProjection = Formulas.Formulas.TheoremCosLine(translateVector, Info.Porjections[servoIndex1], newProjectionAngle);
                double translateVectorAngle = Formulas.Formulas.TheoremSin(translateVector, newProjection, newProjectionAngle);

                walkState turnLeg = walkState.forward;
                if (turnRobot == turnRobot.left)
                {
                    if (servoIndex1 >= 0 && servoIndex1 <= 2)
                        turnLeg = walkState.forward;
                    else if (servoIndex1 >= 3 && servoIndex1 <= 5)
                        turnLeg = walkState.backward;
                }
                else if (turnRobot == turnRobot.right)
                {
                    if (servoIndex1 >= 0 && servoIndex1 <= 2)
                        turnLeg = walkState.backward;
                    else if (servoIndex1 >= 3 && servoIndex1 <= 5)
                        turnLeg = walkState.forward;
                }
                BaseOfMovement.SetLegUp(i, j, newProjection, newProjectionAngle, translateVector, turnLeg, 100, 200, 200, true);
            }

            if (iteration + i != 0)
            {
                int groupIndex = i == 0 ? 1 : 0;
                PutLegGroupOnCircle(groupIndex, changeLegPosState.justDown);
            }
            port.Write("\r\n");
            Thread.Sleep(400);

            for (int j = 0; j < Info.ThreepodServoGroups[i].Length; j++)
            {
                int servoIndex1 = Info.ThreepodServoGroups[i][j][0];
                BaseOfMovement.SetLegDown(i, j, Info.Porjections[servoIndex1], 200, 200, true);
            }
            port.Write("\r\n");
            Thread.Sleep(400);
        }
    }


    static void PutLegGroupOnCircle(int groupIndex, changeLegPosState changeLegPosState)
    {
        for (int j = 0; j < Info.ThreepodServoGroups[groupIndex].Length; j++)
        {
            int servoIndex1 = Info.ThreepodServoGroups[groupIndex][j][0];
            Console.WriteLine("indexOfLeg" + servoIndex1);

            double newProjection = 0;
            double translateVectorAngle = 0;
            translateVectorAngle = Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].currentAngle; // can be negative

            if (servoIndex1 == 1 || servoIndex1 == 4)
            {
                newProjection = hexapodRadius - centerDistanceToCentral;
            }
            else
            {
                double hexapodRadiusAngle = 0;
                if (servoIndex1 == 2 || servoIndex1 == 3)
                    hexapodRadiusAngle = Math.Abs(Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].minAngle) + (180 - Info.DegreeAngles[servoIndex1].Item2 + centerAngleToAngular) * anglePerDegree;
                else if (servoIndex1 == 0 || servoIndex1 == 5)
                    hexapodRadiusAngle = (Info.DegreeAngles[servoIndex1].Item2 + centerAngleToAngular) * anglePerDegree - Math.Abs(Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].minAngle);

                double centerToAngularAngle = Formulas.Formulas.TheoremSin(centerDistanceToAngular, hexapodRadius, hexapodRadiusAngle);
                double newProjectionAngle = (180 * anglePerDegree) - centerToAngularAngle - hexapodRadiusAngle;
                newProjection = Formulas.Formulas.TheoremCosLine(centerDistanceToAngular, hexapodRadius, newProjectionAngle);
            }

            if (Info.AllServos[servoIndex1].minAngle > Info.AllServos[servoIndex1].maxAngle)
                translateVectorAngle = -translateVectorAngle;

            if (changeLegPosState == changeLegPosState.upAndDown)
                BaseOfMovement.SetLegUp(groupIndex, j, newProjection, 0, 0, walkState.forward, 200, 100, 100, true, translateVectorAngle);
            else if (changeLegPosState == changeLegPosState.justDown)
            {
                BaseOfMovement.ServoPosWrite(servoIndex1, Math.Abs(Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].minAngle), true);
                port.Write("T200");
                BaseOfMovement.SetLegDown(groupIndex, j, newProjection, 200, 200, true);
            }
        }
        port.Write("\r\n");
        Thread.Sleep(400);

        if (changeLegPosState == changeLegPosState.upAndDown)
        {
            for (int j = 0; j < Info.ThreepodServoGroups[groupIndex].Length; j++)
            {
                int servoIndex1 = Info.ThreepodServoGroups[groupIndex][j][0];
                BaseOfMovement.SetLegDown(groupIndex, j, Info.Porjections[servoIndex1], 200, 200, true);
            }
            port.Write("\r\n");
            Thread.Sleep(300);
        }
    }
}
