

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Start(int port, string workDirectory)
        {
            logQueue = new ConcurrentQueue<string>();
            errorQueue = new ConcurrentQueue<string>();

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
    }
}
