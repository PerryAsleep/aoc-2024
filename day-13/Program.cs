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
		return GetMinCostToWin(PrizeX, PrizeY);
	}

	public long GetMinCostToWinPart2()
	{
		return GetMinCostToWin(PrizeX + 10000000000000, PrizeY + 10000000000000);
	}

	private long GetMinCostToWin(long prizeX, long prizeY)
	{
		// Binary search bounds.
		var maxAPressesInX = prizeX / A.X;
		var maxAPressesInY = prizeY / A.Y;
		var maxAPresses = Math.Min(maxAPressesInX, maxAPressesInY);
		var minAPresses = 0L;
		var curAPresses = maxAPresses / 2;

		// Cached values.
		var aSlope = (double)A.Y / A.X;
		var bSlope = (double)B.Y / B.X;
		var slopeALessThanB = aSlope < bSlope;

		// Binary search A presses.
		while (true)
		{
			// Shift to B coordinate space to make math easier.
			var destinationXA = A.X * curAPresses;
			var destinationYA = A.Y * curAPresses;
			var targetX = prizeX - destinationXA;
			var targetY = prizeY - destinationYA;

			// equation for B; y = mx+b; y = (B.Y/B.X)x
			var bYAtTargetX = targetX * bSlope;

			var curBPresses = Math.Min(targetX / B.X, targetY / B.Y);
			var destinationX = destinationXA + B.X * curBPresses;
			var destinationY = destinationYA + B.Y * curBPresses;

			// Check if we won.
			if (destinationX == prizeX && destinationY == prizeY)
				return A.Cost * curAPresses + B.Cost * curBPresses;

			// Advance A based on if we overshot in Y or not.
			var over = bYAtTargetX > targetY;
			var increaseA = (over && slopeALessThanB) || (!over && !slopeALessThanB);
			if (increaseA)
			{
				minAPresses = curAPresses;
				var newAPresses = minAPresses + (maxAPresses - minAPresses) / 2;
				if (curAPresses >= newAPresses)
					break;
				curAPresses = newAPresses;
			}
			else
			{
				maxAPresses = curAPresses;
				var newAPresses = minAPresses + (maxAPresses - minAPresses) / 2;
				if (curAPresses <= newAPresses)
					break;
				curAPresses = newAPresses;
			}
		}

		return 0;
	}
}