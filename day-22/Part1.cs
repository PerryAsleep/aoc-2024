internal class Part1
{
	private const string File = "input.txt";

	private const int NumIterations = 2000;
	private List<uint> BuyerInitialValues = new();

	public void Run()
	{
		ParseInput();

		var sum = 0L;
		foreach (var initialValue in BuyerInitialValues)
		{
			sum += GetSecret(initialValue, NumIterations);
		}

		Console.WriteLine(sum);
	}

	private void ParseInput()
	{
		var sr = new StreamReader(File);
		var line = sr.ReadLine();
		while (!string.IsNullOrEmpty(line))
		{
			BuyerInitialValues.Add(uint.Parse(line));
			line = sr.ReadLine();
		}
	}

	private uint GetSecret(uint num, int iterations)
	{
		for (var i = 0; i < iterations; i++)
		{
			num = GenerateNextSecret(num);
		}

		return num;
	}

	private uint GenerateNextSecret(uint num)
	{
		num = ((num * 64) ^ num) % 16777216;
		num = ((num / 32) ^ num) % 16777216;
		num = ((num * 2048) ^ num) % 16777216;
		return num;
	}
}
