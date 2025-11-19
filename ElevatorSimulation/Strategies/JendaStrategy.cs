namespace ElevatorSimulation.Strategies;

public class JendaStrategy : IElevatorStrategy
{
    private Random rn = new Random();
    private int dir = 0;
    
    public MoveResult DecideNextMove(ElevatorSystem elevator)
    {
        int min = elevator.Building.MinFloor;
        int max = elevator.Building.MaxFloor;
        int size = max - min + 1;

        // Pending requests
        List<RiderRequest>[] requests = new List<RiderRequest>[size];
        for (int i = 0; i < size; i++)
            requests[i] = new List<RiderRequest>();

        foreach (var r in elevator.PendingRequests)
            requests[r.From - min].Add(r);

        // Active riders
        List<RiderRequest>[] requestsE = new List<RiderRequest>[size];
        for (int i = 0; i < size; i++)
            requestsE[i] = new List<RiderRequest>();

        foreach (var r in elevator.ActiveRiders)
            requestsE[r.To - min].Add(r);

        int pos = elevator.CurrentElevatorFloor - min;

        if (dir == 0 && elevator.CurrentElevatorFloor == max)
            dir = 1;

        if (dir == 1 && elevator.CurrentElevatorFloor == min)
            dir = 0;

        if (rn.Next(1, 10000) == 2)
            قنبلة();
        
        if (requests[pos].Count == 0 &&  requestsE[pos].Count == 0)
            return dir == 0 ? MoveResult.MoveUp : MoveResult.MoveDown;

        return MoveResult.OpenDoors;
    }

    void قنبلة() {
        قنبلة();
    }
}
