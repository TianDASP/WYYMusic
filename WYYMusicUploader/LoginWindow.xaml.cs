using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WYYMusicUploader
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window ,INotifyPropertyChanged
    {
        public delegate void GetTokenHandler(string value1); //声明委托
        public GetTokenHandler getTokenHandler;                //委托对象
        public string Account { get; set; }
        public string Pwd { get; set; }
        public HttpClient client { get; set; }  
        public LoginWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Account = this.account.Text;
            Pwd = this.pwd.Password;
            var logincls = new { userName = Account, password = Pwd };
            var httpContent = new StringContent(JsonSerializer.Serialize(logincls), Encoding.UTF8, "application/json");
            Uri url = new Uri("http://localhost:5119/api/Login/LoginByUserNameAndPwd");
            var response = await client.PostAsync(url, httpContent);
            if (response.IsSuccessStatusCode)
            {
                string token = await response.Content.ReadAsStringAsync(); 
                getTokenHandler(token);
                this.Close();
            }
            else
            {
                MessageBox.Show("登录失败,请检查用户名和密码!");
            }
        }
    }
}
