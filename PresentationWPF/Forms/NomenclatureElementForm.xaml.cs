using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PresentationWPF.Forms
{
    public partial class NomenclatureElementForm : Window, INotifyPropertyChanged
    {
        private readonly IHandbookController _contorller;
        public event PropertyChangedEventHandler? PropertyChanged;
        private Nomenclature _data = new();
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

        public string? Article
        {
            get { return _data.Arcticle; }
            set { _data.Arcticle = value; OnPropertyChanged(); }
        }

        public string UnitName
        {
            get { return _data.BaseUnit.Name; }
            set { OnPropertyChanged(); }
        }


        public NomenclatureElementForm(Guid id = default)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            if (id != default)
            {
                var data = _contorller.GetHandbook<Nomenclature>(id);

                if (data != null)
                    _data = data;

                _title = _data.Name;
            }

            InitializeComponent();
            Title = _title;
        }

        public NomenclatureElementForm(Nomenclature data)
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
            if(propertyName != "UnitName")
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

        private void btnShowList_Click(object sender, RoutedEventArgs e)
        {
            UnitListForm form = new UnitListForm(true);
            var result = form.ShowDialog();
            if (result != null)
            {
                if (form.SelectedId != default)
                {
                    Guid id = form.SelectedId;
                    var data = _contorller.GetHandbook<Unit>(id);

                    if (data != null)
                    {
                        IsChange = true;
                        _data.BaseUnit = data;
                        OnPropertyChanged("UnitName");
                    }
                }
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (_data.BaseUnit != null)
            {
                var form = new UnitElementForm(_data.BaseUnit.Id);
                form.ShowDialog();
                _data.BaseUnit = _contorller.GetHandbook<Unit>(_data.BaseUnit.Id);
                OnPropertyChanged("UnitName");
            }
        }
    }
}
