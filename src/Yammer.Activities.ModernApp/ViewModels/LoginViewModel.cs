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
// File      : LoginViewModel.cs
//
// ***********************************************************************

using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Caliburn.Micro;
using Windows.UI.Xaml;
using Yammer.Activities.ModernApp.Models;
using Yammer.Activities.ModernApp.Views;
using Yammer.Oss.Api;
using Yammer.Oss.Api.Exceptions;
using Yammer.Oss.Core.WinRT;
using Yammer.Oss.Data;
using Yammer.Oss.Data.Models;

namespace Yammer.Activities.ModernApp.ViewModels
{
	public class LoginViewModel : ViewModelBase, IHandle<ApplicationValidationMessage>
	{
		private readonly IAuthRepository _authRepository;
		private readonly IUserRepository _userRepository;
		private readonly IClientConfiguration _clientConfig;
		private readonly IEventAggregator _eventAggregator;

		public LoginViewModel(INavigationService navigationService,
			IEventAggregator eventAggregator,
			IClientConfiguration clientConfiguration,
			IAuthRepository authRepository,
			IUserRepository userRepository,
			IStateManager stateManager)
			: base(navigationService, stateManager)
		{
			_authRepository = authRepository;
			_userRepository = userRepository;
			_eventAggregator = eventAggregator;
			_clientConfig = clientConfiguration;  
			_eventAggregator.Subscribe(this);
			EnableWebView = false;
		}

		private string _email = "@yammerconnections.com";
		public string Email
		{
			get { return _email; }
			set
			{
				_email = value;
				NotifyOfPropertyChange(() => Email);
				NotifyOfPropertyChange(() => CanLogin);
			}
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				NotifyOfPropertyChange(() => Password);
				NotifyOfPropertyChange(() => CanLogin);
			}
		}

		private bool _hasErrored;
		public bool HasErrored
		{
			get { return _hasErrored; }
			set
			{
				_hasErrored = value;
				NotifyOfPropertyChange(() => HasErrored);
			}
		}

		public async void Login()
		{
			HasErrored = false;
			IsBusy = true;
		    bool ssoEnabled = false;

		    try
		    {
		        await AuthorizationProcessAsync(ssoEnabled);
		    }
		    catch (Exception ex)
		    {
		        ssoEnabled = ex is SSOEnabledException;
		    }
		    if (ssoEnabled)
		    {
		        try
		        {
		            await AuthorizationProcessAsync(ssoEnabled);
		        }
		        catch (Exception ex)
                {
                    var inEx = ex.InnerException ?? ex;
                    _eventAggregator.Publish(new ServerErrorMessage(inEx.Message));
		        }
		    }
		}

	    private async Task AuthorizationProcessAsync(bool ssoEnabled)
	    {
            var authResponse = await OAuthUtils.LaunchSignIn(_clientConfig.ClientId, _clientConfig.RedirectUri, ssoEnabled);
            var credentials = await _authRepository.AuthenticateAppAsync(authResponse.Code);
            UpdateState(credentials);
	    }

		public bool CanLogin
		{
			get
			{
#if DEBUG
				return true;
#else
				return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password); 
#endif
			}
		}

		public void Attributions()
		{
			NavigationService.Navigate<AttributionsView>();

		}

		private bool _enableWebView;
		public bool EnableWebView
		{
			get { return _enableWebView; }
			set
			{
				_enableWebView = value;
				HideWebView = _enableWebView ? Visibility.Visible : Visibility.Collapsed;
				NotifyOfPropertyChange(() => EnableWebView);
				NotifyOfPropertyChange(() => HideWebView);
			}
		}

		public Visibility HideWebView { get; protected set; }

		public void Handle(ApplicationValidationMessage message)
		{
			var code = message.Code;
			AuthenticateApp(code);
		}        

		private async void AuthenticateApp(string code)
		{
			try
			{
				if (string.IsNullOrEmpty(code))
				{
					HasErrored = true;
					return;
				}
				var credentials = await _authRepository.AuthenticateAppAsync(code);
				await UpdateState(credentials);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		private async Task UpdateState(Credentials credentials)
		{
			if (credentials == null)
				throw new UnauthorizedException("Invalid Credentials", HttpStatusCode.SeeOther);

			StateManager.Credentials = credentials;
			StateManager.CurrentUser = null;
			StateManager.CurrentUser = await _userRepository.GetCurrentUserAsync();
			if (credentials != null)
				NavigationService.Navigate<MyContractsView>();
			IsBusy = false;
		}
	}
}
