using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTarkov.Launcher.Controllers
{
    /// <summary>
    /// LogManager
    /// </summary>
    public class LogManager
    {
        private static LogManager _instance;
        public static LogManager Instance => _instance ?? (_instance = new LogManager());

        /// <summary>
        /// directory of log
        /// </summary>
        private string _logDirectory;

        public LogManager()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Logs");
        }

        public void Write(string text)
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
            var fileName = Path.Combine(_logDirectory,$"{DateTime.Now:yyyyMMdd}.log");
            File.AppendAllLines(fileName,new []{ $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{text}" });
        }

        public void Debug(string text) => Write($"[Debug]{text}");

        public void Info(string text)=> Write($"[Info]{text}");

        public void Warning(string text) => Write($"[Warning]{text}");

        public void Error(string text) => Write($"[Error]{text}");
    }
}
