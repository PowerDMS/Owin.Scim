namespace Owin.Scim.Configuration
{
    using System;

    using DryIoc;

    using NContext.EventHandling;
    public class DryIocActivationProvider : IActivationProvider
    {
        private readonly IContainer _Container;

        public DryIocActivationProvider(IContainer container)
        {
            _Container = container;
        }

        public IHandleEvents CreateInstance<TEvent>(Type handler)
        {
            return _Container.Resolve(handler) as IHandleEvents;
        }
    }
}