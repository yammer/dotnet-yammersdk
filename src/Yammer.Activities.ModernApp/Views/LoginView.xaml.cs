using Caliburn.Micro;
// ***********************************************************************
// Provided for Informational Purposes Only
//
// Apache 2.0 License
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may
// not use this file except in compliance with the License. You may obtain
// a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY 
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR 
// PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.
// ***********************************************************************
// Assembly  : Yammer.Activities.ModernApp
// File      : LoginView.xaml.cs
//
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Navigation;
using Yammer.Activities.ModernApp.Common;
using Yammer.Activities.ModernApp.Models;

namespace Yammer.Activities.ModernApp.Views
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class LoginView : Yammer.Activities.ModernApp.Common.LayoutAwarePage	, IHandle<BrowserMessage>
	{
		private readonly IEventAggregator _eventAggregator;

		public LoginView()
		{
			this.InitializeComponent();
			_eventAggregator= App.EventAggregator;
			_eventAggregator.Subscribe(this);
		}

		/// <summary>
		/// Populates the page with content passed during navigation.  Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="navigationParameter">The parameter value passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
		/// </param>
		/// <param name="pageState">A dictionary of state preserved by this page during an earlier
		/// session.  This will be null the first time a page is visited.</param>
		protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
		{
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
		protected override void SaveState(Dictionary<String, Object> pageState)
		{
		}


		public void Handle(BrowserMessage message)
		{
            WebBrowser.ContentLoading += WebBrowser_ContentLoading;
			WebBrowser.LoadCompleted += WebBrowserOnLoadCompleted;
            WebBrowser.NavigationStarting += WebBrowser_NavigationStarting;
            WebBrowser.FrameDOMContentLoaded += WebBrowser_FrameDOMContentLoaded;
            WebBrowser.Navigate(message.NavigateUri);
            
		}

        void WebBrowser_FrameDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (_codeSent)
                return;
            var browser = sender as WebView;
            if (string.IsNullOrEmpty(browser.Source.Query) || !browser.Source.Query.Contains("code"))
            {
                UpdateWebViewState(false);
                return;
            }
        }

        void WebBrowser_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            if (_codeSent)
                return;
            var browser = sender as WebView;
            if (string.IsNullOrEmpty(browser.Source.Query) || !browser.Source.Query.Contains("code"))
            {
                UpdateWebViewState(false);
                return;
            }
        }

        void WebBrowser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (_codeSent)
                return;
            var browser = sender as WebView;
            if (string.IsNullOrEmpty(browser.Source.Query) || !browser.Source.Query.Contains("code"))
            {
                UpdateWebViewState(false);
                return;
            }
            var codeMatch = Regex.Match(browser.Source.Query, "code=(?<code>.+?)$");
        }
										  
		// Helps prevent duplicated events
		private volatile bool _codeSent = false;
		private void WebBrowserOnLoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
		{
			if (_codeSent)
				return;
			
			
			var browser = sender as WebView;
			browser = browser ?? WebBrowser;
			if (string.IsNullOrEmpty(browser.Source.Query) || !browser.Source.Query.Contains("code"))
			{
				UpdateWebViewState(false);
				return;
			}

            AnalyzeQueryString(WebBrowser.Source.Query);
		}
        private void AnalyzeQueryString(string query)
        {
            var message = default(ApplicationValidationMessage);
            var codeMatch = Regex.Match(query, "code=(?<code>.+?)$");
            if (codeMatch.Success)
            {
                UpdateWebViewState(true);
                var code = codeMatch.Groups["code"].Value;
                message = new ApplicationValidationMessage(code);
            }
            else
            {
                message = new ApplicationValidationMessage(null);
            }
            _codeSent = true;
            _eventAggregator.Publish(message);
        }
		private void UpdateWebViewState(bool hide)
		{
		    WeBorder.IsHitTestVisible = true;
		    WebBrowser.IsHitTestVisible = true;
			WebBrowser.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            CredentialsBorder.Visibility = hide ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
