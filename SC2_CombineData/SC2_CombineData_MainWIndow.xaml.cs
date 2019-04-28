using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Xml.Linq;

namespace SC2_CombineData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SC2_CombineData_MainWindow : Window
    {
        #region 声明

        public const string Const_Extension = ".xml";
        public const string Const_XMLRootName = "Catalog";
        public const string Const_XMLIDName = "id";

        #endregion

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
        /// 刷新生成按钮状态
        /// </summary>
        public void RefreshGenerateButton()
        {
            Button_Generate.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
        }

        /// <summary>
        /// 生成合并数据
        /// </summary>
        public void GenerateCombineData()
        {
            List<XDocument> docs = new List<XDocument>();
            Dictionary<string, XElement> dictElement = new Dictionary<string, XElement>();
            Dictionary<string, List<XComment>> dictComment = new Dictionary<string, List<XComment>>();
            XElement existElement;
            XAttribute existAttribut;
            List<XComment> existComments;
            XComment tempComment;
            FileInfo file = null;
#if !DEBUG
            try
#endif
            {
                foreach (SC2_FileListViewItem item in ListView_FileList.Items)
                {
                    file = item.SelectPathControl_FilePath.SelectedFile;
                    XDocument doc = XDocument.Load(file.FullName);
                    docs.Add(doc);
                }

            }
#if !DEBUG
            catch (Exception error)
            {
                string msg = file == null ? "" : $"错误文件:{file.FullName}\r\n";
                msg += error.Message;
                MessageBox.Show(msg);
            }
#endif
            XDocument result = new XDocument
            {
                Declaration = docs[0].Declaration
            };
            XElement resultRoot = new XElement(Const_XMLRootName);
            result.Add(resultRoot);

            foreach (XDocument xml in docs)
            {
                XElement root = xml.Element(Const_XMLRootName);
                if (root == null) continue;

                // 数据实体
                foreach (XElement element in root.Elements())
                {
                    string id = element.Attribute(Const_XMLIDName)?.Value;
                    if (string.IsNullOrEmpty(id)) continue;
                    if (dictElement.ContainsKey(id))
                    {
                        existElement = dictElement[id];
                        existElement.Add(element.Elements());

                        // 数据属性
                        foreach (XAttribute attribute in element.Attributes())
                        {
                            existAttribut = existElement.Attribute(attribute.Name);
                            if (existAttribut == null)
                            {
                                existElement.Add(attribute);
                            }
                            else
                            {
                                existAttribut.Value = attribute.Value;
                            }
                        }
                    }
                    else
                    {
                        existElement = new XElement(element);
                        dictElement[id] = existElement;
                        resultRoot.Add(existElement);
                    }

                    // 注释
                    List<XComment> comments = new List<XComment>();
                    XNode node = element.PreviousNode;
                    while (node != null && node is XComment comment)
                    {
                        comments.Insert(0, comment);
                        node = node.PreviousNode;
                    }
                    if (comments.Count != 0)
                    {
                        if (dictComment.ContainsKey(id))
                        {
                            existComments = dictComment[id];
                            foreach (XComment comment in comments)
                            {
                                if (existComments.Where(r=> r.Value == comment.Value).Count() ==0)
                                {
                                    tempComment = new XComment(comment);
                                    existElement.AddBeforeSelf(tempComment);
                                    existComments.Add(tempComment);
                                }
                            }
                        }
                        else
                        {
                            dictComment[id] = new List<XComment>();
                            foreach (XComment comment in comments)
                            {
                                tempComment = new XComment(comment);
                                existElement.AddBeforeSelf(tempComment);
                                dictComment[id].Add(tempComment);
                            }
                        }
                    }
                }
            }
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
        }

        /// <summary>
        /// 删除按钮点击事件
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
        
        /// <summary>
        /// 生成按钮点击事件
        /// </summary>
        /// <param name="sender">事件控件</param>
        /// <param name="e">响应参数</param>
        private void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            GenerateCombineData();
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
                foreach (object select in view.Items)
                {
                    if (select is SC2_FileListViewItem item)
                    {
                        if (item.SelectPathControl_FilePath.IsPathExist != true)
                        {
                            return false;
                        }
                        FileInfo file = item.SelectPathControl_FilePath.SelectedFile;
                        if (file == null || file.Extension != SC2_CombineData_MainWindow.Const_Extension)
                        {
                            return false;
                        }
                    }
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
