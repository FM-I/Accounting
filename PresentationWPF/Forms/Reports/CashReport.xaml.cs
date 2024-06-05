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
using static PresentationWPF.Forms.Reports.CashReport;

namespace PresentationWPF.Forms.Reports
{
    public class CashGroupsToTotalConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var items = (ReadOnlyObservableCollection<Object>)value;
                decimal total = 0;
                string currency = string.Empty;
                foreach (DataReport gi in items)
                {
                    currency = gi.Currency;
                    total += gi.Value;
                }
                return $"   залишок: " + total.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public partial class CashReport : Window, INotifyPropertyChanged
    {
        public class DataReport
        {
            public Guid ObjectId { get; set; }
            public string Object { get; set; }
            public string Currency { get; set; }
            public decimal Value { get; set; }
        }

        private DateTime? _date;
        public DateTime? Date { get { return _date; } set { _date = value; OnPropertyChanged(); } }

        private string? _object;

        public string? ObjectName
        {
            get { return _object; }
            set { _object = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IAccumulationRegisterController _accumulationRegisterController;
        private readonly IHandbookController _handbookController;
        private Guid? _objectId;
        private Type? _typeObject;

        public ObservableCollection<DataReport> Data { get; set; } = new();

        public CashReport()
        {
            _accumulationRegisterController = DIContainer.ServiceProvider.GetRequiredService<IAccumulationRegisterController>();
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            DataContext = this;
            InitializeComponent();

            var view = (CollectionView)CollectionViewSource.GetDefaultView(report.ItemsSource);
            view.Filter = Filter;
            view.GroupDescriptions.Add(new PropertyGroupDescription("Currency"));

            //report.Columns.Add(new DataGridTextColumn() { Header = "Склад", Binding = new Binding("Warehouse")});
            report.Columns.Add(new DataGridTextColumn() { Header = "Рахунок/каса", Binding = new Binding("Object") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Залишок", Binding = new Binding("Value") });

        }


        public bool Filter(object item)
        {
            DataReport data = item as DataReport;
            bool selectObject = (_objectId == default || data?.ObjectId== _objectId);
            return selectObject;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        private Func<CashInCashBox, bool>? GetCashBoxSelectionFunc()
        {
            Func<CashInCashBox, bool>? func = f =>
            {
                return (Date == default || f.Date <= Date)
                && (_objectId == default || f.CashBoxId == _objectId);
            };

            return func;
        }

        private Func<CashInBankAccount, bool>? GetBankAccountSelectionFunc()
        {
            Func<CashInBankAccount, bool>? func = f =>
            {
                return (Date == default || f.Date <= Date)
                && (_objectId == default || f.BankAccountId == _objectId);
            };

            return func;
        }

        private void createReport_Click(object sender, RoutedEventArgs e)
        {
            Data.Clear();
            var moveList = _accumulationRegisterController.GetListData(GetCashBoxSelectionFunc());

            var leftover = _accumulationRegisterController.GetLeftoverList(moveList,
                g => new { g.CurrencyId, g.CashBoxId },
                s => new
                {
                    Key = s.Key,
                    Value = s.Sum(selector => selector.TypeMove == TypeAccumulationRegisterMove.INCOMING ? selector.Summa : -selector.Summa)
                });

            foreach (var item in leftover)
            {
                Data.Add(new()
                {
                    Currency = _handbookController.GetHandbook<Currency>((Guid)item.Key.CurrencyId)?.Name,
                    Object = _handbookController.GetHandbook<CashBox>((Guid)item.Key.CashBoxId)?.Name,
                    ObjectId = item.Key.CashBoxId,
                    Value = item.Value
                });
            }

            var moveListBank = _accumulationRegisterController.GetListData(GetBankAccountSelectionFunc());

            var leftoverBank = _accumulationRegisterController.GetLeftoverList(moveListBank,
                g => new { g.CurrencyId, g.BankAccountId },
                s => new
                {
                    Key = s.Key,
                    Value = s.Sum(selector => selector.TypeMove == TypeAccumulationRegisterMove.INCOMING ? selector.Summa : -selector.Summa)
                });

            foreach (var item in leftoverBank)
            {
                Data.Add(new()
                {
                    Currency = _handbookController.GetHandbook<Currency>(item.Key.CurrencyId)?.Name,
                    Object = _handbookController.GetHandbook<BankAccount>(item.Key.BankAccountId)?.Name,
                    ObjectId = item.Key.BankAccountId,
                    Value = item.Value
                });
            }
        }

        private void btnClearObject_Click(object sender, RoutedEventArgs e)
        {
            _objectId = null;
            _typeObject = null;
            ObjectName = "";
            CollectionViewSource.GetDefaultView(report.ItemsSource).Refresh();
        }

        private void btnShowListObject_Click(object sender, RoutedEventArgs e)
        {
            var selectTypeForm = new SelectTypeForm();

            if(selectTypeForm.ShowDialog() == null) return;

            _typeObject = selectTypeForm.SelectedType;

            Window? form = _typeObject?.Name switch
            {
                nameof(CashBox) => new CashBoxListForm(true),
                nameof(BankAccount) => new BankAccountListForm(true),
                _ => null
            };

            if (form == null)
                return;

            form.ShowDialog();

            var property = form.GetType().GetProperty("SelectedId");

            if (property == null)
                return;

            var value = property.GetValue(form);

            if (value == null)
                return;

            Guid selctedId = (Guid)value;


            if (selctedId != default)
            {

                var method = _handbookController.GetType().GetMethod("GetHandbook", [typeof(Guid)]).MakeGenericMethod(_typeObject);

                var data = method.Invoke(_handbookController, [selctedId]);

                if (data != null)
                {
                    _objectId = selctedId;
                    ObjectName = (string)data.GetType().GetProperty("Name").GetValue(data);
                    CollectionViewSource.GetDefaultView(report.ItemsSource).Refresh();
                }
            }

        }

        private void btnOpenObject_Click(object sender, RoutedEventArgs e)
        {
            if (_objectId == null)
                return;

            Window? form = _typeObject?.Name switch
            {
                nameof(CashBox) => new CashBoxElementForm((Guid)_objectId),
                nameof(BankAccount) => new BankAccountElementForm((Guid)_objectId),
                _ => null
            };

            if (form == null)
                return;

            form.ShowDialog();

            var method = _handbookController.GetType().GetMethod("GetHandbook", [typeof(Guid)]).MakeGenericMethod(_typeObject);

            var data = method.Invoke(_handbookController, [_objectId]);

            if (data != null)
                ObjectName = (string)data.GetType().GetProperty("Name").GetValue(data);
        }
    }
}
