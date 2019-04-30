﻿using System;
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
using System.Xml.Linq;

namespace SC2_RegexFind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SC2_RegexFind_MainWindow : Window
    {
        #region 常量声明

        public const string Const_NameID = "id";
        public const string Const_NameParent = "parent";
        public const string Const_NameToken = "token";
        public static readonly Regex Const_RegexTokenID = new Regex("(?<=id=\")\\w*(?=\")", RegexOptions.Compiled);
        public static readonly Regex Const_RegexTokenValue = new Regex("(?<=value=\")\\w*(?=\")", RegexOptions.Compiled);
        public static readonly Regex Const_RegexTokenFormat = new Regex("##\\w*##", RegexOptions.Compiled);

        #endregion

        #region 属性字段

        /// <summary>
        /// 全部Token字典
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> DictTokenForElement { set; get; } = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 当前Token字典
        /// </summary>
        public static Dictionary<string, string> DictTokenCurrent { set; get; }

        #endregion

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
        /// 转换Token
        /// </summary>
        /// <param name="match">匹配</param>
        /// <returns>转换结果</returns>
        public static string ConvertToken(Match match)
        {
            string id = match.Value.Substring(2, match.Value.Length - 4);
            if (DictTokenCurrent.ContainsKey(id))
            {
                return DictTokenCurrent[id];
            }
            else
            {
                return match.Value;
            }
        }

        /// <summary>
        /// 移除属性token
        /// </summary>
        /// <param name="element">元素</param>
        public static void RemoveAttributeToken(XElement element)
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                string value = attribute.Value;
                attribute.Value = Const_RegexTokenFormat.Replace(value, ConvertToken);
            }
            foreach (XElement childElement in element.Elements())
            {
                RemoveAttributeToken(childElement);
            }
        }

        /// <summary>
        /// 移除元素token
        /// </summary>
        /// <param name="element">元素</param>
        public static void RemoveElementToken(XElement element)
        {
            string id = element.Attribute(Const_NameID).Value;
            XAttribute parentAtt = element.Attribute("parent");
            DictTokenCurrent = new Dictionary<string, string>();
            if (parentAtt != null)
            {
                string parentID = parentAtt.Value;
                foreach (KeyValuePair<string,string> select in DictTokenForElement[parentID])
                {
                    DictTokenCurrent[select.Key] = select.Value;
                }
            }
            DictTokenCurrent[Const_NameID] = id;
            XNode node = element.FirstNode;            
            while (node != null)
            {
                XNode currentNode = node;
                node = node.NextNode;
                if (currentNode is XProcessingInstruction pi)
                {
                    if (pi.Target != Const_NameToken) continue;
                    string tokenID = Const_RegexTokenID.Match(pi.Data).Value;
                    string tokenValue = Const_RegexTokenValue.Match(pi.Data).Value;
                    DictTokenCurrent[tokenID] = tokenValue;
                    currentNode.Remove();
                }
            }
            DictTokenForElement[id] = DictTokenCurrent;
            foreach (XElement childElement in element.Elements())
            {
                RemoveAttributeToken(childElement);
            }
        }

        /// <summary>
        /// 获取去除Token的文本
        /// </summary>
        /// <param name="text">原始文本</param>
        /// <returns>去除后文本</returns>
        public static string GetRemoveTokenString(string text)
        {
            try
            {
                XDocument document = XDocument.Parse(text);
                return null;
            }
            catch
            {
                return text;
            }

        }

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
        /// 转换按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_Modify_Click(object sender, RoutedEventArgs e)
        {
            XDocument document = XDocument.Parse(TextEditor_SourceText.Text);
            foreach (XElement element in document.Root.Elements())
            {
                RemoveElementToken(element);
            }
            TextEditor_SourceText.Text = document.ToString();
        }

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
