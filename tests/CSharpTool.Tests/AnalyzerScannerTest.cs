using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

[UseReporter(typeof(QuietReporter))]
public class AnalyzerScannerTest
{
    public AnalyzerScannerTest() => Directory.SetCurrentDirectory(AppContext.BaseDirectory);

    // Only mapped rules, only scoped files, and each warning once (the build prints it twice).
    [Fact]
    public void ReportsMappedWarningsForScopedFiles() =>
        Approvals.VerifyJson(AnalyzerScanner.FindingsIn(
            File.ReadAllText("Samples/build-output.txt"), ["src/Lib/Unused.cs"], "/repo"));
}
