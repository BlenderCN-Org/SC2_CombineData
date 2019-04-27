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

        #region 方法

        /// <summary>
        /// 刷新移动按钮状态
        /// </summary>
        public void RefreshMoveButton()
        {
            Button_Up.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            BindingOperations.GetMultiBindingExpression(Button_Down, Button.IsEnabledProperty).UpdateTarget();
        }

        /// <summary>
        /// 刷新生成按钮状态
        /// </summary>
        public void RefreshGenerateButton()
        {
            Button_Generate.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
        }
        #endregion

        #region 控件事件

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
            RefreshMoveButton();
        }


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
            RefreshMoveButton();
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
    /// 选择序号到上移下移可用性转换器
    /// </summary>
    public class ConverterSelectIndexToEnable_Up : IValueConverter
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
            return ((int)value > 0);
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
    /// 选择序号到上移下移可用性转换器
    /// </summary>
    public class ConverterSelectIndexToEnable_Down : IMultiValueConverter
    {
        /// <summary>
        /// 转换函数
        /// </summary>
        /// <param name="values">值数组</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">本地化</param>
        /// <returns>转换结果</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ListView view = values[1] as ListView;
            return (int)values[0] < view.Items.Count - 1;
        }

        /// <summary>
        /// 逆向转换函数
        /// </summary>
        /// <param name="value">值数组</param>
        /// <param name="targetTypes">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">本地化</param>
        /// <returns>转换结果</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

    }

    /// <summary>
    /// ListView生成按钮可用性转换器
    /// </summary>
    public class ConverterListViewToGenerateEnable : IValueConverter
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
            if (value is ListView view)
            {
                foreach (SC2_FileListViewItem item in view.Items)
                {
                    if (item.SelectPathControl_FilePath.IsHaveSelected != true) return false;
                }
            }
            return true;
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
