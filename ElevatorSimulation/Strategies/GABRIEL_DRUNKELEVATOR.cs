using System.Security.Cryptography;

namespace ElevatorSimulation.Strategies;

public class MojeStrategieStrategy : IElevatorStrategy
{
    public MoveResult DecideNextMove(ElevatorSystem elevator)
    {
        //vodka.cs

        Random rnd = new Random();
        int elevator_mood = rnd.Next(-1, 1);

        if (elevator_mood == -1)
        {
            return MoveResult.MoveDown;
        }
        else if (elevator_mood == 1)
        {
            return MoveResult.MoveUp;
        }
        else
        {
            return MoveResult.OpenDoors;
        }
    }
}