using BL.Controllers;
using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static PresentationWPF.Forms.Reports.LeftoverReportForm;

namespace PresentationWPF.Forms.Reports
{
    public class LeftoverGroupsToTotalConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var items = (ReadOnlyObservableCollection<Object>)value;
                double total = 0;
                foreach (DataReport gi in items)
                {
                    total += gi.Quantity;
                }
                return "загалом: " + total.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public partial class LeftoverReportForm : Window, INotifyPropertyChanged
    {
        public class DataReport
        {
            public Guid NomenclatureId { get; set; }
            public Guid WarehouseId { get; set; }
            public string Warehouse { get; set; }
            public string Nomenclature { get; set; }
            public string Unit { get; set; }
            public double Quantity { get; set; }
        }

        private DateTime? _date;
        public DateTime? Date { get { return _date; } set { _date = value; OnPropertyChanged(); } }

        private string? _warehouseName;

        public string? WarehouseName
        {
            get { return _warehouseName; }
            set { _warehouseName = value; OnPropertyChanged(); }
        }

        private string? _nomenclatureName;

        public string? NomenclatureName
        {
            get { return _nomenclatureName; }
            set { _nomenclatureName = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IAccumulationRegisterController _accumulationRegisterController;
        private readonly IHandbookController _handbookController;
        private Guid? _nomenclatureId;
        private Guid? _warehouseId;

        public ObservableCollection<DataReport> Data { get; set; } = new();

        public LeftoverReportForm()
        {
            _accumulationRegisterController = DIContainer.ServiceProvider.GetRequiredService<IAccumulationRegisterController>();
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            DataContext = this;
            InitializeComponent();

            var view = (CollectionView)CollectionViewSource.GetDefaultView(report.ItemsSource);
            view.Filter = Filter;
            view.GroupDescriptions.Add(new PropertyGroupDescription("Warehouse"));

            report.Columns.Add(new DataGridTextColumn() { Header = "Склад", Binding = new Binding("Warehouse")});
            report.Columns.Add(new DataGridTextColumn() { Header = "Номенклатура", Binding = new Binding("Nomenclature") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Кількість", Binding = new Binding("Quantity") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Од.", Binding = new Binding("Unit") });

        }


        public bool Filter(object item)
        {
            DataReport data = item as DataReport;

            bool selectWarehouse = (_warehouseId == default || data?.WarehouseId == _warehouseId);
            bool selectNomenclature = (_nomenclatureId == default || data?.NomenclatureId == _nomenclatureId);

            return selectWarehouse && selectNomenclature;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        private Func<Leftover, bool>? GetSelectionFunc()
        {
            Func<Leftover, bool>? func = f =>
            {
                return (Date == default || f.Date <= Date)
                && (_nomenclatureId == default || f.NomenclatureId == _nomenclatureId)
                && (_warehouseId == default || f.WarehouseId == _warehouseId);
            };

            return func;
        }

        private void createReport_Click(object sender, RoutedEventArgs e)
        {
            Data.Clear();
            var moveList = _accumulationRegisterController.GetListData<Leftover>(GetSelectionFunc());
            var leftover = _accumulationRegisterController.GetLeftoverList(moveList,
                g => new { g.Nomenclature, g.Warehouse },
                s => new
                {
                    Nomenclature = s.Key.Nomenclature,
                    Warehouse = s.Key.Warehouse,
                    Value = s.Sum(selector => selector.TypeMove == TypeAccumulationRegisterMove.INCOMING ? selector.Value : -selector.Value)
                });

            foreach (var item in leftover)
            {
                Data.Add(new()
                {
                    Nomenclature = item.Nomenclature.Name,
                    Unit = _handbookController.GetHandbook<Unit>((Guid)item.Nomenclature.BaseUnitId)?.Name,
                    Warehouse = item.Warehouse.Name,
                    Quantity = item.Value,
                    NomenclatureId = item.Nomenclature.Id,
                    WarehouseId = item.Warehouse.Id
                });
            }
        }

        private void btnClearWarehouse_Click(object sender, RoutedEventArgs e)
        {
            _warehouseId = null;
            WarehouseName = "";
            CollectionViewSource.GetDefaultView(report.ItemsSource).Refresh();
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
                        _warehouseId = form.SelectedId;
                        WarehouseName = data.Name;
                        CollectionViewSource.GetDefaultView(report.ItemsSource).Refresh();
                    }
                }
            }

        }

        private void btnOpenWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (_warehouseId == null)
                return;

            var form = new WarehouseElementForm((Guid)_warehouseId);
            if (form.ShowDialog() != null)
            {
                var data = _handbookController.GetHandbook<Warehouse>((Guid)_warehouseId);
                if (data != null)
                    WarehouseName = data.Name;
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
