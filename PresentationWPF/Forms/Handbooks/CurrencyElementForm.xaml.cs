using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PresentationWPF.Forms
{
    public partial class CurrencyElementForm : Window, INotifyPropertyChanged
    {
        private readonly IHandbookController _contorller;
        public event PropertyChangedEventHandler? PropertyChanged;
        private Currency _data = new();
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


        public CurrencyElementForm(Guid id = default)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            if (id != default)
            {
                var data = _contorller.GetHandbook<Currency>(id);

                if (data != null)
                    _data = data;

                _title = _data.Name;
            }

            InitializeComponent();
            Title = _title;
        }

        public CurrencyElementForm(Currency data)
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
