using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Caliburn.Micro;

namespace Yammer.Activities.ViewModels
{
	public class ItemViewModel : PropertyChangedBase
	{
		private string _lineOne;
		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding.
		/// </summary>
		/// <returns></returns>
		public string LineOne
		{
			get
			{
				return _lineOne;
			}
			set
			{
				if (value != _lineOne)
				{
					_lineOne = value;
					NotifyOfPropertyChange(() => LineOne);
				}
			}
		}

		private string _lineTwo;
		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding.
		/// </summary>
		/// <returns></returns>
		public string LineTwo
		{
			get
			{
				return _lineTwo;
			}
			set
			{
				if (value != _lineTwo)
				{
					_lineTwo = value;
					NotifyOfPropertyChange(() => LineTwo);
				}
			}
		}

		private string _lineThree;
		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding.
		/// </summary>
		/// <returns></returns>
		public string LineThree
		{
			get
			{
				return _lineThree;
			}
			set
			{
				if (value != _lineThree)
				{
					_lineThree = value;
					NotifyOfPropertyChange(() => LineThree);
				}
			}
		}
	}
}