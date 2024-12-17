internal class Part2
{
	private int[] target = new int[] { 2,4,1,2,7,5,4,5,1,3,5,5,0,3,3,0 };

	// Pretty terrible solution.
	public async Task Run()
	{
		// need to output 16 digits
		// need to loop exactly 16 times.
		// min value of A is 7 * 8^(15)					= 35184372088832
		// max value of A is 8^16						= 281474976710656
		// range is 281474976710656 - 35184372088832	= 246290604621824

		var start = 35184372088832;
		var range = 246290604621824;
		var numTasks = 16;
		var rangePerTask = range / numTasks;
		var tasks = new Task[numTasks];
		for (var i = 0; i < numTasks; i++)
		{
			var startVal = start + i * rangePerTask - numTasks;
			var endVal = start + rangePerTask + numTasks * 2;
			tasks[i] = Task.Run(() => { ExecuteInstructions(startVal, endVal); });
		}
		await Task.WhenAll(tasks);
	}

	public long ExecuteInstructions(long start, long end)
	{
		long b;
		long a;
		int targetIndex;
		for (var i = start; i <= end; i++)
		{
			a = i;
			targetIndex = 0;
			do
			{
				b = ((a % 8) ^ 2);
				b = b ^ (a >> (int)b);
				b = b ^ 3;
				if (b % 8 != target[targetIndex])
					break;
				targetIndex++;
				a >>= 3;
			} while (a != 0);

			if (a == 0)
			{
				Console.WriteLine(i);
				return i;
			}
		}
		return 0;
	}
}
