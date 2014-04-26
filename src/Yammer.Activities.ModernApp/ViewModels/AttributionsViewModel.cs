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
// File      : AttributionsViewModel.cs
//
// ***********************************************************************
using Caliburn.Micro;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Yammer.Activities.ModernApp.ViewModels
{
	public class AttributionsViewModel : Screen
	{
		private const string AttributionsFileName = "Attributions.txt";

		public AttributionsViewModel()
		{

		}

		protected override void OnActivate()
		{
			base.OnActivate();


			// Finds the attributions resource in the embedded resources for the current assembly
			var assembly = GetType().GetTypeInfo().Assembly;
			var resources = assembly.GetManifestResourceNames();

			var matchingResourceName = resources.First(r => r.EndsWith(AttributionsFileName));

			using (var sr = new StreamReader(assembly.GetManifestResourceStream(matchingResourceName)))
			{
				AttributionsText = sr.ReadToEnd();
				NotifyOfPropertyChange(() => AttributionsText);
			}
		}

		public string AttributionsText { get; private set; }
	}
}
