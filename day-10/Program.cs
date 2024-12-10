var sr = new StreamReader("input-10.txt");

var grid = new List<List<int>>();
var line = sr.ReadLine();
while (line != null)
{
	var row = new List<int>();
	foreach (var c in line)
		row.Add(c - '0');
	grid.Add(row);
	line = sr.ReadLine();
}

var w = grid[0].Count;
var h = grid.Count;

var scorePart1 = 0;
var scorePart2 = 0;
var visited = new HashSet<int>();
var destinations = new HashSet<int>();
for (var y = 0; y < h; y++)
{
	for (var x = 0; x < w; x++)
	{
		scorePart1 += ScoreTrails(x, y, visited, destinations, 0);
		scorePart2 += ScoreTrails(x, y, visited, null, 0);
	}
}

Console.WriteLine(scorePart1);
Console.WriteLine(scorePart2);

int ScoreTrails(int x, int y, HashSet<int> visited, HashSet<int> destinations, int val)
{
	var pos = Hash(x, y);
	if (!InBounds(x, y))
		return 0;
	if (visited.Contains(pos))
		return 0;
	if (destinations?.Contains(pos) ?? false)
		return 0;
	if (grid[y][x] != val)
		return 0;
	if (val == 9)
	{
		destinations?.Add(pos);
		return 1;
	}

	visited.Add(pos);
	var score = 0;
	score += ScoreTrails(x + 1, y, visited, destinations, val + 1);
	score += ScoreTrails(x - 1, y, visited, destinations, val + 1);
	score += ScoreTrails(x, y + 1, visited, destinations, val + 1);
	score += ScoreTrails(x, y - 1, visited, destinations, val + 1);
	visited.Remove(pos);
	return score;
}

int Hash(int x, int y)
{
	return y * w + x;
}

bool InBounds(int x, int y)
{
	return x >= 0 && x < w && y >= 0 && y < h;
}