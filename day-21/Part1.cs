using System.Text;

internal class Part1
{
	private const string File = "input.txt";

	public const int L = 0;
	public const int D = 1;
	public const int U = 2;
	public const int R = 3;
	public const int NumDirs = 4;
	public const int NumRobotsOnDPads = 2;

	public static readonly List<Tuple<int, int>> MoveDirs = new()
	{
		new Tuple<int, int>(-1, 0), // L
		new Tuple<int, int>(0, 1),  // D
		new Tuple<int, int>(0, -1), // U
		new Tuple<int, int>(1, 0),  // R
	};

	internal abstract class KeyPad
	{
		public abstract KeyPadState CreateInitialKeyPadState();
		protected abstract bool IsValidPos(Tuple<int, int> pos);

		protected List<List<Tuple<int, int>>> GetPossibleSequencesToMoveTo(Tuple<int, int> from, Tuple<int, int> to)
		{
			var result = new List<List<Tuple<int, int>>>();

			// No change in position.
			if (from.Item1 == to.Item1 && from.Item2 == to.Item2)
				return null;

			// Only change in X.
			if (from.Item2 == to.Item2)
			{
				result.Add(GetXPath(from.Item1, to.Item1));
				return result;
			}

			// Only change in Y.
			if (from.Item1 == to.Item1)
			{
				result.Add(GetYPath(from.Item2, to.Item2));
				return result;
			}

			// Change in both X and Y. Try X then Y, and then Y then X.
			var xPath = GetXPath(from.Item1, to.Item1);
			var yPath = GetYPath(from.Item2, to.Item2);

			var xThenY = new List<Tuple<int, int>>();
			xThenY.AddRange(xPath);
			xThenY.AddRange(yPath);
			if (IsPathValid(from, xThenY))
				result.Add(xThenY);

			var yThenX = new List<Tuple<int, int>>();
			yThenX.AddRange(yPath);
			yThenX.AddRange(xPath);
			if (IsPathValid(from, yThenX))
				result.Add(yThenX);

			return result;
		}

		protected List<List<Tuple<int, int>>> AddButtonPressToSequences(List<List<Tuple<int, int>>> sequences)
		{
			// There are existing sequences. Each one needs a button press move added to it.
			if (sequences != null)
			{
				foreach (var sequence in sequences)
				{
					sequence.Add(null);
				}
				return sequences;
			}

			// No existing sequences. We still need to one which has one move to press the button.
			Tuple<int, int> buttonPress = null;
			var movesToButtonPress = new List<Tuple<int, int>>() { buttonPress };
			sequences = new List<List<Tuple<int, int>>>() { movesToButtonPress };
			return sequences;
		}

		private bool IsPathValid(Tuple<int, int> from, List<Tuple<int, int>> path)
		{
			var pos = from;
			foreach (var move in path)
			{
				pos = new Tuple<int, int>(pos.Item1 + move.Item1, pos.Item2 + move.Item2);
				if (!IsValidPos(pos))
					return false;
			}
			return true;
		}

		private static List<Tuple<int, int>> GetXPath(int x1, int x2)
		{
			var path = new List<Tuple<int, int>>();
			while (x1 < x2)
			{
				path.Add(new Tuple<int, int>(1, 0));
				x1++;
			}

			while (x1 > x2)
			{
				path.Add(new Tuple<int, int>(-1, 0));
				x1--;
			}
			return path;
		}

		private static List<Tuple<int, int>> GetYPath(int y1, int y2)
		{
			var path = new List<Tuple<int, int>>();
			while (y1 < y2)
			{
				path.Add(new Tuple<int, int>(0, 1));
				y1++;
			}

			while (y1 > y2)
			{
				path.Add(new Tuple<int, int>(0, -1));
				y1--;
			}
			return path;
		}

		protected static int GetCostToMove(Tuple<int, int> from, Tuple<int, int> to)
		{
			return Math.Abs(to.Item1 - from.Item1) + Math.Abs(to.Item2 - from.Item2);
		}
	}

	/// <summary>
	///     +---+---+
	///     | ^ | A |
	/// +---+---+---+
	/// | < | v | > |
	/// +---+---+---+
	/// </summary>
	internal class DPad : KeyPad
	{
		public static readonly List<Tuple<int, int>> PosDirs = new()
		{
			new Tuple<int, int>(0, 1), // L
			new Tuple<int, int>(1, 1), // D
			new Tuple<int, int>(1, 0), // U
			new Tuple<int, int>(2, 1), // R
		};
		public static readonly Tuple<int, int> PosA = new(2, 0);

		public override KeyPadState CreateInitialKeyPadState()
		{
			return new KeyPadState(PosA);
		}

