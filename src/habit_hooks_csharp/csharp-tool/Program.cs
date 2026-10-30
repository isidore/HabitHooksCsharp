// Usage: csharp-tool comments [--include-file-header] [--include-doc-comments] -- file...
//        csharp-tool smells -- file...
var command = args.FirstOrDefault();
var rest = args.Skip(1).ToArray();
var separator = Array.IndexOf(rest, "--");
var flags = separator < 0 ? [] : rest[..separator];
var files = separator < 0 ? rest : rest[(separator + 1)..];

switch (command)
{
    case "comments":
        var options = new CommentOptions(
            IncludeFileHeader: flags.Contains("--include-file-header"),
            IncludeDocComments: flags.Contains("--include-doc-comments"));
        Console.Write(CommentScanner.Scan(files, options));
        return 0;
    case "smells":
        Console.Write(SmellScanner.Scan(files));
        return 0;
    default:
        Console.Error.WriteLine($"csharp-tool: unknown command '{command}'; expected: comments, smells");
        return 2;
}
