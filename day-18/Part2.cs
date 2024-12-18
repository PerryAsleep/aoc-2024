internal class Part2
{
	private const string File = "input.txt";
	private const int W = 71;
	private const int H = 71;

	private const int L = 0;
	private const int D = 1;
	private const int R = 2;
	private const int U = 3;
	private const int NumDirs = 4;

	private static readonly List<Tuple<int, int>> Dirs = new()
	{
		new Tuple<int, int>(-1, 0),
		new Tuple<int, int>(0, 1),
		new Tuple<int, int>(1, 0),
		new Tuple<int, int>(0, -1),
	};

	private class Node
	{
		public Node(int x, int y)
		{
			X = x;
			Y = y;
		}

		public readonly int X;
		public readonly int Y;
		public readonly Tuple<int, Node>[] Edges = new Tuple<int, Node>[NumDirs];
	}

	private List<Tuple<int, int>> Coords = new();
	private Node[,] Grid = new Node[W, H];
	private bool[,] Path = new bool[W, H];

	public void Run()
	{
		ParseInput();
		CreateGrid();
		ExecuteSearch();
	}

	private void ExecuteSearch()
	{
		for (var i = 0; i < Coords.Count; i++)
		{
			var x = Coords[i].Item1;
			var y = Coords[i].Item2;
			RemoveNode(x, y);
			if (i > 0 && !Path[x, y])
				continue;
			var len = FindShortestPath(Grid[0, 0], Grid[W - 1, H - 1]);
			if (len == int.MaxValue)
			{
				Console.WriteLine($"{x},{y}");
				break;
			}
		}
	}

	private void ParseInput()
	{
		var sr = new StreamReader(File);
		var line = sr.ReadLine();
		while (!string.IsNullOrEmpty(line))
		{
			var parts = line.Split(',');
			Coords.Add(new Tuple<int, int>(int.Parse(parts[0]), int.Parse(parts[1])));
			line = sr.ReadLine();
		}
	}

	private void CreateGrid()
	{
		for (var x = 0; x < W; x++)
		{
			for (var y = 0; y < H; y++)
			{
				Grid[x, y] = new Node(x, y);
			}
		}
		for (var x = 0; x < W; x++)
		{
			for (var y = 0; y < H; y++)
			{
				for (var d = 0; d < NumDirs; d++)
				{
					var nx = x + Dirs[d].Item1;
					var ny = y + Dirs[d].Item2;
					if (!InBounds(nx, ny))
						continue;
					Grid[x, y].Edges[d] = new Tuple<int, Node>(1, Grid[nx, ny]);
				}
			}
		}
	}

	private void RemoveNode(int x, int y)
	{
		Grid[x, y] = null;
		for (var d = 0; d < NumDirs; d++)
		{
			var nx = x + Dirs[d].Item1;
			var ny = y + Dirs[d].Item2;
			if (!InBounds(nx, ny))
				continue;
			if (Grid[nx, ny] == null)
				continue;
			Grid[nx, ny].Edges[Opposite(d)] = null;
		}
	}

	private int FindShortestPath(Node fromNode, Node toNode)
	{
		var openSet = new HashSet<Node>() { fromNode };
		var g = new Dictionary<Node, int> { { fromNode, 0 } };
		var f = new Dictionary<Node, int> { { fromNode, AdmissibleCost(fromNode, toNode) } };
		var bestPathIntoNode = new Dictionary<Node, Node>();

		while (openSet.Count > 0)
		{
			Node current = null;
			var bestScore = int.MaxValue;
			foreach (var node in openSet)
			{
				if (f.TryGetValue(node, out var score) && score < bestScore)
				{
					bestScore = score;
					current = node;
				}
			}

			if (current == toNode)
			{
				RecordPath(bestPathIntoNode, current);
				return g[toNode];
			}

			openSet.Remove(current);
			foreach (var edge in current.Edges)
			{
				if (edge?.Item2 == null)
					continue;
				if (!g.TryGetValue(edge.Item2, out var neighborG))
					neighborG = int.MaxValue;
				var t = g[current] + edge.Item1;
				if (t < neighborG)
				{
					bestPathIntoNode[edge.Item2] = current;
					g[edge.Item2] = t;
					f[edge.Item2] = t + AdmissibleCost(edge.Item2, toNode);
					openSet.Add(edge.Item2);
				}
			}
		}

		return int.MaxValue;
	}

	private void RecordPath(Dictionary<Node, Node> bestPathIntoNode, Node destination)
	{
		for (var x = 0; x < W; x++)
		{
			for (var y = 0; y < H; y++)
			{
				Path[x, y] = false;
			}
		}
		var n = destination;
		while (n != null)
		{
			Path[n.X, n.Y] = true;
			if (!bestPathIntoNode.TryGetValue(n, out n))
				break;
		}
	}

	private int AdmissibleCost(Node fromNode, Node toNode)
	{
		return Math.Abs(toNode.X - fromNode.X) + Math.Abs(toNode.Y - fromNode.Y);
	}

	private int Opposite(int d)
	{
		switch (d)
		{
			case L: return R;
			case R: return L;
			case D: return U;
			case U: return D;
		}
		return 0;
	}

	private bool InBounds(int x, int y)
	{
		return x >= 0 && x < W && y >= 0 && y < H;
	}
}