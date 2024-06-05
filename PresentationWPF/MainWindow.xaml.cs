using BL.Interfaces;
using Domain.Entity.Handbooks;
using Domain.Entity.Registers.Informations;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using PresentationWPF.Forms;
using PresentationWPF.Forms.Documents;
using PresentationWPF.Forms.Handbooks;
using PresentationWPF.Forms.Registers;
using PresentationWPF.Forms.Reports;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PresentationWPF
{
    public partial class MainWindow : Window
    {
        private int TIMER = 3;
        private Button _prevBtn;
        private Grid _prevGrid;
        private BackgroundWorker _backgrounWorker;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IHandbookController _handboolContorller;
        private readonly IInformationRegisterController _informationRegisterController;
        public MainWindow()
        {

            _exchangeRateService = DIContainer.ServiceProvider.GetRequiredService<IExchangeRateService>();
            _handboolContorller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _informationRegisterController = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();

            InitializeComponent();

            _backgrounWorker = new BackgroundWorker();
            _backgrounWorker.DoWork += Bg_DoWork;
            _backgrounWorker.WorkerReportsProgress = true;
            _backgrounWorker.RunWorkerAsync();

        }

        private async void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(TIMER));

                var result = await _exchangeRateService.GetExchangeRatesAsync();
                var currencies = _handboolContorller.GetHandbooks<Currency>(w => !w.DeleteMark && !w.IsDefault);

                foreach (var item in currencies)
                {
                    var data = result.FirstOrDefault(w => w.CurrenyName.ToUpper() == item.Name.ToUpper());

                    if (data != null)
                    {
                        var exchangeRate = new ExchangesRate();
                        exchangeRate.Currency = item;
                        exchangeRate.Date = DateTime.Now;
                        exchangeRate.Rate = data.CurrengeSaleRate;
                        await _informationRegisterController.AddOrUpdateAsync(exchangeRate);
                    }
                }
            }
        }

        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            
            if (_prevBtn != null && _prevBtn != btn)
            {
                _prevBtn.Background = Brushes.Transparent;
                _prevGrid.Visibility = Visibility.Hidden;
            }

            _prevBtn = btn;

            switch (_prevBtn.Name)
            {
                case "showHandbooks":
                    _prevGrid = handbooksGrid;
                    break;
                case "showDocuments":
                    _prevGrid = documentsGrid;
                    break;
                case "showRegister":
                    _prevGrid = registersGrid;
                    break;
                case "showReport":
                    _prevGrid = reportGrid;
                    break;
            }

            if (_prevGrid.Visibility == Visibility.Visible)
            {
                _prevGrid.Visibility = Visibility.Hidden;
                _prevBtn.Background = Brushes.Transparent;
            }
            else
            {
                _prevGrid.Visibility = Visibility.Visible;
                _prevBtn.Background = Brushes.LightBlue;
            }
        }

        private void openForm(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            
            Window form = null;

            switch(btn.Name)
            {
                case "openOrganizationList":
                    form = new OrganizationListForm();
                    break;
                case "openTypePriceList":
                    form = new TypePriceListForm();
                    break;
                case "openBankAccountList":
                    form = new BankAccountListForm();
                    break;
                case "openBankList":
                    form = new BankListForm();
                    break;
                case "openWarehouseList":
                    form = new WarehouseListForm();
                    break;
                case "openUnitList":
                    form = new UnitListForm();
                    break;
                case "openNomenclatureList":
                    form = new NomenclatureListForm();
                    break;
                case "openCashBoxList":
                    form = new CashBoxListForm();
                    break;
                case "openContactList":
                    form = new ContactListForm();
                    break;
                case "openCurrencyList":
                    form = new CurrencyListForm();
                    break;
                case "openClientList":
                    form = new ClientListForm();
                    break;
                case "openBarcodeList":
                    form = new BarcodeListForm();
                    break;
                case "openPriceList":
                    form = new PriceListForm();
                    break;
                case "openExchangeRateList":
                    form = new ExchangeRateListForm();
                    break;
                case "openClientContactList":
                    form = new ClientContactListForm();
                    break;
                case "openClientOrderList":
                    form = new ClientOrderListForm();
                    break;
                case "openSalesInvoiceList":
                    form = new SalesInvoiceListForm();
                    break;
                case "openProviderOrderList":
                    form = new ProviderOrderListForm();
                    break;
                case "openPurchaceInvoiceList":
                    form = new PurchaceInvoiceListForm();
                    break;
                case "openInCashOrderList":
                    form = new InCashOrderListForm();
                    break;
                case "openOutCashOrderList":
                    form = new OutCashOrderListForm();
                    break;
                case "openInBankAccountOrderList":
                    form = new InBankAccountOrderListForm();
                    break;
                case "openOutBankAccountOrderList":
                    form = new OutBankAccontOrderListForm();
                    break;
                case "openLeftoverReport":
                    form = new LeftoverReportForm();
                    break;
                case "openClientProviderDebtReport":
                    form = new ClientProviderDebtReportForm();
                    break;
                case "openSalesReport":
                    form = new SalesReport();
                    break;
                case "openPurchaseReport":
                    form = new PurchaseReport();
                    break;
                case "openCashReport":
                    form = new CashReport();
                    break;
            }

            if (form != null)
                form.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}