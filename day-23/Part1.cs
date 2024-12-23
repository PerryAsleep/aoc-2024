internal class Part1
{
	private const string File = "input.txt";

	private class Node
	{
		public Node(string id)
		{
			Id = id;
		}

		public readonly string Id;
		public readonly Dictionary<string, Node> Neighbors = new();
	}

	private readonly Dictionary<string, Node> AllNodes = new();

	public void Run()
	{
		ParseInput();

		var sets = GetAllSetsOfThree();

		var count = 0;
		foreach (var set in sets)
		{
			foreach (var setKvp in set)
			{
				if (setKvp.Key.StartsWith('t'))
				{
					count++;
					break;
				}
			}
		}

		Console.WriteLine(count);
	}

	private void ParseInput()
	{
		var sr = new StreamReader(File);
		var line = sr.ReadLine();
		while (!string.IsNullOrEmpty(line))
		{
			var parts = line.Split("-");
			AllNodes.TryAdd(parts[0], new Node(parts[0]));
			AllNodes.TryAdd(parts[1], new Node(parts[1]));
			AllNodes[parts[0]].Neighbors.TryAdd(parts[1], AllNodes[parts[1]]);
			AllNodes[parts[1]].Neighbors.TryAdd(parts[0], AllNodes[parts[0]]);
			line = sr.ReadLine();
		}
	}

	private List<Dictionary<string, Node>> GetAllSetsOfThree()
	{
		HashSet<int> setIds = new();
		List<Dictionary<string, Node>> sets = new();

		foreach (var aKvp in AllNodes)
		{
			foreach (var bKvp in aKvp.Value.Neighbors)
			{
				foreach (var cKvp in bKvp.Value.Neighbors)
				{
					if (cKvp.Value.Neighbors.ContainsKey(aKvp.Key))
					{
						var setHash = HashIds(aKvp.Key, bKvp.Key, cKvp.Key);
						if (!setIds.Contains(setHash))
						{
							setIds.Add(setHash);
							sets.Add(new Dictionary<string, Node>
							{
								[aKvp.Key] = aKvp.Value,
								[bKvp.Key] = bKvp.Value,
								[cKvp.Key] = cKvp.Value,
							});
						}
					}
				}
			}
		}

		return sets;
	}

	private int HashIds(string a, string b, string c)
	{
		var l = new List<string>() { a, b, c };
		l.Sort();
		return HashCode.Combine(l[0], l[1], l[2]);
	}
}
