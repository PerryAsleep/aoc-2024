

var grid = new Grid();
Guard guard = null;

var sr = new StreamReader("input-06.txt");
var line = sr.ReadLine();
var y = 0;
while (line != null)
{
	grid.W = line.Length;
	grid.H = y + 1;
	var row = new List<Tile>();
	var x = 0;
	foreach(var c in line)
	{
		var t = new Tile();
		if (c == '#')
			t.Occupied = true;
		if (c == '^')
			guard = new Guard(grid, Facing.Up, x, y);
		row.Add(t);
		x++;
	}
	grid.Tiles.Add(row);
	y++;

	line = sr.ReadLine();
}

// Part 1.
while(guard.Move()) { }
Console.WriteLine(grid.GetNumVisitedTiles());

// Part 2.
var numLoops = 0;
for (var ty = 0; ty < grid.H; ty++)
{
	for (var tx = 0; tx < grid.H; tx++)
	{
		grid.Tiles[ty][tx].TemporaryObstacle = true;
		while (guard.Move() && !guard.InLoop) { }
		if (guard.InLoop)
			numLoops++;
		grid.Reset();
		guard.Reset();
	}
}
Console.WriteLine(numLoops);

class Grid
{
	public List<List<Tile>> Tiles = new();
	public int W;
	public int H;

	public int GetNumVisitedTiles()
	{
		var numVisitedTiles = 0;
		foreach (var row in Tiles)
		{
			foreach (var tile in row)
			{
				if (tile.Visited != null)
					numVisitedTiles++;
			}
		}
		return numVisitedTiles;
	}

	public void Reset()
	{
		foreach (var row in Tiles)
		{
			foreach (var tile in row)
			{
				tile.Visited = null;
				tile.TemporaryObstacle = false;
			}
		}
	}
}

class Tile
{
	public bool Occupied;
	public Facing? Visited;
	public bool TemporaryObstacle;

	public bool IsOccupied()
	{
		return Occupied || TemporaryObstacle;
	}
}

enum Facing
{
	Up,
	Down,
	Left,
	Right
}

class Guard
{
	readonly Grid Grid;
	public bool InLoop = false;
	Facing Facing = Facing.Up;
	public int X;
	public int Y;

	private readonly Facing StartingFacing;
	private readonly int StartingX;
	private readonly int StartingY;

	public Guard(Grid InGrid, Facing InFacing, int InX, int InY)
	{
		Grid = InGrid;
		StartingFacing = InFacing;
		StartingX = InX;
		StartingY = InY;
		Reset();
	}

	public void Reset()
	{
		Facing = StartingFacing;
		X = StartingX;
		Y = StartingY;
		InLoop = false;
	}

	public bool Move()
	{
		var NextX = X;
		var NextY = Y;
		switch(Facing)
		{
			case Facing.Up: NextY--; break;
			case Facing.Down: NextY++; break;
			case Facing.Left: NextX--; break;
			case Facing.Right: NextX++; break;
		}

		if (NextX < 0 || NextX >= Grid.W || NextY < 0 || NextY >= Grid.H)
			return false;

		while (Grid.Tiles[NextY][NextX].IsOccupied())
		{
			Facing = RotateRight(Facing);
			return Move();
		}

		X = NextX;
		Y = NextY;
		if (Grid.Tiles[Y][X].Visited == Facing)
			InLoop = true;
		Grid.Tiles[Y][X].Visited = Facing;
		return true;
	}

	private static Facing RotateRight(Facing facing)
	{
		switch (facing)
		{
			case Facing.Up: return Facing.Right;
			case Facing.Down: return Facing.Left;
			case Facing.Left: return Facing.Up;
			case Facing.Right: return Facing.Down;
		}
		return Facing.Right;
	}
}