using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

// Builds the project with the smell analyzers turned on (see analyzers/habit-hooks.targets),
// and reports their warnings for the files in scope.
public static partial class AnalyzerScanner
{
    static readonly Dictionary<string, string> Smells = new()
    {
        ["CA1506"] = "high-coupling",
        ["CA1822"] = "unused-member",
        ["CA1823"] = "unused-field",
        ["CS0168"] = "unused-variable",
        ["CS0219"] = "unused-variable",
        ["IDE0051"] = "unused-private-member",
        ["IDE0052"] = "unused-private-member",
        ["IDE0060"] = "unused-parameter",
        ["IDE0005"] = "unused-import",
    };

    [GeneratedRegex(@"^(?<file>[^(]+)\((?<line>\d+),(?<column>\d+)\): warning (?<rule>[A-Za-z]+\d+): (?<message>.*?)(?: \[[^\]]*\])?$")]
    private static partial Regex Warning();

    public static string Scan(IEnumerable<string> paths, string? project)
    {
        var files = paths.Where(File.Exists).ToArray();
        return files.Length == 0 ? "[]" : FindingsIn(Build(project), files, Directory.GetCurrentDirectory());
    }

    public static string FindingsIn(string buildOutput, IEnumerable<string> files, string root)
    {
        var scoped = files.Select(file => Path.GetFullPath(file, root)).ToHashSet();
        var findings = buildOutput.Split('\n')
            .Select(line => Warning().Match(line.Trim()))
            .Where(match => match.Success && Smells.ContainsKey(match.Groups["rule"].Value))
            .Where(match => scoped.Contains(Path.GetFullPath(match.Groups["file"].Value, root)))
            .Select(match => ToIssue(match, root))
            .Distinct()
            .GroupBy(issue => issue.Smell)
            .Select(group => new Finding(group.Key, new object(), group.Select(issue => issue.Issue).ToArray()))
            .ToArray();
        return findings.Length == 0
            ? "[]"
            : JsonSerializer.Serialize(findings, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    static AnalyzerIssue ToIssue(Match match, string root)
    {
        // Forward slashes on every OS, like the paths habit-hooks passes in.
        var file = Path.GetRelativePath(root, Path.GetFullPath(match.Groups["file"].Value, root)).Replace('\\', '/');
        var rule = match.Groups["rule"].Value;
        var details = new IssueDetails(file, int.Parse(match.Groups["line"].Value),
            $"{rule}: {match.Groups["message"].Value}", "roslyn:" + rule);
        return new AnalyzerIssue(Smells[rule], new Issue(file, details));
    }

    static string Build(string? project)
    {
        var targets = Path.Combine(AppContext.BaseDirectory, "analyzers", "habit-hooks.targets");
        var arguments = new List<string> { "build" };
        if (project != null) arguments.Add(project);
        arguments.AddRange(["-c", "Debug", "--no-incremental", "-v", "quiet", "-nologo",
            $"-p:CustomAfterMicrosoftCommonTargets={targets}"]);

        var start = new ProcessStartInfo("dotnet", arguments) { RedirectStandardOutput = true, RedirectStandardError = true };
        using var process = Process.Start(start)!;
        var error = process.StandardError.ReadToEndAsync();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output + error.Result;
    }

    record AnalyzerIssue(string Smell, Issue Issue);
}
