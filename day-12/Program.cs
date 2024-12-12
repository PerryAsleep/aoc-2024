
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
		public Dir OutsideDir;
		public int Length;
	}

	private Tuple<int, int> GetEdgeCoord(int gridX, int gridY, Dir outsideDir)
	{
		switch (outsideDir)
		{
			case Dir.L:
				return new(gridX * 2, gridY * 2 + 1);
			case Dir.D:
				return new((gridX * 2) + 1, (gridY + 1) * 2);
			case Dir.U:
				return new((gridX * 2) + 1, gridY * 2);
			case Dir.R:
				return new((gridX + 1) * 2, gridY * 2 + 1);
		}
		return null;
	}

	public void AddEdge(int x, int y, Dir outsideDir)
	{
		var ec = GetEdgeCoord(x, y, outsideDir);
		Edge edge = null;

		// Check left.
		if (OccupiedEdgeCoords.TryGetValue(new Tuple<int, int>(ec.Item1 - 2, ec.Item2), out var l))
		{
			if (l.OutsideDir != outsideDir)
			{
				l = null;
			}

			// Extend left edge.
			else
			{
				l.Length++;
				edge = l;
			}
		}

		// Check right.
		if (OccupiedEdgeCoords.TryGetValue(new Tuple<int, int>(ec.Item1 + 2, ec.Item2), out var r))
		{
			if (r.OutsideDir == outsideDir)
			{
				// Join left and right edge.
				if (l != null)
				{
					// Remove right edge.
					Edges.Remove(r);
					// Extend left edge.
					l.Length += (r.Length + 1);
					// Update right edge coords to point to new extended edge.
					for (var rx = r.X; rx < r.X + r.Length * 2; rx+=2)
						OccupiedEdgeCoords[new Tuple<int, int>(rx, r.Y)] = l;
				}

				// Extend right edge.
				else
				{
					r.X-=2;
					r.Length++;
				}

				edge = r;
			}
		}

		// Check up.
		if (OccupiedEdgeCoords.TryGetValue(new Tuple<int, int>(ec.Item1, ec.Item2 - 2), out var u))
		{
			if (u.OutsideDir != outsideDir)
			{
				u = null;
			}

			// Extend up edge.
			else
			{
				u.Length++;
				edge = u;
			}
		}

		// Check down.
		if (OccupiedEdgeCoords.TryGetValue(new Tuple<int, int>(ec.Item1, ec.Item2 + 2), out var d))
		{
			if (d.OutsideDir == outsideDir)
			{
				// Join up and down edge.
				if (u != null)
				{
					// Remove down edge.
					Edges.Remove(d);
					// Extend up edge.
					u.Length += (d.Length + 1);
					// Update down edge coords to point to new extended edge.
					for (var dy = d.Y; dy < d.Y + d.Length * 2; dy+=2)
						OccupiedEdgeCoords[new Tuple<int, int>(d.X, dy)] = u;
				}

				// Extend down edge.
				else
				{
					d.Y-=2;
					d.Length++;
				}

				edge = d;
			}
		}

		// No adjacent edges. Add new edge.
		if (edge == null)
		{
			edge = new Edge()
			{
				X = ec.Item1,
				Y = ec.Item2,
				Length = 1,
				OutsideDir = outsideDir,
			};
			Edges.Add(edge);
		}

		// Record position of this edge.
		OccupiedEdgeCoords.Add(ec, edge);
	}

	public int GetNumEdges()
	{
		return Edges.Count;
	}

	public int Perimeter;
	public int Area;

	private readonly Dictionary<Tuple<int, int>, Edge> OccupiedEdgeCoords = new();
	private readonly HashSet<Edge> Edges = new();
}