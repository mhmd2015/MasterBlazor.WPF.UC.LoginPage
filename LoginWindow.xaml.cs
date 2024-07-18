using MasterBlazor.WPF.Lib.Auth;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;


namespace MasterBlazor.WPF.UC.LoginPage
{
    public partial class LoginWindow : Window
    {
        bool isLoading = true;
        //string token = "";
        public string siteUrl { get; set; } = string.Empty;

        private AuthManager authManager;
        //public RestManager restManager;

        public event EventHandler<String> LoginCompleted;

        public LoginWindow(string siteUrl = "")
        {
            this.siteUrl = siteUrl;
            InitializeComponent();
            authManager = new AuthManager(siteUrl);

            InitializeWebView();
        }

        async void InitializeWebView()
        {
            if (string.IsNullOrEmpty(siteUrl)) { return; }



            await webView.EnsureCoreWebView2Async(null); // Important for initialization

            webView.Source = new Uri(siteUrl); // Replace


            // Optionally handle NavigationStarting for callback processing
            // ...

            webView.NavigationStarting += async (sender, args) =>
            {
                //if (args.Uri.StartsWith("myapp://auth-callback")) // In case the WPF registered in the ADFS
                if (args.Uri.StartsWith(siteUrl) || args.Uri.StartsWith(siteUrl + "/")) // Check for callback URI
                {
                    if (args.Uri.EndsWith("/_trust"))
                    {
                        return;
                    }
                    if (args.Uri.Contains("Authenticate.aspx")) //sharepoint
                    {
                        return;
                    }
                    if (isLoading)
                    {
                        isLoading = false;
                        return;
                    }
                    args.Cancel = true; // Prevent WebView2 from navigating


                    await authManager.Initialize(webView);


                    string FedAuth = authManager.GetCookie("FedAuth");
                    // After successful login and cookie storage:
                    if (FedAuth != null)
                    {
                        authManager.Save();
                        
                        if (LoginCompleted != null)
                            LoginCompleted?.Invoke(this, "");
                    }
                    isLoading = true;

                }
            };
        }

    }

}
