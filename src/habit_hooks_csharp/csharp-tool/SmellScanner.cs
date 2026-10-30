using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// Structural smells read from the syntax tree, without building the project.
// Thresholds match the Java plugin's PMD ruleset.
public static class SmellScanner
{
    const int MaxParameters = 4;

    public static string Scan(IEnumerable<string> paths)
    {
        var issues = paths.Where(File.Exists).SelectMany(SmellsInFile).ToArray();
        if (issues.Length == 0)
        {
            return "[]";
        }

        var findings = issues
            .GroupBy(issue => issue.Smell)
            .Select(group => new Finding(group.Key, new object(), group.Select(issue => issue.Issue).ToArray()));
        return JsonSerializer.Serialize(findings, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    static IEnumerable<SmellIssue> SmellsInFile(string path)
    {
        var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path);
        var root = tree.GetRoot();
        return TooManyParameters(root).Select(smell => smell.ToIssue(path, tree));
    }

    static IEnumerable<Smell> TooManyParameters(SyntaxNode root) =>
        root.DescendantNodes().OfType<BaseMethodDeclarationSyntax>()
            .Where(method => method.ParameterList.Parameters.Count >= MaxParameters)
            .Select(method => new Smell("too-many-parameters", method,
                $"{NameOf(method)} has {method.ParameterList.Parameters.Count} parameters (reported at {MaxParameters} or more)"));

    static string NameOf(BaseMethodDeclarationSyntax method) => method switch
    {
        MethodDeclarationSyntax m => m.Identifier.Text,
        ConstructorDeclarationSyntax c => c.Identifier.Text,
        _ => method.Kind().ToString()
    };

    record Smell(string Name, SyntaxNode Node, string Message)
    {
        public SmellIssue ToIssue(string path, SyntaxTree tree)
        {
            var line = tree.GetLineSpan(Node.Span).StartLinePosition.Line + 1;
            return new SmellIssue(Name, new Issue(path, new IssueDetails(path, line, Message, "csharp:" + Name)));
        }
    }

    record SmellIssue(string Smell, Issue Issue);
}
