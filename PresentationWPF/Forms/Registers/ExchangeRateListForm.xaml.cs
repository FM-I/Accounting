using BL.Interfaces;
using Domain.Entity.Registers.Informations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Data;

namespace PresentationWPF.Forms.Registers
{
    public partial class ExchangeRateListForm : Window
    {
        private readonly IInformationRegisterController _controller;
        private readonly IDbContext _context;
        public ExchangeRateListForm()
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();
            _context = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();
            _context.SavedChanges += context_SavedChanges;

            InitializeComponent();

            context_SavedChanges(null, null);
        }

        private void context_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            var list = _controller.GetListData<ExchangesRate>();
            List<ListItem> items = new List<ListItem>();
            foreach (var item in list)
            {
                items.Add(new ListItem(item.Currency.Name, item.CurrencyId, item.Rate, item.Date));
            }

            dataList.ItemsSource = items;

            var view = (CollectionView)CollectionViewSource.GetDefaultView(dataList.ItemsSource);
            view.Filter = ListFilter;
        }

        private void dataList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListItem item = (ListItem)dataList.SelectedItem;

            if (item == null)
                return;

            var elementForm = new ExchangeRateElementForm(item.Date, item.CurrencyId);
            elementForm.Show();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new ExchangeRateElementForm();
            elementForm.Show();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;

            if (MessageBox.Show("Видалити обраний запис?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _controller.DeleteAsync<ExchangesRate>(w => w.Date == item.Date && w.CurrencyId == item.CurrencyId);
            }
        }

        private void Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Search.Text))
                Placeholder.Visibility = Visibility.Hidden;
            else
                Placeholder.Visibility = Visibility.Visible;

            if (dataList.ItemsSource != null)
                CollectionViewSource.GetDefaultView(dataList.ItemsSource).Refresh();
        }

        private bool ListFilter(object item)
        {
            if (String.IsNullOrEmpty(Search.Text))
                return true;
            else
                return ((item as ListItem).CurrencyName.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var elementForm = new ExchangeRateElementForm(item.Date, item.CurrencyId, true);
            elementForm.Show();
        }

        private record ListItem(string CurrencyName, Guid CurrencyId, double Rate, DateTime Date);
    }
}