		public static (KeyPadState, int) GetCostToMove(KeyPadState state, Tuple<int, int> move)
		{
			if (move == null)
				return (new KeyPadState(PosA), GetCostToMove(state.Pos, PosA) + 1);
			for (var d = 0; d < NumDirs; d++)
				if (Equals(move, MoveDirs[d]))
					return (new KeyPadState(PosDirs[d]), GetCostToMove(state.Pos, PosDirs[d]) + 1);
			return (null, int.MaxValue);
		}

		protected override bool IsValidPos(Tuple<int, int> pos)
		{
			return pos.Item1 >= 0 && pos.Item1 <= 2 && pos.Item2 >= 0 && pos.Item2 <= 1 && !(pos.Item1 == 0 && pos.Item2 == 0);
		}

		public (KeyPadState, List<List<Tuple<int, int>>>) GetPossibleSequencesToEnterMoveOnNextKeyPad(KeyPadState state, Tuple<int, int> moveOnNextKeyPad)
		{
			var (newState, sequences) = moveOnNextKeyPad == null ? GetPossibleSequencesToMoveToA(state) : GetPossibleSequencesToMoveToDir(state, moveOnNextKeyPad);
			sequences = AddButtonPressToSequences(sequences);
			return (newState, sequences);
		}

		private (KeyPadState, List<List<Tuple<int, int>>>) GetPossibleSequencesToMoveToDir(KeyPadState state, Tuple<int, int> moveOnNextKeyPad)
		{
			for (var d = 0; d < NumDirs; d++)
				if (Equals(moveOnNextKeyPad, MoveDirs[d]))
					return GetPossibleSequencesToMoveToDir(state, d);
			return (null, null);
		}

		private (KeyPadState, List<List<Tuple<int, int>>>) GetPossibleSequencesToMoveToDir(KeyPadState state, int d)
		{
			return (new KeyPadState(PosDirs[d]), GetPossibleSequencesToMoveTo(state.Pos, PosDirs[d]));
		}

		private (KeyPadState, List<List<Tuple<int, int>>>) GetPossibleSequencesToMoveToA(KeyPadState state)
		{
			return (new KeyPadState(PosA), GetPossibleSequencesToMoveTo(state.Pos, PosA));
		}
	}

	/// <summary>
	/// +---+---+---+
	/// | 7 | 8 | 9 |
	/// +---+---+---+
	/// | 4 | 5 | 6 |
	/// +---+---+---+
	/// | 1 | 2 | 3 |
	/// +---+---+---+
	///     | 0 | A |
	///     +---+---+
	/// </summary>
	internal class PinPad : KeyPad
	{
		public static readonly List<Tuple<int, int>> PosNums = new()
		{
			new Tuple<int, int>(1, 3),
			new Tuple<int, int>(0, 2),
			new Tuple<int, int>(1, 2),
			new Tuple<int, int>(2, 2),
			new Tuple<int, int>(0, 1),
			new Tuple<int, int>(1, 1),
			new Tuple<int, int>(2, 1),
			new Tuple<int, int>(0, 0),
			new Tuple<int, int>(1, 0),
			new Tuple<int, int>(2, 0),
		};
		public static readonly Tuple<int, int> PosA = new(2, 3);

		public override KeyPadState CreateInitialKeyPadState()
		{
			return new KeyPadState(PosA);
		}

		protected override bool IsValidPos(Tuple<int, int> pos)
		{
			return pos.Item1 >= 0 && pos.Item1 <= 2 && pos.Item2 >= 0 && pos.Item2 <= 3 && !(pos.Item1 == 0 && pos.Item2 == 3);
		}

		public (KeyPadState, List<List<Tuple<int, int>>>) GetPossibleSequencesToEnterNum(KeyPadState state, int num)
		{
			return (new KeyPadState(PosNums[num]), AddButtonPressToSequences(GetPossibleSequencesToMoveTo(state.Pos, PosNums[num])));
		}

		public (KeyPadState, List<List<Tuple<int, int>>>) GetPossibleSequencesToEnterA(KeyPadState state)
		{
			return (new KeyPadState(PosA), AddButtonPressToSequences(GetPossibleSequencesToMoveTo(state.Pos, PosA)));
		}
	}

	internal class KeyPadState
	{
		public readonly Tuple<int, int> Pos;
		public KeyPadState(Tuple<int, int> inPos)
		{
			Pos = inPos;
		}
	}

	private readonly List<string> Codes = new();

