using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Yammer.Activities.Models;

namespace Yammer.Activities.Controls
{
	public partial class UserLoginControl : UserControl
	{
		private const string _UsernameFirstText = "Type your email";
		private readonly IEventAggregator _eventAggregator;
		public UserLoginControl()
		{
			InitializeComponent();
			_eventAggregator = App.EventAggregator;

			Login.Tap += Login_Tap;
			UserEmailWatermarkText.Tap += UserEmailWatermarkTextTap;

			UserEmailTextBox.TextChanged +=UserEmailTextBoxTextChanged;
			UserEmailTextBox.LostFocus += UserEmailTextBoxLostFocus;
			UserEmailTextBox.KeyDown += (sender, e) =>
			{
				if (string.IsNullOrEmpty(UserEmailTextBox.Text)) return;
				 if (e.Key == Key.Enter)
					 PasswordTextBox.Focus();
			};
			PasswordTextBox.PasswordChanged += PasswordTextBox_PasswordChanged;
			PasswordTextBox.LostFocus += PasswordTextBox_LostFocus;
			PasswordTextBox.KeyDown += (sender, e) =>
			{
				if (string.IsNullOrEmpty(UserEmailTextBox.Text)) return;
				if (e.Key == Key.Enter)
					Login_Tap(sender, new GestureEventArgs());				
			};
		}

		void UserEmailWatermarkTextTap(object sender, GestureEventArgs e)
		{
			UserEmailTextBox.Focus();
		}

		void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			Login.IsEnabled = CanLogin;
		}

		void UserEmailTextBoxLostFocus(object sender, RoutedEventArgs e)
		{
			Login.IsEnabled = CanLogin;
		}

		bool ValidPassword()
		{
			return !string.IsNullOrWhiteSpace(PasswordTextBox.Password);
		}

		bool InvalidUserName()
		{
			return string.IsNullOrEmpty(UserEmailTextBox.Text);
		}

		void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			Login.IsEnabled = CanLogin;
		}

		void UserEmailTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			UserEmailWatermarkText.Visibility = InvalidUserName() ? Visibility.Visible : Visibility.Collapsed;
			Login.IsEnabled = CanLogin;
		}

		void Login_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			if (!CanLogin) return;
			
			_eventAggregator.Publish(new LoginMessage()
			{
				UserEmail = UserEmailTextBox.Text,
				UserPassword=PasswordTextBox.Password,
			});
		}
		
		public bool CanLogin
		{
			get
			{
				return UserEmailWatermarkText.Visibility==Visibility.Collapsed && ValidPassword();
			}
		}
	}
}
