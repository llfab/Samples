// ===========================================================================
//                     B I T S   O F   N A T U R E
// ===========================================================================
//
// This file is part of the Bits Of Nature source code.
//
// © 2018 Bits Of Nature GmbH. All rights reserved.
// ===========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using BitsOfNature.Core;
using BitsOfNature.Core.Utils;

namespace CrossPlatformApp
{

#pragma warning disable CS8600 
#pragma warning disable CS8603 
#pragma warning disable CS8604 
    /// <summary>
    ///     Globally accessible service repository
    /// </summary>
    public partial class ServiceLocator : IServiceRegistry
    {
        #region Service Class
        /// <summary>
        ///     Service class
        /// </summary>
        private class Service
        {
            #region Private Attributes
            /// <summary>
            ///     The service instance
            /// </summary>
            public object Instance;

            /// <summary>
            ///     The list of types under which the service can be obtained
            /// </summary>
            public Type[] Types;
            #endregion

            #region Construction
            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="instance">
            ///     The service instance
            /// </param>
            /// <param name="types">
            ///     The types under which the instance is obtainable (must all be assignable from the instance type!)
            /// </param>
            public Service(object instance, params Type[] types)
            {
                Instance = instance;
                Types = types;
            }
            #endregion

            /// <summary>
            ///     Checks whether this service was registered for the specified type
            /// </summary>
            /// <param name="type">
            ///     The type to be checked
            /// </param>
            /// <param name="exactType">
            ///     <c>true</c>, if exact type match must be required (i.e. <paramref name="type"/> is the concrete 
            ///     type of <see cref="Instance"/>), <c>false</c> otherwise (i.e. <paramref name="type"/> may be
            ///     an interface)
            /// </param>
            /// <returns>
            ///     The check result
            /// </returns>
            public bool Matches(Type type, bool exactType = false)
            {
                return (exactType) ? (Instance.GetType() == type) : Types.Contains(type);
            }

            #region Object Overrides
            /// <summary>
            ///     <see cref="object.ToString"/>
            /// </summary>
            public override string ToString()
            {
                return string.Format("{0} ({1})", Instance.GetType().FullName, string.Join(", ", Types.Select(t => t.Name)));
            }
            #endregion

        }
        #endregion

        #region Private Attribute
        /// <summary>
        ///     The list of services
        /// </summary>
        private List<Service> currentServices;

        /// <summary>
        ///     A stack holding the service frames.
        /// </summary>
        private Stack<List<Service>> serviceStack;
        #endregion

        #region Construction
        /// <summary>
        ///     Constructor
        /// </summary>
        internal ServiceLocator()
        {
            this.serviceStack = new Stack<List<Service>>();
            Push();
        }
        #endregion

        #region Service Management
        /// <summary>
        ///     Pushes a new service frame on the registry's stack.
        /// </summary>
        public void Push()
        {
            this.currentServices = new List<Service>();
            serviceStack.Push(currentServices);
        }

        /// <summary>
        ///     Pops the current service frame from the registry's stack
        ///     and disposes all registered services of that popped frame.
        /// </summary>
        public void Pop()
        {
            Assert.That(serviceStack.Count > 1, "Illegal state!"); // yes 1!!! The first one is the default frame.

            foreach (Service service in serviceStack.Pop())
            {
                if (service is IDisposable) { ((IDisposable)service).Dispose(); }
            }

            this.currentServices = serviceStack.Peek();
        }

        /// <summary>
        ///     Registers a service. The sevice can later be queried using its exact type (i.e. using the type <paramref name="service"/>.GetType())
        /// </summary>
        /// <param name="service">
        ///     The service to be registered
        /// </param>
        public void Register(object service)
        {
            bool exists = currentServices.Any(s => s.Matches(service.GetType(), true));
            Assert.That(!exists, "Service of type {0} was registered earlier!", service.GetType().FullName);

            currentServices.Add(new Service(service, service.GetType()));
        }

