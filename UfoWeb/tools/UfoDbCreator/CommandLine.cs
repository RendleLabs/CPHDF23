namespace UfoDbCreator;

public static class CommandLine
{
    public static bool TryParse(string[] args, out string? dbPath, out bool migrate)
    {
        dbPath = null;
        migrate = false;
        if (args.Length == 0) return false;

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "-m":
                case "--migrate":
                    migrate = true;
                    break;
                default:
                    var fullPath = Path.GetFullPath(arg);
                    var directory = Path.GetDirectoryName(fullPath);
                    if (directory is { Length: > 0 })
                    {
                        Directory.CreateDirectory(directory);
                        dbPath = fullPath;
                    }
                    break;
            }
        }

        return dbPath is not null;
    }
}