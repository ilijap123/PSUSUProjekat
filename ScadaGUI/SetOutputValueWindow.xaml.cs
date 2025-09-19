using DataConcentrator;
using PLCSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Interaction logic for SetOutputValueWindow.xaml
    /// </summary>
    public partial class SetOutputValueWindow : Window
    {
        public Tag SelectedTag = new Tag();
        private PLCSimulatorManager PLC_Simulator;
        public bool IsUpdate = false;
        public SetOutputValueWindow(Tag TagInfo, PLCSimulatorManager PLC)
        {
            InitializeComponent();
            PLC_Simulator = PLC;

            if (TagInfo != null)
            {

                SelectedTag = TagInfo;

                outputValue.Text = TagInfo.CurrentValue.ToString();

                this.DataContext = SelectedTag;
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult value = MessageBox.Show("Are you sure?", "Deleting output value info", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (value == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                double setPoint;
                if (!double.TryParse(outputValue.Text, out setPoint))
                {
                    MessageBox.Show("Vrednost mora biti broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (setPoint < SelectedTag.LowLimit || setPoint > SelectedTag.HighLimit)
                {
                    MessageBox.Show("Unesite vrednost unutar opsega!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (SelectedTag.Type == TagTypes.DO && (setPoint != 0 && setPoint != 1))
                {
                    MessageBox.Show("Digitalni izlaz zahteva 0 ili 1 kao vrednost!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                if (SelectedTag.Type == TagTypes.DO)
                {
                    PLC_Simulator.SetDigitalValue(SelectedTag.IOAddress, setPoint);
                }
                else if (SelectedTag.Type == TagTypes.AO)
                {
                    PLC_Simulator.SetAnalogValue(SelectedTag.IOAddress, setPoint);
                }


                Context.Instance.SaveChanges();

                this.Close();
            }
        }

        private bool ValidateInput()
        {
            bool retVal = true;
            if (String.IsNullOrEmpty(outputValue.Text))
            {
                outputValue.BorderBrush = Brushes.Red;
                outputValueTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                outputValue.ClearValue(Border.BorderBrushProperty);
                outputValueTxt.Visibility = Visibility.Hidden;
            }

            return retVal;
        }
    }
}

