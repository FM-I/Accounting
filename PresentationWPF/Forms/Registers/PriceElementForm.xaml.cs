using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PresentationWPF.Forms.Registers
{
    public partial class PriceElementForm : Window, INotifyPropertyChanged
    {
        private record DataKey(DateTime Date, Guid NomenclatureId, Guid TypePriceId);

        private readonly IInformationRegisterController _controller;
        private readonly IHandbookController _handbookController;
        private Price _data = new();
        private DataKey _prevData;
        private bool _isChange;
        private bool _isNew = true;
        public event PropertyChangedEventHandler? PropertyChanged;
        public bool IsChange { get { return _isChange; } set { _isChange = value; if (_isChange) Title = Title + "*"; else Title = "Ціна"; } }

        private string _nomenclatureName;
        private string _typePrice;
        private string _price = string.Empty;
        
        public DateTime Period
        {
            get { return _data.Date; }
            set { _data.Date = value; OnPropertyChanged(); }
        }

        public string NomenclatureName
        {
            get { return _nomenclatureName; }
            set { _nomenclatureName = value; OnPropertyChanged(); }
        }

        public string TypePriceName
        {
            get { return _typePrice; }
            set { _typePrice = value; OnPropertyChanged(); }
        }

        public string Price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(); }
        }

        public PriceElementForm(DateTime date = default, Guid nomenclatureId = default, Guid typePriceId = default, bool isCopy = false)
        {
            DataContext = this;
            _controller = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            Period = DateTime.Now;

            if (nomenclatureId != default && typePriceId != default)
            {
                var data = _controller.GetListData<Price>(0, 0, w => w.Date == date && w.NomenclatureId == nomenclatureId && w.TypePriceId == typePriceId);

                if (data != null && data.Count > 0)
                {
                    _data = data.First();
                    NomenclatureName = _data.Nomenclature.Name;
                    TypePriceName = _data.TypePrice.Name;
                    
                    if(_data.Value != 0)
                        Price = _data.Value.ToString();

                    Period = _data.Date;
                    _isNew = false;

                    _prevData = new(_data.Date, _data.NomenclatureId, _data.TypePriceId);
                }
            }

            if (isCopy)
                _isNew = isCopy;

            InitializeComponent();

            IsChange = false;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            decimal res = 0;
            
            if (!Regex.IsMatch(_price, @"^[0-9]*\,?[0-9]+$")
                || !decimal.TryParse(_price, out res))
            {
                MessageBox.Show("Значення ціни вказано не вірно!!!", "Помилка");
                return;
            };

            _data.Value = res;

            var result = _data.CheckDataComplection();

            if (result.Success)
            {
                string message = "";
                if (_isNew)
                {
                    var data = _controller.GetListData<Price>(selectionFunc: w => w.Date == _data.Date && w.NomenclatureId == _data.NomenclatureId && w.TypePriceId == _data.TypePriceId);

                    if (data != null && data.Count > 0)
                        message = "Запис з подібними ключами уже є в таблиці!!!";

                }
                else if (_prevData != null
                    && (_prevData.NomenclatureId != _data.NomenclatureId
                    || _prevData.TypePriceId != _data.TypePriceId
                    || _prevData.Date != _data.Date))
                {
                    var data = _controller.GetListData<Price>(selectionFunc: w => w.Date == _data.Date && w.NomenclatureId == _data.NomenclatureId && w.TypePriceId == _data.TypePriceId);

                    if (data != null && data.Count > 0)
                    {
                        message = "Запис з подібними ключами уже є в таблиці!!!";
                    }
                    else
                    {
                        _isNew = true;
                        await _controller.DeleteAsync<Price>(w => w.Date == _prevData.Date && w.NomenclatureId == _prevData.NomenclatureId && w.TypePriceId == _prevData.TypePriceId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show(message, "Помилка");
                    return;
                }

                _data.Nomenclature = null;
                _data.TypePrice = null;
                
                await _controller.AddOrUpdateAsync(_data, _isNew);
                Price = _data.Value.ToString();
                IsChange = false;
                _isNew = false;
                _prevData = new(_data.Date, _data.NomenclatureId, _data.TypePriceId);
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
                var form = new Window();

                switch (button.Name)
                {
                    case "btnOpenNomenclature":

                        if (_data.NomenclatureId == default)
                            return;

                        form = new NomenclatureElementForm(_data.NomenclatureId);

                        break;

                    case "btnOpenTypePrice":

                        if (_data.TypePriceId == default)
                            return;

                        form = new TypePriceElementForm(_data.TypePriceId);

                        break;
                    default: return;
                }

                var res = form.ShowDialog();

                if (res != null)
                {
                    if (button.Name == "btnShowListNomenclature")
                    {
                        var data = _handbookController.GetHandbook<Nomenclature>(_data.NomenclatureId);
                        if (data != null)
                            NomenclatureName = data.Name;
                    }
                    else
                    {
                        var data = _handbookController.GetHandbook<TypePrice>(_data.TypePriceId);
                        if (data != null)
                            TypePriceName = data.Name;
                    }
                }

            }
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if (button != null)
            {
                var form = new Window();

                switch (button.Name)
                {
                    case "btnShowListNomenclature":

                        form = new NomenclatureListForm(true);

                        break;

                    case "btnShowListTypePrice":

                        form = new TypePriceListForm(true);

                        break;
                    default: return;
                }

                var res = form.ShowDialog();

                if (res != null)
                {
                    Guid id = Guid.Empty;
                    if (button.Name == "btnShowListNomenclature")
                    {
                        id = ((NomenclatureListForm)form).SelectedId;
                        var data = _handbookController.GetHandbook<Nomenclature>(id);
                        if (data != null)
                        {
                            _data.NomenclatureId = id;
                            NomenclatureName = data.Name;

                            if (!IsChange)
                                IsChange = true;
                        }
                    }
                    else
                    {
                        id = ((TypePriceListForm)form).SelectedId;
                        var data = _handbookController.GetHandbook<TypePrice>(id);

                        if (data != null)
                        {
                            _data.TypePriceId = id;
                            TypePriceName = data.Name;
                            if (!IsChange)
                                IsChange = true;
                        }
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if (button != null)
            {
                switch (button.Name)
                {
                    case "btnClearNomenclature":

                        NomenclatureName = "";
                        _data.Nomenclature = null;
                        _data.NomenclatureId = Guid.Empty;

                        break;

                    case "btnClearTypePrice":

                        TypePriceName = "";
                        _data.TypePrice = null;
                        _data.TypePriceId = Guid.Empty;

                        break;
                    default: return;
                }

            }
        }

        private void PriceInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

            if(e.Text == ",")
            {
                var res = _price.FirstOrDefault(w => w.Equals(','));
                if(res != default)
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
