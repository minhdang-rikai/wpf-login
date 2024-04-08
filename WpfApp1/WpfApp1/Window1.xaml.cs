using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        //public ObservableCollection<DataItem> Items { get; set; }

        public Window1()
        {
            InitializeComponent();

            //Items = new ObservableCollection<DataItem>();

            //for (int i = 0; i < 6; i++)
            //{
            //    var item = new DataItem
            //    {
            //        Column1 = "Value " + i,
            //        Column2 = "Value " + i,
            //        Column3 = "Value " + i,
            //    };
            //    Items.Add(item);
            //}

            //listView.ItemsSource = Items;

            List<MyData> dataList = new List<MyData>()
            {
                new MyData() { Column1 = "Data 1-1", Column2 = "Data 1-2", Column3 = "Data 1-3" },
                new MyData() { Column1 = "Data 2-1", Column2 = "Data 2-2", Column3 = "Data 2-3" },
                new MyData() { Column1 = "Data 3-1", Column2 = "Data 3-2", Column3 = "Data 3-3" }
            };

            listView.ItemsSource = dataList;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class MyData
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
    }

    //public class DataItem : INotifyPropertyChanged
    //{
    //    private string _column1;
    //    public string Column1
    //    {
    //        get { return _column1; }
    //        set
    //        {
    //            _column1 = value;
    //            OnPropertyChanged("Column1");
    //        }
    //    }

    //    private string _column2;
    //    public string Column2
    //    {
    //        get { return _column2; }
    //        set
    //        {
    //            _column2 = value;
    //            OnPropertyChanged("Column2");
    //        }
    //    }
        
    //    private string _column3;
    //    public string Column3
    //    {
    //        get { return _column3; }
    //        set
    //        {
    //            _column3 = value;
    //            OnPropertyChanged("Column3");
    //        }
    //    }
        

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
}
