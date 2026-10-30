using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

[UseReporter(typeof(QuietReporter))]
public class SmellScannerTest
{
    public SmellScannerTest() => Directory.SetCurrentDirectory(AppContext.BaseDirectory);

    [Fact]
    public void ReportsSmells() =>
        Approvals.VerifyJson(SmellScanner.Scan(["Samples/Smells.cs"]));
}
