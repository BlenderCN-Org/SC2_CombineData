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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SC2_CombineData_MainWindow : Window
    {
        #region 属性字段


        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2_CombineData_MainWindow()
        {
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
    }
}
