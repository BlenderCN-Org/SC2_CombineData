using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SC2_CombineData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SC2_CombineData_MainWindow : Window
    {
        #region 属性字段

       public static SC2_CombineData_MainWindow MainWindow { set; get; }
        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2_CombineData_MainWindow()
        {
            MainWindow = this;
            InitializeComponent();
        }

        #endregion

        #region 控件方法

        /// <summary>
        /// 添加按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            SC2_FileListViewItem item = new SC2_FileListViewItem();
            Binding binding = new Binding("ActualWidth")
            {
                ElementName = "ListView_FileList"
            };
            item.SetBinding(SC2_FileListViewItem.ItemWidthProperty, binding);
            ListView_FileList.Items.Add(item);
        }

        #endregion

        #region 控件事件

        /// <summary>
        /// 获得焦点事件
        /// </summary>
        /// <param name="sender">事件控件</param>
        /// <param name="e">响应参数</param>
        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            List<SC2_FileListViewItem> items = ListView_FileList.SelectedItems.OfType<SC2_FileListViewItem>().ToList();
            foreach (SC2_FileListViewItem item in items)
            {
                ListView_FileList.Items.Remove(item);
            }
        }

        #endregion
    }

    #region Converter

    /// <summary>
    /// 选择项到删除按钮可用转换器
    /// </summary>
    public class ConverterSelectItemToEnable_Delete : IValueConverter
    {
        /// <summary>
        /// 转换函数
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">本地化信息</param>
        /// <returns>转换结果</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        /// <summary>
        /// 反向转回函数
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">本地化信息</param>
        /// <returns>转换结果</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }


    /// <summary>
    /// 选择序号到上移下移转换器
    /// </summary>
    public class ConverterSelectIndexToEnable_UpOrDown : IValueConverter
    {
        /// <summary>
        /// 转换函数
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">本地化信息</param>
        /// <returns>转换结果</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUp = bool.Parse(parameter as string);
            return isUp ? ((int)value > 0) : ((int)value < SC2_CombineData_MainWindow.MainWindow.ListView_FileList.Items.Count - 1);
        }

        /// <summary>
        /// 反向转回函数
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">本地化信息</param>
        /// <returns>转换结果</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }
    #endregion
}
