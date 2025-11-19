using ElevatorSimulation.Strategies;

namespace ElevatorSimulation;

/// <summary>
/// Centralized runner for elevator simulations with support for both verbose and silent modes.
/// </summary>
public class SimulationRunner
{
	private readonly Building _building;

	/// <summary>
	/// Creates a new simulation runner.
	/// </summary>
	/// <param name="building">The building configuration to use</param>
	public SimulationRunner(Building building)
	{
		_building = building;
	}

	/// <summary>
	/// Runs a single simulation with the given strategy and seed.
	/// </summary>
	/// <param name="strategy">The elevator strategy to test</param>
	/// <param name="seed">Random seed for request generation</param>
	/// <param name="timeForRequests">Number of time steps to generate requests</param>
	/// <param name="requestDensity">Probability of generating a request each time step (0.0 to 1.0)</param>
	/// <param name="silentMode">If true, suppresses console output during simulation</param>
	/// <param name="strategyName">Optional name to display in verbose mode</param>
	/// <returns>Statistics from the completed simulation</returns>
	public Statistics RunSimulation(
		IElevatorStrategy strategy,
		int seed,
		int timeForRequests,
		double requestDensity,
		bool silentMode = false,
		string strategyName = null)
	{
		// Print header in verbose mode
		if (!silentMode && strategyName != null)
		{
			Console.WriteLine(new string('=', 60));
			Console.WriteLine($"  {strategyName}");
			Console.WriteLine(new string('=', 60));
		}

		// Create elevator system
		var elevator = new ElevatorSystem(strategy, _building)
		{
			SilentMode = silentMode
		};

		// Generate random requests
		var random = new Random(seed);
		var requestsTimeline = Enumerable.Range(0, timeForRequests)
			.Select(_ => GenerateRandomRequest(_building, random, requestDensity))
			.ToList();

		// Run simulation
		elevator.RunSimulation(requestsTimeline);

		// Print summary in verbose mode
		if (!silentMode)
		{
			Console.WriteLine($"\n[{elevator.CurrentTime:00}] ✅ Simulation completed");
			elevator.Statistics.PrintSummary();
		}

		return elevator.Statistics;
	}

	/// <summary>
	/// Generates a random request with given probability.
	/// </summary>
	/// <param name="building">The building configuration</param>
	/// <param name="random">Random number generator</param>
	/// <param name="requestDensity">Probability of generating a request (0.0 to 1.0)</param>
	/// <returns>A random request or null if no request is generated</returns>
	public static RiderRequest GenerateRandomRequest(Building building, Random random, double requestDensity)
	{
		if (random.NextDouble() > requestDensity)
		{
			return null; // No request this tick
		}

		return building.CreateRandomRequest(random, 0); // Time will be set by elevator
	}
}
