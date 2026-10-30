using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

[UseReporter(typeof(QuietReporter))]
public class AnalyzerScannerTest
{
    public AnalyzerScannerTest() => Directory.SetCurrentDirectory(AppContext.BaseDirectory);

    // Only mapped rules, only scoped files, and each warning once (the build prints it twice).
    [Fact]
    public void ReportsMappedWarningsForScopedFiles()
    {
        // The sample log uses /repo; swap in a root that is a full path on this OS.
        var root = Path.GetFullPath("repo");
        var buildOutput = File.ReadAllText("Samples/build-output.txt")
            .Replace("/repo/", root + Path.DirectorySeparatorChar)
            .Replace("/src/Lib/", $"{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}Lib{Path.DirectorySeparatorChar}");
        Approvals.VerifyJson(AnalyzerScanner.FindingsIn(buildOutput, ["src/Lib/Unused.cs"], root));
    }
}
