using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PresentationWPF.Common
{
    public class ItemProduct : INotifyPropertyChanged
    {
        private double? _quantity = null;
        private double? _price = null;
        private string _nomenclatureName = string.Empty;
        private string _unitName = string.Empty;

        public event EventHandler OnChange;

        public Guid Id { get; set; }
        public Guid NomenclatureId { get; set; }
        public Guid UnitId { get; set; }
        public string NomenclatureName
        {
            get { return _nomenclatureName; }
            set { _nomenclatureName = value; OnPropertyChanged(); }
        }
        public string UnitName
        {
            get { return _unitName; }
            set { _unitName = value; OnPropertyChanged(); }
        }

        public double? Quantity
        {
            get { return _quantity; }
            set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(Summa)); }
        }

        public double? Price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(); OnPropertyChanged(nameof(Summa)); }
        }

        public double? Summa
        {
            get { return _price != null && _quantity != null ? _price * _quantity : null; }
            set { OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
            OnChange?.Invoke(this, new());
        }
    }
}
