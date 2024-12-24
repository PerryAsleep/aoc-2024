internal class Part1
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
	private readonly Dictionary<string, bool?> Values = new();

	public void Run()
	{
		ParseInput();

		var zIndex = 0;
		var zValues = new List<bool>();
		while (true)
		{
			var id = "z" + zIndex.ToString("00");
			if (!Values.ContainsKey(id))
				break;
			if (Values[id] == null)
				ComputeValue(id);
			zValues.Add((bool)Values[id]);
			zIndex++;
		}

		var value = 0L;
		for (var i = zValues.Count - 1; i >= 0; i--)
		{
			if (!zValues[i])
				continue;
			value += 1L << i;
		}

		Console.WriteLine(value);
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

				Values.TryAdd(n.Id, null);
				Values.TryAdd(n.Inputs[0], null);
				Values.TryAdd(n.Inputs[1], null);
			}

			line = sr.ReadLine();
		}
	}

	private void ComputeValue(string id)
	{
		if (Values[id] != null)
			return;
		var n = Nodes[id];
		ComputeValue(n.Inputs[0]);
		ComputeValue(n.Inputs[1]);
		switch (n.Operation)
		{
			case Operation.Or:
				Values[n.Id] = (bool)Values[n.Inputs[0]] || (bool)Values[n.Inputs[1]];
				break;
			case Operation.And:
				Values[n.Id] = (bool)Values[n.Inputs[0]] && (bool)Values[n.Inputs[1]];
				break;
			case Operation.Xor:
				Values[n.Id] = (bool)Values[n.Inputs[0]] ^ (bool)Values[n.Inputs[1]];
				break;
		}
	}
}
