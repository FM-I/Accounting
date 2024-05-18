﻿using BL.Controllers;
using BL.Interfaces;
using Domain.Entity.Handbooks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PresentationWPF.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PresentationWPF.Forms
{
    public partial class NomenclatureListForm : Window
    {
        private readonly IHandbookController _controller;
        private readonly IDbContext _context;
        private bool _select;
        private bool _onlyGroup;
        private bool _reloadItems = true;
        private bool _reloadGroups = true;

        public Guid SelectedId { get; set; }

        public NomenclatureListForm(bool select = false, bool onlyGroup = false)
        {
            _controller = DIContainer.ServiceProvider.GetRequiredService<IHandbookController>();
            _context = DIContainer.ServiceProvider.GetRequiredService<IDbContext>();
            _context.SavedChanges += context_SavedChanges;
            _select = select;
            _onlyGroup = onlyGroup;

            InitializeComponent();

            context_SavedChanges(null, null);
        }

        private void context_SavedChanges(object? sender, SavedChangesEventArgs e)
        {
            if (_reloadItems)
            {

                var list = _controller.GetHandbooks<Nomenclature>(where => !where.IsGroup);
                List<ListItem> items = new List<ListItem>();
                foreach (var item in list)
                {
                    if (_onlyGroup && !item.IsGroup)
                        continue;

                    items.Add(new ListItem(item.Id, item.Code, item.Name, item.DeleteMark, item.BaseUnit?.Name, item.Arcticle, item.Parent?.Id));
                }
                dataList.ItemsSource = items;

                var view = (CollectionView)CollectionViewSource.GetDefaultView(dataList.ItemsSource);
                view.Filter = ListFilter;
                _reloadItems = false;
            }

            if (_reloadGroups)
            {
                treeGroups_Initialized(null, null);
                _reloadGroups = false;
            }
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

            var elementForm = new NomenclatureElementForm(item.Id);
            elementForm.Show();
            _reloadItems = true;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new NomenclatureElementForm();
            elementForm.Show();
            _reloadItems = true;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetHandbook<Nomenclature>(item.Id);

            MessageBoxResult result;
            if (item.DeleteMark)
                result = MessageBox.Show("Обраний об'єкт помічений на видалення. Зняти помітку?", "", MessageBoxButton.YesNo);
            else
                result = MessageBox.Show("Помітити на видалення обраний об'єкт?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes && data != null)
            {
                data.DeleteMark = !data.DeleteMark;
                await _controller.AddOrUpdateAsync(data);
                _reloadItems = true;
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
            var group = (GroupData)treeGroups.SelectedItem;

            bool findText = false;
            bool findGroup = false;

            if (String.IsNullOrEmpty(Search.Text))
                findText = true;
            else
                findText = ((item as ListItem).DataName.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0);

            findGroup = ((item as ListItem).ParentId == group?.Id);

            return findText && findGroup;
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dataList.SelectedItem == null)
                return;

            ListItem item = (ListItem)dataList.SelectedItem;
            var data = _controller.GetHandbook<Nomenclature>(item.Id);
            if (data != null)
            {
                var elementForm = new NomenclatureElementForm((Nomenclature)data.DeepCopy());
                elementForm.Show();
                _reloadItems = true;
            }
        }

        private void createGroupBtn_Click(object sender, RoutedEventArgs e)
        {
            var elementForm = new NomenclatureGroupForm();
            elementForm.Show();
            _reloadGroups = true;
        }

        private void treeGroups_Initialized(object sender, EventArgs e)
        {
            var data = _controller.GetHandbooks<Nomenclature>(w => w.IsGroup);

            List<GroupData> list = [new() { Id = null, Content = "Без групи", Image = null}];
            AddChildren(data, list, null);

            treeGroups.ItemsSource = list;
        }

        private void AddChildren(List<Nomenclature> data, List<GroupData> list, Nomenclature parent)
        {
            foreach (var item in data)
            {
                if (item.Parent?.Id == parent?.Id)
                {
                    list.Add(new GroupData { Id = item.Id, Content = item.Name });
                    AddChildren(data, list.Last().Items, item);
                }
            }
        }

        private void treeGroups_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ((TreeView)sender).SelectedItem as GroupData;

            if(item == null || item.Id == null || item.Id == Guid.Empty)
                return;

            var form = new NomenclatureGroupForm((Guid)item.Id);
            form.ShowDialog();
            _reloadGroups = true;
        }

        private record ListItem(Guid Id, string Code, string DataName, bool DeleteMark, string UnitName, string? Article, Guid? ParentId);


        private void treeGroups_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ClickCount > 1)
            {
                e.Handled = true;
                treeGroups_MouseDoubleClick(sender, e);
            }
        }

        private void treeGroups_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CollectionViewSource.GetDefaultView(dataList.ItemsSource).Refresh();
        }

        private class GroupData()
        {
            public object Content { get; set; }
            public Guid? Id { get; set; }
            public object Image { get; set; } = new BitmapImage(new Uri("D:\\Politeh\\2024\\Project\\Accounting\\PresentationWPF\\Images\\folder.png"));
            public List<GroupData> Items { get; set; } = new();

        }
    }
}
