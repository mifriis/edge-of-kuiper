using System;

public class GameTime
{
    private readonly DateTime GameStartDate = new DateTime(2078, 1, 1);
    private const long tickAccelerationConstant = 7; //1 real day, is 7 game days
    public DateTime RealGameStartTime {get;}

    public DateTime GetCurrentInGameTime()
    {
        var elapsedRealTime = DateTime.Now.Subtract(RealGameStartTime);

        var elapsedGameTime = new TimeSpan(elapsedRealTime.Ticks * tickAccelerationConstant);

        return GameStartDate.Add(elapsedGameTime);
    }

    public GameTime(DateTime gameStartTime)
    {
        RealGameStartTime = gameStartTime;
    }

    public bool IsBefore(DateTime otherTime) => GetCurrentInGameTime() < otherTime;

    public static bool operator <(GameTime left, GameTime right) => left.GetCurrentInGameTime() < right.GetCurrentInGameTime();
    public static bool operator >(GameTime left, GameTime right) => left.GetCurrentInGameTime() > right.GetCurrentInGameTime();
}