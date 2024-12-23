using System.Text;

internal class Part2
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
		WriteClique(GetMaximumClique());
	}

	private void WriteClique(Dictionary<string, Node> clique)
	{
		// Make list of sorted ids.
		var idList = new List<string>();
		foreach (var kvp in clique)
			idList.Add(kvp.Key);
		idList.Sort();

		// Write formatted output.
		var sb = new StringBuilder();
		var first = true;
		foreach (var id in idList)
		{
			if (!first)
				sb.Append(',');
			sb.Append(id);
			first = false;
		}

		Console.WriteLine(sb);
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

	private Dictionary<string, Node> GetMaximumClique()
	{
		var largestClique = new Dictionary<string, Node>();
		foreach (var n in AllNodes)
		{
			var clique = GetMaximumClique(n.Value);
			if (clique.Count > largestClique.Count)
				largestClique = clique;
		}

		return largestClique;
	}

	private Dictionary<string, Node> GetMaximumClique(Node r)
	{
		var largestClique = new Dictionary<string, Node>
		{
			{ r.Id, r },
		};
		foreach (var n in AllNodes)
			if (IsAdjacentToAllNodes(n.Value, largestClique))
				largestClique.Add(n.Key, n.Value);
		return largestClique;
	}

	private bool IsAdjacentToAllNodes(Node n, Dictionary<string, Node> otherNodes)
	{
		foreach (var otherNode in otherNodes)
			if (!n.Neighbors.Contains(otherNode))
				return false;
		return true;
	}
}
