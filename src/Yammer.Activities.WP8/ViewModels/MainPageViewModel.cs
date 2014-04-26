using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Yammer.Activities.Common;
using Yammer.Activities.Models;
using Yammer.Activities.Resources;
using Yammer.Oss.Api;
using Yammer.Oss.Api.Activity;
using Yammer.Oss.Api.AutoComplete;
using Yammer.Oss.Api.Exceptions;
using Yammer.Oss.Api.OAuth;
using Yammer.Oss.Api.Utils;
using Yammer.Oss.Core;
using Yammer.Oss.Core.Serialization;
using Yammer.Oss.Data;
using Yammer.Oss.Data.Models;
using Yammer.Oss.Data.WinRT.Models;
using Yammer.Oss.Models;
using Constants = Yammer.Oss.Core.Constants;

namespace Yammer.Activities.ViewModels
{
	public class MainPageViewModel : ViewModelBase, IHandle<LoginMessage>, IHandle<BrowserAuthMessage>
    {
        #region Repositories
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUsersRepository _repository;
        private readonly IOpenGraphRepository _openGraphRepository;
        private readonly IMessageRepository _messageRepository;
        #endregion

        #region Private Variables
        private IEventAggregator _eventAggregator;
        private readonly IClientConfiguration _clientConfig; 
        
        public BindableCollection<AutoCompleteUser> Assignees { get; set; }
        private readonly List<AutoCompleteUser> _selectedAssignees;

		private string _sampleProperty = "Sample Runtime Property Value";
		private string _title;
		private string _firstItemTitle;
		private bool _userLoggedIn;
		private bool _item1IsBusy;
		private bool _enableLoginControl;

		#endregion

		#region Ctors

		protected MainPageViewModel()
		{   
            LoadData();
		}

		public MainPageViewModel(INavigationService navigationService,
			IEventAggregator eventAggregator,
			IClientConfiguration clientConfiguration,
			IAuthRepository authRepository,
			IUserRepository userRepository,
            IUsersRepository usersRepository,
            IOpenGraphRepository openGraphRepository,
            IMessageRepository messageRepository,
			IStateManager stateManager)
			: base(navigationService, stateManager)
		{
			_clientConfig = clientConfiguration;
			_authRepository = authRepository;
			_userRepository = userRepository;
		    _repository = usersRepository;
		    _openGraphRepository = openGraphRepository;
		    _messageRepository = messageRepository;

			_item1IsBusy = false;
			_enableLoginControl = true;            

			OpenGraphObjects = new ObservableCollection<OpenGraphObjectViewModel>();
			_eventAggregator = eventAggregator;
			_eventAggregator.Subscribe(this);		

            _title = "Yammer Activities";
            _firstItemTitle = "Please Login";
            _activitycreationError = Visibility.Collapsed;
            _selectedAssignees = new List<AutoCompleteUser>();
		    
		}

		#endregion

		#region Binding Properties

		public string Title
		{
			get { return _title; }
			set
			{
				if (string.CompareOrdinal(_title, value) == 0) return;
				_title = value;
				NotifyOfPropertyChange(() => _title);
			}
		}

		public string FirstItemTitle
		{
			get
			{
				return _firstItemTitle;
			}
			set
			{
				if (string.CompareOrdinal(_firstItemTitle, value) == 0) return;
				_firstItemTitle = value;
				NotifyOfPropertyChange(() => FirstItemTitle);
			}
		}

		public bool UserLoggedIn
		{
			get
			{
				return _userLoggedIn;
			}
			set
			{
				if (_userLoggedIn == value) return;
				_userLoggedIn = value;
                IsBusy = false;
				NotifyOfPropertyChange(() => UserLoggedIn);
				NotifyOfPropertyChange(() => ShowSecondaryPanoramaItems);
				NotifyOfPropertyChange(() => HideLogin);
			}
		}

