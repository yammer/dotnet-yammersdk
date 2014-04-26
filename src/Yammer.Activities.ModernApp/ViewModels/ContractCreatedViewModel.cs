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
// File      : ContractCreatedViewModel.cs
//
// ***********************************************************************
using Caliburn.Micro;
using Yammer.Oss.Data;

namespace Yammer.Activities.ModernApp.ViewModels
{
	public class ContractCreatedViewModel : ViewModelBase
	{
		public ContractCreatedViewModel(INavigationService navigationService, IStateManager stateManager)
			: base(navigationService, stateManager)
		{
		}

		public string Title { get; set; }

		public int AssigneesCount { get; set; }
	}
}
