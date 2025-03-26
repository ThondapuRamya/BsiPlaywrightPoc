using System.Diagnostics;
using System.IO;
using System.Text;
using TechTalk.SpecFlow;

namespace BsiPlaywrightPoc.Hooks
{
    [Binding]
    public sealed class LivingDocHooks
    {
        //[AfterTestRun(Order = HookOrdering.Maximum)]
        public static void GenerateLivingDocReport()
        {
            // Check if LivingDoc is installed, and if not, install it
            if (!IsLivingDocInstalled())
            {
                InstallLivingDoc();
            }

            // Generate the report and copy to the root
            GenerateLivingDoc();
        }

        private static bool IsLivingDocInstalled()
        {
            try
            {
                // Check if livingdoc is available globally
                var processInfo = new ProcessStartInfo("livingdoc", "--version")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private static void InstallLivingDoc()
        {
            try
            {
                // Install the SpecFlow.Plus.LivingDoc.CLI globally if not installed
                var processInfo = new ProcessStartInfo("dotnet", "tool install --global SpecFlow.Plus.LivingDoc.CLI")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    throw new Exception($"Failed to install LivingDoc CLI: {error}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error installing LivingDoc CLI: {ex.Message}");
            }
        }

        private static async Task GenerateLivingDoc()
        {
            try
            {
                // Get the base directory of the currently executing assembly
                var binDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Specify paths to your test assembly and TestExecution.json
                var assemblyPath = Path.Combine(binDirectory, "BsiPlaywrightPoc.dll");
                var testExecutionJsonPath = Path.Combine(binDirectory, "TestExecution.json");
                var livingDocPath = Path.Combine(binDirectory, "LivingDoc.html");

                // Check if both the required files exist before running the command
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"Test assembly not found at: {assemblyPath}");
                }

                if (!File.Exists(testExecutionJsonPath))
                {
                    throw new FileNotFoundException($"Test execution file not found at: {testExecutionJsonPath}");
                }

                // Escape the file paths by wrapping them in double quotes
                var assemblyPathEscaped = $"\"{assemblyPath}\"";
                var testExecutionJsonPathEscaped = $"\"{testExecutionJsonPath}\"";
                var livingDocPathEscaped = $"\"{livingDocPath}\"";

                // Run the command to generate the LivingDoc report
                var processInfo = new ProcessStartInfo("livingdoc", $"test-assembly {assemblyPathEscaped} -t {testExecutionJsonPathEscaped} -o {livingDocPathEscaped}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process
                {
                    StartInfo = processInfo,
                    EnableRaisingEvents = true
                };

                var output = new StringBuilder();
                var error = new StringBuilder();

                process.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
                process.ErrorDataReceived += (sender, args) => error.AppendLine(args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for the process to exit asynchronously
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Failed to generate LivingDoc report: {error}");
                }

                // After successful generation, copy the report to the root directory
                var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "LivingDoc.html");
                File.Copy(livingDocPath, destinationPath, true); // Copy and overwrite if the file exists
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating LivingDoc report: {ex.Message}");
            }
        }
    }
}
