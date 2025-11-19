namespace ElevatorSimulation.Strategies;

/// <summary>
/// Goes back and forth, stops only when neccessary.
/// —MgJbr
/// </summary>
public class MgJbr3BusStrategy : IElevatorStrategy
{
	public MoveResult DecideNextMove(ElevatorSystem elevator)
	{
		// open doors only when neccesary
		if (elevator.PendingRequests.Exists((r) => r.From == elevator.CurrentElevatorFloor) || elevator.ActiveRiders.Exists((r) => r.To == elevator.CurrentElevatorFloor))
		{
			return MoveResult.OpenDoors;
		}

		// move otherwise
		if (elevator.CurrentElevatorFloor == elevator.Building.MinFloor)
		{
			return MoveResult.MoveUp;
		}
		else if (elevator.CurrentElevatorFloor == elevator.Building.MaxFloor)
		{
			return MoveResult.MoveDown;
		}
		else if (elevator.CurrentElevatorDirection == Direction.Up)
		{
			return MoveResult.MoveUp;
		}
		else if (elevator.CurrentElevatorDirection == Direction.Down)
		{
			return MoveResult.MoveDown;
		}

		// oh well
		return MoveResult.NoAction;
	}
}
