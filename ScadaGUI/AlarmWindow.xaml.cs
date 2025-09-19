using DataConcentrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace ScadaGUI
{
    public partial class AlarmWindow : Window
    {
        public Alarm NewAlarm = new Alarm();
        public bool IsUpdate = false;
        public AlarmWindow(Alarm alarmInfo)
        {
            InitializeComponent();
            this.DataContext = NewAlarm;

            alarmHighLowCombo.ItemsSource = new List<string>() { "HIGH", "LOW"};

            var AITagNames = Context.Instance.Tags.Where(tag => tag.Type == TagTypes.AI).Select(tag => tag.NameId).ToList();
            tagNameIdCombo.ItemsSource = AITagNames;

            if (alarmInfo != null)
            {

                NewAlarm.AlarmId = alarmInfo.AlarmId;
                NewAlarm.TagNameId = alarmInfo.TagNameId;
                NewAlarm.AlarmThreshold = alarmInfo.AlarmThreshold;
                NewAlarm.AlarmHighLow = alarmInfo.AlarmHighLow;
                NewAlarm.AlarmMessage = alarmInfo.AlarmMessage;
             
                IsUpdate = true;
            }
            else
            {
                IsUpdate = false;
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult value = MessageBox.Show("Are you sure?", "Deleting alarm info", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (value == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                if (IsUpdate)
                {
                    Alarm updateAlarm = (from alarm in Context.Instance.Alarms
                                     where alarm.AlarmId == NewAlarm.AlarmId
                                     select alarm).FirstOrDefault();

                    if (updateAlarm != null)
                    {
                        updateAlarm.TagNameId = NewAlarm.TagNameId;
                        updateAlarm.AlarmThreshold = NewAlarm.AlarmThreshold;
                        updateAlarm.AlarmHighLow = NewAlarm.AlarmHighLow;
                        updateAlarm.AlarmMessage = NewAlarm.AlarmMessage;
                    }
                }
                else
                {
                    Context.Instance.Alarms.Add(NewAlarm);
                }

                Context.Instance.SaveChanges();
                this.Close();
            }
        }
        
        private bool ValidateInput()
        {
            bool retVal = true;

            if (String.IsNullOrEmpty(alarmThreshold.Text))
            {
                alarmThreshold.BorderBrush = Brushes.Red;
                alarmThresholdTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                alarmThreshold.ClearValue(Border.BorderBrushProperty);
                alarmThresholdTxt.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrEmpty(alarmMessage.Text))
            {
                alarmMessage.BorderBrush = Brushes.Red;
                alarmMessageTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                alarmMessage.ClearValue(Border.BorderBrushProperty);
                alarmMessageTxt.Visibility = Visibility.Hidden;
            }

            if (alarmHighLowCombo.SelectedIndex == -1)
            {
                alarmHighLowComboTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                alarmHighLowComboTxt.Visibility = Visibility.Hidden;
            }

            if (tagNameIdCombo.SelectedIndex == -1)
            {
                tagNameIdComboTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                tagNameIdComboTxt.Visibility = Visibility.Hidden;
            }

            Tag SelectedTag = (from tag in Context.Instance.Tags
                     where tag.NameId == tagNameIdCombo.Text
                     select tag).FirstOrDefault();
            
            if (NewAlarm.AlarmThreshold < SelectedTag.LowLimit || NewAlarm.AlarmThreshold > SelectedTag.HighLimit)
            {
                MessageBox.Show("Alarmna vrednost mora biti unutar opsega vrednosti nadgledane promenljive!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                retVal = false;
            }

            return retVal;
        }

    }
}
