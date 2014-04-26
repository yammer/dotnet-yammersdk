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
// File      : App.xaml.cs
//
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using Yammer.Activities.ModernApp.Extensions;
using Yammer.Activities.ModernApp.Views;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227
using Yammer.Oss.Api;
using Yammer.Oss.Api.Clients;
using Yammer.Oss.Api.OAuth;
using Yammer.Oss.Core.Serialization;
using Yammer.Oss.Data;
using Yammer.Oss.Data.Repositories;

namespace Yammer.Activities.ModernApp
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : CaliburnApplication
	{
		private IUnityContainer _container;

		//#region
		//yammerconnections.com info
        //private const String ApiClientId = "RVJKaZL9SLS6Tkl1UsUog";
        //private const String ApiClientSecret = "R5RgTxnSy9Oo14fdIthQ3hK8AX1DnE3Cw5WySRZGY";
        //private const string BaseUri = "http://www.yammer.com";
        //private const string UploadBaseUri = "http://https://files2.yammer.com";
        //private const string RedirectUri = "yammeractivitiesapp://sampleApp";
        //#endregion

		public OAuthClientInfo MyOAuthClientInfo
		{
			get
			{
				return Resources["MyOAuthClientInfo"] as OAuthClientInfo;
			}
		}

		public YammerBaseUris MyYammerUris
		{
			get
			{
				return Resources["MyYammerUris"] as YammerBaseUris;
			}
		}

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();
			this.Suspending += OnSuspending;
		}

		#region OnLaunch-OnSuspending
		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used when the application is launched to open a specific file, to display
		/// search results, and so forth.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			DisplayRootView<LoginView>();
		}

	    protected override void OnActivated(IActivatedEventArgs args)
	    {
	        base.OnActivated(args);
	        var protocolActivated = args as ProtocolActivatedEventArgs;
	        if (protocolActivated != null)
	        {
	            //var authMsg = new AuthenticationMessage(protocolActivated.Uri);
	        }
	    }

	    /// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}
		#endregion

		/// <summary>
		/// Override this to provide an IoC specific implementation.
		/// </summary>
		/// <param name="service">The service to locate.</param>
		/// <param name="key">The key to locate.</param>
		/// <returns>The located service.</returns>
		protected override Object GetInstance(Type service, String key)
		{
			return _container.Resolve(service, key);
		}

		/// <summary>
		/// Override this to provide an IoC specific implementation
		/// </summary>
		/// <param name="service">The service to locate.</param>
		/// <returns>The located services.</returns>
		protected override IEnumerable<Object> GetAllInstances(Type service)
		{
			return _container.ResolveAll(service);
		}

		/// <summary>
		/// Override this to provide an IoC specific implementation.
		/// </summary>
		/// <param name="instance">The instance to perform injection on.</param>
		protected override void BuildUp(Object instance)
		{
			_container.BuildUp(instance);
		}

		/// <summary>
		/// Prepares the view first.
		/// </summary>
		/// <param name="rootFrame">The root frame.</param>
		protected override void PrepareViewFirst(Frame rootFrame)
		{

			_container.RegisterInstance<INavigationService>(new FrameAdapter(RootFrame, false),
															new ContainerControlledLifetimeManager());
		}

		protected override void Configure()
		{
			base.Configure();

			_configureIoc();

			EventAggregator = _container.Resolve<IEventAggregator>();
		}

	    private void _configureIocOAuth()
	    {
            _container = new UnityContainer();

            // This is only used by Caliburn
            _container.RegisterSingletonType<IEventAggregator, EventAggregator>();

            _container
                .RegisterSingletonType<IQueryStringSerializer, QueryStringSerializer>()
                .RegisterInstance<ISerializer>(new JsonDotNetSerializer())
                .RegisterInstance<IDeserializer>(new JsonDotNetDeserializer())
                .RegisterInstance<IWebProxy>(WebRequest.DefaultWebProxy)
                .RegisterSingletonType<ICookieStore, CookieStore>();

            _container
                .RegisterInstance<IClientConfiguration>(new ClientConfiguration(
                    MyOAuthClientInfo,
                    ProductInfoHeaderValue.Parse("Yammer_Activities_ModenApp/1.0.0.0"),
                    MyYammerUris));

            _container.RegisterSingletonType<IAuthClient, AuthClient>(
                new InjectionConstructor(typeof(IClientConfiguration),
                    typeof(IQueryStringSerializer),
                    typeof(ISerializer),
                    typeof(IDeserializer),
                    _container.Resolve<OAuthResponseErrorHandler>(),
                    typeof(ICookieStore),
                    typeof(IWebProxy)));
	    }
		private void _configureIocClients()
        {
				_container.RegisterSingletonType<IAutoCompleteClient, AutoCompleteClient>(
					new InjectionConstructor(typeof(IClientConfiguration),
											 typeof(IQueryStringSerializer),
											 typeof(ISerializer),
											 typeof(IDeserializer),
											 _container.Resolve<OAuthResponseErrorHandler>(),
											 typeof(ICookieStore),
											 typeof(IWebProxy)))
				.RegisterSingletonType<IOpenGraphClient, OpenGraphClient>(
					new InjectionConstructor(typeof(IClientConfiguration),
											 typeof(IQueryStringSerializer),
											 typeof(ISerializer),
											 typeof(IDeserializer),
											 _container.Resolve<OAuthResponseErrorHandler>(),
											 typeof(ICookieStore),
											 typeof(IWebProxy)))
				.RegisterSingletonType<IUserClient, UserClient>(
					new InjectionConstructor(typeof(IClientConfiguration),
											 typeof(IQueryStringSerializer),
											 typeof(ISerializer),
											 typeof(IDeserializer),
											 _container.Resolve<OAuthResponseErrorHandler>(),
											 typeof(ICookieStore),
											 typeof(IWebProxy)))
				.RegisterSingletonType<IMessageClient, MessageClient>(
					new InjectionConstructor(typeof(IClientConfiguration),
											 typeof(IQueryStringSerializer),
											 typeof(ISerializer),
											 typeof(IDeserializer),
											 _container.Resolve<OAuthResponseErrorHandler>(),
											 typeof(ICookieStore),
											 typeof(IWebProxy)));


			_container
				.RegisterSingletonType<IStateManager, StateManager>()
				.RegisterSingletonType<IAuthRepository, AuthRepository>()
				.RegisterSingletonType<IUsersRepository, UsersRepository>()
				.RegisterSingletonType<IOpenGraphRepository, OpenGraphRepository>()
				.RegisterSingletonType<IUserRepository, UserRepository>()
				.RegisterSingletonType<IMessageRepository, MessageRepository>();
		}

	    private void _configureIoc()
	    {
	        _configureIocOAuth();
            _configureIocClients();
	    }

		public static IEventAggregator EventAggregator { get; private set; }

	    protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
	    {
	        base.OnShareTargetActivated(args);
	    }

	    protected override void OnFileActivated(FileActivatedEventArgs args)
	    {
	        base.OnFileActivated(args);
	    }

	    protected override void OnResuming(object sender, object e)
	    {
	        base.OnResuming(sender, e); 
	    }
	}
}
