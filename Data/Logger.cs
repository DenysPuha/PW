using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class Logger : ILogger
    {
        #region ctor
        private Logger() {
            string mainDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
            string directory = Path.Combine(mainDir, "log");
            Directory.CreateDirectory(directory);
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            filepath = Path.Combine(directory, $"{timeStamp}.json");

            queue = new BlockingCollection<Log>(boundedCapacity: 10000);
            ThreadStart ts = new ThreadStart(LoggerLoop);
            _thread = new System.Threading.Thread(ts);
            _thread.IsBackground = true;
            _thread.Start();
        }
        #endregion ctor

        #region publicAPI
        public static Logger LoggerInstance => _loggerInstance.Value;
        public void AddToQueue(DateTime time, string message, IVector position, IVector velocity)
        {
            if (!isDisposed && !queue.IsAddingCompleted)
            {
                Log log = new Log(time, message, position, velocity);
                if (!queue.TryAdd(log))
                {
                    Interlocked.Increment(ref _skippedLogs);
                }
            }
        }

        #endregion publicAPI

        #region IDisposable
        public void Dispose()
        {
            if (!isDisposed)
            {
                Debug.WriteLine(_skippedLogs);
                queue.CompleteAdding();
                isDisposed = true;
                _thread.Join();
            }
        }
        #endregion IDisposable

        #region private

        private int _skippedLogs;
        private readonly string filepath;
        private Thread _thread;
        private Boolean isDisposed = false;
        private readonly BlockingCollection<Log> queue;

        private void LoggerLoop() {
            using StreamWriter writer = new(filepath, false, Encoding.UTF8);
            foreach(Log log in queue.GetConsumingEnumerable())
            {
                try {
                    string json = JsonSerializer.Serialize(log);
                    writer.WriteLine(json);
                    writer.Flush();
                    if (_skippedLogs > 0)
                    {
                        writer.WriteLine($"Skipped logs: {_skippedLogs}");
                        writer.Flush();
                    }
                }
                catch(Exception e) {
                    Debug.WriteLine($"Logging error: {e.Message}");
                }
            }
        }

        private static readonly Lazy<Logger> _loggerInstance = new Lazy<Logger>(() => new Logger());
        #endregion private

        internal class Log
        {
            public DateTime time { get; set; }
            public string message { get; set; }
            public IVector position { get; set; }
            public IVector velocity { get; set; }

            internal Log(DateTime time, string message, IVector position, IVector velocity)
            {
                this.time = time;
                this.message = message;
                this.position = position;
                this.velocity = velocity;
            }
        }
    }
}
