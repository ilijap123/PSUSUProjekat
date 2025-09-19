using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{
    public class ActivatedAlarm: INotifyPropertyChanged
    {
        private int activatedAlarmId;
        private int alarmId;
        private string tagName;
        private string alarmMessage;
        private DateTime alarmActivationTime;
        private bool isActive;
        private DateTime? alarmDeactivationTime;
        public event PropertyChangedEventHandler PropertyChanged;

        [Key]
        public int ActivatedAlarmId
        {
            get { return activatedAlarmId; }
            set
            {
                activatedAlarmId = value;
                OnPropertyChanged("ActivatedAlarmId");
            }
        }

        public int AlarmId
        {
            get { return alarmId; }
            set
            {
                alarmId = value;
                OnPropertyChanged("AlarmId");
            }
        }

        public string TagName
        {
            get { return tagName; }
            set
            {
                tagName = value;
                OnPropertyChanged("TagName");
            }
        }

        public string AlarmMessage
        {
            get { return alarmMessage; }
            set
            {
                alarmMessage = value;
                OnPropertyChanged("AlarmMessage");
            }
        }

        public DateTime AlarmActivationTime
        {
            get { return alarmActivationTime; }
            set
            {
                alarmActivationTime = value;
                OnPropertyChanged("AlarmActivationTime");
            }
        }

        [NotMapped]
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public DateTime? AlarmDeactivationTime
        {
            get { return alarmDeactivationTime; }
            set
            {
                alarmDeactivationTime = value;
                OnPropertyChanged("AlarmDeactivationTime");
            }
        }

        public bool AlarmActivation(double currentValue, Alarm alarm)
        {
            if (alarm.AlarmHighLow == "LOW" && !isActive)
                return currentValue <= alarm.AlarmThreshold;
            else if (alarm.AlarmHighLow == "HIGH" && !isActive)
                return currentValue >= alarm.AlarmThreshold;
            return false;
        }

        public bool AlarmDeactivation(double currentValue, Alarm alarm)
        {
            if (alarm.AlarmHighLow == "LOW" && isActive)
                return currentValue > alarm.AlarmThreshold;
            else if (alarm.AlarmHighLow == "HIGH" && isActive)
                return currentValue < alarm.AlarmThreshold;
            return false;
        }

        public override string ToString()
        {
            return $"ACTIVATED ALARM!\nActivated alarm Id: {ActivatedAlarmId}\nAlarm Id: {AlarmId}\nTag name: {TagName}\nAlarm message: {AlarmMessage}";
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
