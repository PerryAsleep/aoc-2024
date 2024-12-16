internal class Part2
{
	private const string File = "input.txt";
	private const int RotationCost = 1000;

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
		public Node(int x, int y, int facing)
		{
			X = x;
			Y = y;
			Facing = facing;
		}

		public readonly int X;
		public readonly int Y;
		public readonly int Facing;
		public readonly Tuple<int, Node>[] Edges = new Tuple<int, Node>[NumDirs];
	}

	private Node StartNode;
	private Node[] EndNodes = new Node[NumDirs];

	private int W;
	private int H;

	public void Run()
	{
		ParseInput();

		var shortestPath = int.MaxValue;
		var bestNumNodes = int.MaxValue;
		foreach (var endNode in EndNodes)
		{
			var (pathCost, numNodes) = FindNumDistinctPositionsOnBestPaths(StartNode, endNode);
			if (pathCost < shortestPath)
			{
				shortestPath = pathCost;
				bestNumNodes = numNodes;
			}
		}

		Console.WriteLine(bestNumNodes);
	}

	private void ParseInput()
	{
		var (startPos, endPos) = ParseDimensions();

		var grid = CreateGrid();
		RemoveWalls(grid);
		ConnectNodes(grid);

		StartNode = grid[startPos.Item1][startPos.Item2][R];
		for (var d = 0; d < NumDirs; d++)
			EndNodes[d] = grid[endPos.Item1][endPos.Item2][d];
	}

	private (Tuple<int, int>, Tuple<int, int>) ParseDimensions()
	{
		var sr = new StreamReader(File);
		Tuple<int, int> start = null;
		Tuple<int, int> end = null;
		H = 0;

		var line = sr.ReadLine();
		while (!string.IsNullOrEmpty(line))
		{
			var x = 0;
			foreach (var c in line)
			{
				if (c == 'S')
					start = new Tuple<int, int>(x, H);
				else if (c == 'E')
					end = new Tuple<int, int>(x, H);
				x++;
			}

			W = line.Length;
			H++;
			line = sr.ReadLine();
		}

		return (start, end);
	}

	private List<List<List<Node>>> CreateGrid()
	{
		var grid = new List<List<List<Node>>>();
		for (var x = 0; x < W; x++)
		{
			var column = new List<List<Node>>();
			for (var y = 0; y < H; y++)
			{
				var dirList = new List<Node>();
				for (var d = 0; d < NumDirs; d++)
					dirList.Add(new Node(x, y, d));
				column.Add(dirList);
			}

			grid.Add(column);
		}

		return grid;
	}

	private void RemoveWalls(List<List<List<Node>>> grid)
	{
		var sr = new StreamReader(File);
		var line = sr.ReadLine();
		var y = 0;
		while (line != null)
		{
			var x = 0;
			foreach (var c in line)
			{
				if (c == '#')
					grid[x][y] = null;
				x++;
			}

			y++;
			line = sr.ReadLine();
		}
	}

	private void ConnectNodes(List<List<List<Node>>> grid)
	{
		for (var x = 0; x < W; x++)
		{
			for (var y = 0; y < H; y++)
			{
				if (grid[x][y] == null)
					continue;

				for (var fromDir = 0; fromDir < NumDirs; fromDir++)
				{
					var fromNode = grid[x][y][fromDir];

					for (var toDir = 0; toDir < NumDirs; toDir++)
					{
						var nx = x + Dirs[toDir].Item1;
						var ny = y + Dirs[toDir].Item2;
						if (!InBounds(nx, ny))
							continue;
						var toNode = grid[nx][ny]?[toDir];
						if (toNode == null)
							continue;
						var numRotations = GetNumRotations(fromDir, toDir);
						var cost = 1 + numRotations * RotationCost;
						fromNode.Edges[toDir] = new Tuple<int, Node>(cost, toNode);
					}
				}
			}
		}
	}

	private (int, int) FindNumDistinctPositionsOnBestPaths(Node fromNode, Node toNode)
	{
		var openSet = new HashSet<Node>() { fromNode };
		var g = new Dictionary<Node, int> { { fromNode, 0 } };
		var f = new Dictionary<Node, int> { { fromNode, AdmissibleCost(fromNode, toNode) } };
		var bestPathsIntoNode = new Dictionary<Node, List<Node>>();

		var found = false;
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

			// Do not stop searching. There may be multiple best paths.
			if (current == toNode)
				found = true;

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
					bestPathsIntoNode[edge.Item2] = new List<Node>() { current };
					g[edge.Item2] = t;
					f[edge.Item2] = t + AdmissibleCost(edge.Item2, toNode);
					openSet.Add(edge.Item2);
				}
				else if (t == neighborG)
				{
					bestPathsIntoNode[edge.Item2].Add(current);
				}
			}
		}

		if (found)
			return (g[toNode], CountDistinctPositionsOnBestPaths(bestPathsIntoNode, fromNode, toNode));
		return (int.MaxValue, int.MaxValue);
	}

	private int CountDistinctPositionsOnBestPaths(Dictionary<Node, List<Node>> bestPathsIntoNode, Node start, Node destination)
	{
		var nodesToCheck = new HashSet<Node>() { destination };
		var checkedPositions = new HashSet<Tuple<int, int>>();
		while (nodesToCheck.Count > 0)
		{
			var node = PopAny(nodesToCheck);
			checkedPositions.Add(new Tuple<int, int>(node.X, node.Y));
			if (node == start)
				continue;
			foreach (var entry in bestPathsIntoNode[node])
				nodesToCheck.Add(entry);
		}

		return checkedPositions.Count;
	}

	private Node PopAny(HashSet<Node> nodes)
	{
		using var enumerator = nodes.GetEnumerator();
		if (enumerator.MoveNext())
		{
			var node = enumerator.Current;
			nodes.Remove(node);
			return node;
		}

		return null;
	}

	private int AdmissibleCost(Node fromNode, Node toNode)
	{
		return Math.Abs(toNode.X - fromNode.X) + Math.Abs(toNode.Y - fromNode.Y) +
		       GetMinNumRotations(fromNode, toNode) * RotationCost;
	}

	private int GetMinNumRotations(Node fromNode, Node toNode)
	{
		var dx = toNode.X - fromNode.X;
		var dy = toNode.Y - fromNode.Y;
		if (fromNode.Facing == L)
		{
			if (dx == 0)
				return dy == 0 ? 0 : 1;
			return dx > 0 ? 2 : dy != 0 ? 1 : 0;
		}

		if (fromNode.Facing == R)
		{
			if (dx == 0)
				return dy == 0 ? 0 : 1;
			return dx < 0 ? 2 : dy != 0 ? 1 : 0;
		}

		if (fromNode.Facing == U)
		{
			if (dy == 0)
				return dx == 0 ? 0 : 1;
			return dy > 0 ? 2 : dx != 0 ? 1 : 0;
		}

		if (fromNode.Facing == D)
		{
			if (dy == 0)
				return dx == 0 ? 0 : 1;
			return dy < 0 ? 2 : dx != 0 ? 1 : 0;
		}

		return 0;
	}

	private int GetNumRotations(int fromDir, int toDir)
	{
		const int halfRot = NumDirs / 2;
		if (fromDir == toDir)
			return 0;
		var mod = Math.Abs(toDir - fromDir) % halfRot;
		return mod == 0 ? halfRot : mod;
	}

	private bool InBounds(int x, int y)
	{
		return x >= 0 && x < W && y >= 0 && y < H;
	}
}
