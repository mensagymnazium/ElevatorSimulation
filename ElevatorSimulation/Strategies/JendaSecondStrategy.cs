namespace ElevatorSimulation.Strategies;

public class JendaSecondStrategy : IElevatorStrategy {
    private int target;
    private int dir;
    
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


        if (elevator.PendingRequests.Count == 0 && elevator.ActiveRiders.Count == 0) {
            target = (int)Math.Floor((decimal)(elevator.Building.MaxFloor - elevator.Building.MinFloor)/2);
        }
        
        int current = elevator.CurrentElevatorFloor - elevator.Building.MinFloor;

        if (dir == 0) {
            int index = 0;
            for (int i = 0; i < requests.Length; i++)
                if (requests[i].Count > 0 || requestsE[i].Count > 0)
                    index = i;

            target = index;
            if (index == current)
                dir = 1;
        }

        if (dir == 1) {
            int index = requests.Length - 1;
            for (int i = index; i >= 0; i--)
                if (requests[i].Count > 0 || requestsE[i].Count > 0)
                    index = i;
            
            target = index;
            if (index == current)
                dir = 0;
        }
        
        return followTarget(elevator, requests, requestsE);


        return MoveResult.OpenDoors;
    }

    MoveResult followTarget(ElevatorSystem elevator, List<RiderRequest>[] requests, List<RiderRequest>[] requestsE) {
        int current = elevator.CurrentElevatorFloor - elevator.Building.MinFloor;
        int totalPas = 0;

        foreach (var l in requests)
            totalPas += l.Count;
        foreach (var r in requestsE)
            totalPas += r.Count;

        if (totalPas == 0 && current == target) {
            return MoveResult.NoAction;
        }

        if (requests[current].Count == 0 && requestsE[current].Count == 0) {
            if (target < current) return MoveResult.MoveDown;
            return MoveResult.MoveUp;
        }

        return MoveResult.OpenDoors;
    }
}