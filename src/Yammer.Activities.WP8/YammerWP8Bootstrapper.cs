using Caliburn.Micro;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using Yammer.Activities.ViewModels;
using Yammer.Oss.Api;
using Yammer.Oss.Api.Clients;
using Yammer.Oss.Api.OAuth;
using Yammer.Oss.Core.Serialization;
using Yammer.Oss.Data;
using Yammer.Oss.Data.Repositories;

namespace Yammer.Activities
{
	public class YammerWp8Bootstrapper : PhoneBootstrapper
	{
		#region

		public readonly TimeSpan DefaultTimeoutSeconds = new TimeSpan(0, 0, 30);
		#endregion

		public PhoneContainer Container { get; protected set; }
		public INavigationService NavigationService { get; protected set; }
		public IEventAggregator EventAggregator { get; protected set; }

		public PhoneApplicationFrame AppRootFrame { get { return RootFrame; } }
		protected override void Configure()
		{
			Container = new PhoneContainer();

			if (!Execute.InDesignMode)
			{
				Container.RegisterPhoneServices(RootFrame);
			}

			NavigationService = Container.GetInstance(typeof(INavigationService), null) as INavigationService;
			EventAggregator = Container.GetInstance(typeof(IEventAggregator), null) as IEventAggregator;

			RegisterServices();
			RegisterViewModels();
			

			AddCustomConventions();
			
		}

		private void RegisterServices()
		{
			RegisterLowLevelObjects();
			Container.RegisterWithAppSettings(typeof(IStateManager), (string)null, typeof(StateManager));

			RegisterClients();
			RegisterRepos();
		}