	public void Run()
	{
		ParseInput();

		var result = 0;
		foreach (var code in Codes)
		{
			var sequence = GetShortestSequence(code);
			PrintSequence(code, sequence);
			result += sequence.Count * int.Parse(code.Substring(0, code.Length - 1));
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

	private List<Tuple<int, int>> GetShortestSequence(string code)
	{
		var possibleSequences = GetPossibleSequencesForPinPad(code);
		var sequence = ChooseBestSequence(possibleSequences);
		for (var dpadIndex = 0; dpadIndex < NumRobotsOnDPads; dpadIndex++)
		{
			possibleSequences = GetPossibleSequencesForDPad(sequence);
			sequence = ChooseBestSequence(possibleSequences);
		}
		return sequence;
	}

	private List<List<Tuple<int, int>>> GetPossibleSequencesForPinPad(string code)
	{
		var pinPad = new PinPad();
		var pinPadState = pinPad.CreateInitialKeyPadState();
		var possibleSequences = new List<List<Tuple<int, int>>>();

		var codeIndex = 0;
		var state = pinPadState;
		while (codeIndex< code.Length)
		{
			var pinPadKey = code[codeIndex];

			List<List<Tuple<int, int>>> sequencesToNextPinPadKey = null;
			KeyPadState newState;
			if (int.TryParse(pinPadKey.ToString(), out var pinPadNum))
				(newState, sequencesToNextPinPadKey) = pinPad.GetPossibleSequencesToEnterNum(state, pinPadNum);
			else
				(newState, sequencesToNextPinPadKey) = pinPad.GetPossibleSequencesToEnterA(state);

			if (possibleSequences.Count > 0)
			{
				var newSequences = new List<List<Tuple<int, int>>>();
				foreach (var possibleSequence in possibleSequences)
				{
					foreach (var nextSequence in sequencesToNextPinPadKey)
					{
						var combined = new List<Tuple<int, int>>();
						combined.AddRange(possibleSequence);
						combined.AddRange(nextSequence);
						newSequences.Add(combined);
					}
				}
				possibleSequences = newSequences;
			}
			else
			{
				possibleSequences = sequencesToNextPinPadKey;
			}

			state = newState;
			codeIndex++;
		}

		return possibleSequences;
	}

	private List<List<Tuple<int, int>>> GetPossibleSequencesForDPad(List<Tuple<int, int>> sequence)
	{
		var dPad = new DPad();
		var state = dPad.CreateInitialKeyPadState();
		var possibleSequences = new List<List<Tuple<int, int>>>();

		var i = 0;
		foreach (var move in sequence)
		{
			var (newState, sequencesToNext) = dPad.GetPossibleSequencesToEnterMoveOnNextKeyPad(state, move);

			if (possibleSequences.Count > 0)
			{
				var newSequences = new List<List<Tuple<int, int>>>();
				foreach (var possibleSequence in possibleSequences)
				{
					foreach (var nextSequence in sequencesToNext)
					{
						var combined = new List<Tuple<int, int>>();
						combined.AddRange(possibleSequence);
						combined.AddRange(nextSequence);
						newSequences.Add(combined);
					}
				}
				possibleSequences = newSequences;
			}
			else
			{
				possibleSequences = sequencesToNext;
			}
			state = newState;
			i++;
		}

		return possibleSequences;
	}

	private List<Tuple<int, int>> ChooseBestSequence(List<List<Tuple<int, int>>> possibleSequences)
	{
		var dPad = new DPad();

		// Choose best sequence.
		var bestCost = int.MaxValue;
		var bestIndex = 0;
		for (var sequenceIndex = 0; sequenceIndex < possibleSequences.Count; sequenceIndex++)
		{
			var sequence = possibleSequences[sequenceIndex];
			var dPadState = dPad.CreateInitialKeyPadState();
			var totalCost = 0;
			foreach (var move in sequence)
			{
				(dPadState, var cost) = DPad.GetCostToMove(dPadState, move);
				totalCost += cost;
			}

			if (totalCost < bestCost)
			{
				bestCost = totalCost;
				bestIndex = sequenceIndex;
			}
		}

		return possibleSequences[bestIndex];
	}

	private void PrintSequence(string code, List<Tuple<int, int>> sequence)
	{
		var sb = new StringBuilder();
		sb.Append($"{code}: ");
		foreach (var move in sequence)
		{
			if (move == null)
				sb.Append("A");
			else if (move.Equals(MoveDirs[U]))
				sb.Append("U");
			else if (move.Equals(MoveDirs[D]))
				sb.Append("D");
			else if (move.Equals(MoveDirs[L]))
				sb.Append("L");
			else if (move.Equals(MoveDirs[R]))
				sb.Append("R");
		}
		Console.WriteLine(sb);
	}
}
