internal class Part2
{
	private const string File = "input.txt";

	private const int NumIterations = 2000;
	private List<uint> BuyerInitialValues = new();
	private List<List<int>> BuyerSequences = new();
	private List<List<int?>> BuyerDeltas = new();
	private List<int[]> PossibleSequences = new();
	private HashSet<int> HashedSequences = new();

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
		var num = start;
		for (var i = 0; i < iterations; i++)
		{
			sequence.Add((int)num % 10);
			if (i == 0)
				deltas.Add(null);
			else
				deltas.Add(sequence[i] - sequence[i - 1]);

			if (i >= 5)
			{
				var hash = HashCode.Combine(deltas[i], deltas[i - 1], deltas[i - 2], deltas[i - 3]);
				if (!HashedSequences.Contains(hash))
				{
					PossibleSequences.Add(new int[4]
						{ (int)deltas[i - 3], (int)deltas[i - 2], (int)deltas[i - 1], (int)deltas[i] });
					HashedSequences.Add(hash);
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

	private long GetMostBananas()
	{
		var mostBananas = long.MinValue;
		foreach (var sequence in PossibleSequences)
		{
			var numBananas = GetNumBananas(sequence);
			if (numBananas > mostBananas)
			{
				mostBananas = numBananas;
			}
		}
		return mostBananas;
	}

	private long GetNumBananas(int[] sequence)
	{
		var numBananas = 0L;
		var numDeltasInSequence = sequence.Length;
		for (var buyerIndex = 0; buyerIndex < BuyerDeltas.Count; buyerIndex++)
		{
			var deltas = BuyerDeltas[buyerIndex];
			for (var deltaIndex = 1; deltaIndex < deltas.Count; deltaIndex++)
			{
				var match = true;
				for (var d = 0; d < numDeltasInSequence; d++)
				{
					if (deltaIndex + d >= deltas.Count || deltas[deltaIndex + d] != sequence[d])
					{
						match = false;
						break;
					}
				}

				if (match)
				{
					numBananas += BuyerSequences[buyerIndex][deltaIndex + numDeltasInSequence - 1];
					break;
				}
			}
		}

		return numBananas;
	}
}
