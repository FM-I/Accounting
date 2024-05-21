using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using PresentationWPF.Forms.Handbooks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PresentationWPF.Forms.Registers
{
    public partial class ClientContactElementForm : Window, INotifyPropertyChanged
    {
        private record DataKey(Guid ClientId, Guid ContactId);

        private readonly IInformationRegisterController _controller;
        private readonly IHandbookController _handbookController;
        private ClientContact _data = new();
        private DataKey _prevData;
        private bool _isChange;
        private bool _isNew = true;
        public event PropertyChangedEventHandler? PropertyChanged;
        public bool IsChange { get { return _isChange; } set { _isChange = value; if (_isChange) Title = Title + "*"; else Title = "Контакт контрагента"; } }

        private string _clientName;
        private string _contactName;
        public string ClientName
        {
            get { return _clientName; }
            set { _clientName = value; OnPropertyChanged(); }
        }

        public string ContactName
        {
            get { return _contactName; }
            set { _contactName = value; OnPropertyChanged(); }
        }

        public ClientContactElementForm(Guid clientId = default, Guid contactId = default, bool isCopy = false)
        {
            DataContext = this;
            _controller = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            if (clientId != default && contactId != default)
            {
                var data = _controller.GetListData<ClientContact>(selectionFunc: w => w.ContactId == contactId && w.ClientId == clientId);

                if (data != null && data.Count > 0)
                {
                    _data = data.First();
                    ClientName = _data.Client.Name;
                    ContactName = _data.Contact.Name;
                    _isNew = false;

                    _prevData = new(_data.ClientId, _data.ContactId);
                }
            }

            if (isCopy)
                _isNew = isCopy;

            InitializeComponent();

            IsChange = false;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = _data.CheckDataComplection();

            if (result.Success)
            {
                string message = "";
                if (_isNew)
                {
                    var data = _controller.GetListData<ClientContact>(selectionFunc: w => w.ClientId == _data.ClientId && w.ContactId == _data.ContactId);

                    if (data != null && data.Count > 0)
                        message = "Запис з подібними ключами уже є в таблиці!!!";

                }
                else if (_prevData != null
                    && (_prevData.ClientId != _data.ClientId
                    || _prevData.ContactId != _data.ContactId))
                {
                    var data = _controller.GetListData<ClientContact>(selectionFunc: w => w.ClientId == _data.ClientId && w.ContactId == _data.ContactId);

                    if (data != null && data.Count > 0)
                    {
                        message = "Запис з подібними ключами уже є в таблиці!!!";
                    }
                    else
                    {
                        _isNew = true;
                        await _controller.DeleteAsync<ClientContact>(w => w.ClientId == _prevData.ClientId && w.ContactId == _prevData.ContactId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show(message, "Помилка");
                    return;
                }

                _data.Client = null;
                _data.Contact = null;
                await _controller.AddOrUpdateAsync(_data, _isNew);
                IsChange = false;
                _isNew = false;
                _prevData = new(_data.ClientId, _data.ContactId);
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
                    case "btnOpenClient":

                        if (_data.ClientId == default)
                            return;

                        form = new ClientElementForm(_data.ClientId);

                        break;

                    case "btnOpenContact":

                        if (_data.ContactId == default)
                            return;

                        form = new ContactElementForm(_data.ContactId);

                        break;
                    default: return;
                }

                var res = form.ShowDialog();

                if (res != null)
                {
                    if (button.Name == "btnOpenClient")
                    {
                        var data = _handbookController.GetHandbook<Client>(_data.ClientId);
                        if (data != null)
                            ClientName = data.Name;
                    }
                    else
                    {
                        var data = _handbookController.GetHandbook<Contact>(_data.ContactId);
                        if (data != null)
                            ContactName = data.Name;
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
                    case "btnShowListClient":

                        form = new ClientListForm(true);

                        break;

                    case "btnShowListContact":

                        form = new ContactListForm(true);

                        break;
                    default: return;
                }

                var res = form.ShowDialog();

                if (res != null)
                {
                    Guid id = Guid.Empty;
                    if (button.Name == "btnShowListClient")
                    {
                        id = ((ClientListForm)form).SelectedId;
                        var data = _handbookController.GetHandbook<Client>(id);
                        if (data != null)
                        {
                            _data.ClientId = id;
                            ClientName = data.Name;

                            if (!IsChange)
                                IsChange = true;
                        }
                    }
                    else
                    {
                        id = ((ContactListForm)form).SelectedId;
                        var data = _handbookController.GetHandbook<Contact>(id);

                        if (data != null)
                        {
                            _data.ContactId = id;
                            ContactName = data.Name;
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

                        ClientName = "";
                        _data.Client = null;
                        _data.ClientId = Guid.Empty;

                        break;

                    case "btnClearContact":

                        ContactName = "";
                        _data.Contact = null;
                        _data.ContactId = Guid.Empty;

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
