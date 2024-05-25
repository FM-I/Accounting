using BL.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.DocumentTables;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using PresentationWPF.Forms.Handbooks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PresentationWPF.Forms.Documents
{
    public partial class PurchaceInvoiceElementForm : Window, INotifyPropertyChanged
    {

        private bool _isChange;
        public bool IsChange
        {
            get { return _isChange; }
            set { _isChange = value; Title = _isChange ? Title += "*" : Title.TrimEnd('*'); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<ItemProduct> _products = new();

        public ObservableCollection<ItemProduct> Products { get { return _products; } set { _products = value; } }

        private readonly IHandbookController _handbookController;
        private readonly IDocumentController _documentController;
        private readonly IInformationRegisterController _infoController;
        private List<Guid> _removedItemProducts = new();
        private PurchaceInvoice _data = new();

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

        public string? WarehouseName
        {
            get { return _data.Warehouse?.Name; }
            set { OnPropertyChanged(); }
        }

        public string? CurrencyName
        {
            get { return _data.Currency?.Name; }
            set { OnPropertyChanged(); }
        }

        public double? CurrencyRate
        {
            get { return _data.CurrencyRate; }
            set { OnPropertyChanged(); }
        }

        public string? TypePriceName
        {
            get { return _data.TypePrice?.Name; }
            set { OnPropertyChanged(); }
        }

        public string? Number
        {
            get { return _data?.Number; }
            set { OnPropertyChanged(); }
        }

        public string? OrderName
        {
            get { return _data.ProviderOrder != null ? $"Замовлення постачальника {_data.ProviderOrder.Number} від {_data.ProviderOrder.Date}" : null; }
            set { OnPropertyChanged(); }
        }

        public DateTime Date
        {
            get { return _data.Date; }
            set { _data.Date = value; OnPropertyChanged(); }
        }

        public PurchaceInvoiceElementForm(Guid id = default)
        {
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _documentController = DIContainer.ServiceProvider.GetRequiredService<IDocumentController>();
            _infoController = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();

            if (id != default)
            {
                var data = _documentController.GetDocument<PurchaceInvoice>(id);
                if (data != null)
                {
                    _data = data;

                    foreach (var item in _data.Products)
                    {
                        _products.Add(new()
                        {
                            Id = item.Id,
                            NomenclatureId = item.NomenclatureId,
                            UnitId = item.UnitId,
                            NomenclatureName = item.Nomenclature.Name,
                            UnitName = item.Unit.Name,
                            Quantity = item.Quantity,
                            Price = (double)item.Price,
                            Summa = (double)item.Summa
                        });

                        var product = _products.Last();
                        product.OnChange += ProductItem_OnChange;
                    }
                }
            }

            InitializeComponent();
            DataContext = this;
            UnConducted.IsEnabled = _data.Conducted;
        }

        public PurchaceInvoiceElementForm(PurchaceInvoice data)
        {
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _documentController = DIContainer.ServiceProvider.GetRequiredService<IDocumentController>();
            _infoController = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();

            _data = data;

            foreach (var item in _data.Products)
            {
                _products.Add(new()
                {
                    Id = item.Id,
                    NomenclatureId = item.NomenclatureId,
                    UnitId = item.UnitId,
                    NomenclatureName = item.Nomenclature.Name,
                    UnitName = item.Unit.Name,
                    Quantity = item.Quantity,
                    Price = (double)item.Price,
                    Summa = (double)item.Summa
                });

                var product = _products.Last();
                product.OnChange += ProductItem_OnChange;
            }

            InitializeComponent();
            DataContext = this;
            UnConducted.IsEnabled = _data.Conducted;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
            if (!_isChange)
                IsChange = true;
        }

        private void AddItemProduct_Click(object sender, RoutedEventArgs e)
        {
            ItemProduct item = new();
            item.OnChange += ProductItem_OnChange;
            Products.Add(item);
        }

        private void ProductItem_OnChange(object? sender, EventArgs e)
        {
            if (!_isChange)
                IsChange = true;
        }

        private void RemoveItemProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsTable.SelectedItem != null)
            {
                var item = (ItemProduct)ProductsTable.SelectedItem;
                _removedItemProducts.Add(item.Id);
                Products.Remove(item);
            }
        }

        private void SearchItemProduct_Click(object sender, RoutedEventArgs e)
        {
            var form = new BarcodeInput();
            var res = form.ShowDialog();

            if (res != null)
            {
                if (form.Nomenclature != null)
                {
                    var item = Products.FirstOrDefault(w => w.NomenclatureId == form.Nomenclature.Id && w.UnitId == form.Unit.Id);

                    if (item != null)
                    {
                        ++item.Quantity;
                    }
                    else
                    {
                        Products.Add(new ItemProduct()
                        {
                            NomenclatureId = form.Nomenclature.Id,
                            NomenclatureName = form.Nomenclature.Name,
                            UnitId = form.Unit.Id,
                            UnitName = form.Unit.Name,
                            Quantity = 1
                        });

                        item = Products.Last();

                        if (_data.TypePrice != null)
                        {
                            var prices = _infoController.GetLastDateData<Price>(selectionFunc: s => s.TypePriceId == _data.TypePrice.Id && s.NomenclatureId == item.NomenclatureId);

                            if (prices != null && prices.Count != 0)
                            {
                                var priceData = prices.FirstOrDefault();
                                item.Price = priceData != null ? (double)priceData.Value : 0;
                            }
                        }
                    }
                }
            }

        }

        private void btnClearNomenclature_Click(object sender, RoutedEventArgs e)
        {
            var item = ProductsTable.SelectedItem as ItemProduct;

            if (item == null)
            {
                ProductsTable.SelectedItem = ((Button)sender).DataContext;
                item = ProductsTable.SelectedItem as ItemProduct;
            }

            if (item == null)
                return;

            item.NomenclatureName = string.Empty;
            item.NomenclatureId = Guid.Empty;
        }

        private void btnShowListNomenclature_Click(object sender, RoutedEventArgs e)
        {
            ProductsTable.SelectedItem = ((Button)sender).DataContext;
            var item = ProductsTable.SelectedItem as ItemProduct;

            if (item == null)
                return;

            var form = new NomenclatureListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Nomenclature>(form.SelectedId);
                    if (data != null)
                    {
                        item.NomenclatureName = data.Name;
                        item.NomenclatureId = data.Id;
                        item.UnitName = data.BaseUnit?.Name;
                        item.UnitId = (Guid)data?.BaseUnitId;

                        if (_data.TypePrice != null && (item.Price == 0 || item.Price == null))
                        {
                            var prices = _infoController.GetLastDateData<Price>(selectionFunc: s => s.TypePriceId == _data.TypePrice.Id && s.NomenclatureId == data.Id);

                            if (prices != null && prices.Count != 0)
                            {
                                var priceData = prices.FirstOrDefault();
                                item.Price = priceData != null ? (double)priceData.Value : 0;
                            }
                        }
                    }
                }
            }
        }

        private void btnOpenNomenclature_Click(object sender, RoutedEventArgs e)
        {
            var item = ProductsTable.SelectedItem as ItemProduct;

            if (item == null)
            {
                ProductsTable.SelectedItem = ((Button)sender).DataContext;
                item = ProductsTable.SelectedItem as ItemProduct;
            }

            if (item == null || item.NomenclatureId == default)
                return;

            var form = new NomenclatureElementForm(item.NomenclatureId);

            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Nomenclature>(item.NomenclatureId);
                if (data != null)
                {
                    item.NomenclatureName = data.Name;
                }
            }
        }

        private void btnClearUnit_Click(object sender, RoutedEventArgs e)
        {
            var item = ProductsTable.SelectedItem as ItemProduct;

            if (item == null)
            {
                ProductsTable.SelectedItem = ((Button)sender).DataContext;
                item = ProductsTable.SelectedItem as ItemProduct;
            }

            if (item == null)
                return;

            item.UnitName = string.Empty;
            item.UnitId = Guid.Empty;
        }

        private void btnShowListUnit_Click(object sender, RoutedEventArgs e)
        {
            var item = ProductsTable.SelectedItem as ItemProduct;

            if (item == null)
            {
                ProductsTable.SelectedItem = ((Button)sender).DataContext;
                item = ProductsTable.SelectedItem as ItemProduct;
            }

            if (item == null)
                return;

            var form = new UnitListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Unit>(form.SelectedId);
                    if (data != null)
                    {
                        item.UnitName = data.Name;
                        item.UnitId = data.Id;
                    }
                }
            }
        }

        private void btnOpenUnit_Click(object sender, RoutedEventArgs e)
        {
            var item = ProductsTable.SelectedItem as ItemProduct;

            if (item == null)
            {
                ProductsTable.SelectedItem = ((Button)sender).DataContext;
                item = ProductsTable.SelectedItem as ItemProduct;
            }

            if (item == null || item.UnitId == default)
                return;

            var form = new UnitElementForm(item.UnitId);

            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Unit>(item.UnitId);
                if (data != null)
                {
                    item.UnitName = data.Name;
                }
            }
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

        private void btnClearWarehouse_Click(object sender, RoutedEventArgs e)
        {
            _data.Warehouse = null;
            WarehouseName = "";
        }

        private void btnShowListWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var form = new WarehouseListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Warehouse>(form.SelectedId);
                    if (data != null)
                    {
                        _data.Warehouse = data;
                        WarehouseName = data.Name;
                    }
                }
            }

        }

        private void btnOpenWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (_data.Warehouse == null)
                return;

            var form = new WarehouseElementForm(_data.Warehouse.Id);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Warehouse>(_data.Warehouse.Id);
                if (data != null)
                {
                    _data.Warehouse = data;
                    WarehouseName = data.Name;
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

        private void btnClearypePrice_Click(object sender, RoutedEventArgs e)
        {
            _data.TypePrice = null;
            TypePriceName = "";
        }

        private void btnShowListypePrice_Click(object sender, RoutedEventArgs e)
        {
            var form = new TypePriceListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default && form.SelectedId != _data.TypePrice?.Id)
                {
                    var data = _handbookController.GetHandbook<TypePrice>(form.SelectedId);
                    if (data != null)
                    {
                        _data.TypePrice = data;
                        TypePriceName = data.Name;

                        var res = MessageBox.Show("Тип ціни змінено, перерахувати ціни?", "Питання", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (res == MessageBoxResult.Yes)
                        {
                            var products = new List<Guid>();

                            foreach (var item in _products)
                            {
                                products.Add(item.NomenclatureId);
                            }

                            var prices = _infoController.GetLastDateData<Price>(selectionFunc: s => s.TypePriceId == data.Id && products.Contains(s.NomenclatureId));

                            if (prices == null)
                                prices = new();

                            foreach (var item in Products)
                            {
                                var priceData = prices.FirstOrDefault(w => w.NomenclatureId == item.NomenclatureId);
                                item.Price = priceData != null ? (double)priceData.Value : 0;
                            }
                        }
                    }
                }
            }
        }

        private void btnOpenTypePrice_Click(object sender, RoutedEventArgs e)
        {
            if (_data.TypePrice == null)
                return;

            var form = new TypePriceElementForm(_data.TypePrice.Id);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<TypePrice>(_data.TypePrice.Id);
                if (data != null)
                {
                    _data.TypePrice = data;
                    TypePriceName = data.Name;
                }
            }
        }

        private void btnClearOrder_Click(object sender, RoutedEventArgs e)
        {
            _data.ProviderOrder = null;
            OrderName = "";
        }

        private void btnShowListOrder_Click(object sender, RoutedEventArgs e)
        {
            var form = new ProviderOrderListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default && form.SelectedId != _data.ProviderOrder?.Id)
                {
                    var data = _documentController.GetDocument<ProviderOrder>(form.SelectedId);
                    if (data != null)
                    {
                        _data.ProviderOrder = data;
                        OrderName = "";

                        var res = MessageBox.Show("Перезаповнити товари відповідно до замовлення?", "Питання", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (res == MessageBoxResult.Yes)
                        {
                            _products.Clear();

                            var products = new List<Guid>();

                            foreach (var item in data.Products)
                            {
                                _products.Add(new()
                                {
                                    Id = item.Id,
                                    NomenclatureId = item.NomenclatureId,
                                    UnitId = item.UnitId,
                                    NomenclatureName = item.Nomenclature.Name,
                                    UnitName = item.Unit.Name,
                                    Quantity = item.Quantity,
                                    Price = (double)item.Price,
                                    Summa = (double)item.Summa
                                });

                                var product = _products.Last();
                                product.OnChange += ProductItem_OnChange;
                            }
                        }
                    }
                }
            }
        }

        private void btnOpenOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_data.ProviderOrder == null)
                return;

            var form = new ProviderOrderElementForm(_data.ProviderOrder.Id);
            if (form.ShowDialog() != null)
            {
                var data = _documentController.GetDocument<ProviderOrder>(_data.ProviderOrder.Id);
                if (data != null)
                {
                    _data.ProviderOrder = data;
                    OrderName = "";
                }
            }
        }


        private bool CheckDataComplection()
        {
            var message = "";
            int number = 1;
            foreach (var item in Products)
            {
                if (item.NomenclatureId == default)
                    message += "В рядку " + number + " не заповнена номенклатура\n";

                if (item.UnitId == default)
                    message += "В рядку " + number + " не заповнена од. виміру\n";

                if (item.Quantity == null || item.Quantity == 0)
                    message += "В рядку " + number + " не заповнена кількість\n";

                if (item.Price == null || item.Price == 0)
                    message += "В рядку " + number + " не заповнена ціна\n";

                ++number;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message);
                return false;
            }

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
            if (CheckDataComplection())
            {
                _data.Products.Clear();
                foreach (var item in Products)
                {
                    _data.Products.Add(new()
                    {
                        Id = item.Id,
                        Quantity = (double)item.Quantity,
                        Price = (decimal)item.Price,
                        Summa = (decimal)item.Summa,
                        NomenclatureId = item.NomenclatureId,
                        UnitId = item.UnitId
                    }); ;
                }

                if (_data.Date == default)
                    _data.Date = DateTime.Now;

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

                await _documentController.RemoveRangeAsync<PurchaceInvoiceProduct>(w => _removedItemProducts.Contains(w.Id));
                _removedItemProducts.Clear();

                int i = 0;
                foreach (var item in _data.Products)
                {
                    _products[i].Id = item.Id;
                    ++i;
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
