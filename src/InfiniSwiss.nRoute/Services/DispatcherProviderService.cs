﻿using System.Reactive.Concurrency;

namespace nRoute.Services
{
    public class DispatcherProviderService : IDispatcherProviderService
    {
        public IDispatcher Dispatcher { get; } = new WpfDispatcherAdapter();

        public IScheduler DispatcherScheduler => System.Reactive.Concurrency.DispatcherScheduler.Current;
    }
}
