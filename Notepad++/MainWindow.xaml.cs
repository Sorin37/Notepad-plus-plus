using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace Notepad__
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int fileCounter = 1;
        int currentTab = 0;
        ObservableCollection<TabVM> Tabs = new ObservableCollection<TabVM>();

        public object Interaction { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            var tab1 = new TabVM()
            {
                Header = $"New text file {fileCounter}",
                Content = ""
            };
            Tabs.Add(tab1);
            AddNewPlusButton();

            MyTabControl.ItemsSource = Tabs;
            MyTabControl.SelectionChanged += MyTabControl_SelectionChanged;
            TreeViewInit();
        }
        private void TreeViewInit()
        {
            TreeViewItem c = new TreeViewItem();
            c.Header = "C:";
            c.Expanded += new RoutedEventHandler(OnExpanded);
            TreeViewItem toCollapse = new TreeViewItem();
            toCollapse.Header = "toCollapse";
            c.Items.Add(toCollapse);
            c.Tag = "C:\\";
            c.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(Tabs[0].CounterTheme);
            TreeView.Items.Add(c);
            TreeViewItem d = new TreeViewItem();
            d.Header = "D:";
            d.Expanded += new RoutedEventHandler(OnExpanded);
            d.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(Tabs[0].CounterTheme);
            TreeViewItem toCollapsed = new TreeViewItem();
            toCollapsed.Header = "toCollapse";
            d.Items.Add(toCollapsed);
            d.Tag = "D:\\";
            TreeView.Items.Add(d);
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem currentItem = e.Source as TreeViewItem;
            if ((File.GetAttributes(currentItem.Tag.ToString()) & FileAttributes.Directory) == FileAttributes.Directory)
                if ((currentItem.Items[0] as TreeViewItem).Header.ToString() == "toCollapse")
                {
                    currentItem.Items.Remove(currentItem.Items[0]);
                    string[] directories = Directory.GetDirectories(currentItem.Tag.ToString());
                    foreach (string directory in directories)
                    {
                        TreeViewItem newItem = new TreeViewItem();
                        newItem.Header = System.IO.Path.GetFileName(directory);
                        newItem.Tag = directory;
                        newItem.Expanded += OnExpanded;
                        TreeViewItem toCollapse = new TreeViewItem();
                        toCollapse.Header = "toCollapse";
                        newItem.Items.Add(toCollapse);
                        newItem.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(Tabs[0].CounterTheme);
                        currentItem.Items.Add(newItem);
                    }

                    string[] files = Directory.GetFiles(currentItem.Tag.ToString());
                    foreach (string file in files)
                    {
                        TreeViewItem currentFile = new TreeViewItem();
                        currentFile.Header = System.IO.Path.GetFileName(file);
                        currentFile.Tag = file;
                        currentFile.MouseDoubleClick += TreeViewItem_MouseDoubleClick;
                        currentFile.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(Tabs[0].CounterTheme);
                        currentItem.Items.Add(currentFile);
                    }
                }
        }

        private void MyTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                var pos = MyTabControl.SelectedIndex;
                if (pos != 0 && pos == Tabs.Count - 1) //last tab
                {
                    var tab = Tabs.Last();
                    ConvertPlusToNewTab(tab);
                    AddNewPlusButton();
                }
                currentTab = pos;
            }


        }

        void ConvertPlusToNewTab(TabVM tab)
        {
            fileCounter++;
            tab.Header = $"New text file {fileCounter}";
            tab.IsPlaceholder = false;
            tab.Content = "";
            tab.Theme = Tabs[0].Theme;
            tab.CounterTheme = Tabs[0].CounterTheme;
        }

        void AddNewPlusButton()
        {
            var plusTab = new TabVM()
            {
                Header = "+",
                IsPlaceholder = true,
                Theme = "",
                CounterTheme = ""
            };
            Tabs.Add(plusTab);
        }



        private void OnTabCloseClick(object sender, RoutedEventArgs e)
        {
            var tab = (sender as Button).DataContext as TabVM;
            if (Tabs.Count > 2)
            {
                var index = Tabs.IndexOf(tab);
                if (Tabs[index].Color == "Red")
                {
                    System.Windows.Forms.MessageBoxButtons buttons = System.Windows.Forms.MessageBoxButtons.YesNo;
                    System.Windows.Forms.DialogResult result =
                        System.Windows.Forms.MessageBox.Show("Do you want to save this file?",
                                                             "This file is not saved.",
                                                             buttons);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        Button_Click_Save(sender, e);
                    }
                }
                if (index == Tabs.Count - 2)//last tab before [+]
                {
                    MyTabControl.SelectedIndex--;
                }
                Tabs.RemoveAt(index);
            }
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (Tabs[currentTab].Path != "")
            {
                File.WriteAllText(Tabs[currentTab].Path, Tabs[currentTab].Content);
            }
            else
            {
                Button_Click_SaveAs(sender, e);
            }
            Tabs[currentTab].Color = "Black";
        }
        private void Button_Click_SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, Tabs[currentTab].Content);
                Tabs[currentTab].Path = sfd.FileName;
            }
            Tabs[currentTab].Color = "Black";
        }
        private void Button_Click_ChangeName(object sender, RoutedEventArgs e)
        {
            string oldHeader = Tabs[currentTab].Header;
            Tabs[currentTab].Header = Microsoft.VisualBasic.Interaction.
                InputBox("Please input the new name:",
                         "Change name",
                         "New name");
            Tabs[currentTab].Header += ".txt";
            if (Tabs[currentTab].Path != "")
            {
                Tabs[currentTab].Path = Tabs[currentTab].Path.Replace(oldHeader, Tabs[currentTab].Header);
            }
        }

        private void Button_Click_OpenFile(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text File|*.txt|All files|*.*";
            if (ofd.ShowDialog() == true)
            {
                Tabs[currentTab].Content = File.ReadAllText(ofd.FileName);
                Tabs[currentTab].Path = ofd.FileName;
                Tabs[currentTab].Header = ofd.FileName.Substring(ofd.FileName.LastIndexOf('\\') + 1);
            }
            Tabs[currentTab].Color = "Black";
        }
        void Window_Closing(object sender, CancelEventArgs e)
        {
            bool unsavedFiles = false;
            foreach (var tab in Tabs)
            {
                if (tab.Color == "Red")
                {
                    unsavedFiles = true;
                    break;
                }
            }
            if (unsavedFiles)
            {
                System.Windows.Forms.MessageBoxButtons buttons = System.Windows.Forms.MessageBoxButtons.YesNo;
                System.Windows.Forms.DialogResult result =
                    System.Windows.Forms.MessageBox.Show("Do you want to save the files?",
                                                         "You have unsaved files.",
                                                         buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Process.Start("shutdown", "/s /t 0");
            Close();
        }
        private void Button_Click_NewFile(object sender, RoutedEventArgs e)
        {

            ConvertPlusToNewTab(Tabs[Tabs.Count - 1]);
            AddNewPlusButton();
        }
        private void OpenURL(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo("chrome.exe");
            psi.Arguments = "https://www.unitbv.ro/";
            Process.Start(psi);
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Tabs[currentTab].Content = File.ReadAllText((sender as TreeViewItem).Tag.ToString());
            Tabs[currentTab].Path = (sender as TreeViewItem).Tag.ToString();
            Tabs[currentTab].Header = System.IO.Path.GetFileName((sender as TreeViewItem).Tag.ToString());
        }

        private void MenuFind(object sender, RoutedEventArgs e)
        {
            string wordToFind = Microsoft.VisualBasic.Interaction.
               InputBox("What word would you like to search?",
                        "Find...",
                        "");
            if (wordToFind != "")
            {
                Tabs[currentTab].Content = Tabs[currentTab].Content.Replace(wordToFind, $"*{wordToFind}*");

            }
        }
        private void MenuReplace(object sender, RoutedEventArgs e)
        {
            string result;
            result = Microsoft.VisualBasic.Interaction.
               InputBox("What word would you like to replace?",
                        "Replace...",
                        "*Word to be replaced* *Word to replace with*");
            if (result != "")
            {
                string toBeReplaced = result.Substring(0, result.IndexOf(' ')),
                       toReplace = result.Substring(result.IndexOf(' ') + 1);
                Tabs[currentTab].Content = Tabs[currentTab].Content.Replace(toBeReplaced, toReplace);
            }
        }
        private void MenuReplaceAll(object sender, RoutedEventArgs e)
        {
            string result;
            result = Microsoft.VisualBasic.Interaction.
               InputBox("What word would you like to replace?",
                        "Replace in all files...",
                        "*Word to be replaced* *Word to replace with*");
            if (result != "")
            {
                string toBeReplaced = result.Substring(0, result.IndexOf(' ')),
                       toReplace = result.Substring(result.IndexOf(' ') + 1);
                for (int i = 0; i < Tabs.Count - 1; i++)
                {
                    Tabs[i].Content = Tabs[i].Content.Replace(toBeReplaced, toReplace);
                }
            }
        }
        public void ClickCut(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ceva");
        }
        public void Button_Click_ChangeTheme(object sender, RoutedEventArgs e)
        {
            for (int i =0;i<Tabs.Count-1;i++)
            {
                string aux = Tabs[i].Theme;
                Tabs[i].Theme = Tabs[i].CounterTheme;
                Tabs[i].CounterTheme = aux;
            }
            TreeView.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Tabs[0].Theme);
            MyMenu.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Tabs[0].Theme);
            MyMenu.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString(Tabs[0].CounterTheme);
            MyTabControl.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(Tabs[0].Theme);
            TreeView.Items.Clear();
            TreeViewInit();
        }
    }
}
