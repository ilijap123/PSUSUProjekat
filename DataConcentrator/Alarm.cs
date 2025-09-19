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
    public class Alarm : INotifyPropertyChanged
    {
        private int alarmId;
        private string tagNameId;
        private double alarmThreshold;
        private string alarmHighLow;
        private string alarmMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        [Key]
        public int AlarmId
        {
            get { return alarmId; }
            set
            {
                alarmId = value;
                OnPropertyChanged("AlarmId");
            }

        }

        public string TagNameId
        {
            get { return tagNameId; }
            set
            {
                tagNameId = value;
                OnPropertyChanged("TagNameId");
            }
        }

        public double AlarmThreshold
        {
            get { return alarmThreshold; }
            set
            {
                alarmThreshold = value;
                OnPropertyChanged("AlarmThreshold");
            }
        }

        public string AlarmHighLow
        {
            get { return alarmHighLow; }
            set
            {
                alarmHighLow = value;
                OnPropertyChanged("AlarmHighLow");
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

        public override string ToString()
        {
            return $"Id: {AlarmId}\nTagNameId:{TagNameId} \nAlarmThreshold: {AlarmThreshold} \nAlarmHighLow: {AlarmHighLow} \nAlarmMessage: {AlarmMessage}" ;
        }
  
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
