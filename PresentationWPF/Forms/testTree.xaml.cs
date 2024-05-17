using BL.Controllers;
using Domain.Entity.Handbooks;
using Infrastructure;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PresentationWPF.Forms
{
    public partial class testTree : Window
    {
        private class Data()
        {
            public object Content { get; set; }
            public object Image { get; set; } = new BitmapImage(new Uri("D:\\Politeh\\2024\\Project\\Accounting\\PresentationWPF\\Images\\folder.png"));

            public List<Data> Items { get; set; } = new();
        }

        public testTree()
        {
            InitializeComponent();
        }

        private void tree_Initialized(object sender, EventArgs e)
        {
            var db = new AppDbContext();
            var ctr = new HandbookController(db);

            var data = ctr.GetHandbooks<Nomenclature>(w => w.IsGroup);

            List<Data> list = new List<Data>();
            AddChildren(data, list, null);

            //foreach (var item in data)
            //{
            //    if(item.Parent == null)
            //    {
            //        list.Add(new Data() { Content = item.Name });
            //        AddChildren(data, list.Last().Items, item);
            //    }

            //}

            tree.ItemsSource = list;
        }

        private void AddChildren(List<Nomenclature> data, List<Data> list, Nomenclature parent)
        {
            foreach (var item in data)
            {
                if (item.Parent?.Id == parent?.Id) 
                {
                    list.Add(new Data { Content = item.Name });
                    AddChildren(data, list.Last().Items, item);
                }
            }
        }
    }
}
