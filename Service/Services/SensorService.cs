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

        private FileWriter _validDataFileWriter;
        private FileWriter _invalidDataFileWriter;
        private SensorSample _lastSample;           // Comparing last sample values with current sample values and raise event if needed

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
                _lastSample = sensorSample;     // Before this compare sensorSample with _lastSample to check if event should be raised
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
    }
}
