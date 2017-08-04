using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
                                                                 "google.com",
                                                                 "bing.com",
                                                                 "oreilly.com",
                                                                 "simple-talk.com",
                                                                 "microsoft.com",
                                                                 "facebook.com",
                                                                 "twitter.com",
                                                                 "reddit.com",
                                                                 "baidu.com",
                                                                 "bbc.co.uk"
                                                             };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetButton_OnClick(object sender, RoutedEventArgs e)
        {
			//foreach (string domain in s_Domains)
			//{
			//	AddAFavicon(domain);
			//}
			AddFaviconsOrdered(s_Domains, 0);
        }

		// Shared completion callback function - favicons appear not in order, but according to the download speed.
        private void AddAFavicon(string domain)
        {
			// You must create a new instance of a WebClient to be able to complete in a background.
			WebClient webClient = new WebClient();
			//byte[] bytes = webClient.DownloadData("http://" + domain + "/favicon.ico");
			//Image imageControl = MakeImageControl(bytes);
			//m_WrapPanel.Children.Add(imageControl);
			webClient.DownloadDataCompleted += OnWebClientDownloadDataCompleted;
			webClient.DownloadDataAsync(new Uri("http://" + domain + "/favicon.ico"));
        }

		// Anonimous callback function and recursion - correct order of favicons 
		private void AddFaviconsOrdered(List<string> domains, int index)
		{
			WebClient webClient = new WebClient();
			// No sooner is the new backgrownd operation started than the previous favicon is downloaded.
			webClient.DownloadDataCompleted += (sender, args) =>
			{
				Image imageControl = MakeImageControl(args.Result);
				m_WrapPanel.Children.Add(imageControl);

				// If we have just added the last image to the panel, no new recursive call.
				if (++index != domains.Count)
				{
					AddFaviconsOrdered(domains, index);
				}
			};
			// Initiate the background operation.
			webClient.DownloadDataAsync(new Uri("http://" + domains[index] + "/favicon.ico"));
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

		private void OnWebClientDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs args)
		{
			Image imageControl = MakeImageControl(args.Result);
			m_WrapPanel.Children.Add(imageControl);
		}
    }
}
