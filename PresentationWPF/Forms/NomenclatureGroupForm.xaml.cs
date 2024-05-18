using BL.Interfaces;
using Domain.Entity.Handbooks;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PresentationWPF.Forms
{
    public partial class NomenclatureGroupForm : Window, INotifyPropertyChanged
    {
        private readonly IHandbookController _contorller;
        public event PropertyChangedEventHandler? PropertyChanged;
        private Nomenclature _data = new() { IsGroup = true };
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
            {return _data.Name;}
            set
            {
                _data.Name = value;
                OnPropertyChanged();
            }
        }

        private string _groupName;

        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; OnPropertyChanged(); }
        }


        public NomenclatureGroupForm(Guid id = default)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            if (id != default)
            {
                var data = _contorller.GetHandbook<Nomenclature>(id);

                if (data != null)
                    _data = data;

                _title = _data.Name;
                GroupName = _data?.Parent?.Name;
            }

            InitializeComponent();
            Title = _title;
        }

        public NomenclatureGroupForm(Nomenclature data)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _data = data;
            _title = _data.Name;
            GroupName = _data.Parent?.Name;
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
                Title = _data.Name;
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
            if (propertyName != "GroupName")
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
        private void btnOpenGroup_Click(object sender, RoutedEventArgs e)
        {
            if (_data.Parent != null)
            {
                var form = new NomenclatureGroupForm(_data.Parent.Id);
                form.ShowDialog();
                _data.Parent = _contorller.GetHandbook<Nomenclature>(_data.Parent.Id);
                OnPropertyChanged("GroupName");
            }
        }

        private void btnShowListGroup_Click(object sender, RoutedEventArgs e)
        {
            NomenclatureGroupListForm form = new NomenclatureGroupListForm();
            var result = form.ShowDialog();
            if (result != null)
            {
                if (form.SelectedId != default)
                {
                    Guid id = form.SelectedId;
                    var data = _contorller.GetHandbook<Nomenclature>(id);

                    if (data != null && data.Id != _data.Id)
                    {
                        IsChange = true;
                        _data.ParentId = data.Id;
                        _data.Parent = null;
                        GroupName = data.Name;
                    }
                }
            }
        }

        private void btnClearGroup_Click(object sender, RoutedEventArgs e)
        {
            _data.ParentId = null;
            _data.Parent = null;
            GroupName = "";
        }
    }
}
