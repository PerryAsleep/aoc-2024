internal class Part2
{
	private const string File = "input.txt";

	private enum Operation
	{
		And,
		Or,
		Xor,
	}

	private class Node
	{
		public Node(string id, Operation op, List<string> inputs)
		{
			Id = id;
			Operation = op;
			Inputs = inputs;
		}

		public readonly string Id;
		public readonly Operation Operation;
		public readonly List<string> Inputs;
	}

	private readonly Dictionary<string, Node> Nodes = new();
	private readonly Dictionary<string, List<string>> Inputs = new();
	private readonly Dictionary<string, bool?> Values = new();
	private readonly HashSet<string> IncorrectOutputs = new();
	private int MaxDigit;

	public void Run()
	{
		ParseInput();

		VerifyOrGates();
		VerifyAndGates();
		VerifyXorGates();

		var list = new List<string>();
		foreach (var incorrectOutput in IncorrectOutputs)
			list.Add(incorrectOutput);
		list.Sort();
		Console.WriteLine(string.Join(',', list));
	}

	private void ParseInput()
	{
		var sr = new StreamReader(File);
		var line = sr.ReadLine();
		var readingInitialValues = true;
		while (line != null)
		{
			if (line.Length == 0)
			{
				readingInitialValues = false;
				line = sr.ReadLine();
			}

			if (readingInitialValues)
			{
				var parts = line.Split(": ");
				Values.Add(parts[0], parts[1] == "1");
				MaxDigit = Math.Max(MaxDigit, int.Parse(parts[0].Substring(1, 2)));
			}
			else
			{
				var parts = line.Split(" -> ");
				var output = parts[1];
				parts = parts[0].Split(" ");
				var inputs = new List<string>() { parts[0], parts[2] };
				var op = Operation.And;
				switch (parts[1])
				{
					case "AND":
						op = Operation.And;
						break;
					case "OR":
						op = Operation.Or;
						break;
					case "XOR":
						op = Operation.Xor;
						break;
				}

				var n = new Node(output, op, inputs);
				Nodes.Add(n.Id, n);
				Inputs.TryAdd(inputs[0], new List<string>());
				Inputs.TryAdd(inputs[1], new List<string>());
				Inputs[inputs[0]].Add(output);
				Inputs[inputs[1]].Add(output);
				Values.TryAdd(n.Id, null);
				Values.TryAdd(n.Inputs[0], null);
				Values.TryAdd(n.Inputs[1], null);
			}

			line = sr.ReadLine();
		}
	}

	private void VerifyOrGates()
	{
		foreach (var n in Nodes)
		{
			if (n.Value.Operation == Operation.Or)
			{
				// Final full adder OR gate.
				if (n.Key == GetOutputGateId(MaxDigit + 1))
					VerifyOutputs(n.Key);

				// Standard full adder OR gate.
				else
					VerifyOutputs(n.Key, Operation.Xor, Operation.And);
			}
		}
	}

	private void VerifyAndGates()
	{
		foreach (var n in Nodes)
		{
			if (n.Value.Operation == Operation.And)
			{
				// Half adder AND gate.
				if (((n.Value.Inputs[0].StartsWith("x") && n.Value.Inputs[1].StartsWith("y"))
				     || (n.Value.Inputs[0].StartsWith("y") && n.Value.Inputs[1].StartsWith("x")))
				    && n.Value.Inputs[0].EndsWith("00") && n.Value.Inputs[1].EndsWith("00"))
				{
					VerifyOutputs(n.Key, Operation.Xor, Operation.And);
				}

				// Full adder AND gate.
				else
				{
					VerifyOutputs(n.Key, Operation.Or);
				}
			}
		}
	}

	private void VerifyXorGates()
	{
		foreach (var n in Nodes)
		{
			if (n.Value.Operation == Operation.Xor)
			{
				// First XOR gate.
				if ((n.Value.Inputs[0].StartsWith("x") && n.Value.Inputs[1].StartsWith("y"))
				    || (n.Value.Inputs[0].StartsWith("y") && n.Value.Inputs[1].StartsWith("x")))
				{
					// Half adder first XOR gate.
					if (n.Value.Inputs[0].EndsWith("00") && n.Value.Inputs[1].EndsWith("00"))
					{
						VerifyOutputs(n.Key);
					}

					// Full adder first XOR gate.
					else
					{
						VerifyOutputs(n.Key, Operation.Xor, Operation.And);
					}
				}

				// Full adder second XOR gate.
				else
				{
					VerifyOutputs(n.Key);
				}
			}
		}
	}

	private void VerifyOutputs(string id, Operation oA, Operation oB)
	{
		if (!Inputs.ContainsKey(id))
		{
			IncorrectOutputs.Add(id);
			return;
		}

		if (Inputs[id].Count != 2)
		{
			IncorrectOutputs.Add(id);
			return;
		}

		var nodeA = Nodes[Inputs[id][0]];
		var nodeB = Nodes[Inputs[id][1]];
		if (nodeA.Operation == oA && nodeB.Operation == oB)
			(nodeA, nodeB) = (nodeB, nodeA);
		if (nodeA.Operation != oB || nodeB.Operation != oA)
		{
			IncorrectOutputs.Add(id);
		}
	}

	private void VerifyOutputs(string id, Operation oA)
	{
		if (!Inputs.ContainsKey(id))
		{
			IncorrectOutputs.Add(id);
			return;
		}

		if (Inputs[id].Count != 1)
		{
			IncorrectOutputs.Add(id);
			return;
		}

		var node = Nodes[Inputs[id][0]];
		if (node.Operation != oA)
		{
			IncorrectOutputs.Add(id);
		}
	}

	private void VerifyOutputs(string id)
	{
		if (Inputs.ContainsKey(id))
		{
			IncorrectOutputs.Add(id);
		}
	}

	private string GetOutputGateId(int digit)
	{
		return "z" + digit.ToString("00");
	}
}
