using System;
using System.Collections.Generic;
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
    /// SC2_FileListViewItem.xaml 的交互逻辑
    /// </summary>
    public partial class SC2_FileListViewItem : ListViewItem
    {

        #region 属性

        /// <summary>
        /// 路径文本依赖项
        /// </summary>
        public static readonly DependencyProperty PathTextProperty = DependencyProperty.Register(nameof(PathText), typeof(string), typeof(SC2_FileListViewItem));
        /// <summary>
        /// 路径文本属性
        /// </summary>
        public string PathText
        {
            set { SetValue(PathTextProperty, value); }
            get { return (string)GetValue(PathTextProperty); }
        }

        /// <summary>
        /// 路径文本依赖项
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(SC2_FileListViewItem));
        /// <summary>
        /// 路径文本属性
        /// </summary>
        public double ItemWidth
        {
            set { SetValue(ItemWidthProperty, value); }
            get { return (double)GetValue(ItemWidthProperty); }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2_FileListViewItem()
        {
            InitializeComponent();
            ItemWidth = this.Width;
            SelectPathControl_FilePath.DefaultDirectory = Environment.CurrentDirectory;
        }

        #endregion

        #region 控件事件

        /// <summary>
        /// 获得焦点事件
        /// </summary>
        /// <param name="sender">事件控件</param>
        /// <param name="e">响应参数</param>
        private void SelectPathControl_FilePath_GotFocus(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
        }

        /// <summary>
        /// 路径存在变化事件
        /// </summary>
        /// <param name="sender">事件控件</param>
        /// <param name="e">响应参数</param>
        private void SelectPathControl_FilePath_IsPathExistHandler(object sender, RoutedEventArgs e)
        {
            SC2_CombineData_MainWindow.MainWindow.RefreshGenerateButton();
        }

        #endregion
    }
}
