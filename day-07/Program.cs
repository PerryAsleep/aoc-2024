var sr = new StreamReader("input-07.txt");

var results =  new List<long>();
var valueLists = new List<List<int>>();

var line = sr.ReadLine();
while (line != null)
{
	var parts = line.Split(':');
	results.Add(long.Parse(parts[0]));

	var values = new List<int>();
	var valuesStr = parts[1].Trim().Split(' ');
	foreach (var v in valuesStr)
		values.Add(int.Parse(v));
	valueLists.Add(values);

	line = sr.ReadLine();
}

// Part 1 / 2.
var result = 0L;
for (var i = 0; i < valueLists.Count; i++)
{
	if (ParseEquation(results[i], valueLists[i]))
		result += results[i];
}
Console.WriteLine(result);

bool ParseEquation(long result, List<int> values)
{
	var operatorCombinations = Helpers.Combinations.CreateCombinations<Operator>(values.Count - 1);
	foreach (var operatorSequence in operatorCombinations)
	{
		if (ParseEquationWithOperators(result, values, operatorSequence))
		{
			return true;
		}
	}
	return false;
}

bool ParseEquationWithOperators(long result, List<int> values, Operator[] operators)
{
	var operatorIndex = 0;
	var r = (long)values[0];
	while (operatorIndex < operators.Length)
	{
		switch (operators[operatorIndex])
		{
			case Operator.Add:
				r += values[operatorIndex + 1];
				break;
			case Operator.Multiply:
				r *= values[operatorIndex + 1];
				break;
			case Operator.Concat:
				var digits = Math.Floor(Math.Log10(values[operatorIndex + 1]) + 1);
				r = values[operatorIndex + 1] + r * (long)Math.Pow(10, digits);
				break;
		}
		operatorIndex++;
	}
	return r == result;
}

enum Operator
{
	Multiply,
	Add,
	Concat
}