using System;
using System.IO;
using System.IO.Compression;

namespace Horse.Engine.Utils
{
    public static class LogManager
    {
        private static readonly string BaseFileLocation = Environment.CurrentDirectory + @"\Logs";
        private static readonly string LogFile = BaseFileLocation + @".\log-"+DateTime.Today.ToString("MM-dd-yyyy")+".htm";
        private static StreamWriter _log;
        private static readonly string _head = "<html>\r\n<head>\r\n<link rel=\'stylesheet\'" +
                                      " href=\'https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css\' " +
                                      "integrity=\'sha384-rwoIResjU2yc3z8GV/NPeZWAv56rSmLldC3R/AZzGRnGxQQKnKkoFVhFQhNUwEyJ\' crossorigin=" +
                                      "\'anonymous\'>\r\n<script src=\'https://code.jquery.com/jquery-3.1.1.slim.min.js\' " +
                                      "integrity=\'sha384-A7FZj7v+d/sdmMqp/nOQwliLvUsJfDHW+k9Omg/a/EheAdgtzNs3hpfag6Ed950n\' " +
                                      "crossorigin=\'anonymous\'></script>\r\n<script " +
                                      "src=\'https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js\' " +
                                      "integrity=\'sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb\' " +
                                      "crossorigin=\'anonymous\'></script>\r\n" +
                                      "<script src=\'https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/js/bootstrap.min.js\' " +
                                      "integrity=\'sha384-vBWWzlZJ8ea9aCX4pEW3rVHjgjt7zpkNpZk+02D9phzyeVkE+jo0ieGizqPLForn\' " +
                                      "crossorigin=\'anonymous\'></script>\r\n<style>\r\n</style>\r\n</head>\r\n\r\n" +
                                      "<link rel='stylesheet' href='.\\log.css'><body>";

        private static readonly string _close = "\r\n<script src='.\\log.js'></script>\r\n</body>\r\n</html>";
        private static bool _closed;
        private static bool _opened;

        /// <summary>
        /// Create a log file for latest run
        /// </summary>
        public static void InitLogFile()
        {
            if (_opened)
                return;
            if(!Directory.Exists(BaseFileLocation))
                Directory.CreateDirectory(BaseFileLocation);
            if (File.Exists(LogFile))
                File.Delete(LogFile);
            var logFs = new FileStream(LogFile,FileMode.OpenOrCreate,FileAccess.Write);
            File.SetAttributes(LogFile, FileAttributes.Normal);
            _log = new StreamWriter(logFs);
            _log.WriteLine(_head);
            _opened = true;
        }

        /// <summary>
        /// Log an error to the log file
        /// </summary>
        /// <param name="error">The error message</param>
        public static void LogError(string error)
        {
            lock (_log)
            {
                if (_closed)
                    return;
                const string eleO = "<error class=\'text-danger\'>";
                var eleC = " <br/>"+DateTime.Now+"</error>";
                _log.WriteLine(eleO + error + eleC);
            }
        }

        /// <summary>
        /// Log a warning to the log file
        /// </summary>
        /// <param name="warning">The warning message</param>
        public static void LogWarning(string warning)
        {
            lock (_log)
            {
                if (_closed)
                    return;
                const string eleO = "<warning class=\'text-warning\'>";
                var eleC = " <br/>" + DateTime.Now + "</warning>";
                _log.WriteLine(eleO + warning + eleC);
            }
        }

        /// <summary>
        /// Log a general message
        /// </summary>
        /// <param name="log">The message to log</param>
        public static void Log(string log)
        {
            lock (_log)
            {
                if (_closed)
                    return;
                const string eleO = "<info class=\'text-info\'>";
                var eleC = " <br/>" + DateTime.Now + "</info>";
                _log.WriteLine(eleO + log + eleC);
            }
        }

        /// <summary>
        /// Close the log file
        /// </summary>
        public static void CloseLog()
        {
            lock (_log)
            {
                if (_closed)
                    return;
                _log.Write(_close);
                _log.Close();
                _closed = true;
            }
        }

        public static void EmailLogs()
        {
            CloseLog();
            var zipFile = Environment.CurrentDirectory + @".\logs.zip";
            while (File.Exists(zipFile))
                File.Delete(zipFile);
            try
            {
                ZipFile.CreateFromDirectory(BaseFileLocation, zipFile, CompressionLevel.Optimal, false);
                EmailForm.SendEmail(zipFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"connected");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
