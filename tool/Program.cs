var message =
    @"Visual Studio Solution reference checker

Checks that .Net Framework C# projects in a *.SLN file does not
use file references to projects in the same solution. The right thing
is to use project references, to get the dependencies right.

";

Console.WriteLine(message);

core.Lib.Analyze(args[0]);