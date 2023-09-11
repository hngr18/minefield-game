<Query Kind="Program" />

private double targetProbability = 18.5;

void Main()
{
	(long ctrTotal, long ctrTrue) = (0, 0);

	var rand = new Random();
	var sw = Stopwatch.StartNew();
	while (sw.Elapsed.TotalSeconds < 5)
	{

		ctrTotal++;
	}
	
	ctrTotal.Dump("Total");
	ctrTrue.Dump("True");

	(targetProbability).Dump("Percentage True Target");
	((ctrTrue / ctrTotal) * 100).Dump("Percentage True Actual");
}

// You can define other methods, fields, classes and namespaces here