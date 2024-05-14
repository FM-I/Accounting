using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PresentationWPF.Forms
{
    public partial class UnitElementForm : Window, INotifyPropertyChanged
    {
        private readonly IHandbookController _contorller;
        public event PropertyChangedEventHandler? PropertyChanged;
        private Unit _data = new();
        private string _title = string.Empty;
        private bool _isChange;
        public bool IsChange { get { return _isChange; } set { _isChange = value; if (_isChange) Title = _title + "*"; else Title = _title; } }

        public string Code
        {
            get
            {
                return _data.Code;
            }
            set
            {
                _data.Code = value;
                OnPropertyChanged();
            }
        }

        public string NameData
        {
            get
            {
                return _data.Name;
            }
            set
            {
                _data.Name = value;
                OnPropertyChanged();
            }
        }

        public double Coefficient
        {
            get
            {
                return _data.Coefficient;
            }
            set
            {
                _data.Coefficient = value;
                OnPropertyChanged();
            }
        }

        public UnitElementForm(Guid id = default)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            if (id != default)
            {
                var data = _contorller.GetHandbook<Unit>(id);

                if (data != null)
                    _data = data;

                _title = _data.Name;
            }

            InitializeComponent();
            Title = _title;
        }

        public UnitElementForm(Unit data)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _data = data;
            _title = _data.Name;
            InitializeComponent();
            Title = _title;
            IsChange = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = _data.CheckDataComplection();

            if (result.Success)
            {
                await _contorller.AddOrUpdateAsync(_data);
                OnPropertyChanged(nameof(Code));
                IsChange = false;
            }
            else
            {
                var messageText = "Поля " + string.Join(", ", result.Properties) + " незаповнені!!!";
                MessageBox.Show(messageText, "Помилка");
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
            IsChange = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsChange
                && MessageBox.Show("Дані було змінено. Збергти зміни?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Button_Click(null, null);
                e.Cancel = IsChange;
            }
        }
    }
}
