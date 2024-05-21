using BL.Interfaces;
using Domain.Entity.Documents;
using Domain.Entity.Handbooks;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace PresentationWPF.Forms.Documents
{
    public partial class ClientOrderElementForm : Window, INotifyPropertyChanged
    {
        public class ItemProduct : INotifyPropertyChanged
        {
            private double? _quantity = null;
            private double? _price = null;
            private string _nomenclatureName = string.Empty;
            private string _unitName = string.Empty;

            public Guid Id { get; set; }
            public Guid NomenclatureId { get; set; }
            public Guid UnitId { get; set; }
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

            public double? Quantity 
            { 
                get { return _quantity; } 
                set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(Summa)); } 
            }
            
            public double? Price 
            {
                get { return _price; } 
                set { _price = value; OnPropertyChanged(); OnPropertyChanged(nameof(Summa)); } 
            }

            public double? Summa 
            { 
                get { return _price != null && _quantity != null ? _price * _quantity : null; }
                set { OnPropertyChanged(); }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this ,new(propertyName));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<ItemProduct> _products = new();
        
        public ObservableCollection<ItemProduct> Products { get { return _products; } set { _products = value; } }
        
        private readonly IHandbookController _handbookController;
        private readonly IDocumentController _documentController;

        private ClientOrder _data = new();

        public string? OrganizationName 
        { 
            get { return _data.Organization?.Name; }
            set { OnPropertyChanged(); }
        }

        public ClientOrderElementForm(Guid id = default)
        {
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _documentController = DIContainer.ServiceProvider.GetRequiredService<IDocumentController>();

            if(id != default)
            {
                var data = _documentController.GetDocument<ClientOrder>(id);
                if(data != null)
                {
                    _data = data;
                }
            }

            InitializeComponent();
            DataContext = this;
        }

        public ClientOrderElementForm(ClientOrder data) 
        {
            InitializeComponent();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        private void AddItemProduct_Click(object sender, RoutedEventArgs e)
        {
            Products.Add(new ItemProduct());
        }

        private void RemoveItemProduct_Click(object sender, RoutedEventArgs e)
        {
            if(ProductsTable.SelectedItem != null)
                Products.Remove((ItemProduct)ProductsTable.SelectedItem);
        }

        private void SearchItemProduct_Click(object sender, RoutedEventArgs e)
        {
            var form = new BarcodeInput();
            var res = form.ShowDialog();

            if(res != null)
            {
                if(form.Nomenclature != null)
                {
                    var item = Products.FirstOrDefault(w => w.NomenclatureId == form.Nomenclature.Id && w.UnitId == form.Unit.Id);

                    if(item != null)
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
            var item = ProductsTable.SelectedItem as ItemProduct;

            if (item == null)
            {
                ProductsTable.SelectedItem = ((Button)sender).DataContext;
                item = ProductsTable.SelectedItem as ItemProduct;
            }

            if (item == null)
                return;

            var form = new NomenclatureListForm(true);
            if(form.ShowDialog() != null)
            {
                if(form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Nomenclature>(form.SelectedId);
                    if (data != null)
                    {
                        item.NomenclatureName = data.Name;
                        item.NomenclatureId = data.Id;
                        item.UnitName = data.BaseUnit?.Name;
                        item.UnitId = (Guid)data?.BaseUnitId;
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
            
            if(form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Nomenclature>(item.NomenclatureId);
                if(data != null)
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
                if(form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Organization>(form.SelectedId);
                    if(data != null)
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
    }

}
