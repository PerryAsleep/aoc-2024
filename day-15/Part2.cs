using System.Text;

internal class Part2
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
		BoxL,
		BoxR,
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
		Render();
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
					foreach (var c in line)
					{
						switch (c)
						{
							case '#':
								row.Add(Actor.Wall);
								row.Add(Actor.Wall);
								break;
							case 'O':
								row.Add(Actor.BoxL);
								row.Add(Actor.BoxR);
								break;
							case '@':
								row.Add(Actor.Robot);
								RobotPos = new Tuple<int, int>(x, H - 1);
								row.Add(Actor.None);
								break;
							case '.':
							default:
								row.Add(Actor.None);
								row.Add(Actor.None);
								break;
						}

						x+=2;
					}
					W = x;

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
		var stop = false;
		while (!stop)
		{
			var key = Console.ReadKey(true);
			switch (key.KeyChar)
			{
				case 'w':
					Move(Dir.U);
					Console.WriteLine(Dir.U);
					Render();
					break;
				case 'a':
					Move(Dir.L);
					Console.WriteLine(Dir.L);
					Render();
					break;
				case 's':
					Move(Dir.D);
					Console.WriteLine(Dir.D);
					Render();
					break;
				case 'd': Move(Dir.R);
					Move(Dir.R);
					Console.WriteLine(Dir.R);
					Render();
					break;
				case 'x':
					stop = true;
					break;
			}
		}

		foreach (var dir in Instructions)
		{
			Move(dir);
		}
	}

	private void Move(Dir dir)
	{
		var boxesToMove = new List<Tuple<int, int>>();
		var exposedEdges = new List<Tuple<int, int>>();
		var edgesToRemove = new List<Tuple<int, int>>();
		var edgesToAdd = new List<Tuple<int, int>> { new(RobotPos.Item1, RobotPos.Item2) };
		var movement = Movements[dir];
		while (edgesToAdd.Count > 0)
		{
			// Update edges.
			foreach (var edge in edgesToRemove)
				exposedEdges.Remove(edge);
			edgesToRemove.Clear();
			foreach (var edge in edgesToAdd)
				exposedEdges.Add(edge);
			edgesToAdd.Clear();

			var blocked = false;
			foreach (var edge in exposedEdges)
			{
				var x = edge.Item1 + movement.Item1;
				var y = edge.Item2 + movement.Item2;

				if (!InBounds(x, y))
				{
					blocked = true;
					break;
				}

				if (Grid[y][x] == Actor.Wall)
				{
					blocked = true;
					break;
				}

				if (Grid[y][x] == Actor.BoxL || Grid[y][x] == Actor.BoxR)
				{
					// Add the box to our list of boxes needing to be moved.
					if (Grid[y][x] == Actor.BoxL)
						boxesToMove.Add(new(x, y));
					else
						boxesToMove.Add(new(x - 1, y));

					// This edge is no longer exposed. We need to remove it.
					edgesToRemove.Add(edge);

					// Add the exposed edges from the new box. Would be better to generalize this.
					switch (dir)
					{
						case Dir.L:
							edgesToAdd.Add(new(x - 1, y));
							break;
						case Dir.R:
							edgesToAdd.Add(new(x + 1, y));
							break;
						case Dir.U:
							if (Grid[y][x] == Actor.BoxL)
							{
								edgesToAdd.Add(new(x, y));
								edgesToAdd.Add(new(x + 1, y));
							}
							else
							{
								edgesToAdd.Add(new(x, y));
								edgesToAdd.Add(new(x - 1, y));
							}
							break;
						case Dir.D:
							if (Grid[y][x] == Actor.BoxL)
							{
								edgesToAdd.Add(new(x, y));
								edgesToAdd.Add(new(x + 1, y));
							}
							else
							{
								edgesToAdd.Add(new(x, y));
								edgesToAdd.Add(new(x - 1, y));
							}
							break;
					}

					continue;
				}
			}

			// Can't move.
			if (blocked)
				break;

			// Need to keep checking.
			if (edgesToAdd.Count > 0)
				continue;

			// Clear all the boxes and the robot.
			Grid[RobotPos.Item2][RobotPos.Item1] = Actor.None;
			if (boxesToMove.Count > 0)
			{
				foreach (var box in boxesToMove)
				{
					Grid[box.Item2][box.Item1] = Actor.None;
					Grid[box.Item2][box.Item1 + 1] = Actor.None;
				}
			}

			// Set updated positions of the boxes and the robot.
			RobotPos = new Tuple<int, int>(RobotPos.Item1 + movement.Item1, RobotPos.Item2 + movement.Item2);
			Grid[RobotPos.Item2][RobotPos.Item1] = Actor.Robot;
			if (boxesToMove.Count > 0)
			{
				foreach (var box in boxesToMove)
				{
					Grid[box.Item2 + movement.Item2][box.Item1 + movement.Item1] = Actor.BoxL;
					Grid[box.Item2 + movement.Item2][box.Item1 + movement.Item1 + 1] = Actor.BoxR;
				}
			}
		}
	}

	private void Render()
	{
		var sb = new StringBuilder();
		for (var y = 0; y < H; y++)
		{
			for (var x = 0; x < W; x++)
			{
				switch (Grid[y][x])
				{
					case Actor.BoxL: sb.Append('['); break;
					case Actor.BoxR: sb.Append(']'); break;
					case Actor.Robot: sb.Append('@'); break;
					case Actor.Wall: sb.Append('#'); break;
					case Actor.None: sb.Append('.'); break;
				}
			}
			sb.Append("\n");
		}
		sb.Append("\n");
		Console.Write(sb);
	}

	private int ScoreGrid()
	{
		var score = 0;
		for (var x = 0; x < W; x++)
		{
			for (var y = 0; y < H; y++)
			{
				if (Grid[y][x] == Actor.BoxL)
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
