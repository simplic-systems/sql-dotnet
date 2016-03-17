using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.SharpDevelop.Editor;
using Microsoft.Win32;
using SqlDotNet;
using SqlDotNet.Compiler;
using SqlDotNet.Executor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
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

namespace SqlDotNet.Debugger
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IErrorListener
    {
        #region Private Member
        private Label errorMessageLabelNormal;
        private Label errorMessageLabelError;
        private ObservableCollection<ErrorObject> eoList;
        private ITextMarkerService textMarkerService;
        
        private string scriptPath;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            eoList = new ObservableCollection<ErrorObject>();
            errorGridView.ItemsSource = eoList;

            errorMessageLabelNormal = new Label();
            errorMessageLabelNormal.Content = "Error-Messages";
            errorMessageLabelNormal.Padding = new Thickness();

            errorMessageLabelError = new Label();
            errorMessageLabelError.Content = "Error-Messages (x)";
            errorMessageLabelError.Foreground = new SolidColorBrush(Colors.Red);
            errorMessageLabelError.BorderBrush = new SolidColorBrush(Colors.Red);
            errorMessageLabelError.Padding = new Thickness();

            errorMessageTabItem.Header = errorMessageLabelNormal;


            InitializeTextMarkerService();

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SqlDotNet.Debugger.sql.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    codeTextBox.SyntaxHighlighting =
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                        ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }

            try
            {
                scriptPath = System.IO.File.ReadAllText("LastCode.settings");
                codeTextBox.Text = File.ReadAllText(scriptPath);
            }
            catch { }
        }
        #endregion

        #region Public Methods

        #region [InitializeTextMarkerService]
        /// <summary>
        /// Init service
        /// </summary>
        private void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(codeTextBox.Document);
            codeTextBox.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            codeTextBox.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)codeTextBox.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
            {
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            }
            this.textMarkerService = textMarkerService;
        }
        #endregion

        #endregion

        #region [Event Handler]
        #region [Error GridView MouseDoubleClick]
        private void errorGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (errorGridView.SelectedItem != null)
            {
                codeTextBox.Select((errorGridView.SelectedItem as ErrorObject).Index, (errorGridView.SelectedItem as ErrorObject).Length);
            }
        }
        #endregion

        #region [Syntax Tree MouseDoubleClick]
        /// <summary>
        /// Mouse double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tree.SelectedItem != null)
            {
                SyntaxTreeNode node = (tree.SelectedItem as SyntaxTreeNode);

                if (node.Token != null)
                {
                    try
                    {
                        int start = node.Token.Index.Item1;
                        int length = node.Token.Index.Item2 - node.Token.Index.Item1;

                        codeTextBox.Select(start, length);
                        codeTextBox.Focus();
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region [Report]
        /// <summary>
        /// Report errors
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        /// <param name="index"></param>
        /// <param name="endIndex"></param>
        /// <param name="token"></param>
        public void Report(string errorCode, string errorMessage, int index, int endIndex, RawToken token)
        {
            errorMessageTabItem.Header = errorMessageLabelError;

            eoList.Add(new ErrorObject()
                {
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage,
                    Index = index,
                    Length = endIndex - index,
                    Token = token
                });

            ITextMarker marker = textMarkerService.Create(index, endIndex - index);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = System.Windows.Media.Colors.Red;
        }
        #endregion

        /// <summary>
        /// Compile the script in the editor
        /// </summary>
        private void Compile(bool launch = false)
        {
            try
            {
                outputTextBox.Text = "";
                outputTextBox.Text += "----------------------------------------------------------------------";
                outputTextBox.Text += "\r\nCompile ...";

                errorMessageTabItem.Header = errorMessageLabelNormal;
                textMarkerService.RemoveAll(m => true);
                eoList.Clear();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                Sql parser = new Sql(new Executor(), this);
                
                var res = parser.Compile(codeTextBox.Text);
                outputTextBox.Text += "\r\nCompile-Time: " + watch.ElapsedMilliseconds.ToString();

                this.tree.ItemsSource = res.EntryPoint.Children;

                ilCodeTextBox.Text = "";
                if (res.ILCode != null)
                {
                    ilCodeTextBox.Text = Encoding.UTF8.GetString((res.ILCode as MemoryStream).ToArray());
                }

                if (launch)
                {
                    parser.Execute(res, new List<QueryParameter>());
                }
            }
            catch (Exception ex)
            {
                eoList.Add(new ErrorObject()
                {
                    ErrorCode = "Compiler-Error",
                    ErrorMessage = ex.Message,
                    Index = 0,
                    Length = 0,
                    Token = null
                });

                errorMessageTabItem.Header = errorMessageLabelError;
                //errorTextBox.Text = ex.Message;
            }
        }

        #endregion

        private void newFilemenuItem_Click(object sender, RoutedEventArgs e)
        {
            scriptPath = null;
            codeTextBox.Text = "";
        }

        private void openFileItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            var result = dialog.ShowDialog();

            if (result.Value == true)
            {
                scriptPath = dialog.FileName;
                codeTextBox.Text = File.ReadAllText(scriptPath);
            }
        }

        private void saveFileItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*";
                var result = dialog.ShowDialog();

                if (result.Value == true)
                {
                    scriptPath = dialog.FileName;
                    File.WriteAllText(scriptPath, codeTextBox.Text);
                    System.IO.File.WriteAllText("LastCode.settings", scriptPath);
                }
            }
            else
            {
                File.WriteAllText(scriptPath, codeTextBox.Text);
                System.IO.File.WriteAllText("LastCode.settings", scriptPath);
            }
        }

        private void saveFileAsItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            if (!string.IsNullOrWhiteSpace(scriptPath))
            {
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(scriptPath);
                dialog.FileName = System.IO.Path.GetFileNameWithoutExtension(scriptPath);
            }

            dialog.Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*";
            var result = dialog.ShowDialog();

            if (result.Value == true)
            {
                scriptPath = dialog.FileName;
                File.WriteAllText(scriptPath, codeTextBox.Text);
                System.IO.File.WriteAllText("LastCode.settings", scriptPath);
            }
        }

        private void doCompileMenu_Click(object sender, RoutedEventArgs e)
        {
            Compile();
        }

        private void compileAndLaunchMenu_Click(object sender, RoutedEventArgs e)
        {
            Compile(true);
        }
    }
}
