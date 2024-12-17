using System.Text;

internal class Part1
{
	private const string File = "input.txt";

	private int[] Registers = new int[3];
	private List<int> Instructions = new();
	private List<int> Output = new();
	public void Run()
	{
		ParseInput();
		ExecuteInstructions();

		var sb = new StringBuilder();
		for (var o = 0; o < Output.Count; o++)
		{
			if (o > 0)
				sb.Append(',');
			sb.Append(Output[o]);
		}
		Console.WriteLine(sb);
	}

	private void ParseInput()
	{
		var sr = new StreamReader(File);
		Registers[0] = int.Parse(sr.ReadLine().Split(" ")[2]);
		Registers[1] = int.Parse(sr.ReadLine().Split(" ")[2]);
		Registers[2] = int.Parse(sr.ReadLine().Split(" ")[2]);
		sr.ReadLine();
		foreach (var str in sr.ReadLine().Split(' ')[1].Split(','))
			Instructions.Add(int.Parse(str));
	}

	public void ExecuteInstructions()
	{
		var i = 0;
		while (true)
		{
			if (i + 1 >= Instructions.Count)
				break;

			var opcode = Instructions[i];
			var operand = Instructions[i + 1];
			var combo = operand;
			if (combo > 3)
				combo = Registers[operand - 4];
			i += 2;
			switch (opcode)
			{
				// adv
				case 0:
					Registers[0] = (int)(Registers[0] / Math.Pow(2, combo));
					break;
				// bxl
				case 1:
					Registers[1] ^= operand;
					break;
				// bst
				case 2:
					Registers[1] = combo % 8;
					break;
				// jnz
				case 3:
					if (Registers[0] != 0)
						i = operand;
					break;
				// bxc
				case 4:
					Registers[1] ^= Registers[2];
					break;
				// out
				case 5:
					Output.Add(combo % 8);
					break;
				// bdv
				case 6:
					Registers[1] = (int)(Registers[0] / Math.Pow(2, combo));
					break;
				// cdv
				case 7:
					Registers[2] = (int)(Registers[0] / Math.Pow(2, combo));
					break;
			}
		}
	}
}
