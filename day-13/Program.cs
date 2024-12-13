var sr = new StreamReader("input.txt");

var games = new List<Game>();

var line = sr.ReadLine();
while (line != null)
{
	var a = new Button();
	var str = line.Split("X+")[1].Split(", Y+");
	a.X = int.Parse(str[0]);
	a.Y = int.Parse(str[1]);
	a.Cost = 3;
	
	line = sr.ReadLine();
	var b = new Button();
	str = line.Split("X+")[1].Split(", Y+");
	b.X = int.Parse(str[0]);
	b.Y = int.Parse(str[1]);
	b.Cost = 1;

	line = sr.ReadLine();
	var g = new Game();
	str = line.Split("X=")[1].Split(", Y=");
	g.PrizeX = int.Parse(str[0]);
	g.PrizeY = int.Parse(str[1]);
	g.A = a;
	g.B = b;
	games.Add(g);

	line = sr.ReadLine();
	if (line != null)
		line = sr.ReadLine();
}

// Part 1.
var sum = 0L;
foreach(var game in games)
{
	sum += game.GetMinCostToWinPart1();
}
Console.WriteLine(sum);

// Part 2.
sum = 0L;
foreach (var game in games)
{
	sum += game.GetMinCostToWinPart2();
}
Console.WriteLine(sum);

class Button
{
	public int X;
	public int Y;
	public int Cost;
}

class Game
{
	public Button A;
	public Button B;
	public int PrizeX;
	public int PrizeY;

	public long GetMinCostToWinPart1()
	{
		return GetCostToWin(PrizeX, PrizeY);
	}

	public long GetMinCostToWinPart2()
	{
		return GetCostToWin(PrizeX + 10000000000000, PrizeY + 10000000000000);
	}

	private static (long, long) Intersection(long x1, long y1, long x2, long y2, long x3, long y3, long x4, long y4)
	{
		var px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) /
		         ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
		var py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) /
		         ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
		return (px, py);
	}

	private static bool IsWhole(double d)
	{
		return Math.Abs(d - Math.Truncate(d)) <= double.Epsilon;
	}

	private long GetCostToWin(long prizeX, long prizeY)
	{
		var (ix, iy) = Intersection(0, 0, A.X, A.Y, prizeX - B.X, prizeY - B.Y, prizeX, prizeY);
		if (IsWhole((double)ix / A.X) && IsWhole((double)(prizeX - ix) / B.X))
			return A.Cost * (ix / A.X) + B.Cost * ((prizeX - ix) / B.X);
		return 0;
	}
}