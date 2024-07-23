using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OwlCore.Diagnostics
{
    /// <summary>
    /// Helper for interacting with executable binaries.
    /// </summary>
    public static class ProcessHelpers
    {
        /// <summary>
        /// Adds executable permissions for the given file when running on Linux.
        /// </summary>
        /// <param name="filePath">The path to the file to adjust execute permissions for.</param>
        public static void EnableExecutablePermissions(string filePath)
        {
            // This should only be used on linux.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return;

            RunExecutable("/bin/bash", $"-c \"chmod +x '{filePath}'\"", throwOnError: true);
        }

        /// <summary>
        /// Runs the provided executable with the given arguments.
        /// </summary>
        /// <param name="filePath">The path to the binary file to execute.</param>
        /// <param name="arguments">The execution arguments to provide.</param>
        /// <param name="throwOnError">Whether to throw when stderr is emitted.</param>
        /// <exception cref="System.InvalidOperationException">An error was output during execution.</exception>
        public static (string output, string error) RunExecutable(string filePath, string arguments, bool throwOnError)
        {
            var processStartInfo = new ProcessStartInfo(filePath, arguments)
            {
                CreateNoWindow = true,
            };

            var proc = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();

            var output = string.Empty;
            var error = string.Empty;

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            proc.OutputDataReceived += Process_OutputDataReceived;
            proc.ErrorDataReceived += ProcOnErrorDataReceived;

            proc.WaitForExit();

            proc.OutputDataReceived -= Process_OutputDataReceived;
            proc.ErrorDataReceived -= ProcOnErrorDataReceived;

            void Process_OutputDataReceived(object? sender, DataReceivedEventArgs e)
            {
                if (e.Data is not null)
                    Logger.LogInformation(e.Data);

                output += e.Data;
            }

            void ProcOnErrorDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (!string.IsNullOrWhiteSpace(e.Data) && throwOnError)
                {
                    throw new System.InvalidOperationException($"Error received while running {filePath} {arguments}: {e.Data}");
                }
                
                if (e.Data is not null)
                    Logger.LogError(e.Data);

                error += e.Data;
            }

            return (output, error);
        }
    }
}
