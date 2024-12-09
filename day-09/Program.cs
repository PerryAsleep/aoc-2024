var sr = new StreamReader("input-09.txt");
var line = sr.ReadLine();

var input = new int[line.Length];
var buffer = new List<int?>();
var inputIndex = 0;
var fileId = 0;
var occupied = true;
var freeChunks = new List<Chunk>();
var occupiedChunks = new List<Chunk>();

foreach (var c in line)
{
	var numBlocks = c - '0';
	input[inputIndex] = numBlocks;
	int? block = occupied ? fileId : null;
	var chunkIndex = buffer.Count;
	for (var blockIndex = 0; blockIndex < numBlocks; blockIndex++)
			buffer.Add(block);
	inputIndex++;
	if (occupied)
	{
		fileId++;
		occupiedChunks.Add(new Chunk { Length = numBlocks, Index = chunkIndex, FileId = block });
	}
	else if (numBlocks > 0)
	{
		freeChunks.Add(new Chunk { Length = numBlocks, Index = chunkIndex, FileId = block });
	}

	occupied = !occupied;
}

Console.WriteLine(CompactBufferPartOne(new List<int?>(buffer)));
Console.WriteLine(CompactBufferPartTwo(freeChunks, occupiedChunks));

long CompactBufferPartTwo(List<Chunk> freeChunks, List<Chunk> occupiedChunks)
{
	for (var ocIndex = occupiedChunks.Count - 1; ocIndex >= 0; ocIndex--)
	{
		var oc = occupiedChunks[ocIndex];
		for (var fcIndex = 0; fcIndex < freeChunks.Count; fcIndex++)
		{
			var fc = freeChunks[fcIndex];

			if (oc.Index < fc.Index)
				break;

			if (fc.Length >= oc.Length)
			{
				var originalStartIndex = oc.Index;
				var originalEndIndex = originalStartIndex + oc.Length;

				// Move the occupied chunk to the free chunk.
				oc.Index = fc.Index;
				
				// Update the free chunk that was copied over.
				if (fc.Length == oc.Length)
				{
					freeChunks.RemoveAt(fcIndex);
				}
				else
				{
					fc.Index += oc.Length;
					fc.Length -= oc.Length;
				}

				// Update the free chunk resulting from moving the occupied chunk.
				int? precedingFreeChunkIndex = null;
				int? followingFreeChunkIndex = null;
				var adjacentFcIndex = 0;
				while (adjacentFcIndex < freeChunks.Count)
				{
					if (freeChunks[adjacentFcIndex].Index + freeChunks[adjacentFcIndex].Length == originalStartIndex)
						precedingFreeChunkIndex = adjacentFcIndex;
					if (freeChunks[adjacentFcIndex].Index == originalEndIndex)
						followingFreeChunkIndex = adjacentFcIndex;
					if (precedingFreeChunkIndex != null && followingFreeChunkIndex != null)
						break;
					adjacentFcIndex++;
				}
				if (precedingFreeChunkIndex != null && followingFreeChunkIndex != null)
				{
					freeChunks[(int)precedingFreeChunkIndex].Length =
						freeChunks[(int)followingFreeChunkIndex].Index + freeChunks[(int)followingFreeChunkIndex].Length - freeChunks[(int)precedingFreeChunkIndex].Index;
					freeChunks.RemoveAt((int)followingFreeChunkIndex);
				}
				else if (precedingFreeChunkIndex != null)
				{
					freeChunks[(int)precedingFreeChunkIndex].Length += oc.Length;
				}
				else if (followingFreeChunkIndex != null)
				{
					freeChunks[(int)followingFreeChunkIndex].Index -= oc.Length;
					freeChunks[(int)followingFreeChunkIndex].Length += oc.Length;
				}

				break;
			}
		}
	}

	var result = 0L;
	foreach (var oc in occupiedChunks)
	{
		for (var i = oc.Index; i < oc.Index + oc.Length; i++)
		{
			result += (long)oc.FileId! * i;
		}
	}

	return result;
}

long CompactBufferPartOne(List<int?> buffer)
{
	var s = 0;
	var e = buffer.Count - 1;
	while (true)
	{
		while (buffer[s] != null && s < buffer.Count)
			s++;
		while (buffer[e] == null && e >= 0)
			e--;
		if (e <= s || e == 0 || s == buffer.Count)
			break;
		buffer[s] = buffer[e];
		buffer[e] = null;
		s++;
		e--;
	}

	var result = 0L;
	var i = 0;
	while (true)
	{
		if (buffer[i] == null || i == buffer.Count)
			break;
		result += (long)(i * buffer[i]);
		i++;
	}

	return result;
}

class Chunk
{
	public int Length;
	public int Index;
	public int? FileId;
}