﻿using Autofac;
using Business.Mapping;
using Core.Repositories;
using Core.UnitOfWorks;
using DataAccess.DbContexts;
using DataAccess.Repositories;
using DataAccess.UnitOfWork;
using System.Reflection;
using Module = Autofac.Module;

namespace WebUI.Modules
{
    public class RepoServiceModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            var webAssembly = Assembly.GetExecutingAssembly();
            var repoAssembly = Assembly.GetAssembly(typeof(AppDbContext)); //Herhangi bir dosya adi ile o dosyanin icinde bulundugu assembly alinir.
            var businessAssembly = Assembly.GetAssembly(typeof(MapProfile));


            builder.RegisterAssemblyTypes(webAssembly, repoAssembly, businessAssembly).Where(x => x.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(webAssembly, repoAssembly, businessAssembly).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope();

        }
    }
}