using System.Reflection;
using ElevatorSimulation.Strategies;

namespace ElevatorSimulation;

/// <summary>
/// Manages tournament-style evaluation of multiple elevator strategies.
/// </summary>
public class StrategyTournament
{
	private readonly Building _building;
	private readonly int[] _seeds;
	private readonly SimulationRunner _runner;

	/// <summary>
	/// Creates a new tournament.
	/// </summary>
	/// <param name="building">The building configuration to use</param>
	/// <param name="seeds">Array of random seeds for simulations</param>
	public StrategyTournament(Building building, int[] seeds)
	{
		_building = building;
		_seeds = seeds;
		_runner = new SimulationRunner(building);
	}

	/// <summary>
	/// Runs all strategies and returns results sorted by average total time.
	/// </summary>
	public List<StrategyResult> RunTournament(List<(string Name, IElevatorStrategy Strategy)> strategies)
	{
		var results = new List<StrategyResult>();

		foreach (var (name, strategy) in strategies)
		{
			var result = RunStrategyAcrossAllSeeds(name, strategy);
			results.Add(result);
		}

		// Sort by average total time (ascending - lower is better)
		return results.OrderBy(r => r.AverageTotalTime).ToList();
	}

	/// <summary>
	/// Discovers all strategy classes in the current assembly.
	/// </summary>
	public static List<(string Name, IElevatorStrategy Strategy)> DiscoverStrategies()
	{
		var strategies = new List<(string Name, IElevatorStrategy Strategy)>();

		var strategyType = typeof(IElevatorStrategy);
		var assembly = Assembly.GetExecutingAssembly();

		var strategyTypes = assembly.GetTypes()
			.Where(t => strategyType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
			.OrderBy(t => t.Name);

		foreach (var type in strategyTypes)
		{
			try
			{
				var instance = (IElevatorStrategy)Activator.CreateInstance(type);
				var name = type.Name.Replace("Strategy", "").ToUpper();
				strategies.Add((name, instance));
			}
			catch
			{
				// Skip strategies that can't be instantiated
			}
		}

		return strategies;
	}

	/// <summary>
	/// Runs a single strategy across all seeds and aggregates statistics.
	/// </summary>
	private StrategyResult RunStrategyAcrossAllSeeds(string strategyName, IElevatorStrategy strategy)
	{
		Console.WriteLine($"Running {strategyName}...");

		var allStats = new List<Statistics>();

		for (int i = 0; i < _seeds.Length; i++)
		{
			var seed = _seeds[i];
			var stats = RunSingleSimulation(strategy, seed);
			allStats.Add(stats);

			Console.WriteLine($"  Seed {i + 1}/{_seeds.Length} (seed={seed}): " +
				$"Completed={stats.CompletedCount}, " +
				$"Avg Total={stats.AverageTotalTime:F2}, " +
				$"Avg Wait={stats.AverageWaitTime:F2}");
		}

		// Aggregate statistics across all simulations
		var aggregated = AggregateStatistics(allStats);

		Console.WriteLine($"  ? Overall: Avg Total Time = {aggregated.AverageTotalTime:F2}");
		Console.WriteLine();

		return StrategyResult.FromStatistics(strategyName, aggregated);
	}

	/// <summary>
	/// Runs a single simulation with given strategy and seed.
	/// </summary>
	private Statistics RunSingleSimulation(IElevatorStrategy strategy, int seed)
	{
		return _runner.RunSimulation(
			strategy,
			seed,
			Program.TimeForRequests,
			Program.RequestDensityPercent,
			silentMode: true);
	}

	/// <summary>
	/// Aggregates statistics from multiple simulations.
	/// </summary>
	private Statistics AggregateStatistics(List<Statistics> statsList)
	{
		int totalCompleted = statsList.Sum(s => s.CompletedCount);
		double totalWaitTime = statsList.Sum(s => s.AverageWaitTime * s.CompletedCount);
		double totalTravelTime = statsList.Sum(s => s.AverageTravelTime * s.CompletedCount);
		double totalTotalTime = statsList.Sum(s => s.AverageTotalTime * s.CompletedCount);
		int totalCumulativeTime = statsList.Sum(s => s.TotalCumulativeTime);

		// Create aggregated statistics with proper decimal precision
		return Statistics.CreateAggregated(
			totalCompleted,
			totalCompleted > 0 ? totalWaitTime / totalCompleted : 0,
			totalCompleted > 0 ? totalTravelTime / totalCompleted : 0,
			totalCompleted > 0 ? totalTotalTime / totalCompleted : 0,
			totalCumulativeTime
		);
	}

	/// <summary>
	/// Prints tournament results as a formatted table.
	/// </summary>
	public static void PrintTournamentResults(List<StrategyResult> results)
	{
		Console.WriteLine();
		Console.WriteLine(new string('=', 100));
		Console.WriteLine("TOURNAMENT RESULTS - RANKED BY AVERAGE TOTAL TIME");
		Console.WriteLine(new string('=', 100));
		Console.WriteLine();
		Console.WriteLine($"{"Rank",-6} {"Strategy",-20} {"Avg Total",-12} {"Avg Wait",-12} {"Avg Travel",-12} {"Completed",-10}");
		Console.WriteLine(new string('-', 100));

		int rank = 1;
		foreach (var result in results)
		{
			Console.WriteLine($"{rank,-6} {result.StrategyName,-20} {result.AverageTotalTime,-12:F2} {result.AverageWaitTime,-12:F2} {result.AverageTravelTime,-12:F2} {result.CompletedRequests,-10}");
			rank++;
		}

		Console.WriteLine(new string('=', 100));
		Console.WriteLine();

		// Print winner
		if (results.Count > 0)
		{
			var winner = results[0];
			Console.WriteLine($"?? WINNER: {winner.StrategyName} with average total time of {winner.AverageTotalTime:F2} steps");
			Console.WriteLine();
		}
	}
}
