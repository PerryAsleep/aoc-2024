internal class Part2
{
	private const long Target = 2412754513550330L;
	private const int Digits = 16;

	public void Run()
	{
		Search(0, Digits - 1);
	}

	public bool Search(long a, int i)
	{
		a <<= 3;
		var t = Target % (long)Math.Pow(10, Digits - i);
		for (var r = 0; r < 8; r++)
		{
			if (ExecuteProgram(a + r) == t)
			{
				if (i == 0)
				{
					Console.WriteLine(a + r);
					return true;
				}
				if (Search(a + r, i - 1))
					return true;
			}
		}
		return false;
	}

	public long ExecuteProgram(long a)
	{
		var result = 0L;
		do
		{
			var b = ((a % 8) ^ 2);
			b = b ^ (a >> (int)b);
			b = b ^ 3;
			result = result * 10 + b % 8;
			a >>= 3;
		} while (a != 0);
		return result;
	}
}
