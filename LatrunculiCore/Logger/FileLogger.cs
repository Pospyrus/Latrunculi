using System;
using System.IO;

namespace LatrunculiCore.Logger
{

    class FileLogger
    {
        private static FileLogger instance;
        private static string logFolder;

        private StreamWriter stream;

        public static void init(string appDataFolder) {
            logFolder = $@"{appDataFolder}\Logs\Latrunculi {DateTime.Now.ToString("dd. MM. yyyy HH.mm.ss")}";
            instance = new FileLogger("main");
        }

        public FileLogger(string name)
        {
            initFileStream(name);
        }

        private void initFileStream(string logFileName)
        {
            if (string.IsNullOrEmpty(logFolder))
            {
                throw new InvalidOperationException($"{nameof(FileLogger)} is not initialized.");
            }
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            stream = new StreamWriter($@"{logFolder}\{logFileName}.log");
        }

        public static void WriteGlobal(string text, int indentation = 0) =>
            instance.Write(text, indentation);

        public static void WriteLineGlobal(string text, int indentation = 0) =>
            instance.WriteLine(text, indentation);

        public void Write(string text, int indentation = 0)
        {
            stream.Write(new String(' ', indentation * 4) + text);
        }

        public void WriteLine(string text, int indentation = 0)
        {
            foreach (var line in text.Split('\n'))
            {
                stream.WriteLine(new String(' ', indentation * 4) + line);
            }
        }

        public void Dispose()
        {
            stream?.Close();
            stream?.Dispose();
            stream = null;
        }

        ~FileLogger()
        {
            Dispose();
        }
    }

}