		public Visibility ShowSecondaryPanoramaItems
		{
			get { return _userLoggedIn ? Visibility.Visible : Visibility.Collapsed; }
		}

		public Visibility HideLogin
		{
			get { return _userLoggedIn ? Visibility.Collapsed : Visibility.Visible; }
		}

		/// <summary>
		/// A collection for OpenGraphObjectViewModel objects.
		/// </summary>
        public ObservableCollection<OpenGraphObjectViewModel> OpenGraphObjects { get; private set; }


		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding
		/// </summary>
		/// <returns></returns>
		public string SampleProperty
		{
			get
			{
				return _sampleProperty;
			}
			set
			{
				if (value != _sampleProperty)
				{
					_sampleProperty = value;
					NotifyOfPropertyChange(() => SampleProperty);
				}
			}
		}

		/// <summary>
		/// Sample property that returns a localized string
		/// </summary>
		public string LocalizedSampleProperty
		{
			get
			{
				return AppResources.SampleProperty;
			}
		}

		public bool IsDataLoaded
		{
			get;
			private set;
		}

		public bool Item1IsBusy
		{
			get { return _item1IsBusy; }
			set
			{
				_item1IsBusy = value;
				NotifyOfPropertyChange(() => Item1IsBusy);
				NotifyOfPropertyChange(() => ProgressItem1Visibility);
			}
		}

		public Visibility ProgressItem1Visibility
		{
			get { return Item1IsBusy ? Visibility.Visible : Visibility.Collapsed; }
		}

		public bool EnableLoginControl
		{
			get { return _enableLoginControl; }
			set
			{
				if (_enableLoginControl == value) return;
				_enableLoginControl = value;
				NotifyOfPropertyChange(() => EnableLoginControl);
			}
		}

		#endregion

        #region User Profile
        private string _userMugshot;
        public string UserMugshot
        {
            get { return _userMugshot; }
            set
            {
                _userMugshot = value;
                NotifyOfPropertyChange(() => UserMugshot);
            }
        }

        private Visibility _userProfileVisibility;
        public Visibility UserProfileVisibility
        {
            get { return _userProfileVisibility; }
            set
            {
                _userProfileVisibility = value;
                NotifyOfPropertyChange(() => UserProfileVisibility);
            }
        }

	    private string _userFullName;

	    public string UserFullName
	    {
            get { return _userFullName; }
	        set
	        {
	            _userFullName = value;
                NotifyOfPropertyChange(() => UserFullName);
	        }
	    }

	    private string _jobTitle;
        public string JobTitle
	    {
            get { return _jobTitle; }
            set
            {
                _jobTitle = value;
                NotifyOfPropertyChange(() => JobTitle);
            }
	    }
        #endregion

        #region Design Data

        /// <summary>
		/// Creates and adds a few OpenGraphObjectViewModel objects into the Items collection.
		/// </summary>
		public void LoadData()
		{
			// Sample data; replace with real data
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime one",
				LineTwo = "Maecenas praesent accumsan bibendum",
				LineThree =
					"Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime two",
				LineTwo = "Dictumst eleifend facilisi faucibus",
				LineThree =
					"Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime three",
				LineTwo = "Habitant inceptos interdum lobortis",
				LineThree =
					"Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime four",
				LineTwo = "Nascetur pharetra placerat pulvinar",
				LineThree =
					"Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime five",
				LineTwo = "Maecenas praesent accumsan bibendum",
				LineThree =
					"Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime six",
				LineTwo = "Dictumst eleifend facilisi faucibus",
				LineThree =
					"Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime seven",
				LineTwo = "Habitant inceptos interdum lobortis",
				LineThree =
					"Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime eight",
				LineTwo = "Nascetur pharetra placerat pulvinar",
				LineThree =
					"Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime nine",
				LineTwo = "Maecenas praesent accumsan bibendum",
				LineThree =
					"Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime ten",
				LineTwo = "Dictumst eleifend facilisi faucibus",
				LineThree =
					"Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime eleven",
				LineTwo = "Habitant inceptos interdum lobortis",
				LineThree =
					"Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime twelve",
				LineTwo = "Nascetur pharetra placerat pulvinar",
				LineThree =
					"Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime thirteen",
				LineTwo = "Maecenas praesent accumsan bibendum",
				LineThree =
					"Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime fourteen",
				LineTwo = "Dictumst eleifend facilisi faucibus",
				LineThree =
					"Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime fifteen",
				LineTwo = "Habitant inceptos interdum lobortis",
				LineThree =
					"Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat"
			});
			this.OpenGraphObjects.Add(new OpenGraphObjectViewModel()
			{
				LineOne = "runtime sixteen",
				LineTwo = "Nascetur pharetra placerat pulvinar",
				LineThree =
					"Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum"
			});

