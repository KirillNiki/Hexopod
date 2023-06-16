namespace Support;

public static class Info
{
    public struct ServoInfo
    {
        public double pin { get; init; }
        public double minAngle { get; init; }
        public double maxAngle { get; init; }
        public double startAngle { get; init; }
        public double currentAngle { get; set; }
    }


    public static ServoInfo[] AllServos = new ServoInfo[LegInfo.legsCount * 3]
    {
        new ServoInfo() with { pin = 6, minAngle = 1550, maxAngle = 1000, startAngle = 1550 , currentAngle = 1200 },  //0
        new ServoInfo() with { pin = 9, minAngle = 1900, maxAngle = 1100, startAngle = 1450 , currentAngle = 1350 },  //1
        new ServoInfo() with { pin = 12, minAngle = 1900, maxAngle = 1350, startAngle = 1300 , currentAngle = 1600 }, //2
        new ServoInfo() with { pin = 21, minAngle = 1000, maxAngle = 1550, startAngle = 1600 , currentAngle = 1200 }, //3
        new ServoInfo() with { pin = 24, minAngle = 1000, maxAngle = 1800, startAngle = 1450 , currentAngle = 1550 }, //4
        new ServoInfo() with { pin = 27, minAngle = 1450, maxAngle = 2000, startAngle = 1450 , currentAngle = 1700 }, //5
        new ServoInfo() with { pin = 5, minAngle = 2250, maxAngle = 650, startAngle = 1450 , currentAngle = 1750 },   //6
        new ServoInfo() with { pin = 8, minAngle = 2350, maxAngle = 750, startAngle = 1550 , currentAngle = 1350 },   //7
        new ServoInfo() with { pin = 11, minAngle = 2350, maxAngle = 750, startAngle = 1550 , currentAngle = 3500 },  //8
        new ServoInfo() with { pin = 22, minAngle = 550, maxAngle = 2150, startAngle = 1350 , currentAngle = 1550 },  //9
        new ServoInfo() with { pin = 25, minAngle = 550, maxAngle = 2150, startAngle = 1350 , currentAngle = 1550 },  //10
        new ServoInfo() with { pin = 28, minAngle = 600, maxAngle = 2200, startAngle = 1400 , currentAngle = 1600 },  //11
        new ServoInfo() with { pin = 4, minAngle = 750, maxAngle = 2350, startAngle = 1500 , currentAngle = 1450 },   //12
        new ServoInfo() with { pin = 7, minAngle = 800, maxAngle = 2400, startAngle = 1550 , currentAngle = 1500 },   //13
        new ServoInfo() with { pin = 10, minAngle = 800, maxAngle = 2400, startAngle = 1550 , currentAngle = 1500 },  //14
        new ServoInfo() with { pin = 23, minAngle = 2150, maxAngle = 550, startAngle = 1400 , currentAngle = 1450 },  //15
        new ServoInfo() with { pin = 26, minAngle = 2150, maxAngle = 550, startAngle = 1400 , currentAngle = 1450 },  //16
        new ServoInfo() with { pin = 29, minAngle = 2100, maxAngle = 500, startAngle = 1350 , currentAngle = 1400 }   //17
    };


    /// <summary>
    /// (int, int, int)[] <c>DegreeAngles</c> for first 6 servo which level is first,  represents angle betwin forwardRey and rey that you want: (servoIndex, minAngle, maxAngle)
    /// </summary>
    public static readonly (int, int, int)[] DegreeAngles = new (int, int, int)[LegInfo.legsCount] {
        new (0, 145, 85),
        new (1, 130, 45),
        new (2, 95, 50),
        new (3, 95, 50),
        new (4, 130, 45),
        new (5, 145, 85)
    };
}
