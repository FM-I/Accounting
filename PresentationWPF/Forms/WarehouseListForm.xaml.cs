using BL.Interfaces;
using Domain.Entity.Handbooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Data;

namespace PresentationWPF.Forms
{
    public partial class WarehouseListForm : Window
    {
        private readonly IHandbookController _controller;
        private readonly IDbContext _context;
        public WarehouseListForm()
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _context = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();
            _context.SavedChanges += context_SavedChanges;

            InitializeComponent();

            var list = _controller.GetHandbooks<Warehouse>();
            List<ListItem> items = new List<ListItem>();
            foreach (var item in list)
            {
                items.Add(new ListItem(item.Id, item.Code, item.Name));
            }
            dataList.ItemsSource = items;

            var view = (CollectionView)CollectionViewSource.GetDefaultView(dataList.ItemsSource);
            view.Filter = ListFilter;

        }

        private void context_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            //dataList.Items.Clear();
            //var list = _controller.GetHandbooks<Warehouse>();

            //foreach (var item in list)
            //{
            //    dataList.Items.Add(new ListItem(item.Id, item.Code, item.Name));
            //}
        }

        private void dataList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListItem item = (ListItem)dataList.SelectedItem;
            var elementForm = new WarehouseElementForm(item.Id);
            elementForm.Show();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new WarehouseElementForm();
            elementForm.Show();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetHandbook<Warehouse>(item.Id);

            if(data != null)
            {
                data.DeleteMark = true;
                await _controller.AddOrUpdateAsync(data);
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
                return ((item as ListItem).DataName.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private record ListItem(Guid Id, string Code, string DataName, string ProductImagePath = "/Images/exclaim.png");
    }

    
}
