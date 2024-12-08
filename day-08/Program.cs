var sr = new StreamReader("input-08.txt");

var antennas = new Dictionary<char, List<Tuple<int, int>>>();
var antinodes = new HashSet<Tuple<int, int>>();
var line = sr.ReadLine();
var y = 0;
var w = 0;
var h = 0;
while (line != null)
{
	var x = 0;
	foreach(var c in line)
	{
		if (c != '.')
		{
			antennas.TryAdd(c, new List<Tuple<int, int>>());
			antennas[c].Add(new Tuple<int, int>(x, y));
		}
		x++;
		w = x;
	}
	y++;
	h = y;
	line = sr.ReadLine();
}

foreach (var matchingAntennas in antennas)
{
	var positions = matchingAntennas.Value;
	for (var i = 0; i < positions.Count; i++)
	{
		for (var j = i + 1; j < positions.Count; j++)
		{
			var dx = positions[j].Item1 - positions[i].Item1;
			var dy = positions[j].Item2 - positions[i].Item2;

			// Part 1.
			//var x1 = positions[i].Item1 - dx;
			//var y1 = positions[i].Item2 - dy;
			//var x2 = positions[j].Item1 + dx;
			//var y2 = positions[j].Item2 + dy;
			//if (InBounds(x1, y1))
			//	antinodes.Add(new Tuple<int, int>(x1, y1));
			//if (InBounds(x2, y2))
			//	antinodes.Add(new Tuple<int, int>(x2, y2));

			// Part 2.
			var lx = positions[i].Item1;
			var ly = positions[i].Item2;
			while (InBounds(lx, ly))
			{
				antinodes.Add(new Tuple<int, int>(lx, ly));
				lx += dx;
				ly += dy;
			}
			lx = positions[i].Item1 - dx;
			ly = positions[i].Item2 - dy;
			while (InBounds(lx, ly))
			{
				antinodes.Add(new Tuple<int, int>(lx, ly));
				lx -= dx;
				ly -= dy;
			}
		}
	}
}

Console.WriteLine(antinodes.Count);

bool InBounds(int x, int y)
{
	return x >= 0 && x < w && y >= 0 && y < h;
}