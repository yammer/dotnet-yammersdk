using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Yammer.Activities.Models
{
	public sealed class BrowserAuthMessage
	{
		public IReadOnlyDictionary<string,string> QueryString { get; private set; }
		public NavigationMode NavigationMode { get; private set; }
		#region Ctor
		public BrowserAuthMessage(IDictionary<string, string> queryString, NavigationMode navigationMode)
		{
			QueryString = new ReadOnlyDictionary<string, string>(queryString);
			NavigationMode = navigationMode;
		}
		#endregion
	}

	public sealed class ApplicationValidationMessage
	{
		public string Code { get; private set; }

		public ApplicationValidationMessage(string code)
		{
			Code = code;
		}
	}
}
