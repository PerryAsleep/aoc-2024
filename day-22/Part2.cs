internal class Part2
{
	private const string File = "input.txt";

	private const int NumIterations = 2000;
	private List<uint> BuyerInitialValues = new();
	private List<List<int>> BuyerSequences = new();
	private List<List<int?>> BuyerDeltas = new();
	private Dictionary<int, int> HashedCounts = new();

	public void Run()
	{
		ParseInput();

		for (var i = 0; i < BuyerInitialValues.Count; i++)
		{
			GenerateSecretsAndDeltas(BuyerInitialValues[i], NumIterations, BuyerSequences[i], BuyerDeltas[i]);
		}

		Console.WriteLine(GetMostBananas());
	}

	private void ParseInput()
	{
		var sr = new StreamReader(File);
		var line = sr.ReadLine();
		while (!string.IsNullOrEmpty(line))
		{
			BuyerInitialValues.Add(uint.Parse(line));
			BuyerSequences.Add(new List<int>());
			BuyerDeltas.Add(new List<int?>());
			line = sr.ReadLine();
		}
	}

	private void GenerateSecretsAndDeltas(uint start, int iterations, List<int> sequence, List<int?> deltas)
	{
		HashSet<int> hashesThisSequence = new HashSet<int>();
		var num = start;
		for (var i = 0; i < iterations; i++)
		{
			var bananas = (int)num % 10;
			sequence.Add(bananas);
			if (i == 0)
				deltas.Add(null);
			else
				deltas.Add(sequence[i] - sequence[i - 1]);

			if (i >= 5)
			{
				var hash = HashCode.Combine(deltas[i], deltas[i - 1], deltas[i - 2], deltas[i - 3]);
				HashedCounts.TryAdd(hash, 0);
				if (!hashesThisSequence.Contains(hash))
				{
					hashesThisSequence.Add(hash);
					HashedCounts[hash] += bananas;
				}
			}

			num = GenerateNextSecret(num);
		}
	}

	private uint GenerateNextSecret(uint num)
	{
		num = ((num * 64) ^ num) % 16777216;
		num = ((num / 32) ^ num) % 16777216;
		num = ((num * 2048) ^ num) % 16777216;
		return num;
	}

	private int GetMostBananas()
	{
		var mostBananas = int.MinValue;
		foreach (var kvp in HashedCounts)
			mostBananas = Math.Max(mostBananas, kvp.Value);
		return mostBananas;
	}
}
