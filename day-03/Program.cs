using System.Text;
using System.Text.RegularExpressions;

var regexPattern = @"mul\([0-9]+\,[0-9]+\)";
var regexSubPattern = @"mul\(([0-9]+)\,([0-9]+)\)";
var text = File.ReadAllText("input-03.txt");

// Part 1
int CountMultiplies(string text)
{
	var result = 0;
	var matchCollection = Regex.Matches(text, regexPattern, RegexOptions.None);
	foreach (Match match in matchCollection)
	{
		var mulString = match.Groups[0].Captures[0].Value;
		var subResult = Regex.Match(mulString, regexSubPattern);
		if (subResult.Success && subResult.Groups.Count == 3)
		{
			if (!int.TryParse(subResult.Groups[1].Value, out var value01))
			{
				continue;
			}
			if (!int.TryParse(subResult.Groups[2].Value, out var value02))
			{
				continue;
			}
			result += value01 * value02;
		}
	}

	return result;
}
Console.WriteLine(CountMultiplies(text));

// Part 2
// Remove everything between "don't" and "do"
var sb = new StringBuilder();
var i = 0;
var inDont = false;
while (i < text.Length)
{
	if (i + 6 < text.Length
	    && text[i] == 'd'
	    && text[i + 1] == 'o'
	    && text[i + 2] == 'n'
	    && text[i + 3] == '\''
	    && text[i + 4] == 't'
	    && text[i + 5] == '('
	    && text[i + 6] == ')')
	{
		i += 5;
		inDont = true;
		continue;
	}
	if (i + 3 < text.Length
	    && text[i] == 'd'
	    && text[i + 1] == 'o'
	    && text[i + 2] == '('
	    && text[i + 3] == ')')
	{
		i += 2;
		inDont = false;
		continue;
	}

	if (!inDont)
	{
		sb.Append(text[i]);
	}
	i++;
}
text = sb.ToString();
Console.WriteLine(text);
Console.WriteLine(CountMultiplies(text));