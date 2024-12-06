var sr = new StreamReader("sample-05.txt");

var rules = new Dictionary<int, List<int>>();
var updates = new List<List<int>>();

var line = sr.ReadLine();
var readingRules = true;
while (line != null)
{
	if (line.Length == 0)
	{
		readingRules = false;
		line = sr.ReadLine();
		continue;
	}
	if (readingRules)
	{
		var parts = line.Split('|');
		int.TryParse(parts[0], out var lower);
		int.TryParse(parts[1], out var higher);
		if (!rules.ContainsKey(higher))
			rules.Add(higher, new List<int>());
		rules[higher].Add(lower);
	}
	else
	{
		var values = line.Split(',');
		var update = new List<int>();
		foreach (var v in values)
		{
			int.TryParse(v, out var i);
			update.Add(i);
		}
		updates.Add(update);
	}
	line = sr.ReadLine();
}

var comparer = new PageComparer(rules);

var middleSum = 0;
var incorrectMiddleSum = 0;
foreach (var update in updates)
{
	var sortedUpdate = new List<int>(update);
	sortedUpdate.Sort(comparer);
	if (sortedUpdate.SequenceEqual(update))
		middleSum += update[update.Count / 2];
	else
		incorrectMiddleSum += sortedUpdate[update.Count / 2];
}

// Part 1.
Console.WriteLine(middleSum);

// Part 2.
Console.WriteLine(incorrectMiddleSum);

class PageComparer : IComparer<int>
{
	private readonly Dictionary<int, List<int>> Rules;

	public PageComparer(Dictionary<int, List<int>> rules)
	{
		Rules = rules;
	}

	public int Compare(int page1, int page2)
	{
		if (Rules.TryGetValue(page1, out var lowerThanOne) && lowerThanOne.Contains(page2))
			return 1;
		if (Rules.TryGetValue(page2, out var lowerThanTwo) && lowerThanTwo.Contains(page1))
			return -1;
		return 0;
	}
}