internal class Part1
{
	private const string File = "input.txt";
	private int W;
	private int H;
	private const int NumMovesPerCheat = 2;

	private const int NumDirs = 4;

	private static readonly List<Tuple<int, int>> Dirs = new()
	{
		new Tuple<int, int>(-1, 0),
		new Tuple<int, int>(0, 1),
		new Tuple<int, int>(1, 0),
		new Tuple<int, int>(0, -1),
	};

	private int[,] Grid;
	private Dictionary<int, int> Cheats = new();

	private int StartX, StartY, EndX, EndY;

	public void Run()
	{
		ParseInput();
		ExecuteSearch();
		Console.WriteLine(GetNumCheats(100));
	}

	private int GetNumCheats(int minSavings)
	{
		var numCheats = 0;
		foreach (var cheatCount in Cheats)
		{
			if (cheatCount.Key >= minSavings)
				numCheats += cheatCount.Value;
		}

		return numCheats;
	}

	private void ExecuteSearch()
	{
		var x = StartX;
		var y = StartY;
		while (!(x == EndX && y == EndY))
		{
			//Cheat
			for (var d = 0; d < NumDirs; d++)
			{
				var nx = x + Dirs[d].Item1 * NumMovesPerCheat;
				var ny = y + Dirs[d].Item2 * NumMovesPerCheat;
				if (InBounds(nx, ny) && Grid[nx, ny] != int.MaxValue)
				{
					var savings = Grid[nx, ny] - Grid[x, y] - NumMovesPerCheat;
					if (savings > 0)
					{
						Cheats.TryGetValue(savings, out var numCheats);
						Cheats[savings] = numCheats + 1;
					}
				}
			}

			Advance(ref x, ref y);
		}
	}

	private void Advance(ref int x, ref int y)
	{
		for (var d = 0; d < NumDirs; d++)
		{
			var nx = x + Dirs[d].Item1;
			var ny = y + Dirs[d].Item2;
			if (InBounds(nx, ny) && Grid[nx, ny] == Grid[x, y] + 1)
			{
				x = nx;
				y = ny;
				return;
			}
		}
	}


	private void ParseInput()
	{
		ParseDimensions();
		CreateGrid();
		SetCosts();
	}

	private void ParseDimensions()
	{
		var sr = new StreamReader(File);
		H = 0;

		var line = sr.ReadLine();
		while (!string.IsNullOrEmpty(line))
		{
			var x = 0;
			foreach (var c in line)
			{
				if (c == 'S')
				{
					StartX = x;
					StartY = H;
				}
				else if (c == 'E')
				{
					EndX = x;
					EndY = H;
				}

				x++;
			}

			W = line.Length;
			H++;
			line = sr.ReadLine();
		}
	}

	private void CreateGrid()
	{
		Grid = new int[W, H];
		var sr = new StreamReader(File);
		var line = sr.ReadLine();
		var y = 0;
		while (!string.IsNullOrEmpty(line))
		{
			var x = 0;
			foreach (var c in line)
			{
				Grid[x, y] = c == '#' ? int.MaxValue : 0;
				x++;
			}

			y++;
			line = sr.ReadLine();
		}
	}

	private void SetCosts()
	{
		var x = StartX;
		var y = StartY;
		var moves = 0;
		while (!(x == EndX && y == EndY))
		{
			moves++;
			for (var d = 0; d < NumDirs; d++)
			{
				var nx = x + Dirs[d].Item1;
				var ny = y + Dirs[d].Item2;
				if (InBounds(nx, ny) && Grid[nx, ny] == 0 && !(nx == StartX && ny == StartY))
				{
					Grid[nx, ny] = moves;
					x = nx;
					y = ny;
					break;
				}
			}
		}
	}

	private bool InBounds(int x, int y)
	{
		return x >= 0 && x < W && y >= 0 && y < H;
	}
}
