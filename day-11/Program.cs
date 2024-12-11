var sr = new StreamReader("input.txt");

var line = sr.ReadLine();
var valuesStr = line.Split(' ');
var stoneCounts = new Dictionary<long, long>();
foreach (var s in valuesStr)
	stoneCounts.Add(int.Parse(s), 1);

// Part 1.
//Console.WriteLine(ProcessStones(25));

// Part 2.
Console.WriteLine(ProcessStones(75));

long ProcessStones(int iterations)
{
	for (var i = 0; i < iterations; i++)
	{
		var newStoneCounts = new Dictionary<long, long>();
		foreach (var kvp in stoneCounts)
		{
			var stoneNumber = kvp.Key;
			var stoneCount = kvp.Value;

			if (stoneNumber == 0L)
			{
				AddCount(newStoneCounts, 1L, stoneCount);
				continue;
			}

			var digits = (int)Math.Floor(Math.Log10(stoneNumber) + 1);
			if (digits % 2 == 0)
			{
				var str = stoneNumber.ToString();
				var halfLen = str.Length / 2;
				AddCount(newStoneCounts, int.Parse(str.Substring(0, halfLen)), stoneCount);
				AddCount(newStoneCounts, int.Parse(str.Substring(halfLen, halfLen)), stoneCount);
				continue;
			}

			AddCount(newStoneCounts, stoneNumber * 2024, stoneCount);
		}

		stoneCounts = newStoneCounts;
	}

	var sum = 0L;
	foreach (var kvp in stoneCounts)
		sum += kvp.Value;
	return sum;
}

void AddCount(Dictionary<long, long> newStoneCounts, long newStoneNumber, long oldStoneCount)
{
	var count = oldStoneCount;
	if (newStoneCounts.TryGetValue(newStoneNumber, out var stoneCount))
		count += stoneCount;
	newStoneCounts[newStoneNumber] = count;
}