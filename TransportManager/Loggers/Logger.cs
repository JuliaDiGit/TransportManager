using System;
using System.IO;
using Logger.Abstract;

namespace Logger
{
    public class Logger : ILogger
    {
        private readonly string _pathTrace;
        private const string FileTrace = "Trace.txt";
        
        private readonly string _pathError;
        private const string FileError = "Error.txt";
        
        public Logger(string path)
        {
            string folderDate = DateTime.Now.ToString("d");
            string pathFolderDate = Path.Combine(path, folderDate);
            if (!Directory.Exists(pathFolderDate))
            {
                Directory.CreateDirectory(pathFolderDate);
            }

            _pathTrace = Path.Combine(pathFolderDate, FileTrace);
            if (!File.Exists(_pathTrace))
            {
                TextWriter writer = new StreamWriter(_pathTrace);
                writer.WriteLine();
                writer.Close();
            }
            
            _pathError = Path.Combine(pathFolderDate, FileError);
            if (!File.Exists(_pathError))
            {
                TextWriter writer = new StreamWriter(_pathError);
                writer.WriteLine();
                writer.Close();
            }
        }
        
        public void Trace(string log)
        {
            TextReader reader = new StreamReader(_pathTrace);
            string file = reader.ReadToEnd();
            reader.Close();
            
            TextWriter writer = new StreamWriter(_pathTrace);
            writer.WriteLine(file + log);
            writer.Close();
        }
        
        public void Error(string log)
        {
            TextReader reader = new StreamReader(_pathError);
            string file = reader.ReadToEnd();
            reader.Close();
            
            TextWriter writer = new StreamWriter(_pathError);
            writer.WriteLine(file + log);
            writer.Close();
        }
    }
}