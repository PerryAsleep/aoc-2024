using System.Text;

var sr = new StreamReader("input.txt");

//var w = 11;
//var h = 7;
var w = 101;
var h = 103;
var w2 = w / 2;
var h2 = h / 2;
var iterations = 100;

var line = sr.ReadLine();
var positions = new List<Tuple<int, int>>();
var velocities = new List<Tuple<int, int>>();
while (line != null)
{
	var parts = line.Split(" v=");
	var posStrs = parts[0].Substring(2).Trim().Split(",");
	var x = int.Parse(posStrs[0]);
	var y = int.Parse(posStrs[1]);
	positions.Add(new Tuple<int, int>(x, y));

	var velStrs = parts[1].Split(",");
	x = int.Parse(velStrs[0]);
	y = int.Parse(velStrs[1]);
	velocities.Add(new Tuple<int, int>(x, y));

	line = sr.ReadLine();
}

// Part 1.
var quadCounts = new int[4];
for (var i = 0; i < positions.Count; i++)
{
	var x = Mod(positions[i].Item1 + velocities[i].Item1 * iterations, w);
	var y = Mod(positions[i].Item2 + velocities[i].Item2 * iterations, h);
	if (x < w2 && y < h2)
		quadCounts[0]++;
	else if (x < w2 && y > h2)
		quadCounts[1]++;
	else if (x > w2 && y < h2)
		quadCounts[2]++;
	else if (x > w2 && y > h2)
		quadCounts[3]++;
}
var result = 1;
foreach (var quadCount in quadCounts)
	result *= quadCount;
Console.WriteLine(result);

// Part 2.
var grid = new List<List<bool>>();
Console.WriteLine(GetNumStepsToLookLikeATree());

void InitGrid()
{
	for (var x = 0; x < w; x++)
	{
		grid.Add(new List<bool>());
		for (var y = 0; y < h; y++)
			grid[x].Add(false);
	}
}

void ClearGrid()
{
	for (var x = 0; x < w; x++)
		for (var y = 0; y < h; y++)
			grid[x][y] = false;
}

int GetNumStepsToLookLikeATree()
{
	InitGrid();
	var bestScore = 0;
	var bestIteration = 0;
	var maxIterations = 10000;
	for (var i = 0; i < maxIterations; i++)
	{
		ClearGrid();
		AdvanceBots(i);
		var score = GetTreeScore();
		if (score > bestScore)
		{
			bestScore = score;
			bestIteration = i;
		}
	}
	RenderIteration(bestIteration);
	return bestIteration;
}

void RenderIteration(int iteration)
{
	AdvanceBots(iteration);
	var sb = new StringBuilder();
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			sb.Append(grid[x][y] ? '*' : ' ');
		}
		sb.Append("\n");
	}
	sb.Append("\n");
	Console.Write(sb);
}

void AdvanceBots(int iteration)
{
	for (var i = 0; i < positions.Count; i++)
	{
		var x = Mod(positions[i].Item1 + velocities[i].Item1 * iteration, w);
		var y = Mod(positions[i].Item2 + velocities[i].Item2 * iteration, h);
		grid[x][y] = true;
	}
}

int GetTreeScore()
{
	var score = 0;
	for (var x = 0; x < w; x++)
		for (var y = 0; y < h; y++)
			score += GetLocScore(x, y);
	return score;
}

int GetLocScore(int x, int y)
{
	if (!grid[x][y])
		return 0;
	var score = 0;
	for (var rx = x - 1; rx < x + 1; rx++)
	{
		for (var ry = y - 1; ry < y + 1; ry++)
		{
			if (!InBounds(rx, ry))
				continue;
			if (rx == x && ry == y)
				continue;
			if (!grid[rx][ry])
				continue;
			if (rx == x || ry == y)
				score += 2;
			else
				score += 1;
		}
	}
	return score;
}

bool InBounds(int x, int y)
{
	return x >= 0 && x < w && y >= 0 && y < h;
}

int Mod(int x, int m)
{
	var r = x % m;
	return r < 0 ? r + m : r;
}
