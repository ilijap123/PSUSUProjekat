using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using DataConcentrator;

namespace ScadaGUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public Tag NewTag = new Tag();
        public bool IsUpdate = false;
        public AddWindow(Tag tagInfo)
        {
            InitializeComponent();
            this.DataContext = NewTag;

            typeCombo.ItemsSource = new List<string>() { "DI", "DO", "AI", "AO" };
            List<string> IOAddressList = new List<string>() { "ADDR001", "ADDR002", "ADDR003", "ADDR004", "ADDR005", "ADDR006",
                                                         "ADDR007", "ADDR008", "ADDR009", "ADDR010", "ADDR011", "ADDR012",
                                                         "ADDR013", "ADDR014", "ADDR015", "ADDR016"};

            if (tagInfo != null)
            {
                var usedAddresses = Context.Instance.Tags
                                         .Where(tag => tag.NameId != tagInfo.NameId)
                                         .Select(tag => tag.IOAddress)
                                         .ToList();

                IOAddressList = IOAddressList.Except(usedAddresses).ToList();
                ioAddress.ItemsSource = IOAddressList;
            }
            else
            {
                var usedAddresses = Context.Instance.Tags.Select(t => t.IOAddress).ToList();
                var freeAddresses = IOAddressList.Except(usedAddresses).ToList();
                ioAddress.ItemsSource = freeAddresses;
            }



            if (tagInfo != null)
            {
                NewTag.Type = tagInfo.Type;
                NewTag.NameId = tagInfo.NameId;
                NewTag.Description = tagInfo.Description;
                NewTag.IOAddress = tagInfo.IOAddress;
                NewTag.ScanTime = tagInfo.ScanTime;
                NewTag.OnOffScan = tagInfo.OnOffScan;
                NewTag.LowLimit = tagInfo.LowLimit;
                NewTag.HighLimit = tagInfo.HighLimit;
                NewTag.Units = tagInfo.Units;   
                NewTag.InitialValue = tagInfo.InitialValue;

                IsUpdate = true;
                nameId.IsReadOnly = true;
            }
            else
            {
                IsUpdate = false;
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult value = MessageBox.Show("Are you sure?", "Deleting tag info", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
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
                    Tag updateTag = (from tag in Context.Instance.Tags
                                             where tag.NameId == NewTag.NameId
                                             select tag).FirstOrDefault();
                    if (updateTag != null)
                    {
                        updateTag.Type = NewTag.Type;
                        updateTag.NameId = NewTag.NameId;
                        updateTag.Description = NewTag.Description;
                        updateTag.IOAddress= NewTag.IOAddress;
                        updateTag.ScanTime= NewTag.ScanTime; 
                        updateTag.OnOffScan = NewTag.OnOffScan;
                        updateTag.LowLimit= NewTag.LowLimit;
                        updateTag.HighLimit= NewTag.HighLimit;
                        updateTag.Units= NewTag.Units;
                        updateTag.InitialValue = NewTag.InitialValue;
                        updateTag.CurrentValue = NewTag.InitialValue ?? 0;
                    }
                }
                else
                {
                    NewTag.CurrentValue = NewTag.InitialValue ?? 0;
                    Context.Instance.Tags.Add(NewTag);
                }
                Context.Instance.SaveChanges();
                this.Close();
            }
        }
        private bool ValidateInput()
        {
            bool retVal = true;

            if (String.IsNullOrEmpty(nameId.Text))
            {
                nameId.BorderBrush = Brushes.Red;
                nameIdTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                nameId.ClearValue(Border.BorderBrushProperty);
                nameIdTxt.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrEmpty(description.Text))
            {
                description.BorderBrush = Brushes.Red;
                descriptionTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                description.ClearValue(Border.BorderBrushProperty);
                descriptionTxt.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrEmpty(highLimit.Text) && (NewTag.Type == TagTypes.AI || NewTag.Type == TagTypes.AO))
            {
                highLimit.BorderBrush = Brushes.Red;
                highLimitTxt2.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                highLimit.ClearValue(Border.BorderBrushProperty);
                highLimitTxt2.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrEmpty(lowLimit.Text) && (NewTag.Type == TagTypes.AI || NewTag.Type == TagTypes.AO))
            {
                lowLimit.BorderBrush = Brushes.Red;
                lowLimitTxt2.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                lowLimit.ClearValue(Border.BorderBrushProperty);
                lowLimitTxt2.Visibility = Visibility.Hidden;
            }


            if (typeCombo.SelectedIndex == -1)
            {
                typeComboTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                typeComboTxt.Visibility = Visibility.Hidden;
            }

            if (ioAddress.SelectedIndex == -1)
            {
                ioAddressTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                ioAddress.Visibility = Visibility.Hidden;
            }


            if (((string)typeCombo.SelectedItem == "AI" || (string)typeCombo.SelectedItem == "DI") && String.IsNullOrEmpty(scanTime.Text))
            {
                scanTime.BorderBrush = Brushes.Red;
                scanTimeTxt2.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                scanTimeTxt2.Visibility = Visibility.Hidden;
            }

            if (((string)typeCombo.SelectedItem == "DO" || (string)typeCombo.SelectedItem == "DI") && !String.IsNullOrEmpty(lowLimit.Text))
            {
                lowLimit.BorderBrush = Brushes.Red;
                lowLimitTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                lowLimitTxt.Visibility = Visibility.Hidden;
            }

            if (((string)typeCombo.SelectedItem == "DO" || (string)typeCombo.SelectedItem == "DI") && !String.IsNullOrEmpty(highLimit.Text))
            {
                highLimit.BorderBrush = Brushes.Red;
                highLimitTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                highLimitTxt.Visibility = Visibility.Hidden;
            }

            if (((string)typeCombo.SelectedItem == "DO" || (string)typeCombo.SelectedItem == "DI") && !String.IsNullOrEmpty(units.Text))
            {
                units.BorderBrush = Brushes.Red;
                unitsTxt.Visibility = Visibility.Visible;
                retVal = false;
            }
            else
            {
                unitsTxt.Visibility = Visibility.Hidden;
            }

            if (((string)typeCombo.SelectedItem == "AI" || (string)typeCombo.SelectedItem == "DI") && !String.IsNullOrEmpty(initialValue.Text))
            {
                initialValue.BorderBrush = Brushes.Red;
                retVal = false;
            }

            if((string)typeCombo.SelectedItem == "DO" && !String.IsNullOrEmpty(initialValue.Text))
            {
                if (initialValue.Text == "0" || initialValue.Text == "1")
                {
                    initialValueTxt.Visibility = Visibility.Hidden;
                }
                else
                {
                    initialValueTxt.Visibility = Visibility.Visible;
                    initialValue.BorderBrush = Brushes.Red;
                    retVal = false;
                }
            }

            
            if ((string)typeCombo.SelectedItem == "AO" && (!String.IsNullOrEmpty(lowLimit.Text) || !String.IsNullOrEmpty(highLimit.Text)))
            {
                double initVal = double.Parse(initialValue.Text);
                double high = double.Parse(highLimit.Text);
                double low = double.Parse(lowLimit.Text);
                if (initVal < low || initVal > high)
                {
                    initialValueTxt2.Visibility = Visibility.Visible;
                    retVal = false;
                }
                else
                {
                    initialValueTxt2.Visibility = Visibility.Hidden;
                }
            }
                return retVal;
        }

       

    }
}
