using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Activities
{
	internal static class AppVersion
	{
		private static Version _mVersion;
		public static Version Version
		{
			get
			{
				if(_mVersion == default(Version))
				{
					var lcAssembly = Assembly.GetExecutingAssembly();
					var parts = lcAssembly.FullName.Split(',');
					var lcVersionStr = parts[1].Split('=')[1];
					_mVersion = new Version(lcVersionStr);
				}
				return _mVersion;
			}
		}
	}
}
