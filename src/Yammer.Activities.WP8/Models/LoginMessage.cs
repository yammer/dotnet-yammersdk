using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Activities.Models
{
	public class LoginMessage
	{
		public bool LoginSuccessful { get; set; }
		public string Message { get; set; }

		public string UserEmail { get; set; }
		public string UserPassword { get; set; }
	}
}
