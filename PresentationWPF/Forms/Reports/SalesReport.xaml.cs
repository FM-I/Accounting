using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using PresentationWPF.Forms.Handbooks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static PresentationWPF.Forms.Reports.SalesReport;

namespace PresentationWPF.Forms.Reports
{
    public class SalesGroupsToTotalConverter : IValueConverter
    {
        private Currency? _currency;
        public SalesGroupsToTotalConverter()
        {
            _currency = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>().GetHandbook<Currency>(w => w.IsDefault);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var items = (ReadOnlyObservableCollection<Object>)value;
                double summa = 0;
                double q = 0;
                double price = 0;
                int count = 0;
                string unit = string.Empty;
                foreach (DataReport gi in items)
                {
                    summa += gi.Summa;
                    q += gi.Quantity;
                    price += gi.Price;
                    unit = gi.Unit;
                    if(summa > 0)
                        ++count;
                }

                return $"кількість: {q} {unit}   ціна: {Math.Round(price / (count > 0 ? count : 1), 2)}   сума: {summa} {_currency?.Name}";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public partial class SalesReport : Window, INotifyPropertyChanged
    {
        public class DataReport
        {
            public Guid ClientId { get; set; }
            public Guid NomenclatureId { get; set; }
            public string Unit { get; set; }
            public string Client { get; set; }
            public string Nomenclature { get; set; }
            public string Currency { get; set; }
            public double Price { get; set; }
            public double Summa { get => Price * Quantity; }
            public double Quantity { get; set; }
        }

        private DateTime? _dateStart;
        public DateTime? DateStart { get { return _dateStart; } set { _dateStart = value; OnPropertyChanged(); } }

        private DateTime? _dateEnd;
        public DateTime? DateEnd { get { return _dateEnd; } set { _dateEnd = value; OnPropertyChanged(); } }


        private string? _clientName;
        public string? ClientName
        {
            get { return _clientName; }
            set { _clientName = value; OnPropertyChanged(); }
        }

        private string? _nomenclatureName;
        public string? NomenclatureName
        {
            get { return _nomenclatureName; }
            set { _nomenclatureName = value; OnPropertyChanged(); }
        }

        private Currency? _currency;

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IAccumulationRegisterController _accumulationRegisterController;
        private readonly IHandbookController _handbookController;
        private Guid? _clientId;
        private Guid? _nomenclatureId;

        public ObservableCollection<DataReport> Data { get; set; } = new();

        public SalesReport()
        {
            _accumulationRegisterController = DIContainer.ServiceProvider.GetRequiredService<IAccumulationRegisterController>();
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            _currency = _handbookController.GetHandbook<Currency>(w => w.IsDefault);

            DataContext = this;
            InitializeComponent();

            var view = (CollectionView)CollectionViewSource.GetDefaultView(report.ItemsSource);
            view.Filter = Filter;
            view.GroupDescriptions.Add(new PropertyGroupDescription("Nomenclature"));

            //report.Columns.Add(new DataGridTextColumn() { Header = "Номенклатура", Binding = new Binding("Nomenclature") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Контрагента", Binding = new Binding("Client") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Кількість", Binding = new Binding("Quantity") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Ціна", Binding = new Binding("Price") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Сумма", Binding = new Binding("Summa") });
            //report.Columns.Add(new DataGridTextColumn() { Header = "Валюта", Binding = new Binding("Currency") });

        }

        public bool Filter(object item)
        {
            DataReport data = item as DataReport;

            bool selectWarehouse = _clientId == default || data?.ClientId == _clientId;
            bool selectNomenclature = _nomenclatureId == default || data?.NomenclatureId == _nomenclatureId;

            return selectWarehouse && selectNomenclature;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        private void createReport_Click(object sender, RoutedEventArgs e)
        {
            Data.Clear();

            var moveList = _accumulationRegisterController.GetListData<Sale>(s =>
                s.Date >= (DateStart == default ? DateTime.MinValue : DateStart)
                && s.Date <= (DateEnd == default ? DateTime.Now : DateEnd));

            var groupData = moveList.GroupBy(g => new { g.Nomenclature, g.Client }).Select(s => new
            {
                Nomenclature = s.Key.Nomenclature,
                Client = s.Key.Client,
                Quantity = s.Sum(selector => selector.Quantity),
                Summa = s.Sum(selector => selector.Quantity * (double)selector.Price)
            }).ToList();

            foreach (var item in moveList)
            {
                Data.Add(new()
                {
                    Nomenclature = item.Nomenclature.Name,
                    Unit = _handbookController.GetHandbook<Unit>((Guid)item.Nomenclature.BaseUnitId)?.Name,
                    Client = item.Client.Name,
                    Price = (double)item.Price,
                    Quantity = item.Quantity,
                    ClientId = item.Client.Id,
                    Currency = _currency?.Name,
                    NomenclatureId = item.NomenclatureId
                });
            }
        }

        private void btnClearClient_Click(object sender, RoutedEventArgs e)
        {
            _clientId = null;
            ClientName = "";
            CollectionViewSource.GetDefaultView(report.ItemsSource).Refresh();
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
                        _clientId = form.SelectedId;
                        ClientName = data.Name;
                        CollectionViewSource.GetDefaultView(report.ItemsSource).Refresh();
                    }
                }
            }

        }

        private void btnOpenClient_Click(object sender, RoutedEventArgs e)
        {
            if (_clientId == null)
                return;

            var form = new ClientElementForm((Guid)_clientId);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Client>((Guid)_clientId);
                if (data != null)
                    ClientName = data.Name;
            }
        }

        private void btnClearNomenclature_Click(object sender, RoutedEventArgs e)
        {
            _nomenclatureId = null;
            NomenclatureName = "";
            CollectionViewSource.GetDefaultView(report.ItemsSource).Refresh();
        }

        private void btnShowListNomenclature_Click(object sender, RoutedEventArgs e)
        {
            var form = new NomenclatureListForm(true);
            if (form.ShowDialog() != null)
            {
                if (form.SelectedId != default)
                {
                    var data = _handbookController.GetHandbook<Nomenclature>(form.SelectedId);
                    if (data != null)
                    {
                        _nomenclatureId = form.SelectedId;
                        NomenclatureName = data.Name;
                        CollectionViewSource.GetDefaultView(report.ItemsSource).Refresh();
                    }
                }
            }

        }

        private void btnOpenNomenclature_Click(object sender, RoutedEventArgs e)
        {
            if (_nomenclatureId == null)
                return;

            var form = new NomenclatureElementForm((Guid)_nomenclatureId);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Nomenclature>((Guid)_nomenclatureId);
                if (data != null)
                    NomenclatureName = data.Name;
            }
        }
    }
}
