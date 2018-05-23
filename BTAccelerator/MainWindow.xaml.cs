using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.UI;
using System.Windows;
using System.Windows.Controls;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string LatestTag { get; set; }

        public Visibility CanUpdate => LatestTag != GithubTag? Visibility.Hidden:Visibility.Visible;
        public Visibility HasUpdate => LatestTag == GithubTag ? Visibility.Hidden : Visibility.Visible;
        private string GithubUrl => "https://github.com/guillaumejay/BTAccelerator/releases/latest";
        public string GithubTag { get; set; }
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private string ConstantsSubDir = @"BattleTech_Data\StreamingAssets\data\constants";
        DataContractJsonSerializer MechMovementSerializer = new DataContractJsonSerializer(typeof(MechMovement));
        public MainWindow()
        {
            InitializeComponent();
            Version assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}";
            GithubTag = "v" + version;
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
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response =  client.GetAsync(GithubUrl).Result;
            response.EnsureSuccessStatusCode();
            string responseUri = response.RequestMessage.RequestUri.ToString();
            int pos = responseUri.LastIndexOf("/");
            LatestTag = responseUri.Substring(pos + 1);

            OnPropertyChanged(nameof(HasUpdate));
            OnPropertyChanged(nameof(CanUpdate));
        }

       

        private string _BaseDirectory = @"E:\Games\SteamLibrary\steamapps\common\BATTLETECH";
        private string _status;

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

            var jsonFilenames = GetAllMovementFiles();
            if (jsonFilenames == null)
                return;
            int modified = 0;
            foreach (string jsonFilename in jsonFilenames.Where(x => x.ToLower().EndsWith("json")))
            {

                MechMovement mech = LoadMechMovement(jsonFilename);
                if (mech == null)
                {
                    break;
                }

                string backup = Path.ChangeExtension(jsonFilename, "bak");

                if (!File.Exists(backup) || string.IsNullOrWhiteSpace(mech.Accelerated))
                    File.Copy(jsonFilename, backup, true);

                mech.WalkVelocity *= multiplier;
                mech.RunVelocity *= multiplier;
                mech.SprintVelocity *= multiplier;
                mech.LimpVelocity *= multiplier;
                mech.WalkAcceleration *= multiplier;
                mech.SprintAcceleration *= multiplier;
                mech.Accelerated = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

                string content = new JavaScriptSerializer().Serialize(mech);
                File.WriteAllText(jsonFilename, JsonFormatter.FormatOutput(content));
                modified++;
            }
            Status = $"{modified} mech movement file{(modified > 1 ? "s" : "")} modified";
        }

        private MechMovement LoadMechMovement(string jsonFilename)
        {
            using (FileStream fs = new FileStream(jsonFilename, FileMode.Open))
            {
                var mech = MechMovementSerializer.ReadObject(fs) as MechMovement;
                if (mech == null)
                {
                    MessageBox.Show(
                        $"Tried to edit a non mech movement file {Path.GetFileName(jsonFilename)}, bailing");
                    return null;
                }
                return mech;
            }
        }

        private void btnOriginalMovement_Click(object sender, RoutedEventArgs e)
        {
            int restored = 0;
            var jsonFilenames = GetAllMovementFiles();
            if (jsonFilenames == null)
                return;
            foreach (string backupFile in jsonFilenames.Where(x => x.ToLower().EndsWith("bak")))
            {
                string original = Path.ChangeExtension(backupFile, "json");
                var mech = LoadMechMovement(original);
                if (string.IsNullOrWhiteSpace(mech.Accelerated))
                    continue; // original file has not been accelerated
                              //if (File.Exists(original))
                              //   File.Delete(original);
                File.Copy(backupFile, original, true);
                restored++;
            }

            Status = $"{restored} mech movement file{(restored > 1 ? "s" : "")} restored";
        }

        private string[] GetAllMovementFiles()
        {
            string movementSubdir = @"BattleTech_Data\StreamingAssets\data\movement";
            string[] jsonFilenames;
            try
            {
                jsonFilenames = Directory.GetFiles($"{_BaseDirectory}\\{movementSubdir}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Movement file list failed: {ex.ToString()}");
                return null;
            }
            return jsonFilenames;
        }

        string AudioConstantFile => Path.Combine(_BaseDirectory, ConstantsSubDir, "AudioConstants.json");
        private void btnUpdateSoundDelay_Click(object sender, RoutedEventArgs e)
        {
            AudioConstants ac = LoadAudioConstantFile();
            string backup = Path.ChangeExtension(AudioConstantFile, "bak");
            if (!File.Exists(backup))
                File.Copy(AudioConstantFile, backup);
            ac.AttackPreFireDuration = 0;
            ac.AttackAfterFireDelay = 0;
            ac.AttackPreFireDuration = 0;
            ac.AttackAfterCompletionDuration = 0;
            ac.audioFadeDuration = 0;
            ac.Accelerated = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

            SaveAudioContantsFile(ac);
            Status = $"Sound delay modified";
        }

        private void SaveAudioContantsFile(AudioConstants ac)
        {
            string content = new JavaScriptSerializer().Serialize(ac);
            File.WriteAllText(AudioConstantFile, JsonFormatter.FormatOutput(content));
        }

        private AudioConstants LoadAudioConstantFile()
        {
            string content = File.ReadAllText(AudioConstantFile);
            var ac = new JavaScriptSerializer().Deserialize<AudioConstants>(content);
            return ac;
        }

        private void btnOriginalSoundDelay_Click(object sender, RoutedEventArgs e)
        {
            AudioConstants ac = LoadAudioConstantFile();
            ac.AttackPreFireDuration = 1;
            ac.AttackAfterFireDelay = 0.5;
            ac.AttackPreFireDuration = 1;
            ac.AttackAfterCompletionDuration = 2;
            ac.audioFadeDuration = 2.5;
            ac.Accelerated = null;
            SaveAudioContantsFile(ac);
            Status = $"Sound delay restored";
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtMultiplier.Text = 2.0.ToString("0.0");
            Status = "Waiting for orders";
            Title = "BTAccelerator " + GithubTag;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void SelectMovement(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            tb?.SelectAll();
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
