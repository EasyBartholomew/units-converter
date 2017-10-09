using Converter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

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
                AutoCalculate();
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
                AutoCalculate();

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
                AutoCalculate();
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
            ToClipboardCommand = new Command(() =>
            {
                if(ToValue != null)
                    Clipboard.SetText(ToValue);
            });

            // Implement clipboard paste action
            FromClipboardCommand = new Command(() =>
            {
                FromValue = Clipboard.GetText();
            });
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

        /// <summary>
        /// Try calculate convertion
        /// </summary>
        private void AutoCalculate()
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
