using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace SC2_ModelDetails
{
    /// <summary>
    /// SC2_ModelDetails_MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SC2_ModelDetails_MainWindow : Window
    {
        #region 常量声明

        public const string Const_CDPath = "cd ../../../TestM3/m3addon";
        public const string Const_PythonPath = "py -3 m3ToXml.py ";
        public const string Const_NameFailList = "FailedModel.log";
        public const string Const_NameSuccessList = "SuccessModel.log";
        public static readonly Regex Const_RegexTextureinDetails = new Regex("(?<=<imagePath>)([^\\/:*?\"<>|\\r\\n]+[\\/])*[^\\/:*?\"<>|\\r\\n]+(?=</imagePath>)", RegexOptions.Compiled);

        #endregion

        #region 属性字段

        /// <summary>
        /// 模型列表
        /// </summary>
        public List<FileInfo> ModelList { set; get; }

        /// <summary>
        /// 贴图列表
        /// </summary>
        public List<string> TextureList { set; get; }

        /// <summary>
        /// 基本路径长度
        /// </summary>
        public int BasePathLength { set; get; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2_ModelDetails_MainWindow()
        {
            InitializeComponent();
            TextBox_BasePath.Text = @"C:\Game\StarCraft II\Mods\Game\";
            TextEditor_ModPaths.Text = 
@"Delphinium_Model_1.0.SC2Mod
Delphinium_Model_Patch_1.0.SC2mod";
        }

        #endregion

        #region 方法

        /// <summary>
        /// 读取模型详情
        /// </summary>
        /// <param name="file">模型</param>
        /// <param name="textureList">贴图列表</param>
        public static void ReadModelDetails(FileInfo file, ref List<string> textureList)
        {
            StreamReader sr = new StreamReader(file.FullName);
            string details = sr.ReadToEnd();
            sr.Close();
            MatchCollection matchs = Const_RegexTextureinDetails.Matches(details);
            foreach (Match match in matchs)
            {
                textureList.Add(match.Value.Replace('/', '\\'));
            }
        }

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
                    string fullPath = TextBox_BasePath.Text + path;
                    if (Directory.Exists(fullPath))
                    {
                        list.Insert(0, new DirectoryInfo(fullPath));
                    }
                    else
                    {
                        others.Add(fullPath);
                    }
                }
            }
            if (others.Count != 0) throw new Exception(ListToString(others));
            BasePathLength = TextBox_BasePath.Text.Length;
            return list;
        }

        /// <summary>
        /// 获取存在的文件
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="subIndex">截取位置</param>
        /// <param name="modelList">模型列表</param>
        /// <param name="textureList">贴图列表</param>
        private void GetExistFiles(DirectoryInfo dir, int subIndex, ref List<FileInfo> modelList, ref List<string> textureList)
        {
            foreach (FileInfo childFile in dir.GetFiles())
            {
                if (childFile.Extension == ".m3")
                {
                    modelList.Add(childFile);
                }
                if (childFile.Extension == ".dds" || childFile.Extension == ".tga")
                {
                    textureList.Add(childFile.FullName.Substring(subIndex));
                }
            }
            foreach (DirectoryInfo childDir in dir.GetDirectories())
            {
                GetExistFiles(childDir, subIndex, ref modelList, ref textureList);
            }
        }

        /// <summary>
        /// 通过脚本获取贴图
        /// </summary>
        /// <param name="mod">模型文件名</param>
        /// <param name="length">基本路径长度</param>
        /// <param name="faildList">失败文件列表</param>
        /// <returns>贴图列表</returns>
        private void CallPythonScriptGetTexture(FileInfo file, int length, ref List<string> successList, ref List<string> faildList)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            process.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            process.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            process.StartInfo.CreateNoWindow = true;//不显示程序窗口
            process.Start();//启动程序
            //向cmd窗口发送输入信息
            process.StandardInput.WriteLine(Const_CDPath);
            process.StandardInput.AutoFlush = true;
            DirectoryInfo dir = new DirectoryInfo("Temp\\" + file.DirectoryName.Substring(length));
            if (!dir.Exists) dir.Create();
            string msg = $"{Const_PythonPath}" + "\"" + file.FullName + "\" -o \"" + dir.FullName + "\" & exit";
            process.StandardInput.WriteLine(msg);
            process.StandardInput.AutoFlush = true;
            process.StandardOutput.ReadLine();
            process.StandardOutput.ReadLine();
            process.StandardOutput.ReadLine();
            string log = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            string xmlPath = $"{dir.FullName}\\{file.Name}";
            StreamWriter streamWriter = new StreamWriter($"{xmlPath}.log");
            streamWriter.Write(log);
            streamWriter.Close();
            string result = log.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Last();
            if (!result.StartsWith("1 files found, 1 converted"))
            {
                faildList.Add(file.FullName);
            }
            else
            {
                successList.Add($"{xmlPath}.xml");
            }
        }

        #endregion

        #region 控件事件

        /// <summary>
        /// 生成文件按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_GenerateFileList_Click(object sender, RoutedEventArgs e)
        {
            List<DirectoryInfo> modList = GetModList();
            List<FileInfo> modelList = new List<FileInfo>();
            List<string> textureList = new List<string>();
            foreach (DirectoryInfo dir in modList)
            {
                GetExistFiles(dir, dir.FullName.Length + 1, ref modelList, ref textureList);
            }
            ModelList = modelList;
            TextureList = textureList;
            TextEditor_ModelList.Text = ListToString(modelList.Select(r=>r.FullName).ToList());
            TextEditor_InsideTextureList.Text = ListToString(textureList);
        }

        /// <summary>
        /// 生成模型Xml按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_GenerateModelXml_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("Temp")) Directory.Delete("Temp", true);
            List<string> faildList = new List<string>();
            List<string> successList = new List<string>();
            Parallel.ForEach(ModelList, (file) =>
            {
                CallPythonScriptGetTexture(file, BasePathLength, ref successList, ref faildList);
            });
            StreamWriter streamWriter = new StreamWriter(Const_NameFailList, false);
            streamWriter.Write(ListToString(faildList));
            streamWriter.Close();
            streamWriter = new StreamWriter(Const_NameSuccessList, false);
            streamWriter.Write(ListToString(successList));
            streamWriter.Close();
            MessageBox.Show("生成模型Xml完成！");
        }

        /// <summary>
        /// 生成使用贴图列表按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_GenerateUseTextureList_Click(object sender, RoutedEventArgs e)
        {
            StreamReader streamReader = new StreamReader(Const_NameSuccessList);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            string[] pathArray = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<FileInfo> xmlList = new List<FileInfo>();
            foreach (string path in pathArray)
            {
                FileInfo file = new FileInfo(path);
                if (!file.Exists) throw new Exception();
                xmlList.Add(file);
            }
            List<string> textureList = new List<string>();
            Parallel.ForEach(xmlList, (file) =>
            {
                ReadModelDetails(file, ref textureList);
            });
            textureList = textureList.GroupBy(r => r).Select(r => r.First()).ToList();
            TextEditor_ResultTextureList.Text = ListToString(textureList);
            MessageBox.Show("生成使用贴图完成！");
        }

        /// <summary>
        /// 生成缺少贴图列表按钮点击事件
        /// </summary>
        /// <param name="sender">响应对象</param>
        /// <param name="e">响应参数</param>
        private void Button_GenerateLostTextureList_Click(object sender, RoutedEventArgs e)
        {
           
        }
        #endregion

    }
}
