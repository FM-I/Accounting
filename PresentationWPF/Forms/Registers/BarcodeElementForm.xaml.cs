using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PresentationWPF.Forms.Registers
{
    public partial class BarcodeElementForm : Window, INotifyPropertyChanged
    {
        private readonly IInformationRegisterController _controller;
        private readonly IHandbookController _handbookController;
        private Barcode _data = new();
        private bool _isChange;
        private bool _isNew = true;
        public event PropertyChangedEventHandler? PropertyChanged;
        public bool IsChange { get { return _isChange; } set { _isChange = value; if (_isChange) Title = Title + "*"; else Title = "Штрихкод"; } }

        private string _nomenclatureName;
        private string _unitName;
        public string NomenclatureName 
        {
            get { return _nomenclatureName; }
            set { _nomenclatureName = value; OnPropertyChanged(); }
        }

        public string UnitName
        {
            get { return _unitName; }
            set { _unitName = value; OnPropertyChanged(); }
        }

        public string Barcode
        {
            get { return _data?.Value; }
            set { _data.Value = value; OnPropertyChanged(); }
        }

        public BarcodeElementForm(Guid nomenclatureId = default, Guid unitId = default, bool isCopy = false)
        {
            DataContext = this;
            _controller = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            if (nomenclatureId != default && unitId != default)
            {
                var data = _controller.GetListData<Barcode>(0, 0, w => w.NomenclatureId == nomenclatureId && w.UnitId == unitId);

                if(data != null && data.Count > 0)
                {
                    _data = data.First();
                    NomenclatureName = _data.Nomenclature.Name;
                    UnitName = _data.Unit.Name;
                    Barcode = _data.Value;
                    _isNew = false;
                }
            }

            if(isCopy)
                _isNew = isCopy;

            InitializeComponent();

            IsChange = false;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = _data.CheckDataComplection();

            if (result.Success)
            {
                if (_isNew)
                {
                    var data = _controller.GetListData<Barcode>(selectionFunc: w => w.NomenclatureId == _data.NomenclatureId && w.UnitId == _data.UnitId);

                    if(data != null && data.Count > 0)
                    {
                        MessageBox.Show("Запис з подібними ключами уже є в таблиці!!!", "Помилка");
                        return;
                    }

                }

                _data.Nomenclature = null;
                _data.Unit = null;
                await _controller.AddOrUpdateAsync(_data, _isNew);
                IsChange = false;
                _isNew = false;
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
            if(!IsChange)
                IsChange = true;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if(button != null)
            {
                var form = new Window();

                switch (button.Name)
                {
                    case "btnOpenNomenclature":

                        if (_data.NomenclatureId == null && _data.NomenclatureId == default)
                            return;

                        form = new NomenclatureElementForm(_data.NomenclatureId);

                        break;

                    case "btnOpenUnit":

                        if (_data.UnitId == null && _data.UnitId == default)
                            return;

                        form = new UnitElementForm(_data.UnitId);

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
                        var data = _handbookController.GetHandbook<Unit>(_data.UnitId);
                        if (data != null)
                            UnitName = data.Name;
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

                    case "btnShowListUnit":

                        form = new UnitListForm(true);

                        break;
                    default: return;
                }

                var res = form.ShowDialog();

                if(res != null)
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

                            if (string.IsNullOrWhiteSpace(UnitName) && data.BaseUnit != null)
                            {
                                UnitName = data.BaseUnit.Name;
                                _data.UnitId = data.BaseUnit.Id;
                            }

                            if(!IsChange)
                                IsChange = true;
                        }
                    }
                    else
                    {
                        id = ((UnitListForm)form).SelectedId;
                        var data = _handbookController.GetHandbook<Unit>(id);

                        if (data != null)
                        {
                            _data.UnitId = id;
                            UnitName = data.Name;
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

                    case "btnClearUnit":

                        UnitName = "";
                        _data.Unit = null;
                        _data.UnitId = Guid.Empty;

                        break;
                    default: return;
                }

            }
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
