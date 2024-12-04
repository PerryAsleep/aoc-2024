
var sr = new StreamReader("input-01.txt");
var line = sr.ReadLine();
var list01 = new List<int>();
var list02 = new List<int>();

while (line != null)
{
	var entries = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
	if (entries.Length != 2)
	{
		throw new Exception($"Expected 2 entries. Found {entries.Length}");
	}

	if (!int.TryParse(entries[0], out var int01))
	{
		throw new Exception($"Could not parse {entries[0]} as int.");
	}
	list01.Add(int01);
	if (!int.TryParse(entries[1], out var int02))
	{
		throw new Exception($"Could not parse {entries[1]} as int.");
	}
	list02.Add(int02);

	line = sr.ReadLine();
}

list01.Sort();
list02.Sort();

var diff = 0;
for (var i = 0; i < list01.Count; i++)
{
	diff += Math.Abs(list01[i] - list02[i]);
}

// Part 1
Console.WriteLine(diff);

// Part 2
var score = 0;
var list01Index = 0;
var list02Index = 0;
while (list01Index < list01.Count)
{
	while (list02[list02Index] < list01[list01Index] && list02Index < list02.Count)
		list02Index++;

	var previousList02Index = list02Index;
	while (list02[list02Index] == list01[list01Index])
	{
		score += list02[list02Index];
		list02Index++;
	}
	list02Index = previousList02Index;

	list01Index++;
}
Console.WriteLine(score);