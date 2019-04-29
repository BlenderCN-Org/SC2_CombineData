using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace SC2_RegexFind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SC2_RegexFind_MainWindow : Window
    {
        #region 构造寒地

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2_RegexFind_MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 生成正则表达式
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Regex GetRegex(string pattern)
        {
            try
            {
                return new Regex(pattern, RegexOptions.Compiled);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取正则表达式匹配的文本
        /// </summary>
        /// <param name="regex">正则表达式</param>
        /// <param name="input">输入文本</param>
        /// <returns></returns>
        public static List<string> GetMatchText(Regex regex, string input)
        {
            List<string> resultList = new List<string>();
            MatchCollection matchs = regex.Matches(input);
            foreach (Match match in matchs)
            {
                if (match.Length != 0) resultList.Add(match.Value);
            }

            return resultList;
        }

        /// <summary>
        /// 获取匹配文本
        /// </summary>
        /// <returns></returns>
        private List<string> GetMatchText()
        {
            Regex regex = GetRegex(TextBox_Regex.Text);
            if (regex == null) return null;
            return GetMatchText(regex, TextEditor_SourceText.Text);
        }

        #endregion

        #region 控件事件

        /// <summary>
        /// 搜索按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            List<string> result = GetMatchText();
            if (result == null)
            {
                MessageBox.Show("输入不符合正则表达式！");
                return;
            }
            StringBuilder builder = new StringBuilder(result[0]);
            for (int i = 1; i < result.Count; i++)
            {
                builder.Append("\r\n");
                builder.Append(result[i]);
            }
            TextEditor_ResultText.Text = builder.ToString();
        }

        /// <summary>
        /// 比较按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_Compare_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

    }
    #region Converter

    /// <summary>
    /// 选择项到删除按钮可用转换器
    /// </summary>
    public class ConverterRegexPatternToGenerateEnable : IValueConverter
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
            if (value is string pattern)
            {
                return SC2_RegexFind_MainWindow.GetRegex(pattern) != null;
            }
            else
            {
                return false;
            }
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
