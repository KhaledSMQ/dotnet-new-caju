﻿namespace EventSourcingReadOnlyProject.UI.Modules
{
    using Autofac;

    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //
            // Register all Types in EventSourcingReadOnlyProject.Application
            //
            // IPublisher
            // ISubscriber
            // IInputBoundary<>
            // IOutputBoundary<>
            // IEventHandler
            //
            builder.RegisterAssemblyTypes(typeof(Application.UseCases.Register.RegisterInteractor).Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}