		private void RegisterViewModels()
		{
			Container.RegisterHandler(typeof (MainPageViewModel), null, cont =>
			{
				var a = cont.GetInstance(typeof(IClientConfiguration), null) as IClientConfiguration;
				var b = cont.GetInstance(typeof(IAuthRepository), null) as IAuthRepository;
				var c = cont.GetInstance(typeof(IUserRepository), null) as IUserRepository;
                var d = cont.GetInstance(typeof(IUsersRepository), null) as IUsersRepository;
                var e = cont.GetInstance(typeof(IOpenGraphRepository), null) as IOpenGraphRepository;
                var f = cont.GetInstance(typeof(IMessageRepository), null) as IMessageRepository;
				var g = cont.GetInstance(typeof(IStateManager), null) as IStateManager;

				return new MainPageViewModel(NavigationService, EventAggregator, a, b, c, d, e, f, g);
			});
		}
		private void RegisterRepos()
		{
			Container.RegisterSingleton(typeof(IUserRepository), (string)null, typeof(UserRepository));
			Container.RegisterSingleton(typeof(IAuthRepository), (string)null, typeof(AuthRepository));
		    Container.RegisterSingleton(typeof (IUsersRepository), (string) null, typeof (UsersRepository));
            Container.RegisterSingleton(typeof(IOpenGraphRepository), (string)null, typeof(OpenGraphRepository));
		    Container.RegisterSingleton(typeof (IMessageRepository), (string) null, typeof (MessageRepository));
		}
		private void RegisterClients()
		{
			//This is aweful, but the container does NOT support classes with constructors with many parameters
			Container.RegisterHandler(typeof(IUserClient), null, cont =>
			{
				var a = cont.GetInstance(typeof(IClientConfiguration), null) as IClientConfiguration;
				var b = cont.GetInstance(typeof(IQueryStringSerializer), null) as IQueryStringSerializer;
				var c = cont.GetInstance(typeof(ISerializer), null) as ISerializer;
				var d = cont.GetInstance(typeof(IDeserializer), null) as IDeserializer;
				var e = cont.GetInstance(typeof(IResponseErrorHandler), null) as IResponseErrorHandler;
				var f = cont.GetInstance(typeof(ICookieStore), null) as ICookieStore;
				var g = cont.GetInstance(typeof(IWebProxy), null) as IWebProxy;
				g = g.Credentials == null ? null : g;

			    return new UserClient(a, b, c, d, e, f, g);
			});

			Container.RegisterHandler(typeof(IAuthClient), null, cont =>
			{
				var a = cont.GetInstance(typeof(IClientConfiguration), null) as IClientConfiguration;
				var b = cont.GetInstance(typeof(IQueryStringSerializer), null) as IQueryStringSerializer;
				var c = cont.GetInstance(typeof(ISerializer), null) as ISerializer;
				var d = cont.GetInstance(typeof(IDeserializer), null) as IDeserializer;
				var e = cont.GetInstance(typeof(IResponseErrorHandler), null) as IResponseErrorHandler;
				var f = cont.GetInstance(typeof(ICookieStore), null) as ICookieStore;

				return new AuthClient(a, b, c, d, e, f);
			});

            Container.RegisterHandler(typeof(IOpenGraphClient), null, cont =>
            {
                var a = cont.GetInstance(typeof(IClientConfiguration), null) as IClientConfiguration;
                var b = cont.GetInstance(typeof(IQueryStringSerializer), null) as IQueryStringSerializer;
                var c = cont.GetInstance(typeof(ISerializer), null) as ISerializer;
                var d = cont.GetInstance(typeof(IDeserializer), null) as IDeserializer;
                var e = cont.GetInstance(typeof(IResponseErrorHandler), null) as IResponseErrorHandler;
                var f = cont.GetInstance(typeof(ICookieStore), null) as ICookieStore;
                var g = cont.GetInstance(typeof(IWebProxy), null) as IWebProxy;
                g = g.Credentials == null ? null : g;

                return new OpenGraphClient(a, b, c, d, e, f, g);
            });

            Container.RegisterHandler(typeof(IMessageClient), null, cont =>
            {
                var a = cont.GetInstance(typeof(IClientConfiguration), null) as IClientConfiguration;
				var b = cont.GetInstance(typeof(IQueryStringSerializer), null) as IQueryStringSerializer;
				var c = cont.GetInstance(typeof(ISerializer), null) as ISerializer;
				var d = cont.GetInstance(typeof(IDeserializer), null) as IDeserializer;
				var e = cont.GetInstance(typeof(IResponseErrorHandler), null) as IResponseErrorHandler;
				var f = cont.GetInstance(typeof(ICookieStore), null) as ICookieStore;
                var g = cont.GetInstance(typeof(IWebProxy), null) as IWebProxy;
				g = g.Credentials == null ? null : g;

                return new MessageClient(a, b, c, d, e, f, g);
            });
		}

	    public void RegisterConfigurationObject(OAuthClientInfo oAuthData, YammerBaseUris yammerUris)
	    {
	        Container.Instance<IClientConfiguration>(new ClientConfiguration(oAuthData,
	            new ProductInfoHeaderValue("Yammer_Activites", AppVersion.Version.ToString()),
	            yammerUris, DefaultTimeoutSeconds));
	    }

	    private void RegisterLowLevelObjects()
		{
			Container.RegisterPerRequest(typeof(IQueryStringSerializer), (string)null, typeof(QueryStringSerializer));
			Container.RegisterInstance(typeof(ISerializer), (string)null, new JsonDotNetSerializer());
			Container.RegisterPerRequest(typeof(IDeserializer), (string)null, typeof(JsonDotNetDeserializer));
			Container.RegisterInstance(typeof(ICookieStore), (string)null, new CookieStore());
			Container.RegisterPerRequest(typeof(IWebProxy), (string)null, typeof(DefaultProxy));
			Container.RegisterPerRequest(typeof(IResponseErrorHandler), (string)null, typeof(ResponseErrorHandler));
		}
		
		static void AddCustomConventions()
		{
			//ellided  
		}

		protected override object GetInstance(Type service, string key)
		{
			return Container.GetInstance(service, key);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return Container.GetAllInstances(service);
		}

		protected override void BuildUp(object instance)
		{
			Container.BuildUp(instance);
		}
	}

}