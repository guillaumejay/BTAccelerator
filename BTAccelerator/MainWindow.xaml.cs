using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BTAccelerator.jsonDefinitions;

namespace BTAccelerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ConstantsSubDir = @"BattleTech_Data\StreamingAssets\data\constants";
        public MainWindow()
        {
        
            InitializeComponent();

            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        if (sk.GetValue("DisplayName") as string == "BATTLETECH")
                        {
                            _BaseDirectory = sk.GetValue("InstallLocation") as string;
                        }
                    }
                }
            }
        }

        private string _BaseDirectory = @"E:\Games\SteamLibrary\steamapps\common\BATTLETECH";

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            double multiplier;
            try
            {
                multiplier = double.Parse(txtMultiplier.Text);
            }
            catch (Exception)
            {
                MessageBox.Show($"Can't understand multiplier entry: {txtMultiplier.Text}");
                return;
            }

            string movementSubdir = @"BattleTech_Data\StreamingAssets\data\movement";
            string[] jsonFilenames;
            try
            {
                jsonFilenames = Directory.GetFiles($"{_BaseDirectory}\\{movementSubdir}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"JSON file list failed: {ex.ToString()}");
                return;
            }

            foreach (string jsonFilename in jsonFilenames.Where(x=>x.ToLower().EndsWith("json")))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MechMovement));
                MechMovement mech;
                using (FileStream fs = new FileStream(jsonFilename, FileMode.Open))
                {
                     mech = serializer.ReadObject(fs) as MechMovement;
                    if (mech == null)
                    {
                        MessageBox.Show($"Tried to edit a non mech movement file {Path.GetFileName(jsonFilename)}, bailing");
                        return;
                    }

                    string backup = Path.ChangeExtension(jsonFilename, "bak");
                
                    if (!File.Exists(backup))
                        File.Copy(jsonFilename,backup);
                        
                        
                        
                        
                        
                    
                    mech.WalkVelocity *= multiplier;
                    mech.RunVelocity *= multiplier;
                    mech.SprintVelocity *= multiplier;
                    mech.LimpVelocity *= multiplier;
                    mech.WalkAcceleration *= multiplier;
                    mech.SprintAcceleration *= multiplier;
                    
                    
                }
                //serializer.WriteObject(fs, mech);
                string content = new JavaScriptSerializer().Serialize(mech);
            File.WriteAllText(jsonFilename,JsonFormatter.FormatOutput(content));
                
            }
        }

        private void btnUpdateSoundDelay_Click(object sender, RoutedEventArgs e)
        {
            string audioConstantFile = Path.Combine(_BaseDirectory,ConstantsSubDir, "AudioConstants.json");
            string content = File.ReadAllText(audioConstantFile);
            AudioConstants ac = new JavaScriptSerializer().Deserialize<AudioConstants>(content);
            string backup = Path.ChangeExtension(audioConstantFile, "bak");
            if (!File.Exists(backup))
                File.Copy(audioConstantFile, backup);
            ac.AttackPreFireDuration = 0;
            ac.AttackAfterFireDelay = 0;
            ac.AttackPreFireDuration = 0;
            ac.AttackAfterCompletionDuration = 0;
            ac.audioFadeDuration = 0;
            content = new JavaScriptSerializer().Serialize(ac);
            File.WriteAllText(audioConstantFile,JsonFormatter.FormatOutput( content));
        }
    }
}
