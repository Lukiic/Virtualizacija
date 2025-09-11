using Common;
using System;
using System.IO;

namespace OfficeSensor
{
    public class FileReader
    {
        private StreamReader _reader;
        private bool _disposed = false;

        public FileReader(string filePath)
        {
            if (File.Exists(filePath))
            {
                _reader = new StreamReader(filePath);
                _reader.ReadLine();     // Skipping header of CSV
            }
            else
                _reader = null;
        }

        ~FileReader()
        {
            Dispose(false);     // Not necessary but following Dispose design pattern
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
                if (disposing)
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

            try
            {
                string line = _reader.ReadLine();

                if (string.IsNullOrEmpty(line))
                    return (false, new SensorSample());

                string[] parts = line.Split(',');
                SensorSample sensorSample = new SensorSample(
                    double.Parse(parts[1]),
                    double.Parse(parts[3]),
                    double.Parse(parts[5]),
                    double.Parse(parts[4]),
                    DateTime.Now);

                return (true, sensorSample);
            }
            catch
            {
                return (false, new SensorSample());
            }
        }
    }
}
