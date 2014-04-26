using System;

namespace Yammer.Activities.ModernApp.Models
{
	public sealed class BrowserMessage
	{
		public Uri NavigateUri { get; private set; }

		#region Ctor
		public BrowserMessage(string uri)
		{
			NavigateUri=new Uri(uri);
		}

		public BrowserMessage(Uri uri)
		{
			NavigateUri = uri;
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

    public sealed class ServerErrorMessage
    {
        public string Meesage { get; private set; }

        public ServerErrorMessage(string message)
        {
            Meesage = message;
        }
    }
}
