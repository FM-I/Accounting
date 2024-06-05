using BL.Interfaces;
using Domain.Entity.Registers.Informations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Data;

namespace PresentationWPF.Forms.Registers
{
    public partial class PriceListForm : Window
    {
        private readonly IInformationRegisterController _controller;
        private readonly IDbContext _context;
        public PriceListForm()
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IInformationRegisterController>();
            _context = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();
            _context.SavedChanges += context_SavedChanges;

            InitializeComponent();

            context_SavedChanges(null, null);
        }

        private void context_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var list = _controller.GetListData<Price>();
                List<ListItem> items = new List<ListItem>();
                foreach (var item in list)
                {
                    items.Add(new ListItem(item.Nomenclature.Name, item.TypePrice.Name, item.Value, item.Date, item.NomenclatureId, item.TypePriceId));
                }

                dataList.ItemsSource = items;

                var view = (CollectionView)CollectionViewSource.GetDefaultView(dataList.ItemsSource);
                view.Filter = ListFilter;
            });
        }

        private void dataList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListItem item = (ListItem)dataList.SelectedItem;

            if (item == null)
                return;

            var elementForm = new PriceElementForm(item.Date, item.NomenclatureId, item.TypePriceId);
            elementForm.Show();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new PriceElementForm();
            elementForm.Show();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;

            if (MessageBox.Show("Видалити обраний запис?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _controller.DeleteAsync<Price>(w => w.Date == item.Date && w.NomenclatureId == item.NomenclatureId && w.TypePriceId == item.TypePriceId);
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
                return ((item as ListItem).NomenclatureName.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var elementForm = new PriceElementForm(item.Date, item.NomenclatureId, item.TypePriceId, true);
            elementForm.Show();
        }

        private record ListItem(string NomenclatureName, string PriceName, decimal Price, DateTime Date, Guid NomenclatureId, Guid TypePriceId);
    }
}
