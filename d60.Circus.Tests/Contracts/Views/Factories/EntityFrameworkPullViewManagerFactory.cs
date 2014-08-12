﻿using System;
using System.Collections.Generic;
using System.Linq;
using d60.Circus.EntityFramework;
using d60.Circus.Tests.MsSql;
using d60.Circus.Views.Basic;

namespace d60.Circus.Tests.Contracts.Views.Factories
{
    public class EntityFrameworkPullViewManagerFactory : IPullViewManagerFactory, IPushViewManagerFactory
    {
        readonly List<IViewManager> _viewManagers = new List<IViewManager>();
        readonly string _connectionString = TestSqlHelper.ConnectionString;

        public EntityFrameworkPullViewManagerFactory()
        {
            Console.WriteLine("Dropping migration history");
            TestSqlHelper.DropTable("__MigrationHistory");
        }

        public IPullViewManager GetPullViewManager<TView>() where TView : class, IViewInstance, ISubscribeTo, new()
        {
            var viewManager = GetEntityFrameworkViewManager<TView>();

            return new PullOnlyWrapper(viewManager);
        }

        public IPushViewManager GetPushViewManager<TView>() where TView : class, IViewInstance, ISubscribeTo, new()
        {
            var viewManager = GetEntityFrameworkViewManager<TView>();

            return new PushOnlyWrapper(viewManager);
        }

        EntityFrameworkViewManager<TView> GetEntityFrameworkViewManager<TView>()
            where TView : class, IViewInstance, ISubscribeTo, new()
        {
            TestSqlHelper.DropTable(typeof (TView).Name);

            var viewManager = new EntityFrameworkViewManager<TView>(_connectionString);
            MaxDomainEventsBetweenFlushSet += maxEvents => viewManager.MaxDomainEventsBetweenFlush = maxEvents;
            _viewManagers.Add(viewManager);

            return viewManager;
        }

        public TView Load<TView>(string viewId) where TView : class, IViewInstance, ISubscribeTo, new()
        {
            var viewManager = _viewManagers.OfType<EntityFrameworkViewManager<TView>>().FirstOrDefault();

            if (viewManager == null)
            {
                throw new ApplicationException(string.Format("Could not find view manager for views of type {0}", typeof(TView)));
            }

            return viewManager.Load(viewId);

        }

        event Action<int> MaxDomainEventsBetweenFlushSet = delegate { };

        public void SetMaxDomainEventsBetweenFlush(int value)
        {
            MaxDomainEventsBetweenFlushSet(value);
        }
    }
}