        /// <summary>
        ///     Registers a service, obtainable either through <typeparam name="TConcrete"/> or 
        ///     <typeparam name="TInterface"/> (usually an interface)
        /// </summary>
        /// <param name="service">
        ///     The service to be registered
        /// </param>
        /// <typeparam name="TConcrete">
        ///     The concrete service type
        /// </typeparam>
        /// <typeparam name="TInterface">
        ///     The interface type
        /// </typeparam>
        public void Register<TConcrete, TInterface>(TConcrete service)
            where TInterface : class
            where TConcrete : class, TInterface
        {
            // 'TConcrete' serves as kind of a 'primary key' for the services

            Assert.That(service.GetType() == typeof(TConcrete), // not tautologically true... may be an interface
                "Tried to register service without concrete type!");

            bool exists = currentServices.Any(s => s.Matches(typeof(TConcrete), true) || s.Matches(typeof(TInterface), false));
            Assert.That(!exists, "Service of type {0} was registered earlier!", service.GetType().FullName);

            currentServices.Add(new Service(service, typeof(TInterface), typeof(TConcrete)));
        }

        /// <summary>
        ///     Unregisters a service
        /// </summary>
        /// <typeparam name="TConcrete">
        ///     The type of the service to be unregistered, must always be the concrete service type!
        /// </typeparam>
        public void Unregister<TConcrete>() where TConcrete : class
        {
            // 'TService' serves as kind of a 'primary key' for the services

            // Converting null literal or possible null value to non-nullable type.
            Service dead = currentServices.SingleOrDefault(s => s.Matches(typeof(TConcrete), true));
            // Converting null literal or possible null value to non-nullable type.
            Assert.That(dead != null, "Unknown service {0}!", typeof(TConcrete).FullName);
            currentServices.Remove(dead);
        }

        /// <summary>
        ///     Unregisters a service
        /// </summary>
        /// <typeparam name="TConcrete">
        ///     The type of the service to be unregistered, must always be the concrete service type!
        /// </typeparam>
        public void UnregisterAndDispose<TConcrete>() where TConcrete : class
        {
            // 'TService' serves as kind of a 'primary key' for the services

            Service dead = currentServices.SingleOrDefault(s => s.Matches(typeof(TConcrete), true));
            Assert.That(dead != null, "Unknown service {0}!", typeof(TConcrete).FullName);
            currentServices.Remove(dead);

            if (dead is IDisposable) { ((IDisposable)dead).Dispose(); }
        }

        /// <summary>
        ///     Gets the specified service
        /// </summary>
        /// <typeparam name="TService">
        ///     The type identifying the service
        /// </typeparam>
        /// <param name="assertExists">
        ///     <c>true</c>, if the method should throw an exception if the requested service does not exist, 
        ///     <c>false</c> to return <c>null</c> in this case (can be used to probe for services)
        /// </param>
        /// <returns>
        ///     The service
        /// </returns>
        public TService Get<TService>(bool assertExists = true) where TService : class
        {
            return (TService)Get(typeof(TService), assertExists);
        }

        /// <summary>
        ///     Gets the specified service
        /// </summary>
        /// <param name="serviceType">
        ///     The type identifying the service
        /// </param>
        /// <param name="assertExists">
        ///     <c>true</c>, if the method should throw an exception if the requested service does not exist, 
        ///     <c>false</c> to return <c>null</c> in this case (can be used to probe for services)
        /// </param>
        /// <returns>
        ///     The service
        /// </returns>
        public object Get(Type serviceType, bool assertExists = true)
        {
            object[] candidates = serviceStack.SelectMany(l => l).Where(s => s.Matches(serviceType)).Select(s => s.Instance).ToArray();

            switch (candidates.Length)
            {
                case 0:
                    Assert.That(!assertExists, "Could not find service of type {0}!", serviceType.FullName);
                    return null;
                case 1:
                    return candidates[0];
                default:
                    Assert.Never("Service request is ambiguous: {0} is implemented by {1}!", serviceType.FullName,
                        string.Join(", ", candidates.Select(c => c.GetType().FullName)));
                    return null;
            }
        }
        #endregion
    }

#pragma warning restore CS8600 
#pragma warning restore CS8603 
#pragma warning restore CS8604 
}
