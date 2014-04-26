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
// File      : ActivityGenerator.cs
//
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Yammer.Oss.Api.Activity;
using Yammer.Oss.Api.AutoComplete;
using Yammer.Oss.Api.User;
using OpenGraphObject = Yammer.Oss.Api.Activity.OpenGraphObject;

namespace Yammer.Activities.ModernApp.Common
{
	public class ActivityGenerator
	{
		private ActivityEnvelope _activity = new ActivityEnvelope();

		private OpenGraphObject _object;
		public ActivityEnvelope Generate(User actor, string action, string title, string description, string message, IEnumerable<AutoCompleteUser> users)
		{
			var _users = new List<ActivityUser>();
			if (users != null && users.Any())
				_users.AddRange(users.Select(user => new ActivityUser() { Name = user.FullName, Email = user.Email }));

			_object = new OpenGraphObject()
			{
				Url = string.Format("{0}/{1}", Constants.OpenGraphUri, Guid.NewGuid().ToString()),  /* Hardcoded to a constanct for demop.  Url should be to a specific Open Graph entity. */
				Title = title,
				Type = Constants.OpenGraphType,
				Description = description,
				ImageUri = Constants.ImageUri,
			};
			_activity = new ActivityEnvelope()
			{
				Activity = new Activity()
				{
					Actor = new ActivityUser() { Name = actor.FullName, Email = actor.ContactInfo.EmailAddresses[0].Address },
					Action = action,
					Message = message,
					OgObject = _object,
					Users = _users.Any() ? _users.ToArray() : null
				}
			};

			return _activity;
		}
	}
}
