using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using PresentationWPF.Forms;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PresentationWPF
{
    public partial class MainWindow : Window
    {
        private Button _prevBtn;
        private Grid _prevGrid;

        public MainWindow()
        {
            InitializeComponent();
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
            }

            if (form != null)
                form.Show();
        }
    }
}