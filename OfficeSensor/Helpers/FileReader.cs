using Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace OfficeSensor
{
    public class FileReader : IDisposable
    {
        private StreamReader _reader;
        private List<string> _invalidLines;
        private bool _disposed = false;

        public FileReader(string filePath)
        {
            if (File.Exists(filePath))
            {
                _reader = new StreamReader(filePath);
                _reader.ReadLine();     // Skipping header of CSV

                _invalidLines = new List<string>();
            }
            else
                _reader = null;
        }

        ~FileReader()
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
                if (disposing && _reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }
                _disposed = true;
            }
        }

        public (bool, SensorSample) GetSensorSample()
        {
            if (_disposed || _reader == null)
                return (false, new SensorSample());

            string line = string.Empty;

            try
            {
                line = _reader.ReadLine();

                if (string.IsNullOrEmpty(line))
                    return (false, new SensorSample());

                string[] parts = line.Split(',');
                SensorSample sensorSample = new SensorSample(
                    double.Parse(parts[1]),
                    double.Parse(parts[3]),
                    double.Parse(parts[5]),
                    double.Parse(parts[4]),
                    DateTime.Parse(parts[0]));

                return (true, sensorSample);
            }
            catch
            {
                _invalidLines.Add(line);
                return (false, new SensorSample());
            }
        }

        public List<string> GetAllInvalidLines()
        {
            if (_invalidLines == null)
                return new List<string>();

            return _invalidLines;
        }

        public string GetAllRemainingText()
        {
            if (_disposed || _reader == null)
                return string.Empty;

            return _reader.ReadToEnd();
        }
    }
}
