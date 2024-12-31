namespace ReadAll
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length is 0) args = [Environment.CurrentDirectory];
            foreach (var arg in args)
            {
                try
                {
                    var attributes = File.GetAttributes(arg);
                    if (attributes.HasFlag(FileAttributes.Directory)) await ReadDirectoryAsync(new DirectoryInfo(arg), CancellationToken.None);
                    else await ReadFileAsync(new FileInfo(arg), CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read: {arg}...{ex.Message}");
                }
            }
        }

        private static async Task ReadDirectoryAsync(DirectoryInfo dirInfo, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var file in dirInfo.EnumerateFiles())
                {
                    await ReadFileAsync(file, cancellationToken);
                }
                foreach (var directory in dirInfo.EnumerateDirectories().Where(dirInfo => dirInfo.Name is not "." and not ".."))
                {
                    await ReadDirectoryAsync(directory, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read directory: {dirInfo.FullName}...{ex.Message}");
            }
        }

        private static async Task ReadFileAsync(FileInfo fileInfo, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Reading {fileInfo.FullName}");
            try
            {
                var buffer = new byte[65536];
                await using var fileStream = File.OpenRead(fileInfo.FullName);
                while (await fileStream.ReadAsync(buffer, cancellationToken) > 0) { /* Nothing to do */ }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read file: {fileInfo.FullName}...{ex.Message}");
            }
        }
    }
}
