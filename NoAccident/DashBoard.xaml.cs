using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using HandyControl.Controls;
using Newtonsoft.Json;
using NoAccident.Models;

namespace NoAccident
{
    /// <summary>
    /// DashBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashBoard : UserControl
    {
        public const string APP_CONFIG_FILE = "Config.json";

        public readonly string CONFIG_FULL_PATH = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, APP_CONFIG_FILE);

        public AppConfig Config { get; set; } = null;

        public DashBoard()
        {
            InitializeComponent();

            if (true == File.Exists(CONFIG_FULL_PATH))
            {
                try
                {
                    var fileText = File.ReadAllText(CONFIG_FULL_PATH);
                    Config = JsonConvert.DeserializeObject<AppConfig>(fileText);
                }
                catch
                {
                    Config = new AppConfig();
                    Config.AccidentDateTime = DateTime.Now;
                }
            }
            else
            {
                Config = new AppConfig();
                Config.AccidentDateTime = DateTime.Now;
            }

            SaveConfigFile(CONFIG_FULL_PATH);

            dpCurrent.DisplayTime = DateTime.Now;

            if (Config is null)
            {
                MessageBox.Show("설정이 초기화 되지 않았습니다. AppConfig.json을 확인해주세요");
                return;
            }

            Loaded += (s, e) =>
            {
                Task.Run(async () =>
                {
                    DateTime startTime = DateTime.Now;
                    startTime = Config.AccidentDateTime;

                    this.Dispatcher.Invoke(() =>
                    {
                        btSince.Text = $"Since : {startTime.ToString("yyyy-MM-dd HH:mm:ss")}";

                        tbMaxCount.Text = $"{Config.MaxUsers.ToString("N0")}명";
                        tbMaxDay.Text = $"{Config.MaxDay.ToString("N0")}일";
                    });

                    while (true)
                    {
                        int Currentday = (int)(DateTime.Now - startTime).Days;

                        this.Dispatcher.Invoke(() =>
                        {
                            if (DateTime.Now.Hour > 21 || (DateTime.Now.Hour < 7))

                            {
                                root.Visibility = System.Windows.Visibility.Hidden;
                                tbNight.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                root.Visibility = System.Windows.Visibility.Visible;
                                tbNight.Visibility = System.Windows.Visibility.Hidden;
                            }

                            tbCurrentDay.Text = $"{Currentday}일";

                            if (Config.MaxDay < Currentday)
                            {
                                Config.MaxDay = Currentday;

                                tbMaxDay.Text = $"{Config.MaxDay.ToString("N0")}일";

                                SaveConfigFile(CONFIG_FULL_PATH);
                            }
                        });

                        RunAsync();

                        await Task.Delay(1000);
                    }
                });
            };
        }

        private void SaveConfigFile(string strConfigFile)
        {
            try
            {
                var newConfigFile = JsonConvert.SerializeObject(Config, Formatting.Indented);
                if (newConfigFile != null) File.WriteAllText(strConfigFile, newConfigFile);
            }
            catch
            {
            }
        }

        private async Task RunAsync()
        {
            GetCCU();
        }

        private void GetCCU()
        {
            try
            {
                //string strResult = Get(CCU_URL);
                string strResult = Get(Config.API_URL);
                CCUModel ccu = JsonConvert.DeserializeObject<CCUModel>(strResult);
                var allgames = ccu.List.FindAll(x => x.Name.Contains("Game"));
                var summary = allgames.Sum(x => x.Ccu);
                this.Dispatcher.Invoke(() =>
                {
                    if (summary > 0) tbCurrentCount.Text = $"{summary.ToString("N0")}명";
                });

                if ((int)summary > Config.MaxUsers)
                {
                    Config.MaxUsers = (int)summary;

                    this.Dispatcher.Invoke(() =>
                    {
                        tbMaxCount.Text = $"{summary.ToString("N0")}명";
                    });

                    SaveConfigFile(CONFIG_FULL_PATH);
                }
            }
            catch
            {
                this.Dispatcher.Invoke(() =>
                {
                    tbCurrentCount.Text = $"-";
                });
            }
        }

        public string Get(string request)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(request);
            req.Method = "GET";
            req.ContentType = "application/json";

            WebResponse res = req.GetResponse();
            StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}