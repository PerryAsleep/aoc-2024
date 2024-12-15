internal class Part1
{
	private enum Dir
	{
		U,
		D,
		L,
		R,
	}

	private enum Actor
	{
		Robot,
		Box,
		Wall,
		None,
	}

	private readonly Dictionary<Dir, Tuple<int, int>> Movements = new()
	{
		{ Dir.L, new Tuple<int, int>(-1, 0) },
		{ Dir.D, new Tuple<int, int>(0, 1) },
		{ Dir.U, new Tuple<int, int>(0, -1) },
		{ Dir.R, new Tuple<int, int>(1, 0) },
	};

	private readonly List<List<Actor>> Grid = new();
	private readonly List<Dir> Instructions = new();
	private Tuple<int, int> RobotPos = new(0, 0);
	private int W = 0;
	private int H = 0;

	public void Run()
	{
		ParseInput();
		ExecuteInstructions();
		Console.WriteLine(ScoreGrid());
	}

	private void ParseInput()
	{
		var sr = new StreamReader("input.txt");
		var line = sr.ReadLine();
		var readingInstructions = false;
		while (line != null)
		{
			if (!readingInstructions)
			{
				if (line.Length == 0)
				{
					readingInstructions = true;
				}
				else
				{
					var row = new List<Actor>();
					var x = 0;
					H++;
					W = line.Length;
					foreach (var c in line)
					{
						switch (c)
						{
							case '#':
								row.Add(Actor.Wall);
								break;
							case 'O':
								row.Add(Actor.Box);
								break;
							case '@':
								row.Add(Actor.Robot);
								RobotPos = new Tuple<int, int>(x, H - 1);
								break;
							case '.':
							default:
								row.Add(Actor.None);
								break;
						}

						x++;
					}

					Grid.Add(row);
				}
			}
			else
			{
				foreach (var c in line)
				{
					switch (c)
					{
						case '<':
							Instructions.Add(Dir.L);
							break;
						case '^':
							Instructions.Add(Dir.U);
							break;
						case 'v':
							Instructions.Add(Dir.D);
							break;
						case '>':
							Instructions.Add(Dir.R);
							break;
					}
				}
			}

			line = sr.ReadLine();
		}
	}

	private void ExecuteInstructions()
	{
		foreach (var dir in Instructions)
		{
			var movement = Movements[dir];
			var x = RobotPos.Item1;
			var y = RobotPos.Item2;
			var numBoxes = 0;
			while (true)
			{
				x += movement.Item1;
				y += movement.Item2;
				if (!InBounds(x, y))
					break;
				if (Grid[y][x] == Actor.Wall)
					break;
				if (Grid[y][x] == Actor.Box)
				{
					numBoxes++;
					continue;
				}

				if (Grid[y][x] == Actor.None)
				{
					if (numBoxes > 0)
						Grid[y][x] = Actor.Box;
					Grid[RobotPos.Item2][RobotPos.Item1] = Actor.None;
					RobotPos = new Tuple<int, int>(RobotPos.Item1 + movement.Item1, RobotPos.Item2 + movement.Item2);
					Grid[RobotPos.Item2][RobotPos.Item1] = Actor.Robot;
					break;
				}
			}
		}
	}

	private int ScoreGrid()
	{
		var score = 0;
		for (var x = 0; x < W; x++)
		{
			for (var y = 0; y < H; y++)
			{
				if (Grid[y][x] == Actor.Box)
				{
					score += 100 * y + x;
				}
			}
		}

		return score;
	}

	private bool InBounds(int x, int y)
	{
		return x >= 0 && x < W && y >= 0 && y < H;
	}
}