			this.IsDataLoaded = true;
		}

		#endregion

		#region Authentication Logic

		private async void Login(LoginMessage message)
		{
			try
			{
				Item1IsBusy = true;
				OAuthUtils.LaunchSignIn(_clientConfig.ClientId, _clientConfig.RedirectUri);
				//await UpdateState(credentials);
			}
			catch (Exception ex)
			{
				//Debug message
				Debug.WriteLine("{0}: {1}", ex.Message, ex.StackTrace);
			}
		}

		private async void AuthenticateApp(AuthEnvelope authEnvelope)
		{
			if(authEnvelope == null || authEnvelope.AccessToken==null)
				return;	//TODO: an exception might be a better way of informing of an error here

			var accessToken = authEnvelope.AccessToken;
			var credentials = new Credentials
			{
				AuthToken = new Token
				{
					AuthenticatedAt = accessToken.AuthorizedAt,
					ExpiresAt = accessToken.ExpiresAt,
					Key = accessToken.Token
				},
				NetworkId = accessToken.NetworkId,
				UserId = accessToken.UserId,
			};
			await UpdateState(credentials);
		}

		private async Task UpdateState(Credentials credentials)
		{
			if (credentials == null)
				throw new UnauthorizedException("Invalid Credentials", HttpStatusCode.SeeOther);

			StateManager.Credentials = credentials;
			StateManager.CurrentUser = null;
			StateManager.CurrentUser = await _userRepository.GetCurrentUserAsync();
		    UpdateUserProfileData();

			UpdateUi(true);
		}

        private void UpdateUserProfileData()
        {
            var currentUser = (Yammer.Oss.Models.User) StateManager.CurrentUser;
            if (currentUser != null)
            {
                UserMugshot = currentUser.MugshotUri100x100.ToString();
                UserFullName = currentUser.FullName;
                JobTitle = currentUser.JobTitle;
                UserProfileVisibility = Visibility.Visible;
            }
	    }

		private void UpdateUi(bool userLogin)
		{
			Item1IsBusy = false;
			IsBusy = false;
		    IsCreateReady = true;
			UserLoggedIn = userLogin;
		}

		#endregion

		#region Messages Handlers

		public void Handle(LoginMessage message)
		{
			EnableLoginControl = false;
			Login(message);
		}

		public void Handle(BrowserAuthMessage oAuthMessage)
		{
			// Check the arguments from the query string passed to the page.
			HandleOAuthResponse(oAuthMessage.QueryString, oAuthMessage.NavigationMode);
			//AuthenticateApp(code);
		}

		#endregion

		#region OAuth Handling

		private void HandleOAuthResponse(IReadOnlyDictionary<string, string> uriParams, NavigationMode navigationMode)
		{
			// "Approve"
			if (uriParams.ContainsKey(Constants.OAuthParameters.Code) && uriParams.ContainsKey(Constants.OAuthParameters.State) &&
			    navigationMode != NavigationMode.Back)
			{
				var code = uriParams[Constants.OAuthParameters.Code];
				OAuthUtils.HandleApprove(
					_clientConfig.ClientId,
					_clientConfig.ClientSecret,
					code,
					uriParams[Constants.OAuthParameters.State],
					new JsonDotNetDeserializer(),
					onSuccess: AuthenticateApp,	// Get the Authentication response and create a Principal identity for the App
					onCSRF: () =>
					{
						//MessageBox.Show("Unknown 'state' parameter. Discarding the authentication attempt.", "Invalid redirect.",
						//	MessageBoxButton.OK);
					}, onErrorResponse: errorResponse =>
					{
						//Dispatcher.BeginInvoke(
						//	() => MessageBox.Show(errorResponse.OAuthError.ToString(), "Invalid operation", MessageBoxButton.OK));
					}, onException: ex =>
					{
						//Dispatcher.BeginInvoke(() => MessageBox.Show(ex.ToString(), "Unexpected exception!", MessageBoxButton.OK));
					}
					);
			}
				// "Deny"
			else if (uriParams.ContainsKey(Constants.OAuthParameters.Error) && navigationMode != NavigationMode.Back)
			{
				string error, errorDescription;
				error = uriParams[Constants.OAuthParameters.Error];
				uriParams.TryGetValue(Constants.OAuthParameters.ErrorDescription, out errorDescription);

				string msg = string.Format("error: {0}\nerror_description:{1}", error, errorDescription);
				MessageBox.Show(msg, "Error response is received.", MessageBoxButton.OK);

				OAuthUtils.DeleteStoredToken();

				UpdateTokenMessage(false);
			}
            else if (navigationMode == NavigationMode.Back)
            {
                UpdateUi(false);
                EnableLoginControl = true;
            }

			// if token already exist
			if (!string.IsNullOrEmpty(OAuthUtils.AccessToken))
			{
				UpdateTokenMessage(true);
			}
		}

		/// <summary>
		/// Update the UI status of the token existance in IsolatedStorage.
		/// </summary>
		/// <param name="isTokenPresent">Whether the token is present in Isolated Sotrage or not.</param>
		private void UpdateTokenMessage(bool isTokenPresent)
		{
            isTokenPresent = isTokenPresent;
		}

		#endregion

        #region OpenGraphObject Logic

	    public Visibility _activitycreationError;

	    public Visibility ActivityCreationError
	    {
            get { return _activitycreationError; }
	        set
	        {
	            _activitycreationError = value;
	            NotifyOfPropertyChange(() => ActivityCreationError);
	        }
	    }
        private string _activityTitle;
        public string ActivityTitle
        {
            get { return _activityTitle; }
            set
            {
                _activityTitle = value;
                NotifyOfPropertyChange(() => ActivityTitle);
                NotifyOfPropertyChange(() => CanCreate);
            }
        }

        private string _activityBody;
        public string ActivityBody
        {
            get { return _activityBody; }
            set
            {
                _activityBody = value;
                NotifyOfPropertyChange(() => ActivityBody);
                NotifyOfPropertyChange(() => CanCreate);
            }
        }


        public async Task Create()
        {               
            IsCreateReady = false;
            ActivityCreationError = Visibility.Collapsed;
            var activity = default(ActivityEnvelope);

            try
            {
                /* Your code to create a new contract comes here */
                var currentUser = StateManager.CurrentUser;

                var activityGenerator = new ActivityGenerator();

                if (ServerType)
                {
                    activity = activityGenerator.GenerateServerTicket(currentUser, "buildactivities:alert",
                        ActivityTitle, ActivityBody, ActivityBody, null);
                }
                else if (PhoneType)
                {
                    activity = activityGenerator.GeneratePhoneTicket(currentUser, "buildactivities:alert",
                        ActivityTitle, ActivityBody, ActivityBody, null);
                }
                else if (UpdateType)
                {
                    activity = activityGenerator.GenerateUpdateTicket(currentUser,
                        ActivityTitle, ActivityBody, ActivityBody, null);
                }

                await SendMessageAsync(activity); /* Post a message to Yammer */

                await _openGraphRepository.SubmitActivityAsync(activity);
                if (_selectedAssignees.Any())
                {
                    await UpdateAssignees(activityGenerator);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ActivityCreationError = Visibility.Visible;
            }

            // If successful, Add the new activity to the view
            if (ActivityCreationError == Visibility.Collapsed && 
                activity != default(ActivityEnvelope))
            {
                //activity.Activity.OgObject
               OpenGraphObjects.Add(new OpenGraphObjectViewModel()
               {
                   OpenGraphObjectModel = activity.Activity.OgObject,
               });
            }
            ActivityTitle = string.Empty;
            ActivityBody = string.Empty;
            IsCreateReady = true;
        }

	    private async Task UpdateAssignees(ActivityGenerator activityGenerator)
	    {
	        var tasks = new List<Task>();
            var currentUser = StateManager.CurrentUser;

	        foreach (var assignee in _selectedAssignees)
	        {
	            var activity = default(ActivityEnvelope);

                if (ServerType)
                {
                    activity = activityGenerator.GenerateServerTicket(currentUser, "buildactivities:alert",
                        ActivityTitle, ActivityBody, ActivityBody, new AutoCompleteUser[1] { assignee });
                }
                else if (PhoneType)
                {
                    activity = activityGenerator.GeneratePhoneTicket(currentUser, "buildactivities:alert",
                        ActivityTitle, ActivityBody, ActivityBody, new AutoCompleteUser[1] { assignee });
                }
                else if (UpdateType)
                {
                    activity = activityGenerator.GenerateUpdateTicket(currentUser,
                        ActivityTitle, ActivityBody, ActivityBody, new AutoCompleteUser[1] { assignee });
                }
                var activityTask = _openGraphRepository.SubmitActivityAsync(activity); // Submit an activity to Yammer
	            
                tasks.Add(activityTask);
	        }

	        await Task.WhenAll(tasks);
	    }

	    private async Task SendMessageAsync(ActivityEnvelope activityEnvelope)
        {

            var listOfUsers = new List<string>();


            if (_selectedAssignees != null && _selectedAssignees.Any())
                listOfUsers.AddRange(_selectedAssignees.Select(user => string.Format(@"[[user:{0}]]", user.Id.ToString())));

            var messageObject = new MessageObject()
            {
                Body = ActivityBody,
                Title = ActivityTitle,
                Type = activityEnvelope.Activity.OgObject.Type,
                Description = activityEnvelope.Activity.OgObject.Title,
                ImageUri = activityEnvelope.Activity.OgObject.ImageUri,
                Url = activityEnvelope.Activity.OgObject.Url,
                Users = string.Join(",", listOfUsers.ToArray())
            };
            await _messageRepository.PostMessageAsync(messageObject);
        }


        #region UI Logic
        public bool CanCreate
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ActivityTitle)
                    && !string.IsNullOrWhiteSpace(ActivityBody) &&
                    (ServerType || PhoneType || UpdateType);
                //&& _selectedAssignees.Any()
            }
        }

        private bool _isCreateReady;
        public bool IsCreateReady
        {
            get { return _isCreateReady; }
            set
            {
                _isCreateReady = value;
                NotifyOfPropertyChange(() => IsCreateReady);
                NotifyOfPropertyChange(() => CanCreate);
            }
        }

        private bool _serverType;
        public bool ServerType
        {
            get { return _serverType; }
            set
            {
                _serverType = value;
                NotifyOfPropertyChange(() => ServerType);
            }
        }

        private bool _phoneType;
        public bool PhoneType
        {
            get { return _phoneType; }
            set
            {
                _phoneType = value;
                NotifyOfPropertyChange(() => PhoneType);
            }
        }

        private bool _updateType;
        public bool UpdateType
        {
            get { return _updateType; }
            set
            {
                _updateType = value;
                NotifyOfPropertyChange(() => UpdateType);
            }
        }
        #endregion

        #endregion
    }
}