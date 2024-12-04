var sr = new StreamReader("input-04.txt");

var grid = new List<string>();

var line = sr.ReadLine();
var w = 0;
var h = 0;
while (line != null)
{
	w = line.Length;
	h++;
	grid.Add(line);
	line = sr.ReadLine();
}

bool InBounds(int x, int y)
{
	return x >= 0 && x < w && y >= 0 && y < h;
}

(int, int) AdvanceL(int x, int y)
{
	return (x - 1, y);
}

(int, int) AdvanceUL(int x, int y)
{
	return (x - 1, y - 1);
}

(int, int) AdvanceU(int x, int y)
{
	return (x, y - 1);
}

(int, int) AdvanceUR(int x, int y)
{
	return (x + 1, y - 1);
}

(int, int) AdvanceR(int x, int y)
{
	return (x + 1, y);
}

(int, int) AdvanceDR(int x, int y)
{
	return (x + 1, y + 1);
}

(int, int) AdvanceD(int x, int y)
{
	return (x, y + 1);
}

(int, int) AdvanceDL(int x, int y)
{
	return (x - 1, y + 1);
}

bool CheckDir(int x, int y, string target, Func<int, int, (int, int)> Advance)
{
	var i = 0;
	while (InBounds(x, y) && i < target.Length && grid[y][x] == target[i])
	{
		(x, y) = Advance(x, y);
		i++;
	}
	return i == target.Length;
}

int CheckWord(int x, int y, string target)
{
	var matches = 0;
	matches += CheckDir(x, y, target, AdvanceL) ? 1 : 0;
	matches += CheckDir(x, y, target, AdvanceUL) ? 1 : 0;
	matches += CheckDir(x, y, target, AdvanceU) ? 1 : 0;
	matches += CheckDir(x, y, target, AdvanceUR) ? 1 : 0;
	matches += CheckDir(x, y, target, AdvanceR) ? 1 : 0;
	matches += CheckDir(x, y, target, AdvanceDR) ? 1 : 0;
	matches += CheckDir(x, y, target, AdvanceD) ? 1 : 0;
	matches += CheckDir(x, y, target, AdvanceDL) ? 1 : 0;
	return matches;
}

int CheckCrossWord(int x, int y, string target)
{
	var len = target.Length - 1;
	var matches = 0;
	if (CheckDir(x, y, target, AdvanceUL))
	{
		if (CheckDir(x - len, y, target, AdvanceUR) || CheckDir(x, y - len, target, AdvanceDL))
		{
			matches++;
		}
	}
	if (CheckDir(x, y, target, AdvanceUR))
	{
		if (CheckDir(x + len, y, target, AdvanceUL) || CheckDir(x, y - len, target, AdvanceDR))
		{
			matches++;
		}
	}
	if (CheckDir(x, y, target, AdvanceDL))
	{
		if (CheckDir(x - len, y, target, AdvanceDR) || CheckDir(x, y + len, target, AdvanceUL))
		{
			matches++;
		}
	}
	if (CheckDir(x, y, target, AdvanceDR))
	{
		if (CheckDir(x + len, y, target, AdvanceDL) || CheckDir(x, y + len, target, AdvanceUR))
		{
			matches++;
		}
	}
	return matches;
}

// Part 1.
var matches = 0;
for (var x = 0; x < w; x++)
{
	for (var y = 0; y < h; y++)
	{
		matches += CheckWord(x, y, "XMAS");
	}
}
Console.WriteLine(matches);

// Part 2.
matches = 0;
for (var x = 0; x < w; x++)
{
	for (var y = 0; y < h; y++)
	{
		matches += CheckCrossWord(x, y, "MAS");
	}
}
matches /= 2;
Console.WriteLine(matches);
