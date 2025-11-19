namespace ElevatorSimulation.Strategies;

/// <summary>
/// Goes back and forth, stops everywhere.
/// —MgJbr
/// </summary>
public class MgJbr2PaternosterStrategy : IElevatorStrategy
{
	public MoveResult DecideNextMove(ElevatorSystem elevator)
	{
		if (elevator.CurrentTime % 2 == 1)
		{
			return MoveResult.OpenDoors;
		}
		else
		{
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
		}
		return MoveResult.NoAction;
	}
}
