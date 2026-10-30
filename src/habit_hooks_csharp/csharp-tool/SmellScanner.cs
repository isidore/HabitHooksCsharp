using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// Structural smells read from the syntax tree, without building the project.
// Thresholds match the Java plugin's PMD ruleset.
public static class SmellScanner
{
    const int MaxParameters = 4;
    const int MaxStatements = 12;
    const int MaxNesting = 3;
    const int MaxComplexity = 10;

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
        return TooManyParameters(root)
            .Concat(OversizedMethods(root))
            .Concat(DeepNesting(root))
            .Concat(HighComplexity(root))
            .Select(smell => smell.ToIssue(path, tree));
    }

    static IEnumerable<Smell> TooManyParameters(SyntaxNode root) =>
        Methods(root)
            .Where(method => method.ParameterList.Parameters.Count >= MaxParameters)
            .Select(method => new Smell("too-many-parameters", method,
                $"{NameOf(method)} has {method.ParameterList.Parameters.Count} parameters (reported at {MaxParameters} or more)"));

    static IEnumerable<Smell> OversizedMethods(SyntaxNode root) =>
        Methods(root)
            .Select(method => (method, statements: StatementCount(method)))
            .Where(m => m.statements >= MaxStatements)
            .Select(m => new Smell("oversized-function", m.method,
                $"{NameOf(m.method)} has {m.statements} statements (reported at {MaxStatements} or more)"));

    static IEnumerable<Smell> HighComplexity(SyntaxNode root) =>
        Methods(root)
            .Select(method => (method, complexity: CyclomaticComplexity(method)))
            .Where(m => m.complexity >= MaxComplexity)
            .Select(m => new Smell("high-complexity", m.method,
                $"{NameOf(m.method)} has cyclomatic complexity {m.complexity} (reported at {MaxComplexity} or more)"));

    // One path through the method, plus one for each decision point.
    static int CyclomaticComplexity(BaseMethodDeclarationSyntax method) =>
        1 + method.DescendantNodesAndTokens().Count(IsDecision);

    static bool IsDecision(SyntaxNodeOrToken item) => item.Kind() is
        SyntaxKind.IfStatement or SyntaxKind.WhileStatement or SyntaxKind.ForStatement
        or SyntaxKind.ForEachStatement or SyntaxKind.DoStatement or SyntaxKind.CatchClause
        or SyntaxKind.CaseSwitchLabel or SyntaxKind.CasePatternSwitchLabel or SyntaxKind.SwitchExpressionArm
        or SyntaxKind.ConditionalExpression
        or SyntaxKind.AmpersandAmpersandToken or SyntaxKind.BarBarToken or SyntaxKind.QuestionQuestionToken;

    // Reports the control statement that first reaches the limit, not every one inside it.
    static IEnumerable<Smell> DeepNesting(SyntaxNode root) =>
        root.DescendantNodes().OfType<StatementSyntax>()
            .Where(IsNesting)
            .Where(statement => NestingDepth(statement) == MaxNesting)
            .Select(statement => new Smell("deep-nesting", statement,
                $"`{statement.GetFirstToken().Text}` nested {MaxNesting} deep (reported at {MaxNesting} or more)"));

    static bool IsNesting(SyntaxNode node) => node is IfStatementSyntax
        or ForStatementSyntax or ForEachStatementSyntax or WhileStatementSyntax or DoStatementSyntax
        or SwitchStatementSyntax or TryStatementSyntax;

    // `else if` continues its chain rather than nesting inside it.
    static bool IsElseIf(SyntaxNode node) => node is IfStatementSyntax && node.Parent is ElseClauseSyntax;

    static int NestingDepth(SyntaxNode statement) =>
        statement.AncestorsAndSelf()
            .TakeWhile(node => node is not BaseMethodDeclarationSyntax and not LocalFunctionStatementSyntax
                and not AnonymousFunctionExpressionSyntax)
            .Count(node => IsNesting(node) && !IsElseIf(node));

    // Statements in the body, not counting the braces of blocks themselves.
    static int StatementCount(BaseMethodDeclarationSyntax method) =>
        method.DescendantNodes().OfType<StatementSyntax>().Count(statement => statement is not BlockSyntax);

    static IEnumerable<BaseMethodDeclarationSyntax> Methods(SyntaxNode root) =>
        root.DescendantNodes().OfType<BaseMethodDeclarationSyntax>();

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
