using System.Collections.Concurrent;
using System.Diagnostics;
using System.Management.Automation;

namespace QualityTests.IntegrationSupport
{
    public class FuncProcessHost
    {
        private Process? hostReference;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ConcurrentQueue<string> logQueue;
        private ConcurrentQueue<string> errorQueue;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private bool _watching;

        public void Start(uint port, string workDirectory)
        {
            logQueue = new ConcurrentQueue<string>();
            errorQueue = new ConcurrentQueue<string>();

            var pids = GetProcessesUsingPorts(port);
            if(pids != null)
            {
                foreach(var pid in pids.Select(p=> Convert.ToInt32(p)))
                {
                    Process.GetProcessById(pid).Kill();
                }
            }

            this.hostReference = Process.Start(new ProcessStartInfo
            {
                FileName = "func.exe",
                Arguments = $"start --port {port}",
                WorkingDirectory = workDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            });

            if (hostReference != null)
            {
                hostReference.OutputDataReceived += (sender, args) =>
                {
                    if (args != null && args.Data != null)
                    {
                        if (_watching)
                        {
                            logQueue.Enqueue(args.Data);
                        }
                        Trace.WriteLine(args.Data);
                        Console.WriteLine(args.Data);
                    }
                };
                hostReference.ErrorDataReceived += (sender, args) =>
                {
                    if(args != null && args.Data != null)
                    {
                        if (_watching)
                        {
                            errorQueue.Enqueue(args.Data);
                        }
                        Trace.WriteLine(args.Data);
                        Console.WriteLine(args.Data);
                    }
                };

                hostReference.BeginOutputReadLine();
                hostReference.BeginErrorReadLine();
            }
            Thread.Sleep(5000);
        }

        public void Stop()
        {
            this.hostReference?.CloseMainWindow();
            this.hostReference?.Dispose();
        }

        public void StartWatcher()
        {
            logQueue.Clear();
            errorQueue.Clear();
            this._watching = true;
        }

        public string[] GetLogs()
        {
            return this.logQueue.ToArray();
        }

        public string[] GetErrors()
        {
            return this.errorQueue.ToArray();
        }

        private static IEnumerable<uint> GetProcessesUsingPorts(uint id)
        {
            PowerShell ps = PowerShell.Create();
            ps.AddCommand("Get-NetTCPConnection").AddParameter("LocalPort", id);
            return ps.Invoke().Select(p => (uint)p.Properties["OwningProcess"].Value);
        }
    }
}
