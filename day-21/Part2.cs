internal class Part2
{
	private const string File = "input.txt";

	private const int NumPinPadRobots = 25;
	private const int NumRobots = NumPinPadRobots + 1;
	private Dictionary<string, long>[] Caches = new Dictionary<string, long>[NumRobots];
	private readonly List<string> Codes = new();

	/// <summary>
	/// DPad:
	///    +---+---+
	///    | U | A |
	///+---+---+---+
	///| L | D | R |
	///+---+---+---+
	///
	/// PinPad:
	/// +---+---+---+
	/// | 7 | 8 | 9 |
	/// +---+---+---+
	/// | 4 | 5 | 6 |
	/// +---+---+---+
	/// | 1 | 2 | 3 |
	/// +---+---+---+
	///     | 0 | A |
	///     +---+---+
	/// 
	/// For each move from one button to another, the possible sequences of moves on the next
	/// pad in the chain which needs to be input to generate that move. These paths will never
	/// change directions more than once as that would not produce an optimal path on the next pad.
	/// </summary>
	private static readonly Dictionary<string, List<string>> PadMoves = new()
	{
		// Pinpad moves. Only the ones we need.
		{"A1", new() { "ULLA" }},
		{"14", new() { "UA" }},
		{"40", new() { "RDDA" }},
		{"0A", new() { "RA" }},
		{"17", new() { "UUA" }},
		{"70", new() { "RDDDA" }},
		{"16", new() { "RRUA", "URRA" }},
		{"69", new() { "UA" }},
		{"9A", new() { "DDDA" }},
		{"A8", new() { "LUUUA", "UUULA" }},
		{"80", new() { "DDDA" }},
		{"03", new() { "RUA", "URA" }},
		{"3A", new() { "DA" }},
		{"12", new() { "RA" }},
		{"29", new() { "RUUA", "UURA" }},

		// DPad moves.
		{"AA", new() { "A" }},
		{"AL", new() { "DLLA" }},
		{"AD", new() { "DLA", "LDA" }},
		{"AU", new() { "LA" }},
		{"AR", new() { "DA" }},
		{"LA", new() { "RRUA" }},
		{"LL", new() { "A" }},
		{"LD", new() { "RA" }},
		{"LU", new() { "RUA" }},
		{"LR", new() { "RRA" }},
		{"RA", new() { "UA" }},
		{"RL", new() { "LLA" }},
		{"RD", new() { "LA" }},
		{"RU", new() { "LUA", "ULA" }},
		{"RR", new() { "A" }},
		{"UA", new() { "RA" }},
		{"UL", new() { "DLA" }},
		{"UD", new() { "DA" }},
		{"UU", new() { "A" }},
		{"UR", new() { "DRA", "RDA" }},
		{"DA", new() { "RUA", "URA" }},
		{"DL", new() { "LA" }},
		{"DD", new() { "A" }},
		{"DU", new() { "UA" }},
		{"DR", new() { "RA" }},
	};

	public void Run()
	{
		ParseInput();

		var result = 0L;
		foreach (var code in Codes)
		{
			// Clear cache.
			for (var r = 0; r < NumRobots; r++)
				Caches[r] = new Dictionary<string, long>();

			// Prepend A since the robots start at A and need to move from it.
			var codeMoves = "A" + code;

			// Start looping at 1 to account for the prepended A.
			var total = 0L;
			for (var i = 1; i < codeMoves.Length; i++)
				total += GetCount(codeMoves.Substring(i - 1, 2), NumRobots - 1);
			result += total * int.Parse(code.Substring(0, code.Length - 1));
		}
		Console.WriteLine(result);
	}

	private void ParseInput()
	{
		var sr = new StreamReader(File);
		var line = sr.ReadLine();
		while (!string.IsNullOrEmpty(line))
		{
			Codes.Add(line);
			line = sr.ReadLine();
		}
	}

	private long GetCount(string instruction, int r)
	{
		// Get the moves required to execute the instruction.
		var moves = PadMoves[instruction];

		// The final robot. Even with the potential for multiples moves to execute
		// the instruction they will all be the same length.
		if (r == 0)
			return moves[0].Length;

		// Check each possible move to execute the instruction.
		var bestCount = long.MaxValue;
		foreach (var move in moves)
		{
			// Prepend an A.
			// If this is very first move, it must start on A.
			// For all other moves, A must have been pressed last in order to make the next robot press its button.
			var fullMove = "A" + move;

			if (!Caches[r].TryGetValue(fullMove, out var count))
			{
				// Start at 1 to account for the prepended A.
				for (var i = 1; i < fullMove.Length; i++)
					count += GetCount(fullMove.Substring(i - 1, 2), r - 1);
				Caches[r][fullMove] = count;
			}
			bestCount = Math.Min(count, bestCount);
		}
		return bestCount;
	}
}
