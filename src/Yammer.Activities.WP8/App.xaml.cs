using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Yammer.Oss.Api;
using Yammer.Oss.Api.OAuth;
using Yammer.Oss.Api.Utils;

namespace Yammer.Activities
{
	public partial class App : Application
	{
		//public const string StagingUri = "https://www.staging.yammer.com";
		//public const string StagingUploadUri = "https://files.staging.yammer.com";

		//public const string ProductionUri = "https://www.yammer.com";
		//public const string ProductionUploadUri = "https://files2.yammer.com";

		/// <summary>
		/// Provides easy access to the root frame of the Phone Application.
		/// </summary>
		/// <returns>The root frame of the Phone Application.</returns>
		public static PhoneApplicationFrame RootFrame { get; private set; }

		/// <summary>
		/// Constructor for the Application object.
		/// </summary>
		public App()
		{
			// Global handler for uncaught exceptions.
			UnhandledException += Application_UnhandledException;

			// Standard XAML initialization
			InitializeComponent();

			// Show graphics profiling information while debugging.
			if (Debugger.IsAttached)
			{
				// Display the current frame rate counters.
				Application.Current.Host.Settings.EnableFrameRateCounter = true;

				// Show the areas of the app that are being redrawn in each frame.
				//Application.Current.Host.Settings.EnableRedrawRegions = true;

				// Enable non-production analysis visualization mode,
				// which shows areas of a page that are handed off to GPU with a colored overlay.
				//Application.Current.Host.Settings.EnableCacheVisualization = true;

				// Prevent the screen from turning off while under the debugger by disabling
				// the application's idle detection.
				// Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
				// and consume battery power when the user is not using the phone.
				PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
			}

			// Expose EventAggregator as a static member of the App class
			_bootstrapper = Application.Current.Resources["YammerWp8Bootstrapper"] as YammerWp8Bootstrapper;
			EventAgregator = _bootstrapper.EventAggregator;
#if FALSE
			var service = new FirstFloor.XamlSpy.XamlSpyService
			{
				RemoteAddress = "169.254.80.80",
				RemotePort = 4530,
				Password = "iloveyammer"
			};
			this.ApplicationLifetimeObjects.Add(service);
#endif
			_bootstrapper.RegisterConfigurationObject(MyOAuthClientInfo, MyYammerUris);
			InitializePhoneApplication();
		}
		

		private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			if (Debugger.IsAttached)
			{
				// A navigation has failed; break into the debugger
				Debugger.Break();
			}
		}

		// Code to execute on Unhandled Exceptions
		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if(Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				Debugger.Break();
			}
		}

		// Code to execute when the application is launching (eg, from Start)
		// This code will not execute when the application is reactivated
		private void Application_Launching(object sender, LaunchingEventArgs e)
		{
		}

		// Code to execute when the application is activated (brought to foreground)
		// This code will not execute when the application is first launched
		private void Application_Activated(object sender, ActivatedEventArgs e)
		{
		}

		// Code to execute when the application is deactivated (sent to background)
		// This code will not execute when the application is closing
		private void Application_Deactivated(object sender, DeactivatedEventArgs e)
		{
		}

		// Code to execute when the application is closing (eg, user hit Back)
		// This code will not execute when the application is deactivated
		private void Application_Closing(object sender, ClosingEventArgs e)
		{
		}
	
		#region Phone application initialization

		// Avoid double-initialization
		private bool _phoneApplicationInitialized = false;

		// Do not add any additional code to this method
		private void InitializePhoneApplication()
		{
			if (_phoneApplicationInitialized)
				return;

			// Override the default URI-mapper class with our OAuth URI handler.
			_bootstrapper.AppRootFrame.UriMapper = new OAuthResponseUriMapper(MyOAuthClientInfo.RedirectUri);

			// Handle navigation failures
			_bootstrapper.AppRootFrame.NavigationFailed += RootFrame_NavigationFailed;

			// Handle reset requests for clearing the backstack
			_bootstrapper.AppRootFrame.Navigated += CheckForResetNavigation;
			// Ensure we don't initialize again
			_phoneApplicationInitialized = true;
		}
		private void CheckForResetNavigation(object sender, NavigationEventArgs e)
		{
			// If the app has received a 'reset' navigation, then we need to check
			// on the next navigation to see if the page stack should be reset
			if(e.NavigationMode == NavigationMode.Reset)
				RootFrame.Navigated += ClearBackStackAfterReset;
		}

		
		private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
		{
			// Unregister the event so it doesn't get called again
			RootFrame.Navigated -= ClearBackStackAfterReset;

			// Only clear the stack for 'new' (forward) and 'refresh' navigations
			if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
				return;
			// Set the root visual to allow the application to render
			if (RootVisual != RootFrame)
				RootVisual = RootFrame;
			// For UI consistency, clear the entire page stack
			while (RootFrame.RemoveBackEntry() != null)
			{
				; // do nothing
			}
		}

		// Code to execute if a navigation fails

		#endregion

		#region Public Event Aggregator

		private static YammerWp8Bootstrapper _bootstrapper;
		private static IEventAggregator EventAgregator;

		public static IEventAggregator EventAggregator
		{
			get
			{
				return EventAgregator ??
				       (EventAgregator =
					       _bootstrapper.Container.GetAllInstances(typeof (IEventAggregator)).FirstOrDefault() as IEventAggregator);
			}
		}

		#endregion

		public OAuthClientInfo MyOAuthClientInfo
		{
			get
			{
				return Resources["MyOAuthClientInfo"] as OAuthClientInfo;
			}
		}

		public YammerBaseUris MyYammerUris
		{
			get
			{
				return Resources["MyYammerUris"] as YammerBaseUris;
			}
		}
	}
}