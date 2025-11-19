namespace ElevatorSimulation.Strategies
{
    public class MaxJStrategy : IElevatorStrategy
    {
        public MoveResult DecideNextMove(ElevatorSystem elevator)
        {
            //params
            double futureRequestWeight = 0.5;
            double distPow = Math.PI;

            //Console.WriteLine(elevator.CurrentTime);
            if (elevator.PendingRequests.Any(x => x.From == elevator.CurrentElevatorFloor) || elevator.ActiveRiders.Any(x => x.To == elevator.CurrentElevatorFloor))
            {
                //Console.WriteLine("Doors");
                return MoveResult.OpenDoors;
            }

            int floorCount = elevator.Building.MaxFloor - elevator.Building.MinFloor + 1;
            double baseTilePassProbability = 0;
            if (elevator.CurrentTime < Program.TimeForRequests)
            {
                baseTilePassProbability = Program.RequestDensityPercent * futureRequestWeight;
            }

            int floor = elevator.CurrentElevatorFloor;

            double downValue = 0;
            double upValue = 0;

            Dictionary<int, int> requestCountOnFloor = new Dictionary<int, int>();

            foreach (var request in elevator.PendingRequests)
            {
                if (!requestCountOnFloor.ContainsKey(request.From))
                {
                    requestCountOnFloor[request.From] = 0;
                }
                requestCountOnFloor[request.From]++;
            }

            foreach (var request in elevator.ActiveRiders)
            {
                if (!requestCountOnFloor.ContainsKey(request.To))
                {
                    requestCountOnFloor[request.To] = 0;
                }
                requestCountOnFloor[request.To]++;
            }

            for (int i = elevator.Building.MinFloor; i < floor; i++)
            {
                int requestCount = 0;
                if (requestCountOnFloor.ContainsKey(i))
                {
                    requestCount = requestCountOnFloor[i];
                }

                downValue += (baseTilePassProbability + requestCount) * (1.0 / Math.Pow(floor - i, distPow));
            }
            for (int i = floor + 1; i <= elevator.Building.MaxFloor; i++)
            {
                int requestCount = 0;
                if (requestCountOnFloor.ContainsKey(i))
                {
                    requestCount = requestCountOnFloor[i];
                }

                upValue += (baseTilePassProbability + requestCount) * (1.0 / Math.Pow(i - floor, distPow));
            }

            if (upValue >= downValue)
            {
                //Console.WriteLine("Up");
                return MoveResult.MoveUp;
            }
            else
            {
                //Console.WriteLine("Down");
                return MoveResult.MoveDown;
            }
        }
    }
}
