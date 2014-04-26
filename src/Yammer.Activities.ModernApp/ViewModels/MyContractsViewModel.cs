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
// File      : MyContractsViewModel.cs
//
// ***********************************************************************
using Caliburn.Micro;
using Yammer.Activities.ModernApp.Views;
using Yammer.Oss.Data;
using Yammer.Oss.Data.Models;

namespace Yammer.Activities.ModernApp.ViewModels
{
	public class MyContractsViewModel : ViewModelBase
	{
		public BindableCollection<Contract> MyContracts { get; set; }

		public MyContractsViewModel(INavigationService navigationService, IStateManager stateManager)
			: base(navigationService, stateManager)
		{
			this.MyContracts = new BindableCollection<Contract>();

			/* In a real app you would populate your list of contracts from a real data source */

			this.MyContracts.AddRange(new Contract[] { 
				new Contract { Name = "Mouse Bait Supplies", Assignees = "Eric, Julie, Paul" },
				new Contract { Name = "How To Best Annoy Your Human App", Assignees = "Jack, Kate, Hugo, Sawyer" },
				new Contract { Name = "How To Nap e-book", Assignees = "Joffrey, Cersei, Jaime, Tyrion" }});
		}

		public void New()
		{
			NavigationService.Navigate<NewContractView>();
		}
	}
}
