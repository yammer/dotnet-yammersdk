using Caliburn.Micro;
using Yammer.Oss.Api.Activity;

namespace Yammer.Activities.ViewModels
{
    public class OpenGraphObjectViewModel : PropertyChangedBase
    {
        private OpenGraphObject _openGraphObjectModel;
        public OpenGraphObject OpenGraphObjectModel
        {
            get { return _openGraphObjectModel; }
            set
            {
                _openGraphObjectModel = value;
                if (_openGraphObjectModel != null)
                {
                    LineOne = _openGraphObjectModel.Title;
                    LineTwo = _openGraphObjectModel.Description;
                    LineThree = _openGraphObjectModel.ImageUri;
                }
            }
        }


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