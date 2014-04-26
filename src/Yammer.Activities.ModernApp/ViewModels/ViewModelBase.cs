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
// File      : ViewModelBase.cs
//
// ***********************************************************************
using Caliburn.Micro;
using Yammer.Oss.Api.User;
using Yammer.Oss.Data;

namespace Yammer.Activities.ModernApp.ViewModels
{
	public class ViewModelBase : Screen
	{
		protected readonly INavigationService NavigationService;
		protected readonly IStateManager StateManager;

		public ViewModelBase(INavigationService navigationService, IStateManager stateManager)
		{
			this.NavigationService = navigationService;
			this.StateManager = stateManager;
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				NotifyOfPropertyChange(() => IsBusy);
			}
		}

		public User CurrentUser
		{
			get { return this.StateManager.CurrentUser; }
		}
	}
}
