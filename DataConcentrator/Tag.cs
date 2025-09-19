using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{

    public enum TagTypes
    {
        DI,
        AI,
        DO,
        AO
    }

    public class Tag : INotifyPropertyChanged
    {

        #region fields
        private TagTypes type;
        private string nameId;
        private string description;
        private string ioAddress;
        private double currentValue;

        private double? scanTime;
        private bool? onoffScan;
        private double? lowLimit;
        private double? highLimit;
        private string units;
        private double? initialValue;
      

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<Tag> CriticalValueReached;

        #region properties
        public TagTypes Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        [Key]
        public string NameId
        {
            get { return nameId; }
            set
            {
                nameId = value;
                OnPropertyChanged("NameId");
            }

        }
       
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public string IOAddress
        {
            get { return ioAddress; }
            set
            {
                ioAddress = value;
                OnPropertyChanged("IOAddress");
            }
        }

        [NotMapped]
        public double CurrentValue
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                OnPropertyChanged("CurrentValue");
            }
        }

        public double? ScanTime
        {
            get { return scanTime; }
            set
            {
                if (Type == TagTypes.DI || Type == TagTypes.AI)
                {
                    scanTime = value;
                    OnPropertyChanged("ScanTime");
                }
            }
        }

        public bool? OnOffScan
        {
            get { return onoffScan; }
            set
            {
                if (Type == TagTypes.DI || Type == TagTypes.AI)
                {
                    onoffScan = value;
                    OnPropertyChanged("OnOffScan");
                }
                  
            }
        }

        public double? LowLimit
        {
            get { return lowLimit; }
            set
            {
                if (Type == TagTypes.AO || Type == TagTypes.AI)
                {
                    lowLimit = value;
                    OnPropertyChanged("LowLimit");
                }
                    
            }
        }

        public double? HighLimit
        {
            get { return highLimit; }
            set
            {
                if (Type == TagTypes.AO || Type == TagTypes.AI)
                {
                    highLimit = value;
                    OnPropertyChanged("HighLimit");
                }
            }
        }

        public string Units
        {
            get { return units; }
            set
            {
                if (Type == TagTypes.AO || Type == TagTypes.AI)
                {
                    units = value;
                    OnPropertyChanged("Units");
                }  
            }
        }

        public double? InitialValue
        {
            get { return initialValue; }
            set
            {
                if (Type == TagTypes.DO || Type == TagTypes.AO)
                {
                    initialValue = value;
                    OnPropertyChanged("InitialValue");
                }
                
            }
        }

        #endregion


        #region methods
        public override string ToString()
        {
            return $"Type: {Type}\nNameId:{NameId} \nDescription: {Description} \nIOAddress: {IOAddress} \nScanTime: {ScanTime}\n" +
                $"OnOffScan: {OnOffScan} \nLowLimit: {LowLimit} \nHighLimit: {HighLimit} \nUnits: {Units} \nInitialValue: {InitialValue}";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CheckCriticalValue()
        {
            if (!HighLimit.HasValue || !LowLimit.HasValue) return;

            double mid = (HighLimit.Value + LowLimit.Value) / 2;
            double lowerBound = mid - 5;
            double upperBound = mid + 5;

            if (CurrentValue >= lowerBound && CurrentValue <= upperBound)
            {
                CriticalValueReached?.Invoke(this);
            }
        }
        #endregion

    }
}
