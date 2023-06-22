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

namespace FontShift.Application.Utils
{
    /// <summary>
    ///     Globally accessible service repository
    /// </summary>
    public class ServiceLocator
    {
        #region Service Class
        /// <summary>
        ///     Service class
        /// </summary>
        private sealed class Service
        {
            /// <summary>
            ///     The service instance
            /// </summary>
            public object Instance;

            /// <summary>
            ///     The (optional) name for this instance such that
            ///     you can register several instances of the same
            ///     type with different names
            /// </summary>
            public string Name;

            /// <summary>
            ///     The list of types under which the service can be obtained
            /// </summary>
            public Type[] Types;

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="instance">
            ///     The service instance
            /// </param>
            /// <param name="name">
            ///     The (optional) name of the service instance
            /// </param>
            /// <param name="types">
            ///     The types under which the instance is obtainable (must all be assignable from the instance type!)
            /// </param>
            public Service(object instance, string name, params Type[] types)
            {
                Name = name;
                Instance = instance;
                Types = types;
            }

            /// <summary>
            ///     Checks whether this service was registered for the specified type
            /// </summary>
            /// <param name="type">
            ///     The type to be checked
            /// </param>
            /// <param name="name">
            ///     The (optional) name to be checked
            /// </param>
            /// <param name="exactType">
            ///     <c>true</c>, if exact type match must be required (i.e. <paramref name="type"/> is the concrete 
            ///     type of <see cref="Instance"/>), <c>false</c> otherwise (i.e. <paramref name="type"/> may be
            ///     an interface)
            /// </param>
            /// <returns>
            ///     The check result
            /// </returns>
            public bool Matches(Type type, string name, bool exactType = false)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return (exactType) ? (Instance.GetType() == type) : Types.Contains(type);
                }
                else
                {
                    return exactType ? (Instance.GetType() == type && Name.Equals(name)) : (Types.Contains(type) && Name.Equals(name));
                }
            }

            /// <summary>
            ///     <see cref="object.ToString"/>
            /// </summary>
            public override string ToString()
            {
                return string.Format("Instance[{0}] Name[{1}] (Types[{2}])", Name, Instance.GetType().FullName, string.Join(", ", Types.Select(t => t.Name)));
            }

        }
        #endregion

        #region Private Attribute
        /// <summary>
        ///     The list of services
        /// </summary>
        private readonly List<Service> _services;
        #endregion

        #region Construction
        /// <summary>
        ///     Constructor
        /// </summary>
        public ServiceLocator()
        {
            this._services = new List<Service>();
        }
        #endregion

        #region Service Management
        /// <summary>
        ///     Registers a service. The sevice can later be queried using its exact type (i.e. using the type <paramref name="service"/>.GetType())
        /// </summary>
        /// <param name="service">
        ///     The service to be registered
        /// </param>
        public void Register(object service, string name = null)
        {
            bool exists = _services.Any(s => s.Matches(service.GetType(), name, true));

            _services.Add(new Service(service, name, service.GetType()));
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
        public void Register<TConcrete, TInterface>(TConcrete service, string name = null)
            where TInterface : class
            where TConcrete : class, TInterface
        {
            // 'TConcrete' serves as kind of a 'primary key' for the services


            bool exists = _services.Any(s => s.Matches(typeof(TConcrete), name, true) || s.Matches(typeof(TInterface), name, false));

            _services.Add(new Service(service, name, typeof(TInterface), typeof(TConcrete)));
        }

        /// <summary>
        ///     Unregisters a service
        /// </summary>
        /// <typeparam name="TConcrete">
        ///     The type of the service to be unregistered, must always be the concrete service type!
        /// </typeparam>
        public void Unregister<TConcrete>(string name = null) where TConcrete : class
        {
            // 'TService' serves as kind of a 'primary key' for the services

            Service dead = _services.SingleOrDefault(s => s.Matches(typeof(TConcrete), name, true));
            _services.Remove(dead);
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
        public TService Get<TService>(string name = null, bool assertExists = true) where TService : class
        {
            return (TService)Get(typeof(TService), name, assertExists);
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
        public object Get(Type serviceType, string name = null, bool assertExists = true)
        {
            object[] candidates = _services.Where(s => s.Matches(serviceType, name)).Select(s => s.Instance).ToArray();

            switch (candidates.Length)
            {
                case 0:
                    return null;
                case 1:
                    return candidates[0];
                default:
                    return null;
            }
        }
        #endregion
    }
}
