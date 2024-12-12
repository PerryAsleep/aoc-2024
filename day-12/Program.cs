
var grid = new List<List<char>>();
var unvisited = new HashSet<Tuple<int, int>>();
var regions = new List<Region>();
var h = 0;

var adjacentDirs = new List<Tuple<int, int, Dir>>
{
	new(-1, 0, Dir.L),
	new(1, 0, Dir.R),
	new(0, -1, Dir.U),
	new(0, 1, Dir.D),
};

var sr = new StreamReader("input.txt");
var line = sr.ReadLine();
while (line != null)
{
	var row = new List<char>();
	var x = 0;
	foreach (var c in line)
	{
		unvisited.Add(new Tuple<int, int>(x, h));
		row.Add(c);
		x++;
	}
	grid.Add(row);
	line = sr.ReadLine();
	h++;
}

var w = grid[0].Count;

while (unvisited.Count > 0)
{
	var pos = GetNextUnvisitedPosition();
	var r = new Region();
	CheckArea(pos.Item1, pos.Item2, grid[pos.Item2][pos.Item1], r);
	regions.Add(r);
}

// Part 1.
var cost = 0;
foreach (var r in regions)
{
	cost += r.Area * r.Perimeter;
}
Console.WriteLine(cost);

// Part 2.
cost = 0;
foreach (var r in regions)
{
	cost += r.Area * r.GetNumEdges();
}
Console.WriteLine(cost);

Tuple<int, int> GetNextUnvisitedPosition()
{
	using var enumerator = unvisited.GetEnumerator();
	if (enumerator.MoveNext())
	{
		var pos = enumerator.Current;
		unvisited.Remove(pos);
		return pos;
	}

	return null;
}

void CheckArea(int x, int y, char c, Region r)
{
	r.Area++;
	foreach (var dir in adjacentDirs)
	{
		var newX = x + dir.Item1;
		var newY = y + dir.Item2;
		var newPos = new Tuple<int, int>(newX, newY);
		if (!InBounds(newX, newY) || grid[newY][newX] != c)
		{
			r.AddEdge(x, y, dir.Item3);
			r.Perimeter++;
			continue;
		}

		if (!unvisited.Contains(newPos))
			continue;

		unvisited.Remove(newPos);
		CheckArea(newX, newY, c, r);
	}
}

bool InBounds(int x, int y)
{
	return x >= 0 && x < w && y >= 0 && y < h;
}

enum Dir
{
	U,D,L,R,
}

class Region
{
	private class Edge
	{
		public int X;
		public int Y;
		public int Length;
	}

	public void AddEdge(int x, int y, Dir outsideDir)
	{
		Edge edge = null;

		// Check left.
		if (OccupiedEdgeCoords.TryGetValue(new Tuple<int, int, Dir>(x - 1, y, outsideDir), out var l))
		{
			// Extend left edge.
			l.Length++;
			edge = l;
		}

		// Check right.
		if (OccupiedEdgeCoords.TryGetValue(new Tuple<int, int, Dir>(x + 1, y, outsideDir), out var r))
		{
			// Join left and right edge.
			if (l != null)
			{
				// Remove right edge.
				Edges.Remove(r);
				// Extend left edge.
				l.Length += (r.Length + 1);
				// Update right edge coords to point to new extended edge.
				for (var rx = r.X; rx < r.X + r.Length; rx++)
					OccupiedEdgeCoords[new Tuple<int, int, Dir>(rx, r.Y, outsideDir)] = l;
			}

			// Extend right edge.
			else
			{
				r.X--;
				r.Length++;
			}

			edge = r;
		}

		// Check up.
		if (OccupiedEdgeCoords.TryGetValue(new Tuple<int, int, Dir>(x, y - 1, outsideDir), out var u))
		{
			// Extend up edge.
			u.Length++;
			edge = u;
		}

		// Check down.
		if (OccupiedEdgeCoords.TryGetValue(new Tuple<int, int, Dir>(x, y + 1, outsideDir), out var d))
		{
			// Join up and down edge.
			if (u != null)
			{
				// Remove down edge.
				Edges.Remove(d);
				// Extend up edge.
				u.Length += (d.Length + 1);
				// Update down edge coords to point to new extended edge.
				for (var dy = d.Y; dy < d.Y + d.Length; dy++)
					OccupiedEdgeCoords[new Tuple<int, int, Dir>(d.X, dy, outsideDir)] = u;
			}

			// Extend down edge.
			else
			{
				d.Y--;
				d.Length++;
			}

			edge = d;
		}

		// No adjacent edges. Add new edge.
		if (edge == null)
		{
			edge = new Edge()
			{
				X = x,
				Y = y,
				Length = 1,
			};
			Edges.Add(edge);
		}

		// Record position of this edge.
		OccupiedEdgeCoords.Add(new Tuple<int, int, Dir>(x, y, outsideDir), edge);
	}

	public int GetNumEdges()
	{
		return Edges.Count;
	}

	public int Perimeter;
	public int Area;

	private readonly Dictionary<Tuple<int, int, Dir>, Edge> OccupiedEdgeCoords = new();
	private readonly HashSet<Edge> Edges = new();
}