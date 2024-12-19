internal class Part1
{
	private const string File = "input.txt";

	private Dictionary<int, long> Cache = new();
	private List<string> Designs = new();
	private List<string> Towels = new();

	public void Run()
	{
		ParseInput();
		var (numPossible, numDistinct) = CountPossibleDesigns();
		Console.WriteLine(numPossible);
		Console.WriteLine(numDistinct);
	}

	private void ParseInput()
	{
		var sr = new StreamReader(File);
		Towels = sr.ReadLine().Split(", ").ToList();

		// Parse designs.
		sr.ReadLine();
		var line = sr.ReadLine();
		while (!string.IsNullOrEmpty(line))
		{
			Designs.Add(line);
			line = sr.ReadLine();
		}
	}

	private (long, long) CountPossibleDesigns()
	{
		var numPossible = 0L;
		var numDistinct = 0L;
		foreach (var d in Designs)
		{
			Cache.Clear();
			var numValid = NumValid(d, 0);
			if (numValid > 0)
			{
				numPossible++;
				numDistinct += numValid;
			}
		}
		return (numPossible, numDistinct);
	}

	private long NumValid(string d, int i)
	{
		if (i == d.Length)
			return 1;
		if (Cache.ContainsKey(i))
			return Cache[i];

		var sum = 0L;
		foreach (var towel in Towels)
		{
			var valid = true;
			for (var ci = 0; ci < towel.Length; ci++)
			{
				if (!(i + ci < d.Length && d[i + ci] == towel[ci]))
				{
					valid = false;
					break;
				}
			}
			if (valid)
			{
				sum += NumValid(d, i + towel.Length);
			}
		}
		Cache[i] = sum;
		return sum;
	}
}
