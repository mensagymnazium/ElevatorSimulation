namespace ElevatorSimulation.Strategies;

/// <summary>
/// Wii Ball, but the elevator *loves* being in the middle.
/// —MgJbr
/// </summary>
public class MgJbr2CenteredWiiBallStrategy : IElevatorStrategy
{
	public MoveResult DecideNextMove(ElevatorSystem elevator)
	{
		if (SomeoneWantsToGetOn(elevator) || SomeoneWantsToGetOff(elevator))
		{
			return MoveResult.OpenDoors;
		}

		Direction direction = elevator.CurrentElevatorDirection;
		if (direction == Direction.Idle) direction = Direction.Up;
		for (int i = 0; i < 2; i++)
		{
			if (HasWorkInDirection(elevator, direction))
			{
				return MoveInDirection(direction);
			}
			direction = FlipDirection(direction);
		}

		if (IsIdle(elevator)) return MoveTowards(elevator.CurrentElevatorFloor, MiddleFloor(elevator));

		return MoveResult.NoAction;
	}

	private static bool SomeoneWantsToGetOff(ElevatorSystem elevator)
	{
		return elevator.ActiveRiders.Exists((r) => r.To == elevator.CurrentElevatorFloor);
	}

	private static bool SomeoneWantsToGetOn(ElevatorSystem elevator)
	{
		return elevator.PendingRequests.Exists((r) => r.From == elevator.CurrentElevatorFloor && elevator.CurrentElevatorDirection switch
		{
			Direction.Up => r.To > elevator.CurrentElevatorFloor || elevator.CurrentElevatorFloor == elevator.Building.MaxFloor,
			Direction.Down => r.To < elevator.CurrentElevatorFloor || elevator.CurrentElevatorFloor == elevator.Building.MinFloor,
			_ => true,
		});
	}

	private static bool HasWorkInDirection(ElevatorSystem elevator, Direction direction)
	{
		return direction switch
		{
			Direction.Up => elevator.ActiveRiders.Exists((r) => r.To > elevator.CurrentElevatorFloor) || elevator.PendingRequests.Exists((r) => r.From > elevator.CurrentElevatorFloor),
			Direction.Down => elevator.ActiveRiders.Exists((r) => r.To < elevator.CurrentElevatorFloor) || elevator.PendingRequests.Exists((r) => r.From < elevator.CurrentElevatorFloor),
			_ => false,
		};
	}

	private static Direction FlipDirection(Direction direction)
	{
		return direction switch
		{
			Direction.Up => Direction.Down,
			Direction.Down => Direction.Up,
			_ => throw new ArgumentException("Congratulations! You blew the elevator up and everyone died."),
		};
	}

	private static MoveResult MoveInDirection(Direction direction)
	{
		return direction switch
		{
			Direction.Up => MoveResult.MoveUp,
			Direction.Down => MoveResult.MoveDown,
			_ => MoveResult.NoAction,
		};
	}

	private static bool IsIdle(ElevatorSystem elevator)
	{
		return elevator.ActiveRiders.Count == 0 && elevator.PendingRequests.Count == 0;
	}

	private static int MiddleFloor(ElevatorSystem elevator)
	{
		return (elevator.Building.MaxFloor - elevator.Building.MinFloor) / 2 + elevator.Building.MinFloor;
	}

	private static MoveResult MoveTowards(int currentFloor, int targetFloor)
	{
		return (targetFloor - currentFloor) switch
		{
			> 0 => MoveResult.MoveUp,
			< 0 => MoveResult.MoveDown,
			_ => MoveResult.NoAction,
		};
	}
}
