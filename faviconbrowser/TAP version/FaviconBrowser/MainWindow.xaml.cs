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
																 "0",
																 null,
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

		private Task<Image> GetFavicon(string domain)
		{
			int numeric;
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			else if (Int32.TryParse(domain, out numeric))
			{
				throw new ArgumentException("domain");
			}
			return GetFaviconInternal(domain);
		}

		private async Task<Image> GetFaviconInternal(string domain)
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
			// This line is for synchronous exception simulation demo.
			//Task<Image>[] tasks = s_Domains.Select(GetFavicon).ToArray();

			Task<Image>[] tasks = s_Domains.Select(GetFaviconInternal).ToArray();
			// Create the task that aggregates several tasks by calling a Task.WhenAll(Task[]) combinator.
			Task<Image[]> results = Task.WhenAll(tasks);
			System.Diagnostics.Debug.Print(results.Status.ToString()); // Returns "WaitingForActivation".
			try
			{
				foreach (var image in await results)
				{
					ShowFavicon(image);
				}
			}
			catch (Exception ex)
			{
				// We can compare what exception is thrown on "await results" completion with the actual exceptions put to the
				// Task<Image[]>.Exception.InnerExceptions list.
				System.Diagnostics.Debug.Print("Thrown on await: {0}", ex);
				foreach (Exception o in results.Exception.InnerExceptions)
				{
					System.Diagnostics.Debug.Print("Put in the inner exception list: {0}", o);
				}
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
