using System;
using Converter;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Timers;
using System.Windows.Input;
using System.Windows;
using Squirrel;
using Squirrel.Sources;
using UI.Properties;

namespace UI
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Data members

        #region Commands

        /// <summary>
        /// Swap dimension selection command
        /// </summary>
        public ICommand SwapCommand { get; set; }

        /// <summary>
        /// Copy  <see cref="ToValue"/> to system clipboard
        /// </summary>
        public ICommand ToClipboardCommand { get; set; }

        /// <summary>
        /// Paste from clipboard to <see cref="FromValue"/>  
        /// </summary>
        public ICommand FromClipboardCommand { get; set; }

        #endregion

        #region Props


        private IEnumerable<ConvertedDimention> _convertedList;

        /// <summary>
        /// </summary>
        public IEnumerable<ConvertedDimention> ConvertedList
        {
            get { return _convertedList; }
            set
            {
                _convertedList = value;
                OnPropertyChanged("ConvertedList");
            }
        }

        private IEnumerable<string> _units;

        /// <summary>
        /// Enumberble of all units
        /// </summary>
        public IEnumerable<string> Units
        {
            get { return _units; }
            set
            {
                _units = value;
                OnPropertyChanged("Units");
            }
        }

        private IEnumerable<string> _dimentions;
        /// <summary>
        /// Enumereble of all unit dimension
        /// </summary>
        public IEnumerable<string> Dimentions
        {
            get { return _dimentions; }
            set
            {
                _dimentions = value;
                OnPropertyChanged("Dimentions");
            }
        }

        private string _selectedUnit;
        /// <summary>
        /// Selected Unit
        /// </summary>
        public string SelectedUnit
        {
            get { return _selectedUnit; }
            set
            {
                _selectedUnit = value;
                SetSelectedUnitDimetions();
                UpdateCalculatedList();
            }
        }

        private string _fromDimention;
        /// <summary>
        /// Selected dimension to convert from
        /// </summary>
        public string FromDimention
        {
            get { return _fromDimention; }
            set
            {
                _fromDimention = value;
                OnPropertyChanged("FromDimention");
                UpdateCalculationSelected();
                UpdateCalculatedList();
            }
        }

        private string _toDimention;
        /// <summary>
        /// Selected dimension to convert to
        /// </summary>
        public string ToDimention
        {
            get { return _toDimention; }
            set
            {
                _toDimention = value;
                OnPropertyChanged("ToDimention");
                UpdateCalculationSelected();

            }
        }

        private string _message;
        /// <summary>
        /// Message to show about convetion
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private string _fromValue;
        /// <summary>
        /// Value to convert from
        /// </summary>
        public string FromValue
        {
            get { return _fromValue; }
            set
            {
                _fromValue = value;
                OnPropertyChanged("FromValue");
                UpdateCalculationSelected();
                UpdateCalculatedList();
            }
        }

        private string _toValue;
        /// <summary>
        /// Converted value to show
        /// </summary>
        public string ToValue
        {
            get { return _toValue; }
            set
            {
                _toValue = value;
                OnPropertyChanged("ToValue");
            }
        }

        #endregion

        public string CurrentVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        #endregion

        #region Init

        public MainWindowViewModel()
        {
            // Get units enumerable
            Units = UnitsConverter.UnitsDimensionsMap.Keys;

            // Set default value to convert from
            FromValue = "1";

            // Implement swap action
            SwapCommand = new Command(() =>
            {
                var temp = FromDimention;
                FromDimention = ToDimention;
                ToDimention = temp;
            });

            // Implement clipboard copy action
            ToClipboardCommand = new CommandString((text) =>
            {
                Clipboard.SetText(text);
            });

            // Implement clipboard paste action
            FromClipboardCommand = new Command(() =>
            {
                FromValue = Clipboard.GetText();
            });

            var updateChecker = new Timer
            {
                AutoReset = false,
                Enabled = false,
                Interval = 5000
            };

            updateChecker.Elapsed += OnCheckingUpdate;
            updateChecker.Start();

            try
            {
                Deploy.CreateProtocolEntries();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCheckingUpdate(object sender, ElapsedEventArgs e)
        {
            Update();
        }

        private async void Update()
        {
            var hasConnection = await RemoteUtils.CheckForInternetConnectionAsync();

            if (!hasConnection)
                return;

            using (var manager = new UpdateManager(new GithubSource(Resources.GithubUrl, null, false)))
            {
                try
                {
                    await manager.UpdateApp();
                }
                catch
                {
                    // ignored
                }
            }
        }

        #endregion

        #region General methods

        /// <summary>
        /// Set current dimentions of new selected unit
        /// </summary>
        private void SetSelectedUnitDimetions()
        {
            if (UnitsConverter.UnitsDimensionsMap.ContainsKey(SelectedUnit))
                Dimentions = UnitsConverter.UnitsDimensionsMap[SelectedUnit];
        }

        private void UpdateCalculatedList()
        {
            double fromValue = 0;

            // If data not correct yet
            if (SelectedUnit == null ||
                FromDimention == null ||
                !double.TryParse(FromValue, out fromValue) ||
                fromValue == 0)
            {
                ConvertedList = null;
                return;
            }


            // Convert all
            List<ConvertedDimention> list = new List<ConvertedDimention>();
            foreach (var d in Dimentions)
            {
                bool isSuccess2;

                // Convert
                var value = UnitsConverter.Convert(SelectedUnit,
                                                 FromDimention,
                                                 d,
                                                 fromValue,
                                                 out isSuccess2).ToString();
                if (!isSuccess2)
                    value = "---";

                list.Add(new ConvertedDimention(d, value));
            }

            ConvertedList = list;

        }

        /// <summary>
        /// Try calculate convertion
        /// </summary>
        private void UpdateCalculationSelected()
        {
            double fromValue = 0;
            // If data not correct yet
            if (SelectedUnit == null ||
                FromDimention == null ||
                ToDimention == null ||
                !double.TryParse(FromValue, out fromValue) ||
                fromValue == 0)
            {
                ToValue = "----";
                return;
            }

            bool isSuccess;

            // Convert
            ToValue = UnitsConverter.Convert(SelectedUnit,
                                             FromDimention,
                                             ToDimention,
                                             fromValue,
                                             out isSuccess).ToString();


            Message = isSuccess ? "OK" : "Error";

        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
