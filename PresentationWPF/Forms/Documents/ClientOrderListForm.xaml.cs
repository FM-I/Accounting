﻿using BL.Interfaces;
using Domain.Entity.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Data;

namespace PresentationWPF.Forms.Documents
{
    public partial class ClientOrderListForm : Window
    {
        private readonly IDocumentController _controller;
        private readonly IDbContext _context;
        private bool _select;

        public Guid SelectedId { get; set; }

        public ClientOrderListForm(bool select = false)
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IDocumentController>();
            _context = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();
            _context.SavedChanges += context_SavedChanges;
            _select = select;

            InitializeComponent();

            context_SavedChanges(null, null);
        }

        private void context_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var list = _controller.GetDocuments<ClientOrder>();
                List<ListItem> items = new List<ListItem>();

                foreach (var item in list)
                {
                    items.Add(new ListItem(item.Id, item.Number, item.Date, item.DeleteMark, item.Conducted, item.Client.Name, item.Summa));
                }

                dataList.ItemsSource = items;

                var view = (CollectionView)CollectionViewSource.GetDefaultView(dataList.ItemsSource);
                view.Filter = ListFilter;
                view.SortDescriptions.Add(new("Number", System.ComponentModel.ListSortDirection.Descending));
            });
        }

        private void dataList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListItem item = (ListItem)dataList.SelectedItem;

            if (item == null)
                return;

            if (_select)
            {
                SelectedId = item.Id;
                Close();
                return;
            }

            var elementForm = new ClientOrderElementForm(item.Id);
            elementForm.Show();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new ClientOrderElementForm();
            elementForm.Show();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetDocument<ClientOrder>(item.Id);

            MessageBoxResult result;
            if (item.DeleteMark)
                result = MessageBox.Show("Обраний об'єкт помічений на видалення. Зняти помітку?", "", MessageBoxButton.YesNo);
            else
                result = MessageBox.Show("Помітити на видалення обраний об'єкт?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes && data != null)
            {
                data.DeleteMark = !data.DeleteMark;

                if (data.Conducted)
                    await _controller.UnConductedDoumentAsync(data);
                else
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
            bool findText = false;

            if (String.IsNullOrEmpty(Search.Text))
                findText = true;
            else
                findText = ((item as ListItem).ClientName.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0);

            return findText;
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetDocument<ClientOrder>(item.Id);
            if (data != null)
            {
                var elementForm = new ClientOrderElementForm((ClientOrder)data.DeepCopy());
                elementForm.Show();
            }
        }

        private void FillSaleInvoice_Click(object sender, RoutedEventArgs e)
        {
            var item = (ListItem)dataList.SelectedItem;

            if (item == null)
                return;

            var order = _controller.GetDocument<ClientOrder>(item.Id);

            if (order == null)
                return;

            var document = new SaleInvoice();
            document.FillWith(order);
            
            var form = new SalesInvoiceElementForm(document);
            form.Show();
        }

        private record ListItem(Guid Id, string Number, DateTime Date, bool DeleteMark, bool Conducted, string ClientName, decimal Summa);

    }
}
