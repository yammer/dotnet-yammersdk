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
// File      : NewContractViewModel.cs
//
// ***********************************************************************

using System;
using System.Diagnostics;
using Caliburn.Micro;
using Yammer.Activities.ModernApp.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yammer.Oss.Api.Activity;
using Yammer.Oss.Api.AutoComplete;
using Yammer.Oss.Api.User;
using Yammer.Oss.Data;
using Yammer.Oss.Data.WinRT.Models;
//using User = Yammer.Activities.Api.Dtos.User.User;

namespace Yammer.Activities.ModernApp.ViewModels
{
	public class NewContractViewModel : ViewModelBase
	{
		private readonly IUsersRepository _repository;
		private readonly IOpenGraphRepository _openGraphRepository;
		private readonly IUserRepository _userRepository;
		private readonly List<AutoCompleteUser> _selectedAssignees;
		private readonly IMessageRepository _messageRepository;
		public BindableCollection<AutoCompleteUser> Assignees { get; set; }

		public NewContractViewModel(INavigationService navigationService, IUsersRepository repository, IOpenGraphRepository openGraphRepository,
			IUserRepository userRepository, IStateManager stateManager, IMessageRepository messageRepository)
			: base(navigationService, stateManager)
		{
			_repository = repository;
			_openGraphRepository = openGraphRepository;
			_userRepository = userRepository;
			_messageRepository = messageRepository;
			_selectedAssignees = new List<AutoCompleteUser>();

			Assignees = new BindableCollection<AutoCompleteUser>();
		}

		public void AddAssignee(AutoCompleteUser user)
		{
			_selectedAssignees.Add(user);

			NotifyOfPropertyChange(() => CanCreate);
		}

		public async Task Create()
		{
			IsBusy = true;
			HasErrored = false;

			try
			{
				/* Your code to create a new contract comes here */

				var currentUser = await GetCurrentUser();



				var activityGenerator = new ActivityGenerator();

				var activity = activityGenerator.Generate(currentUser, "create", Title, Body, Body, null);

				await SendMessageAsync(activity); /* Post a message to Yammer */

				await _openGraphRepository.SubmitActivityAsync(activity);

				var tasks = new List<Task>();

				foreach (var assignee in _selectedAssignees)
				{
					activity = activityGenerator.Generate(currentUser, "Yammer.Activities:assign", Title, Body, Body,
						new AutoCompleteUser[1] {assignee});
					var activityTask = _openGraphRepository.SubmitActivityAsync(activity); // Submit an activty to Yammer
					tasks.Add(activityTask);

				}

				await Task.WhenAll(tasks);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				HasErrored = true;
			}

			IsBusy = false;

			if (!HasErrored)
			{
				NavigationService
					.UriFor<ContractCreatedViewModel>()
					.WithParam(vm => vm.Title, Title)
					.WithParam(vm => vm.AssigneesCount, _selectedAssignees.Count)
					.Navigate();
			}
		}

		private async Task SendMessageAsync(ActivityEnvelope activityEnvelope)
		{

			var listOfUsers = new List<string>();


			if (_selectedAssignees != null && _selectedAssignees.Any())
				listOfUsers.AddRange(_selectedAssignees.Select(user => string.Format(@"[[user:{0}]]", user.Id.ToString())));

			var messageObject = new MessageObject()
				{
					Body = Body,
					Title = Title,
					Type = activityEnvelope.Activity.OgObject.Type,
					Description = activityEnvelope.Activity.OgObject.Title,
					ImageUri = activityEnvelope.Activity.OgObject.ImageUri,
					Url = activityEnvelope.Activity.OgObject.Url,
					Users = string.Join(",", listOfUsers.ToArray())
				};
			await _messageRepository.PostMessageAsync(messageObject);
		}

		public bool CanCreate
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Title)
					&& !string.IsNullOrWhiteSpace(Body)
					&& _selectedAssignees.Count > 0;
			}
		}

		protected async override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);

			try
			{
				await LoadAssignees();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private async Task LoadAssignees()
		{
			IsBusy = true;

			var users = await _repository.GetUsersAsync();
			Assignees.AddRange(users);

			IsBusy = false;
		}

		private async Task<User> GetCurrentUser()
		{
			return await _userRepository.GetCurrentUserAsync();
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

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				NotifyOfPropertyChange(() => Title);
				NotifyOfPropertyChange(() => CanCreate);
			}
		}

		private string _body;
		public string Body
		{
			get { return _body; }
			set
			{
				_body = value;
				NotifyOfPropertyChange(() => Body);
				NotifyOfPropertyChange(() => CanCreate);
			}
		}
	}
}
