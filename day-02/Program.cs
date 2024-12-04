(bool, int) AreLevelsSafe(List<int> report)
{
	var index = 0;
	var previousLevel = 0;
	var previousIncreasing = false;
	var safe = true;
	foreach (var level in report)
	{
		var increasing = false;
		if (index > 0)
		{
			increasing = level > previousLevel;
			if (index > 1 && increasing != previousIncreasing)
			{
				return (false, index);
			}

			var diff = Math.Abs(level - previousLevel);
			if (diff < 1 || diff > 3)
			{
				return (false, index);
			}

		}

		previousIncreasing = increasing;
		previousLevel = level;
		index++;
	}

	return (true, 0);
}

var sr = new StreamReader("input-02.txt");
var reports = new List<List<int>>();
var reportString = sr.ReadLine();
while (reportString != null)
{
	var levelsStrings = reportString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
	var levels = new List<int>();
	foreach (var levelString in levelsStrings)
	{
		if (!int.TryParse(levelString, out var level))
		{
			throw new Exception($"Could not parse {levelString} as int.");
		}
		levels.Add(level);
	}
	reports.Add(levels);
	reportString = sr.ReadLine();
}

// Part 1
var numSafeReports = 0;
foreach (var report in reports)
{
	var (safe, _) = AreLevelsSafe(report);
	if (safe)
		numSafeReports++;
}
Console.WriteLine(numSafeReports);

// Part 2
numSafeReports = 0;
foreach (var report in reports)
{
	var (safe, firstUnsafeIndex) = AreLevelsSafe(report);
	if (safe)
	{
		numSafeReports++;
		continue;
	}

	var modifiedReport = new List<int>(report);
	var idxToRemove = firstUnsafeIndex - 2;
	while (idxToRemove <= firstUnsafeIndex)
	{
		if (idxToRemove >= 0)
		{
			modifiedReport.RemoveAt(idxToRemove);
			(safe, _) = AreLevelsSafe(modifiedReport);
			if (safe)
			{
				numSafeReports++;
				break;
			}
			modifiedReport.Insert(idxToRemove, report[idxToRemove]);
		}
		idxToRemove++;
	}
}

Console.WriteLine(numSafeReports);