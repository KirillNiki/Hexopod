using Support;

namespace Hexapod;

class WalkParts
{
    public static void SetLegGroupForwardUp(int groupIndex, double mainAngle, bool isThreepod, HexapodControll.turnRobot turnRobot)
    {
        int[][][] servoGroupsLink;
        if (isThreepod)
            servoGroupsLink = Info.ThreepodServoGroups;
        else
            servoGroupsLink = Info.TwopodServoGroups;


        for (int j = 0; j < servoGroupsLink[groupIndex].Length; j++)
        {
            mainAngle = mainAngle >= 360 ? mainAngle % 360 : mainAngle;
            int servoIndex1 = servoGroupsLink[groupIndex][j][0];
            double angleToAdd = Math.Abs(Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].minAngle) - (Info.DegreeAngles[groupIndex + (j * servoGroupsLink.Length)].Item2 - 90) * HexapodControll.anglePerDegree;
            double newProjectionAngle = 0;
            double newProjection = -1;
            HexapodControll.walkState walkState = HexapodControll.walkState.forward;

            double translateVector = TrenslateVectorCount(turnRobot, mainAngle, servoIndex1);


            if (mainAngle > 0 && mainAngle < 180)
            {
                walkState = HexapodControll.walkState.forward;
                newProjectionAngle = NewProjectionAngleCont(servoIndex1, walkState, mainAngle, angleToAdd);
            }
            else if (mainAngle < 360 && mainAngle > 180)
            {
                walkState = HexapodControll.walkState.backward;
                newProjectionAngle = NewProjectionAngleCont(servoIndex1, walkState, mainAngle, angleToAdd);
            }
            else if (mainAngle == 0)
            {
                walkState = HexapodControll.walkState.left;
                if (servoIndex1 == 1)
                    newProjection = HexapodControll.startProjection - translateVector;

                else if (servoIndex1 == 4)
                    newProjection = HexapodControll.startProjection + translateVector;

                else if (servoIndex1 == 0 || servoIndex1 == 3)
                    newProjectionAngle = NewProjectionAngleCont(servoIndex1, HexapodControll.walkState.backward, mainAngle, angleToAdd);

                else if (servoIndex1 == 2 || servoIndex1 == 5)
                    newProjectionAngle = NewProjectionAngleCont(servoIndex1, HexapodControll.walkState.forward, mainAngle, angleToAdd);
            }
            else if (mainAngle == 180)
            {
                walkState = HexapodControll.walkState.right;
                if (servoIndex1 == 1)
                    newProjection = HexapodControll.startProjection + translateVector;

                else if (servoIndex1 == 4)
                    newProjection = HexapodControll.startProjection - translateVector;

                else if (servoIndex1 == 0 || servoIndex1 == 3)
                    newProjectionAngle = NewProjectionAngleCont(servoIndex1, HexapodControll.walkState.forward, mainAngle, angleToAdd);

                else if (servoIndex1 == 2 || servoIndex1 == 5)
                    newProjectionAngle = NewProjectionAngleCont(servoIndex1, HexapodControll.walkState.backward, mainAngle, angleToAdd);
            }


            if (newProjection == -1)
                newProjection = Formulas.Formulas.TheoremCosLine(translateVector, HexapodControll.startProjection, newProjectionAngle);

            double translateVectorAngleToAdd = -1;
            if (mainAngle == 90 || mainAngle == 270)
                translateVectorAngleToAdd = Math.Abs(Info.AllServos[servoIndex1].currentAngle - Info.AllServos[servoIndex1].startAngle);
                
            BaseOfMovement.SetLegUp(groupIndex, j, newProjection, newProjectionAngle, translateVector, walkState, 50, 50, 100, isThreepod, -1, translateVectorAngleToAdd);
        }
    }

    private static double NewProjectionAngleCont(int servoIndex, HexapodControll.walkState walkState, double mainAngle, double angleToAdd)
    {
        double newProjectionAngle = 0;

        if (walkState == HexapodControll.walkState.forward)
        {
            if (servoIndex >= 0 && servoIndex <= 2)
                newProjectionAngle = mainAngle * HexapodControll.anglePerDegree + angleToAdd;

            else if (servoIndex >= 3 && servoIndex <= 5)
                newProjectionAngle = Math.Abs(180 - mainAngle) * HexapodControll.anglePerDegree + angleToAdd;
        }
        else if (walkState == HexapodControll.walkState.backward)
        {
            if (servoIndex >= 0 && servoIndex <= 2)
                newProjectionAngle = (360 - mainAngle) * HexapodControll.anglePerDegree - angleToAdd;

            else if (servoIndex >= 3 && servoIndex <= 5)
                newProjectionAngle = Math.Abs(mainAngle - 180) * HexapodControll.anglePerDegree - angleToAdd;
        }
        newProjectionAngle = newProjectionAngle > 360 * HexapodControll.anglePerDegree ?
                             newProjectionAngle % (360 * HexapodControll.anglePerDegree) : newProjectionAngle;

        return newProjectionAngle;
    }


    public static void SetLegGroupForwardDown(int groupIndex, bool isThreepod)
    {
        int[][][] servoGroupsLink;
        if (isThreepod)
            servoGroupsLink = Info.ThreepodServoGroups;
        else
            servoGroupsLink = Info.TwopodServoGroups;


        for (int j = 0; j < servoGroupsLink[groupIndex].Length; j++)
        {
            int servoIndex1 = servoGroupsLink[groupIndex][j][0];
            BaseOfMovement.SetLegDown(groupIndex, j, Info.Porjections[servoIndex1], 100, 100, isThreepod, HexapodControll.angle);
        }
    }



    public static void SetLegGroupBack(int groupIndex, double mainAngle, bool isThreepod, HexapodControll.turnRobot turnRobot)
    {
        int[][][] servoGroupsLink;
        if (isThreepod)
            servoGroupsLink = Info.ThreepodServoGroups;
        else
            servoGroupsLink = Info.TwopodServoGroups;


        for (int j = 0; j < servoGroupsLink[groupIndex].Length; j++)
        {
            mainAngle = mainAngle >= 360 ? mainAngle % 360 : mainAngle;
            int servoIndex1 = servoGroupsLink[groupIndex][j][0];
            double angleToAdd = Math.Abs(Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].minAngle) - (Info.DegreeAngles[servoIndex1].Item2 - 90) * HexapodControll.anglePerDegree;
            double kAngle = Info.DegreeAngles[servoIndex1].Item2 * HexapodControll.anglePerDegree - Math.Abs(Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].minAngle);

            double translateVector = TrenslateVectorCount(turnRobot, mainAngle, servoIndex1);

            double newProjectionAngle = 0;
            bool minusTranslateVectorAngle = false;
            if (servoIndex1 >= 0 && servoIndex1 <= 2)
            {
                if (mainAngle - angleToAdd / HexapodControll.anglePerDegree < 180)
                {
                    newProjectionAngle = (90 - mainAngle) * HexapodControll.anglePerDegree + kAngle;
                    minusTranslateVectorAngle = true;
                }
                else
                {
                    newProjectionAngle = (mainAngle - 90) * HexapodControll.anglePerDegree - kAngle;
                    minusTranslateVectorAngle = false;
                }
            }
            else if (servoIndex1 >= 3 && servoIndex1 <= 5)
            {
                if (mainAngle - angleToAdd / HexapodControll.anglePerDegree < 180)
                {
                    newProjectionAngle = (mainAngle - 90) * HexapodControll.anglePerDegree + kAngle;
                    minusTranslateVectorAngle = true;
                }
                else
                {
                    newProjectionAngle = (90 + 360 - mainAngle) * HexapodControll.anglePerDegree - kAngle;
                    minusTranslateVectorAngle = false;
                }
            }

            newProjectionAngle = newProjectionAngle > 360 * HexapodControll.anglePerDegree ?
                             newProjectionAngle % (360 * HexapodControll.anglePerDegree) : newProjectionAngle;

            double newProjection = Formulas.Formulas.TheoremCosLine(Info.Porjections[servoIndex1], translateVector, newProjectionAngle);
            double translateVectorAngle = Formulas.Formulas.TheoremSin(translateVector, newProjection, newProjectionAngle);

            if (minusTranslateVectorAngle)
                translateVectorAngle = -translateVectorAngle;
            translateVectorAngle += Math.Abs(Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].minAngle);

            BaseOfMovement.ServoPosWrite(servoIndex1, translateVectorAngle, false);
            BaseOfMovement.SetLegDown(groupIndex, j, newProjection, 100, 100, isThreepod, mainAngle);
        }
    }



    public static void SetLegGroupStart(int groupIndex, bool isThreepod)
    {
        int[][][] servoGroupsLink;
        if (isThreepod)
            servoGroupsLink = Info.ThreepodServoGroups;
        else
            servoGroupsLink = Info.TwopodServoGroups;


        for (int j = 0; j < servoGroupsLink[groupIndex].Length; j++)
        {
            int servoIndex1 = servoGroupsLink[groupIndex][j][0];
            double translateVectorAngle = Math.Abs(Info.AllServos[servoIndex1].startAngle - Info.AllServos[servoIndex1].minAngle);

            BaseOfMovement.ServoPosWrite(servoIndex1, translateVectorAngle, false);
            BaseOfMovement.SetLegDown(groupIndex, j, HexapodControll.startProjection, 100, 100, isThreepod, HexapodControll.angle);
        }
    }



    private static double TrenslateVectorCount(HexapodControll.turnRobot turnRobot, double mainAngle, int servoIndex1)
    {
        double translateVector = 0;
        if (turnRobot == HexapodControll.turnRobot.right)
        {
            if (mainAngle == 90)
            {
                if (servoIndex1 >= 0 && servoIndex1 <= 2)
                    translateVector = HexapodControll.minLegRadius;
                else if (servoIndex1 >= 3 && servoIndex1 <= 5)
                    translateVector = HexapodControll.legRadius;
            }
            else if (mainAngle == 270)
            {
                if (servoIndex1 >= 0 && servoIndex1 <= 2)
                    translateVector = HexapodControll.legRadius;
                else if (servoIndex1 >= 3 && servoIndex1 <= 5)
                    translateVector = HexapodControll.minLegRadius;
            }
        }
        else if (turnRobot == HexapodControll.turnRobot.left)
        {
            if (mainAngle == 90)
            {
                if (servoIndex1 >= 0 && servoIndex1 <= 2)
                    translateVector = HexapodControll.legRadius;
                else if (servoIndex1 >= 3 && servoIndex1 <= 5)
                    translateVector = HexapodControll.minLegRadius;
            }
            else if (mainAngle == 270)
            {
                if (servoIndex1 >= 0 && servoIndex1 <= 2)
                    translateVector = HexapodControll.minLegRadius;
                else if (servoIndex1 >= 3 && servoIndex1 <= 5)
                    translateVector = HexapodControll.legRadius;
            }
        }
        else if (turnRobot == HexapodControll.turnRobot.none)
        {
            translateVector = HexapodControll.legRadius;
        }

        return translateVector;
    }
}