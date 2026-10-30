using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

[UseReporter(typeof(QuietReporter))]
public class CommentScannerTest
{
    public CommentScannerTest() => Directory.SetCurrentDirectory(AppContext.BaseDirectory);

    [Fact]
    public void ReportsComments() =>
        Approvals.VerifyJson(CommentScanner.Scan(["Samples/Comments.cs"]));

    [Fact]
    public void ReportsNothingForMissingFiles() =>
        Assert.Equal("[]", CommentScanner.Scan(["Samples/Missing.cs"]));
}
