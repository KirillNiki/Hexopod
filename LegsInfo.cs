
namespace Support;

public static class LegInfo
{
    public const int legsCount = 6;
    public enum walkLegStates { back, start, forwad };


    public static Leg[] Legs = new Leg[legsCount];
    public static walkLegStates[] WalkLegPairsStates = new walkLegStates[3];


    /// <summary>
    /// int [][] <c>TwopodLegGroups</c> represents leg groups
    /// </summary>
    public static readonly Leg[][] TwopodLegGroups = new Leg[legsCount / 2][]
    {
        new Leg[legsCount / 3] { Legs[0], Legs[3] },
        new Leg[legsCount / 3] { Legs[1], Legs[4] },
        new Leg[legsCount / 3] { Legs[2], Legs[5] },
    };


    /// <summary>
    /// int [][] <c>ThreepodLegGroups</c> represents leg groups
    /// </summary>
    public static readonly Leg[][] ThreepodLegGroups = new Leg[legsCount / 3][]
    {
        new Leg[legsCount / 2] { Legs[0], Legs[2], Legs[4] },
        new Leg[legsCount / 2] { Legs[1], Legs[3], Legs[5] },
    };


    public static void InitLegs()
    {
        for (int i = 0; i < Legs.Length; i++)
        {
            Leg leg = new Leg();
            leg.servo1 = Info.AllServos[i];
            leg.servo2 = Info.AllServos[i + legsCount];
            leg.servo3 = Info.AllServos[i + (legsCount * 2)];
            leg.degreeAngles = Info.DegreeAngles[i];

            Legs[i] = leg;
        }

        for (int i = 0; i < WalkLegPairsStates.Length; i++)
            WalkLegPairsStates[i] = walkLegStates.start;
    }
}
