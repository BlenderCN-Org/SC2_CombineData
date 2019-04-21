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
        public static readonly DependencyProperty PathTextProperty = DependencyProperty.Register("PathText", typeof(string), typeof(SC2_FileListViewItem));
        /// <summary>
        /// 路径文本属性
        /// </summary>
        public string PathText
        {
            set { SelectPathControl_FilePath.PathText = value; SetValue(PathTextProperty, value); }
            get { SetValue(PathTextProperty, SelectPathControl_FilePath.PathText); return (string)GetValue(PathTextProperty); }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2_FileListViewItem()
        {
            InitializeComponent();
        }

        #endregion
    }
}
