using System;
using System.Collections.Generic;
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

namespace SC2_ModelDetails
{
    /// <summary>
    /// SC2_ModelDetails_MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SC2_ModelDetails_MainWindow : Window
    {
        #region 常量声明

        public const string Const_ModelList = "../../../TestXml/ModelList.txt";

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2_ModelDetails_MainWindow()
        {
            InitializeComponent();
            TextEditor_ModPaths.Text = 
@"C:\Game\StarCraft II\Mods\Game\Delphinium_Model_1.0.SC2Mod
C:\Game\StarCraft II\Mods\Game\Delphinium_Model_Patch_1.0.SC2mod";
            if (File.Exists(Const_ModelList))
            {
                StreamReader sr = new StreamReader(Const_ModelList);
                TextEditor_ModelList.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 比较路径
        /// </summary>
        /// <param name="source">源路径</param>
        /// <param name="index">序号</param>
        /// <returns>变体路径</returns>
        public static string GetVariantPath(string source, int index)
        {
            string extension = System.IO.Path.GetExtension(source);
            string newSource = source.Substring(0, source.LastIndexOf('.')) + $"_{index:D2}" + extension;
            return newSource;

        }

        /// <summary>
        /// string列表生成字符串
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns>合并结果</returns>
        private string ListToString(List<string> list)
        {
            StringBuilder builder = new StringBuilder(list[0]);
            for (int i = 1; i < list.Count; i++)
            {
                builder.Append("\r\n");
                builder.Append(list[i]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取Mod路径列表
        /// </summary>
        /// <returns></returns>
        private List<DirectoryInfo> GetModList()
        {
            List<DirectoryInfo> list = new List<DirectoryInfo>();
            List<string> others = new List<string>();
            foreach (string path in TextEditor_ModPaths.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (Directory.Exists(path))
                    {
                        list.Insert(0, new DirectoryInfo(path));
                    }
                    else
                    {
                        others.Add(path);
                    }
                }
            }
            if (others.Count != 0) throw new Exception(ListToString(others));
            return list;
        }

        private List<FileInfo> GetModelList(List<DirectoryInfo> modList)
        {
            List<FileInfo> list = new List<FileInfo>();
            List<string> others = new List<string>();
            foreach (string path in TextEditor_ModelList.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    bool exist = false;
                    foreach (DirectoryInfo dir in modList)
                    {
                        string modelPath = $"{dir.FullName}\\{path}";
                        if (File.Exists(modelPath))
                        {
                            list.Add(new FileInfo(modelPath));
                            exist = true;
                            break;
                        }
                        else
                        {
                            int index = 0;
                            modelPath = GetVariantPath(modelPath, index);
                            while (File.Exists(modelPath))
                            {
                                list.Add(new FileInfo(modelPath));
                                exist = true;
                                index++;
                                modelPath = GetVariantPath(modelPath, index);
                            }
                        }
                    }
                    if (!exist) others.Add(path);
                }
            }
            if (others.Count != 0) throw new Exception(ListToString(others));
            return list;
        }

        #endregion

        #region 控件事件

        /// <summary>
        /// 生成按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_GenerateFileList_Click(object sender, RoutedEventArgs e)
        {
            List<DirectoryInfo> modList = GetModList();
            List<FileInfo> modelPath = GetModelList(modList);
        }
        #endregion
    }
}
