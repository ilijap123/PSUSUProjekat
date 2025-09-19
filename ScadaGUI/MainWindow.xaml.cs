using DataConcentrator;
using PLCSimulator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ScadaGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Tag SelectedTag { get; set; }
        public Alarm SelectedAlarm { get; set; }

        private PLCSimulatorManager PLC;

        private AlarmPublisher AlarmPublisher;

        public MainWindow()
        {

            InitializeComponent();
            
            PLC = new PLCSimulatorManager();
            PLC.StartPLCSimulator();

         
            Context.Instance.Tags.Load();
            Context.Instance.Alarms.Load();
            Context.Instance.ActivatedAlarms.Load();
            tagsGrid.ItemsSource = Context.Instance.Tags.Local;
            alarmsGrid.ItemsSource = Context.Instance.Alarms.Local;
            activatedAlarmsGrid.ItemsSource = Context.Instance.ActivatedAlarms.Local;

            this.DataContext = this;

            ScanningTags();

            SubscribeCriticalEvents();

            AlarmPublisher = new AlarmPublisher();

            AlarmPublisher.AlarmActivated += OnAlarmActivated;
            AlarmPublisher.AlarmDeactivated += OnAlarmDeactivated;
            AlarmPublisher.AlarmsMonitoring();

        }

        private void ReadBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(SelectedTag.ToString());
        }

        private void ReadAlarmBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(SelectedAlarm.ToString());
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow(SelectedTag);
            addWindow.ShowDialog();
        }

        private void SetOutputValueBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTag.Type == TagTypes.DO || SelectedTag.Type == TagTypes.AO)
            {
                SetOutputValueWindow setOutput = new SetOutputValueWindow(SelectedTag, PLC);
                setOutput.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vrednost se moze zadati samo izlaznim tagovima!");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {


            MessageBoxResult value = MessageBox.Show("Are you sure?", "Deleting tag", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (value == MessageBoxResult.Yes)
            {

                Context.Instance.Tags.Remove(SelectedTag);
                Context.Instance.SaveChanges();
            }
        }

        private void DeleteAlarmBtn_Click(object sender, RoutedEventArgs e)
        {


            MessageBoxResult value = MessageBox.Show("Are you sure?", "Deleting alarm", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (value == MessageBoxResult.Yes)
            {

                Context.Instance.Alarms.Remove(SelectedAlarm);
                Context.Instance.SaveChanges();
            }
        }

        private void createBtn_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow(null);
            addWindow.ShowDialog();
        }


        private void createAlarmBtn_Click(object sender, RoutedEventArgs e)
        {
            AlarmWindow addAlarmWindow = new AlarmWindow(null);
            addAlarmWindow.ShowDialog();
        }

        
        private void createReportBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

        
        private void ScanningTags()
        {
            var lastRefreshTimes = new Dictionary<Tag, DateTime>();

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(100); 

                    Dispatcher.Invoke(() =>
                    {
                        bool anyChanged = false; 

                        foreach (var tag in Context.Instance.Tags.Local)
                        {
                            if (tag.Type == TagTypes.AI || tag.Type == TagTypes.DI)
                            {
                                if (tag.OnOffScan == true)
                                {
                                    if (!lastRefreshTimes.ContainsKey(tag))
                                        lastRefreshTimes[tag] = DateTime.MinValue;

                                    var elapsedTime = DateTime.Now - lastRefreshTimes[tag];
                                    if (elapsedTime.TotalMilliseconds >= tag.ScanTime)
                                    {
                                        double newValue = PLC.GetAnalogValue(tag.IOAddress);

                                        if (tag.CurrentValue != newValue)
                                        {
                                            tag.CurrentValue = newValue;
                                            anyChanged = true;
                                            tag.CheckCriticalValue();
                                        }

                                        lastRefreshTimes[tag] = DateTime.Now;
                                    }
                                }
                            }
                        }

                        if (anyChanged)
                            tagsGrid.Items.Refresh();
                    });
                }
            });
        }

        private void OnAlarmActivated(ActivatedAlarm activated)
        {
            Dispatcher.Invoke(() =>
            {
                Context.Instance.ActivatedAlarms.Add(activated);
                Context.Instance.SaveChanges();
                activatedAlarmsGrid.Items.Refresh();
                MessageBox.Show(activated.ToString());
            });
        }

        private void OnAlarmDeactivated(ActivatedAlarm activated)
        {
            Dispatcher.Invoke(() =>
            {
                Context.Instance.SaveChanges();
                activatedAlarmsGrid.Items.Refresh();
             
            });
        }
        private void OnCriticalValueReached(Tag tag)
        {
            string fileName = "CriticalValueLogFile.txt";
            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t{tag.NameId}\t{tag.CurrentValue:F2}";
            System.IO.File.AppendAllLines(fileName, new[] { line });
        }

        private void SubscribeCriticalEvents()
        {
            foreach (var tag in Context.Instance.Tags.Local.Where(t => t.Type == TagTypes.AI))
            {
                tag.CriticalValueReached += OnCriticalValueReached;
            }
        }
        private void GenerateReport()
        {
            string fileName = "CriticalValueLogFile.txt";

            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show("Nema zabeleženih kritičnih vrednosti.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string reportCopyName = $"CriticalValuesReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var lines = System.IO.File.ReadAllLines(fileName).ToList();

            lines.Insert(0, "Report za AI tagove - vrednosti u kritičnom opsegu");
            lines.Insert(1, "---------------------------------------------------");
            lines.Insert(2, "Time\t\tTagName\tCurrentValue");

            System.IO.File.WriteAllLines(reportCopyName, lines);

            MessageBox.Show($"Report je kreiran: {reportCopyName}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}