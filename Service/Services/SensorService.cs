using Common;
using Service.EventArguments;
using System;
using System.Configuration;
using System.ServiceModel;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SensorService : ISensorService
    {
        public delegate void MyEventHandler(object sender, EventArgsWithMessage e);
        public event MyEventHandler OnTransferStarted;
        public event MyEventHandler OnSampleReceived;
        public event MyEventHandler OnTransferCompleted;
        public event MyEventHandler OnWarningRaised;

        public event MyEventHandler VolumeSpike;
        public event MyEventHandler OutOfBandWarning;
        public event MyEventHandler TemperatureSpikeDHT;
        public event MyEventHandler TemperatureSpikeBMP;

        private FileWriter _validDataFileWriter;
        private FileWriter _invalidDataFileWriter;
        private SensorSample _lastSample;
        private int _sampleCount;
        private double _volumeMean;

        private double volumeThreshold = double.Parse(ConfigurationManager.AppSettings["V_threshold"]);
        private double deviationPercent = double.Parse(ConfigurationManager.AppSettings["DeviationThresholdPercent"]);
        private double temperatureDhtThreshold = double.Parse(ConfigurationManager.AppSettings["T_dht_threshold"]);
        private double temperatureBmpThreshold = double.Parse(ConfigurationManager.AppSettings["T_bmp_threshold"]);

        public ServerResponse StartSession(SensorSample meta)
        {
            if (OnTransferStarted != null)
                OnTransferStarted(this, new EventArgsWithMessage("Transfer started."));

            _validDataFileWriter = new FileWriter(ConfigurationManager.AppSettings["validDataFile"]);
            _invalidDataFileWriter = new FileWriter(ConfigurationManager.AppSettings["invalidDataFile"]);

            try
            {
                ValidateData(meta);     // Doesn't throw -> valid data
                _validDataFileWriter.WriteSensorSample(meta);

                _lastSample = meta;
                _sampleCount = 1;
                _volumeMean = meta.Volume;
            }
            catch (Exception ex)
            {
                if (OnWarningRaised != null)
                {
                    if (ex is FaultException<ValidationException> fex)
                        OnWarningRaised(this, new EventArgsWithMessage($"Warning raised: {fex.Detail}"));
                    else
                        OnWarningRaised(this, new EventArgsWithMessage($"Warning raised: {ex}"));
                }

                _invalidDataFileWriter.WriteSensorSample(meta);
                throw ex;
            }

            return new ServerResponse(ResponseStatus.ACK, SessionStatus.IN_PROGRESS);
        }

        public ServerResponse PushSample(SensorSample sensorSample)
        {
            if (OnSampleReceived != null)
                OnSampleReceived(this, new EventArgsWithMessage($"Sample received. Sample values: {sensorSample}"));

            try
            {
                ValidateData(sensorSample);     // Doesn't throw -> valid data
                _validDataFileWriter.WriteSensorSample(sensorSample);

                CompareWithLastSample(sensorSample);
                _lastSample = sensorSample;
            }
            catch (Exception ex)
            {
                if (OnWarningRaised != null)
                {
                    if (ex is FaultException<ValidationException> fex)
                        OnWarningRaised(this, new EventArgsWithMessage($"Warning raised: {fex.Detail}"));
                    else
                        OnWarningRaised(this, new EventArgsWithMessage($"Warning raised: {ex}"));
                }

                _invalidDataFileWriter.WriteSensorSample(sensorSample);
                throw ex;
            }

            return new ServerResponse(ResponseStatus.ACK, SessionStatus.IN_PROGRESS);
        }

        public ServerResponse EndSession()
        {
            if (OnTransferCompleted != null)
                OnTransferCompleted(this, new EventArgsWithMessage("Transfer completed."));

            _validDataFileWriter.Dispose();
            _invalidDataFileWriter.Dispose();

            return new ServerResponse(ResponseStatus.ACK, SessionStatus.COMPLETED);
        }

        private void ValidateData(SensorSample sensorSample)
        {
            if (sensorSample.Volume < 0)
                throw new FaultException<ValidationException>(new ValidationException("Volume", "Volume must be non-negative."));

            if (sensorSample.Pressure <= 0)
                throw new FaultException<ValidationException>(new ValidationException("Pressure", "Pressure must be greater than zero."));

            if (sensorSample.DateTime > DateTime.Now)
                throw new FaultException<ValidationException>(new ValidationException("DateTime", "Date and time cannot be in the future."));
        }

        private void CompareWithLastSample(SensorSample newSensorSample)
        {
            if (_lastSample == null)
                return;

            CompareVolume(newSensorSample.Volume);
            UpdateVolumeMean(newSensorSample.Volume);

           
            
        }

        private void CompareVolume(double newVolumeValue)
        {
            double previousVolumeValue = _lastSample.Volume;

            double delta = newVolumeValue - previousVolumeValue;

            if (delta > volumeThreshold && VolumeSpike != null)
                VolumeSpike(this, new EventArgsWithMessage("Above expected value"));
            else if (delta < -volumeThreshold && VolumeSpike != null)
                VolumeSpike(this, new EventArgsWithMessage("Below expected value"));

            if (newVolumeValue > (1 + deviationPercent / 100) * _volumeMean && OutOfBandWarning != null)
                OutOfBandWarning(this, new EventArgsWithMessage("Above expected value"));

            else if (newVolumeValue < (1 - deviationPercent / 100) * _volumeMean && OutOfBandWarning != null)
                OutOfBandWarning(this, new EventArgsWithMessage("Below expected value"));
        }

        private void UpdateVolumeMean(double newVolumeValue)
        {
            ++_sampleCount;
            _volumeMean += (newVolumeValue - _volumeMean) / _sampleCount;
        }

        

        
    }
}
