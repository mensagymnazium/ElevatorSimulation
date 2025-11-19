namespace ElevatorSimulation.Strategies;

public class JendaChatGPTStrategy : IElevatorStrategy
{
    private int direction = +1; // +1 up, -1 down

    public MoveResult DecideNextMove(ElevatorSystem elevator)
    {
        int cur = elevator.CurrentElevatorFloor;
        int min = elevator.Building.MinFloor;
        int max = elevator.Building.MaxFloor;

        // 1. Open doors if needed
        bool dropoff = elevator.ActiveRiders.Any(r => r.To == cur);
        bool pickup = elevator.PendingRequests.Any(r => r.From == cur);

        if (dropoff || pickup)
            return MoveResult.OpenDoors;

        // 2. Build list of all relevant floors
        var upFloors =
            elevator.PendingRequests.Where(r => r.From > cur).Select(r => r.From)
                .Concat(elevator.ActiveRiders.Where(r => r.To > cur).Select(r => r.To))
                .ToList();

        var downFloors =
            elevator.PendingRequests.Where(r => r.From < cur).Select(r => r.From)
                .Concat(elevator.ActiveRiders.Where(r => r.To < cur).Select(r => r.To))
                .ToList();

        // 3. Continue in same direction if possible
        if (direction == +1 && upFloors.Count > 0)
            return MoveResult.MoveUp;

        if (direction == -1 && downFloors.Count > 0)
            return MoveResult.MoveDown;

        // 4. Otherwise, change direction
        direction *= -1;

        if (direction == +1 && upFloors.Count > 0)
            return MoveResult.MoveUp;

        if (direction == -1 && downFloors.Count > 0)
            return MoveResult.MoveDown;

        // 5. Idle at current floor if literally no requests exist
        return MoveResult.NoAction;
    }
}