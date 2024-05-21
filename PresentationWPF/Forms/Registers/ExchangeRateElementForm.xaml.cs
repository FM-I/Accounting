using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PresentationWPF.Forms.Registers
{
    public partial class ExchangeRateElementForm : Window, INotifyPropertyChanged
    {
        private record DataKey(DateTime Date, Guid CurrencyId);

        private readonly IInformationRegisterController _controller;
        private readonly IHandbookController _handbookController;
        private ExchangesRate _data = new();
        private DataKey _prevData;
        private bool _isChange;
        private bool _isNew = true;
        public event PropertyChangedEventHandler? PropertyChanged;
        public bool IsChange { get { return _isChange; } set { _isChange = value; if (_isChange) Title = Title + "*"; else Title = "Курс"; } }

        private string _currencyName;
        private string _exchangeRate = string.Empty;

        public DateTime Period
        {
            get { return _data.Date; }
            set { _data.Date = value; OnPropertyChanged(); }
        }

        public string CurrencyName
        {
            get { return _currencyName; }
            set { _currencyName = value; OnPropertyChanged(); }
        }

        public string Rate
        {
            get { return _exchangeRate; }
            set { _exchangeRate = value; OnPropertyChanged(); }
        }

        public ExchangeRateElementForm(DateTime date = default, Guid currencyId = default,  bool isCopy = false)
        {
            DataContext = this;
            _controller = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            Period = DateTime.Now;

            if (currencyId != default)
            {
                var data = _controller.GetListData<ExchangesRate>(0, 0, w => w.Date == date && w.CurrencyId == currencyId);

                if (data != null && data.Count > 0)
                {
                    _data = data.First();
                    CurrencyName = _data.Currency.Name;

                    if (_data.Rate != 0)
                        Rate = _data.Rate.ToString();

                    Period = _data.Date;
                    _isNew = false;

                    _prevData = new(_data.Date, _data.CurrencyId);
                }
            }

            if (isCopy)
                _isNew = isCopy;

            InitializeComponent();

            IsChange = false;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            double res = 0;

            if (!Regex.IsMatch(_exchangeRate, @"^[0-9]*\,?[0-9]+$")
                || !double.TryParse(_exchangeRate, out res))
            {
                MessageBox.Show("Значення курсу вказано не вірно!!!", "Помилка");
                return;
            };

            _data.Rate = res;

            var result = _data.CheckDataComplection();

            if (result.Success)
            {
                string message = "";
                if (_isNew)
                {
                    var data = _controller.GetListData<ExchangesRate>(selectionFunc: w => w.Date == _data.Date && w.CurrencyId == _data.CurrencyId);

                    if (data != null && data.Count > 0)
                        message = "Запис з подібними ключами уже є в таблиці!!!";

                }
                else if (_prevData != null
                    && (_prevData.CurrencyId != _data.CurrencyId
                    || _prevData.Date != _data.Date))
                {
                    var data = _controller.GetListData<ExchangesRate>(selectionFunc: w => w.Date == _data.Date && w.CurrencyId == _data.CurrencyId);

                    if (data != null && data.Count > 0)
                    {
                        message = "Запис з подібними ключами уже є в таблиці!!!";
                    }
                    else
                    {
                        _isNew = true;
                        await _controller.DeleteAsync<ExchangesRate>(w => w.Date == _prevData.Date && w.CurrencyId == _prevData.CurrencyId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show(message, "Помилка");
                    return;
                }

                _data.Currency = null;

                await _controller.AddOrUpdateAsync(_data, _isNew);
                Rate = _data.Rate.ToString();
                IsChange = false;
                _isNew = false;
                _prevData = new(_data.Date, _data.CurrencyId);
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
            if (!IsChange)
                IsChange = true;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if (button != null)
            {
                var form = new CurrencyElementForm(_data.CurrencyId);
                var res = form.ShowDialog();

                if (res != null)
                {
                    var data = _handbookController.GetHandbook<Currency>(_data.CurrencyId);
                    if (data != null)
                        CurrencyName = data.Name;
                }
            }
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if (button != null)
            {
                var form = new CurrencyListForm(true);
                var res = form.ShowDialog();

                if (res != null && form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Currency>(form.SelectedId);
                    if (data != null)
                    {
                        _data.CurrencyId = form.SelectedId;
                        CurrencyName = data.Name;

                        if (!IsChange)
                            IsChange = true;
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            CurrencyName = "";
            _data.Currency = null;
            _data.CurrencyId = Guid.Empty;
        }

        private void ExchangeRateInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == ",")
            {
                var res = _exchangeRate.FirstOrDefault(w => w.Equals(','));
                if (res != default)
                {
                    e.Handled = true;
                    return;
                }
            }

            Regex regex = new Regex(@"^[0-9,]+$");
            e.Handled = !regex.IsMatch(e.Text);
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
