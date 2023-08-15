using System.Xml.Linq;

namespace core;

public class Lib
{
    /// <summary>
    /// This function takes a C# project file path and returns the name of the generated assembly
    /// </summary>
    /// <param name="projectFile">The file path of the project file.</param>
    /// <returns>The assembly name from the project file.</returns>
    private static string? GetAssemblyName(string projectFile)
    {
        return XDocument.Load(projectFile)
            .Descendants("{http://schemas.microsoft.com/developer/msbuild/2003}AssemblyName")
            .FirstOrDefault()?
            .Value;
    }

    private static IEnumerable<string> GetAssemblyFileReferences(string projectFile)
    {
        return XDocument.Load(projectFile)
            .Descendants("{http://schemas.microsoft.com/developer/msbuild/2003}Reference")
            .Attributes("Include").Select(x => x.Value.ToLower());
    }


    public static bool Analyze(string slnFilePath)
    {
        bool ret = true;

        var dir = new FileInfo(slnFilePath).DirectoryName + "\\";

        var lines = File.ReadAllLines(slnFilePath);

        var projectFiles = lines
            .Where(line =>
                line.StartsWith(
                    "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\")")) // Guid for C# projects. Todo, check other project types.
            .Select(line => line.Split(',')[1].Trim().Trim('"'))
            .Select(project => Path.GetFullPath(Path.Join(dir) + project)).ToList();

        var knownAssemblies = projectFiles
            .Select(GetAssemblyName)
            .Where(p => p != null)
            .ToDictionary(p => p!.ToLower());

        foreach (var project in projectFiles)
        {
            var q = GetAssemblyFileReferences(project);

            foreach (var rx in q)
                if (knownAssemblies.ContainsKey(rx))
                {
                    Console.WriteLine($"Project {project} filereferences {rx} which is a project in the solution.");
                    ret = false;
                }
        }

        return ret;
    }
}