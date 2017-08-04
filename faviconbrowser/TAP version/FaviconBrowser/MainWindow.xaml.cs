using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FaviconBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly List<string> s_Domains = new List<string>
                                                             {
																 //"google.com",
																 //"bing.com",
																 //"oreilly.com",
																 //"simple-talk.com",
																 //"microsoft.com",
																 //"facebook.com",
																 //"twitter.com",
																 //"reddit.com",
                                                                 "baidu.com",
                                                                 "bbc.co.uk"
                                                             };

        public MainWindow()
        {
            InitializeComponent();
        }

		// This version executes completion (resumes async AddAFavicon method) depending on the download speed, icons order is incorrect.

		//private async void GetButton_OnClick(object sender, RoutedEventArgs e)
		//{
		//	foreach (string domain in s_Domains)
		//	{
		//		AddAFavicon(domain);
		//	}
		//}

		// Break AddAFavicon method to async GetFavicon and sync ShowFavicon methods:

		//private async void AddAFavicon(string domain)
		//{
		//	WebClient webClient = new WebClient();
		//	byte[] bytes = await webClient.DownloadDataTaskAsync("http://" + domain + "/favicon.ico");
		//	Image imageControl = MakeImageControl(bytes);
		//	m_WrapPanel.Children.Add(imageControl);
		//}

		// We have an added advantage of collecting tasks in the calling code rather than voids as in the case of AddAFavicon() method.
		// Therefore, no exception is retrown in the calling code if one of the tasks returned in the faulted state.
		private async Task<Image> GetFavicon(string domain)
		{
			WebClient webClient = new WebClient();
			byte[] bytes = await webClient.DownloadDataTaskAsync("http://" + domain + "/favicon.ico");
			return MakeImageControl(bytes);
		}

		private void ShowFavicon(Image imageControl)
		{
			m_WrapPanel.Children.Add(imageControl);
		}

		// Here are the updated version of the button click handler.
		// Notice that no sooner than all the tasks that are put in the "results" task have finished, the foreach loop starts executing.
		private async void GetButton_OnClick(object sender, RoutedEventArgs e)
		{
			Task<Image>[] tasks = s_Domains.Select(GetFavicon).ToArray();
			// Create the task that aggregates several tasks by calling a Task.WhenAll(Task[]) combinator.
			Task<Image[]> results = Task.WhenAll(tasks);
			System.Diagnostics.Debug.Print(results.Status.ToString()); // Returns "WaitingForActivation".
			foreach(var image in await results)
			{
				ShowFavicon(image);
			}
		}

        private static Image MakeImageControl(byte[] bytes)
        {
            Image imageControl = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(bytes);
            bitmapImage.EndInit();
            imageControl.Source = bitmapImage;
            imageControl.Width = 16;
            imageControl.Height = 16;
            return imageControl;
        }
    }
}
