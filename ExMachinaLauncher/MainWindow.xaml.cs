using ExMachinaLauncher.Entities;
using ExMachinaLauncher.Properties;
using ExMachinaLauncher.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
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

namespace ExMachinaLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoadModInformationService modEntities = new LoadModInformationService();
        ResourceManager rm = new ResourceManager(typeof(Resources));
        string mainPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
        //string mainPath = "D:\\Program Files (x86)\\Ex Machina's\\Ex Machina";

        public MainWindow()
        {
            InitializeComponent();
            modEntities.CreateModEntityList(mainPath);
            LoadModsToUI();
        }

        private void LoadModsToUI()
        {
            if (modEntities.ModEntityList.Count != 0)
            {
                foreach (var modEntity in modEntities.ModEntityList)
                {
                    ListOfMods.Items.Add(modEntity.Name);
                }
                ModDescription.Text = modEntities.ModEntityList[0].Description;
                ModName.Text = modEntities.ModEntityList[0].Name;
                ModImage.Source = modEntities.ModEntityList[0].ImageParth;
                string[] operetionSistems = new string[] { "Windows 7(8)", "Windows Xp" };
                SelectedOS.ItemsSource = operetionSistems;
                ListOfMods.SelectedIndex = 0;
                SelectedOS.SelectedIndex = 0;
                NameLoadMod.Text = modEntities.ParseTxtStateInfo(mainPath);
            }
            else 
            {
                ModDescription.Text = rm.GetString("NoMods");
                ModName.Text = rm.GetString("NoModsName");
                string[] operetionSistems = new string[] { "Windows 7(8)", "Windows Xp" };
                SelectedOS.ItemsSource = operetionSistems;
                ListOfMods.SelectedIndex = 0;
                SelectedOS.SelectedIndex = 0;
                LoadModBtn.IsEnabled = false;
                UnloadModBtn.IsEnabled = false;
            }
        }

        private void ListOfMods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadViewInfomation();
        }

        private void ReloadViewInfomation()
        {
            ModDescription.Text = modEntities.ModEntityList.First(x => x.Name == (string)ListOfMods.SelectedValue).Description;
            ModName.Text = modEntities.ModEntityList.First(x => x.Name == (string)ListOfMods.SelectedValue).Name;
            ModImage.Source = modEntities.ModEntityList.First(x => x.Name == (string)ListOfMods.SelectedValue).ImageParth;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            GameStarter(false);
        }

        private void StartGameConsole_Click(object sender, RoutedEventArgs e)
        {
            GameStarter(true);
        }

        private void GameStarter(bool consoleIndeficator)
        {
            string parametr = string.Empty;
            string console = string.Empty;
            if ((string)SelectedOS.SelectedValue == "Windows 7(8)")
            {
                parametr = " /AFFINITY 0x2";
            }
            if (consoleIndeficator)
            {
                console = "-console";
            }
            string command = String.Format("{0} {1} {2} {3}", "/C start", parametr, "\"hta\" start.exe", console);

            Process logo = new Process();
            logo.StartInfo.FileName = "DEM_Logo.exe";
            logo.Start();
            logo.WaitForExit();

            Process cmd = new Process();
            cmd.StartInfo.WorkingDirectory = mainPath;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = command;
            cmd.Start();
        }

        private void GoToSite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://deusexmachina.fsay.net/");
        }

        private void LoadModToGame_Click(object sender, RoutedEventArgs e)
        {
            string modeState = String.Empty;
            string warningMassage = String.Empty;

            if (NameLoadMod.Text == "No loaded mods")
            {
                modeState = "1";
                warningMassage = "loadModWarning";
            }
            else
            {
                modeState = "2";
                warningMassage = "loadManyModWarning";
            }

            WarningWindow warningWindow = new WarningWindow(rm.GetString(warningMassage));
            if ((bool)warningWindow.ShowDialog())
            {
                LoadModToGameService loading = new LoadModToGameService();
                loading.Load(modEntities.ModEntityList.First(x => x.Name == (string)ListOfMods.SelectedValue), mainPath);
                File.WriteAllLines(mainPath + "\\mods\\StateInfo.txt", new string[] { "Mode state: " + modeState, "Mod Name: " + (string)ListOfMods.SelectedValue });
                NameLoadMod.Text = modEntities.ParseTxtStateInfo(mainPath);
                InfoWindow infoWindow = new InfoWindow((string)ListOfMods.SelectedValue + rm.GetString("infoLoadWindow"));
                infoWindow.ShowDialog();
            }
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            WarningWindow warningWindow = new WarningWindow(rm.GetString("ClearDataWarning"));
            if ((bool)warningWindow.ShowDialog())
            {
                DeleteModService.CleareData(mainPath);
                NameLoadMod.Text = "No loaded mods";
                InfoWindow infoWindow = new InfoWindow(rm.GetString("infoClean"));
                infoWindow.ShowDialog();
            }
        }

        private void UnloadMod_Click(object sender, RoutedEventArgs e)
        {
            var loadedMod = modEntities.ParseTxtStateInfo(mainPath);

            if (loadedMod.Contains("(!!!)"))
            {
                loadedMod = loadedMod.Replace("(!!!)", string.Empty);
                WarningWindow warningWindow = new WarningWindow(rm.GetString("UnloadManyModExeption"));
                if ((bool)warningWindow.ShowDialog()) UnloadModAction(loadedMod);
            }
            else if (loadedMod.Contains("No loaded mods"))
            {
                InfoWindow infoWindow = new InfoWindow(rm.GetString("infoUnloadModExeption"));
                infoWindow.ShowDialog();
            }
            else
            {
                WarningWindow warningWindow = new WarningWindow(rm.GetString("UnloadSingleModExeption"));
                if ((bool)warningWindow.ShowDialog()) UnloadModAction(loadedMod);

            }
        }

        private void UnloadModAction(string loadedMod)
        {

            var modParth = modEntities.ModEntityList.First(x => x.Name == loadedMod).ModParth;
            DirectoryCopyService.DirectoryCopy(mainPath + "\\data\\profiles", modParth + "\\data\\profiles");
            DeleteModService.CleareData(mainPath);
            NameLoadMod.Text = "No loaded mods";
            InfoWindow infoWindow = new InfoWindow((string)ListOfMods.SelectedValue + rm.GetString("infoUnload"));
            infoWindow.ShowDialog();


        }

        private void BackUpMainProfile_Click(object sender, RoutedEventArgs e)
        {
            MainProfileBtnAction(true);
        }

        private void LoadMainProfile_Click(object sender, RoutedEventArgs e)
        {
            MainProfileBtnAction(false);
        }

        private void MainProfileBtnAction(bool backUpMainProfileWarning)
        {
            string warningMassage = String.Empty;
            string infoMassage = String.Empty;
            if (backUpMainProfileWarning)
            {
                warningMassage = "BackUpProfile";
                infoMassage = "infoBackUpProfile";
            }
            else
            {
                warningMassage = "LoadProfile";
                infoMassage = "infoLoadProfile";
            }

            var loadedMod = modEntities.ParseTxtStateInfo(mainPath);

            if (loadedMod.Contains("No loaded mods"))
            {

                WarningWindow warningWindow = new WarningWindow(rm.GetString(warningMassage));
                if ((bool)warningWindow.ShowDialog())
                {
                    var backUpParth = mainPath + "\\mods\\ProfileBackUp\\profiles";
                    if (backUpMainProfileWarning) DirectoryCopyService.DirectoryCopy(mainPath + "\\data\\profiles", backUpParth);
                    else DirectoryCopyService.DirectoryCopy(backUpParth, mainPath + "\\data\\profiles");
                    InfoWindow infoWindow = new InfoWindow(rm.GetString(infoMassage));
                    infoWindow.ShowDialog();
                }
            }
            else
            {
                InfoWindow infoWindow = new InfoWindow(rm.GetString("errorLoadBackUpProfile"));
                infoWindow.ShowDialog();
            }
        }
    }
}
