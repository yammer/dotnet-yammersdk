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
// Assembly  : Yammer.Oss.Core.WinRT
// File      : UnityContainerExtensions.cs
//
// ***********************************************************************

using Microsoft.Practices.Unity;
using System;

namespace Yammer.Activities.ModernApp.Extensions
{
	public static class UnityContainerExtensions
	{
		/// <summary>
		/// Registers the type of the singleton.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <param name="container">The container.</param>
		/// <param name="injectionMembers">The injection members.</param>
		/// <returns>IUnityContainer.</returns>
		public static IUnityContainer RegisterSingletonType<T, T2>(this IUnityContainer container, params InjectionMember[] injectionMembers) where T2 : T
		{
			return container.RegisterType<T, T2>(new ContainerControlledLifetimeManager(), injectionMembers);
		}

		/// <summary>
		/// Registers the type of the singleton.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <param name="container">The container.</param>
		/// <param name="name">The name.</param>
		/// <param name="injectionMembers">The injection members.</param>
		/// <returns>IUnityContainer.</returns>
		public static IUnityContainer RegisterSingletonType<T, T2>(this IUnityContainer container, String name, params InjectionMember[] injectionMembers) where T2 : T
		{
			return container.RegisterType<T, T2>(name, new ContainerControlledLifetimeManager(), injectionMembers);
		}
	}
}