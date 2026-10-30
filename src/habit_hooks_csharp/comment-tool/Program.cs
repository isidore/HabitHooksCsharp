// Usage: comment-tool [--include-file-header] [--include-doc-comments] -- file...
// Without `--`, every argument is a file.
var separator = Array.IndexOf(args, "--");
var flags = separator < 0 ? [] : args[..separator];
var files = separator < 0 ? args : args[(separator + 1)..];

var options = new CommentOptions(
    IncludeFileHeader: flags.Contains("--include-file-header"),
    IncludeDocComments: flags.Contains("--include-doc-comments"));

Console.Write(CommentScanner.Scan(files, options));
return 0;
