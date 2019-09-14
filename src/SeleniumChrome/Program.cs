using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumChrome
{
    class Program
    {
        static void Main(string[] args)
        {
            // ダウンロード場所
            var downloadPath = @"D:\DL";            
            // chromedriver.exeの場所
            var driverPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            
            // chromedriver.exe のログ出力設定
            //  何かあった時に参照する
            //  上書きされるので、残す場合はファイル名にダイムスタンプをつけるとか
            var service = ChromeDriverService.CreateDefaultService(driverPath);
            service.LogPath = @"D:\logs\ChromeDriver.log";
            service.EnableVerboseLogging = false;

            var options = new ChromeOptions();
            
            // ヘッドレスモード
            options.AddArgument("--headless");

            // 通常モード時のダウンロード場所指定
            options.AddUserProfilePreference("download.default_directory", downloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", "false");
            options.AddUserProfilePreference("download.directory_upgrade", "true");

            using (var webDriver = new ChromeDriver(service, options))
            {
                try
                {
                    // ヘッドレスモードモード時のダウンロード場所指定
                    var parameter = new Dictionary<string, object>();
                    parameter["behavior"] = "allow";
                    parameter["downloadPath"] = downloadPath;
                    webDriver.ExecuteChromeCommand("Page.setDownloadBehavior", parameter);

                    // CSVダウンロード操作
                    webDriver.Url = @"http://jusyo.jp/csv/new.php";
                    Debug.WriteLine("表示したよ");
                    var link = webDriver.FindElement(By.XPath("//a[contains(text(), 'csv_tohoku.zip')]"));
                    link.Click();
                    Debug.WriteLine("ダウンロード");                    
                }
                finally
                {
                    // ダウンロード完了を待機。面倒なので 1秒待機
                    //   WebDriverWait を利用したダウンロード完了待ちを作成する？
                    //   ダウンロード中とダウンロード完了を見分けれる？
                    Thread.Sleep(1000);
                    Debug.WriteLine("終了するよ");
                    webDriver.Quit();
                }
            }
        }
    }
}
