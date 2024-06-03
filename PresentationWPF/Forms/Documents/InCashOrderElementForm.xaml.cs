using BL.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using PresentationWPF.Forms.Handbooks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace PresentationWPF.Forms.Documents
{
    public partial class InCashOrderElementForm : Window, INotifyPropertyChanged
    {
        public class OperationItem
        {
            public string Text { get; set; }
            public TypePayment Type { get; set; }
        }

        private bool _isChange;
        public bool IsChange
        {
            get { return _isChange; }
            set { _isChange = value; Title = _isChange ? Title += "*" : Title.TrimEnd('*'); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IHandbookController _handbookController;
        private readonly IDocumentController _documentController;
        private readonly IInformationRegisterController _infoController;
        private InCashOrder _data = new();

        private ObservableCollection<OperationItem> _operations = new()
        {
                new(){ Text = "Від покупця", Type = TypePayment.Client},
                new(){ Text = "Від постачальника", Type = TypePayment.Provider},
                new(){ Text = "Інші", Type = TypePayment.Other},
        };

        public ObservableCollection<OperationItem> Operations { get { return _operations; } set { _operations = value; } }

        public string? OrganizationName
        {
            get { return _data.Organization?.Name; }
            set { OnPropertyChanged(); }
        }

        public string? ClientName
        {
            get { return _data.Client?.Name; }
            set { OnPropertyChanged(); }
        }

        private string? _documentName;
        public string? DocumentBaseName
        {
            get { return _documentName;}
            set { _documentName = value; OnPropertyChanged(); }
        }

        public string? CurrencyName
        {
            get { return _data.Currency?.Name; }
            set { OnPropertyChanged(); }
        }

        public string? CashBoxName
        {
            get { return _data.CashBox?.Name; }
            set { OnPropertyChanged(); }
        }

        public double? CurrencyRate
        {
            get { return _data.CurrencyRate; }
            set { OnPropertyChanged(); }
        }

        private string _summa = string.Empty;
        public string Summa
        {
            get { return _summa; }
            set { _summa = value; OnPropertyChanged(); }
        }

        public string? Number
        {
            get { return _data?.Number; }
            set { OnPropertyChanged(); }
        }

        public DateTime Date
        {
            get { return _data.Date; }
            set { _data.Date = value; OnPropertyChanged(); }
        }

        public InCashOrderElementForm(Guid id = default)
        {
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _documentController = DIContainer.ServiceProvider.GetRequiredService<IDocumentController>();
            _infoController = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();

            if (id != default)
            {
                var data = _documentController.GetDocument<InCashOrder>(id);
                if (data != null)
                {
                    _data = data;
                    Summa = _data.Summa.ToString();
                }
            }
            InitializeComponent();

            if (_data.Operation == TypePayment.Client)
                DocumentBaseName = _data.SaleInvoice?.ToString();
            else if(_data.Operation == TypePayment.Provider)
                DocumentBaseName = _data.PurchaceInvoice?.ToString();
            else
            {
                documentBaseLabel.Visibility = Visibility.Hidden;
                documentBaseInput.Visibility = Visibility.Hidden;
            }

            DataContext = this;
            UnConducted.IsEnabled = _data.Conducted;
            TypeOperation.SelectedItem = Operations.First(w => w.Type == _data.Operation);
            IsChange = false;
        }

        public InCashOrderElementForm(InCashOrder data)
        {
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _documentController = DIContainer.ServiceProvider.GetRequiredService<IDocumentController>();
            _infoController = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();

            _data = data;
            Summa = _data.Summa.ToString();
            
            InitializeComponent();

            if (_data.Operation == TypePayment.Client)
                DocumentBaseName = _data.SaleInvoice?.ToString();
            else if (_data.Operation == TypePayment.Provider)
                DocumentBaseName = _data.PurchaceInvoice?.ToString();
            else
            {
                documentBaseLabel.Visibility = Visibility.Hidden;
                documentBaseInput.Visibility = Visibility.Hidden;
            }

            DataContext = this;
            UnConducted.IsEnabled = _data.Conducted;
            TypeOperation.SelectedItem = Operations.First(w => w.Type == _data.Operation);
            IsChange = false;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
            if (!_isChange)
                IsChange = true;
        }

        private void btnClearOrganization_Click(object sender, RoutedEventArgs e)
        {
            _data.Organization = null;
            OrganizationName = "";
        }

        private void btnShowListOrganization_Click(object sender, RoutedEventArgs e)
        {
            var form = new OrganizationListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Organization>(form.SelectedId);
                    if (data != null)
                    {
                        _data.Organization = data;
                        OrganizationName = data.Name;
                    }
                }
            }
        }

        private void btnOpenOrganization_Click(object sender, RoutedEventArgs e)
        {
            if (_data.Organization == null)
                return;

            var form = new OrganizationElementForm(_data.Organization.Id);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Organization>(_data.Organization.Id);
                if (data != null)
                {
                    _data.Organization = data;
                    OrganizationName = data.Name;
                }
            }
        }

        private void btnClearClient_Click(object sender, RoutedEventArgs e)
        {
            _data.Client = null;
            ClientName = "";
        }

        private void btnShowListClient_Click(object sender, RoutedEventArgs e)
        {
            var form = new ClientListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Client>(form.SelectedId);
                    if (data != null)
                    {
                        _data.Client = data;
                        ClientName = data.Name;
                    }
                }
            }
        }

        private void btnOpenClient_Click(object sender, RoutedEventArgs e)
        {
            if (_data.Client == null)
                return;

            var form = new ClientElementForm(_data.Client.Id);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Client>(_data.Client.Id);
                if (data != null)
                {
                    _data.Client = data;
                    ClientName = data.Name;
                }
            }
        }


        private void btnClearCashBox_Click(object sender, RoutedEventArgs e)
        {
            _data.CashBox = null;
            CashBoxName = "";
        }

        private void btnShowListCashBox_Click(object sender, RoutedEventArgs e)
        {
            var form = new CashBoxListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<CashBox>(form.SelectedId);
                    if (data != null)
                    {
                        _data.CashBox = data;
                        CashBoxName = data.Name;
                    }
                }
            }
        }

        private void btnOpenCashBox_Click(object sender, RoutedEventArgs e)
        {
            if (_data.CashBox == null)
                return;

            var form = new CashBoxElementForm(_data.CashBox.Id);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<CashBox>(_data.CashBox.Id);
                if (data != null)
                {
                    _data.CashBox = data;
                    CashBoxName = data.Name;
                }
            }
        }

        private void btnClearDocumentBase_Click(object sender, RoutedEventArgs e)
        {
            if (_data.Operation == TypePayment.Client)
            {
                _data.SaleInvoice = null;
                _data.SaleInvoiceId = default;
            }
            else if (_data.Operation == TypePayment.Provider)
            {
                _data.PurchaceInvoice = null;
                _data.PurchaceInvoiceId = default;
            }

            DocumentBaseName = "";
        }

        private void btnShowListDocumentBase_Click(object sender, RoutedEventArgs e)
        {
            if (_data.Operation == TypePayment.Client)
            {
                var form = new SalesInvoiceListForm(true);
                if (form.ShowDialog() != null)
                {
                    if (form.SelectedId != default && form.SelectedId != _data.SaleInvoice?.Id)
                    {
                        var data = _documentController.GetDocument<SaleInvoice>(form.SelectedId);
                        if (data != null)
                        {
                            _data.SaleInvoiceId = data.Id;

                            if (MessageBox.Show("Перезаповнити дані докумена?", "Питання", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                _data.FillWith(data);
                                Summa = _data.Summa.ToString();
                                
                                foreach (var item in GetType().GetProperties())
                                {
                                    OnPropertyChanged(item.Name);
                                }
                            }

                            DocumentBaseName = data.ToString();
                        }
                    }
                }

            }
            else if (_data.Operation == TypePayment.Provider)
            {
                var form = new PurchaceInvoiceListForm(true);
                if (form.ShowDialog() != null)
                {
                    if (form.SelectedId != default)
                    {
                        var data = _documentController.GetDocument<PurchaceInvoice>(form.SelectedId);
                        if (data != null)
                        {
                            _data.PurchaceInvoiceId = data.Id;

                            if (MessageBox.Show("Перезаповнити дані докумена?", "Питання", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                _data.FillWith(data);
                                Summa = _data.Summa.ToString();

                                foreach (var item in GetType().GetProperties())
                                {
                                    OnPropertyChanged(item.Name);
                                }
                            }

                            DocumentBaseName = data.ToString();
                        }
                    }
                }
            }
        }

        private void btnOpenDocumentBase_Click(object sender, RoutedEventArgs e)
        {
            if (_data.Operation == TypePayment.Client && _data.SaleInvoiceId != default && _data.SaleInvoiceId != null)
            {
                var form = new SalesInvoiceElementForm((Guid)_data.SaleInvoiceId);
                if (form.ShowDialog() != null)
                {
                    var data = _documentController.GetDocument<SaleInvoice>((Guid)_data.SaleInvoiceId);
                    if (data != null)
                    {
                        DocumentBaseName = data.ToString();
                    }
                }
            }
            else if (_data.Operation == TypePayment.Provider && _data.PurchaceInvoiceId != default && _data.PurchaceInvoiceId != null)
            {
                var form = new PurchaceInvoiceElementForm((Guid)_data.PurchaceInvoiceId);
                if (form.ShowDialog() != null)
                {
                    var data = _documentController.GetDocument<PurchaceInvoice>((Guid)_data.PurchaceInvoiceId);
                    if (data != null)
                    {
                        DocumentBaseName = data.ToString();
                    }
                }
            }
        }

        private void btnClearCurrency_Click(object sender, RoutedEventArgs e)
        {
            _data.Currency = null;
            _data.CurrencyRate = 1;
            CurrencyRate = 1;
            CurrencyName = "";
        }

        private void btnShowListCurrency_Click(object sender, RoutedEventArgs e)
        {
            var form = new CurrencyListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Currency>(form.SelectedId);
                    if (data != null)
                    {
                        var rates = _infoController.GetLastDateData<ExchangesRate>(selectionFunc: s => s.CurrencyId == form.SelectedId);

                        double rate = 1;

                        if (rates != null && rates.Count != 0)
                        {
                            rate = rates.First().Rate;
                        }

                        _data.CurrencyRate = rate;
                        _data.Currency = data;
                        CurrencyName = data.Name;
                        CurrencyRate = rate;
                    }
                }
            }
        }

        private void btnOpenCurrency_Click(object sender, RoutedEventArgs e)
        {
            if (_data.Currency == null)
                return;

            var form = new CurrencyElementForm(_data.Currency.Id);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Currency>(_data.Currency.Id);
                if (data != null)
                {
                    _data.Currency = data;
                    CurrencyName = data.Name;
                }
            }

        }

        private void TypeOperation_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = TypeOperation.SelectedItem as OperationItem;

            if (item == null || _data.Operation == item.Type) return;

            _data.Operation = item.Type;
            documentBaseInput.Visibility = Visibility.Visible;
            documentBaseLabel.Visibility = Visibility.Visible;

            if (_data.Operation == TypePayment.Provider)
                DocumentBaseName = _data.PurchaceInvoice?.ToString();
            else if (_data.Operation == TypePayment.Client)
                DocumentBaseName = _data.SaleInvoice?.ToString();
            else
            {
                DocumentBaseName = "";
                documentBaseLabel.Visibility = Visibility.Hidden;
                documentBaseInput.Visibility = Visibility.Hidden;
            }

            if(!IsChange)
                IsChange = true;
        }

        private void SummaInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == ",")
            {
                var res = _summa.FirstOrDefault(w => w.Equals(','));
                if (res != default)
                {
                    e.Handled = true;
                    return;
                }
            }

            Regex regex = new Regex(@"^[0-9,]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private bool CheckDataComplection()
        {
            var result = _data.CheckDataComplection();

            if (!result.Success)
            {
                var messageText = "Поля " + string.Join(", ", result.Properties) + " незаповнені!!!";
                MessageBox.Show(messageText, "Помилка");
                return false;
            }

            return true;
        }

        private async void WrideDocument(TypeWriteDocument typeWrite)
        {
            decimal res = 0;

            if (!Regex.IsMatch(_summa, @"^[0-9]*\,?[0-9]+$")
                || !decimal.TryParse(_summa, out res))
            {
                MessageBox.Show("Значення суми вказано не вірно!!!", "Помилка");
                return;
            };

            _data.Summa = res;

            if (CheckDataComplection())
            {
                if (_data.Date == default)
                    _data.Date = DateTime.Now;

                _data.SaleInvoice = null;
                _data.PurchaceInvoice = null;

                if(_data.Operation == TypePayment.Other)
                {
                    _data.SaleInvoiceId = default;
                    _data.PurchaceInvoiceId = default;
                }

                switch (typeWrite)
                {
                    case TypeWriteDocument.Write:
                        await _documentController.AddOrUpdateAsync(_data);
                        break;
                    case TypeWriteDocument.Conducted:

                        if (_data.DeleteMark)
                        {
                            MessageBox.Show("Документ помічено на видалення! Проведення неможливе.", "Помилка");
                            return;
                        }

                        var result = await _documentController.ConductedDoumentAsync(_data);
                        if (!result.IsSuccess)
                        {
                            var messageText = string.Join("\n", result.Messages);
                            MessageBox.Show(messageText, "Помилка");
                            return;
                        }
                        UnConducted.IsEnabled = true;
                        break;
                    case TypeWriteDocument.UnConducted:
                        await _documentController.UnConductedDoumentAsync(_data);
                        UnConducted.IsEnabled = false;
                        break;
                }

                OnPropertyChanged(nameof(Number));
                OnPropertyChanged(nameof(Date));
                IsChange = false;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!_data.Conducted)
                WrideDocument(TypeWriteDocument.Write);
            else
                WrideDocument(TypeWriteDocument.Conducted);
        }

        private void Conducted_Click(object sender, RoutedEventArgs e)
        {
            WrideDocument(TypeWriteDocument.Conducted);
        }

        private void UnConducted_Click(object sender, RoutedEventArgs e)
        {
            WrideDocument(TypeWriteDocument.UnConducted);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsChange
                && MessageBox.Show("Дані було змінено. Збергти зміни?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Save_Click(null, null);
                e.Cancel = IsChange;
            }
        }
    }
}
