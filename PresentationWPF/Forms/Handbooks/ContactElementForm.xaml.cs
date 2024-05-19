using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace PresentationWPF.Forms
{
    public partial class ContactElementForm : Window, INotifyPropertyChanged
    {
        private readonly IHandbookController _contorller;
        public event PropertyChangedEventHandler? PropertyChanged;
        private Contact _data = new();
        private string _title = string.Empty;
        private bool _isChange;
        public bool IsChange { get { return _isChange; } set { _isChange = value; if (_isChange) Title = _title + "*"; else Title = _title; } }

        public string Code
        {
            get { return _data.Code; }
            set { _data.Code = value; OnPropertyChanged(); }
        }

        public string NameData
        {
            get { return _data.Name; }
            set { _data.Name = value; }
        }

        public string Email
        {
            get { return _data.Email; }
            set { _data.Email = value; }
        }

        public string PhoneNumber
        {
            get { return _data.PhoneNumber; }
            set { _data.PhoneNumber = value; }
        }

        public ContactElementForm(Guid id = default)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            if (id != default)
            {
                var data = _contorller.GetHandbook<Contact>(id);

                if (data != null)
                    _data = data;

                _title = _data.Name;
            }

            InitializeComponent();
            Title = _title;
        }

        public ContactElementForm(Contact data)
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
                try
                {
                    MailAddress mail = new MailAddress(_data.Email);
                }
                catch (FormatException)
                {
                    var messageText = "Не корректно заповнена пошта!";
                    MessageBox.Show(messageText, "Помилка");
                    return;
                }

                Regex reg = new Regex("^(?:\\+1)?\\s?\\(?\\d{3}\\)?[-.\\s]?\\d{3}[-.\\s]?\\d{4}$");
                if (!reg.IsMatch(_data.PhoneNumber))
                {
                    var messageText = "Не корректно заповнений телефон!\n" +
                        "Введіть номер в одному з наступних форматів:\n" +
                        "(123) 456–7890\r\n123–456–7890\r\n123.456.7890\r\n1234567890\r\n+11234567890";
                    MessageBox.Show(messageText, "Помилка");
                    return;
                }

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
