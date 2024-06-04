using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Accumulations;
using Domain.Enum;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using PresentationWPF.Forms.Handbooks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static PresentationWPF.Forms.Reports.ClientProviderDebtReportForm;

namespace PresentationWPF.Forms.Reports
{
    public class ClientProviderGroupsToTotalConverter : IValueConverter
    {
        private Currency? _currency;
        public ClientProviderGroupsToTotalConverter()
        {
            _currency = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>().GetHandbook<Currency>(w => w.IsDefault);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var items = (ReadOnlyObservableCollection<Object>)value;
                decimal total = 0;
                foreach (DataReport gi in items)
                {
                    total += gi.Summa;
                }
                return "загалом: " + total.ToString() + " " + _currency?.Name;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public partial class ClientProviderDebtReportForm : Window, INotifyPropertyChanged
    {
        public class DataReport
        {
            public Guid ClientId { get; set; }
            public string Client { get; set; }
            public string Organization { get; set; }
            public string Currency { get; set; }
            public decimal Summa { get; set; }
        }

        private DateTime? _date;
        public DateTime? Date { get { return _date; } set { _date = value; OnPropertyChanged(); } }

        private string? _clientName;
        public string? ClientName
        {
            get { return _clientName; }
            set { _clientName = value; OnPropertyChanged(); }
        }

        private Currency? _currency;

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IAccumulationRegisterController _accumulationRegisterController;
        private readonly IHandbookController _handbookController;
        private Guid? _clientId;

        public ObservableCollection<DataReport> Data { get; set; } = new();

        public ClientProviderDebtReportForm()
        {
            _accumulationRegisterController = DIContainer.ServiceProvider.GetRequiredService<IAccumulationRegisterController>();
            _handbookController = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();

            _currency = _handbookController.GetHandbook<Currency>(w => w.IsDefault);

            DataContext = this;
            InitializeComponent();

            var view = (CollectionView)CollectionViewSource.GetDefaultView(report.ItemsSource);
            view.Filter = Filter;
            view.GroupDescriptions.Add(new PropertyGroupDescription("Client"));

            report.Columns.Add(new DataGridTextColumn() { Header = "Організація", Binding = new Binding("Organization") });
            //report.Columns.Add(new DataGridTextColumn() { Header = "Контрагента", Binding = new Binding("Client") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Сумма", Binding = new Binding("Summa") });
            report.Columns.Add(new DataGridTextColumn() { Header = "Валюта", Binding = new Binding("Currency") });

        }


        public bool Filter(object item)
        {
            DataReport data = item as DataReport;

            bool selectWarehouse = (_clientId == default || data?.ClientId == _clientId);

            return selectWarehouse ;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        private Func<ClientsDebt, bool>? GetClientSelectionFunc()
        {
            Func<ClientsDebt, bool>? func = f =>
            {
                return (Date == default || f.Date <= Date)
                && (_clientId == default || f.ClientId == _clientId);
            };

            return func;
        }

        private Func<ProvidersDebt, bool>? GetProviderSelectionFunc()
        {
            Func<ProvidersDebt, bool>? func = f =>
            {
                return (Date == default || f.Date <= Date)
                && (_clientId == default || f.ProviderId == _clientId);
            };

            return func;
        }

        private void createReport_Click(object sender, RoutedEventArgs e)
        {
            Data.Clear();

            if (ClientReport.IsChecked == true)
            {
                var moveList = _accumulationRegisterController.GetListData(GetClientSelectionFunc());

                var leftover = _accumulationRegisterController.GetLeftoverList(moveList,
                g => new { g.Organization, g.Client },
                s => new
                {
                    Client = s.Key.Client,
                    Organization = s.Key.Organization,
                    Value = s.Sum(selector => selector.TypeMove == TypeAccumulationRegisterMove.INCOMING ? selector.Value : -selector.Value)
                });

                foreach (var item in leftover)
                {
                    Data.Add(new()
                    {
                        Organization = item.Organization.Name,
                        Client = item.Client.Name,
                        Summa = item.Value,
                        ClientId = item.Client.Id,
                        Currency = _currency?.Name
                    });
                }

            }
            else
            {
                var moveList = _accumulationRegisterController.GetListData(GetProviderSelectionFunc());

                var leftover = _accumulationRegisterController.GetLeftoverList(moveList,
                g => new { g.Organization, g.Provider },
                s => new
                {
                    Client = s.Key.Provider,
                    Organization = s.Key.Organization,
                    Value = s.Sum(selector => selector.TypeMove == TypeAccumulationRegisterMove.INCOMING ? selector.Value : -selector.Value)
                });

                foreach (var item in leftover)
                {
                    Data.Add(new()
                    {
                        Organization = item.Organization.Name,
                        Client = item.Client.Name,
                        Summa = item.Value,
                        ClientId = item.Client.Id,
                        Currency = _currency?.Name
                    });
                }
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
    }
}
