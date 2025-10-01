using Common;
using System;
using System.IO;

namespace Service
{
    public class FileWriter : IDisposable
    {
        private StreamWriter _writer;
        private bool _disposed = false;

        public FileWriter(string filePath)
        {
            _writer = new StreamWriter(filePath);
        }

        ~FileWriter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _writer != null)
                {
                    _writer.Flush();
                    _writer.Dispose();
                    _writer = null;
                }
                _disposed = true;
            }
        }

        public void WriteSensorSample(SensorSample sensorSample)
        {
            if (!_disposed && _writer != null)
                _writer.WriteLine(sensorSample.ToString());
        }

        public void WriteText(string text)
        {
            if (!_disposed && _writer != null)
                _writer.WriteLine(text);
        }
    }
}
