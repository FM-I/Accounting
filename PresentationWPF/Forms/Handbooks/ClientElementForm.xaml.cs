using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PresentationWPF.Forms.Handbooks
{
    public partial class ClientElementForm : Window, INotifyPropertyChanged
    {
        private class DataType
        {
            public string Text { get; set; }
            public TypesClient Type { get; set; }
        }

        private readonly IHandbookController _contorller;
        public event PropertyChangedEventHandler? PropertyChanged;
        private Client _data = new();
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
            set { _data.Name = value; OnPropertyChanged(); }
        }

        private string _groupName;
        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; OnPropertyChanged(); }
        }


        public ClientElementForm(Guid id = default)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            if (id != default)
            {
                var data = _contorller.GetHandbook<Client>(id);

                if (data != null)
                    _data = data;

                GroupName = _data.Parent?.Name;

                _title = _data.Name;
            }

            List<DataType> list = new()
            {
                new(){ Text = "Клієнт", Type = TypesClient.Client },
                new(){ Text = "Постачальник", Type = TypesClient.Provider }
            };

            InitializeComponent();
            TypeClient.ItemsSource = list;
            Title = _title;
        }

        public ClientElementForm(Client data)
        {
            DataContext = this;
            _contorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _data = data;
            _title = _data.Name;
            GroupName = _data.Parent?.Name;

            List<DataType> list = new()
            {
                new(){ Text = "Клієнт", Type = TypesClient.Client },
                new(){ Text = "Постачальник", Type = TypesClient.Provider }
            };

            InitializeComponent();

            TypeClient.ItemsSource = list;
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
            if (propertyName != "UnitName" && propertyName != "GroupName")
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
            if (_data.ParentId != null)
            {
                var form = new ClientGroupForm((Guid)_data.ParentId);
                form.ShowDialog();
                var data = _contorller.GetHandbook<Client>((Guid)_data.ParentId);
                GroupName = data?.Name;
            }
        }

        private void btnShowListGroup_Click(object sender, RoutedEventArgs e)
        {
            ClientGroupListForm form = new ClientGroupListForm();
            var result = form.ShowDialog();
            if (result != null)
            {
                if (form.SelectedId != default)
                {
                    Guid id = form.SelectedId;
                    var data = _contorller.GetHandbook<Client>(id);

                    if (data != null)
                    {
                        IsChange = true;
                        _data.ParentId = data.Id;
                        _data.Parent = data;
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

        private void TypeClient_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = TypeClient.SelectedItem as DataType;

            if (item == null) return;

            _data.TypeClient = item.Type;
        }
    }
}
