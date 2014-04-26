using System.Collections.Generic;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;
using Yammer.Activities.Models;
using Yammer.Oss.Api.Utils;

namespace Yammer.Activities.Views
{
	public partial class MainPage : PhoneApplicationPage
	{
		// This is the event aggregator from Caliburn.Micro allowing event driven communication among
		// the different components
		private readonly IEventAggregator _eventAggregator;

		#region Ctors
		public MainPage()
		{
			InitializeComponent();

			// Initialize the eventaggregator for this view. This is a nice way to communicate
			// between ViewModels, ViewModels<->Views and other potential sinks
			_eventAggregator = App.EventAggregator;
			_eventAggregator.Subscribe(this);
			
			ProgressBarItem1.IsIndeterminate = true;
		}
		#endregion

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			_eventAggregator.Publish(new BrowserAuthMessage(NavigationContext.QueryString, e.NavigationMode));


		}
	}
}