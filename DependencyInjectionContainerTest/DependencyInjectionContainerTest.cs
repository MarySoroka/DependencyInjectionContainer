using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionContainer;
using DependencyInjectionContainerTest.testClasses;
using DependencyInjectionContainerTest.testClasses.impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyInjectionContainerTest
{
    [TestClass]
    public class DependencyInjectionContainerTest
    {
       
        [TestMethod]
        public void SimpleDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IServiceTwo, ServiceFromInterface>();
            dependencies.Register<AbstractService, ServiceFromAbstract>();
            dependencies.Register<Random, Random>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IServiceTwo>();
            var service2 = provider.Resolve<AbstractService>();
            var service3 = provider.Resolve<Random>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.IsNotNull(service3);
        }

        [TestMethod]
        public void SimpleErrorDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<AbstractService, ErrorAbstractImpl>();
            dependencies.Register<IServiceTwo, ErrorNoPublicConstructorImpl>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<AbstractService>();
            var service2 = provider.Resolve<IServiceTwo>();

            Assert.IsNull(service1);
            Assert.IsNull(service2);
        }

        [TestMethod]
        public void SimpleRecursionDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IRepository, RepositoryImplOne>();
            dependencies.Register<IServiceTwo, ServiceImplOne>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IServiceTwo>();

            Assert.AreEqual("RepositoryImpl is created",(service1 as ServiceImplOne)?.Rep.TestObject());
        }

        [TestMethod]
        public void DoubleRecursionDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<AbstractService, ServiceFromAbstract>();
            dependencies.Register<IRepository, RepositoryImplTwo>();
            dependencies.Register<IServiceTwo, ServiceImplOne>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IServiceTwo>();

            Assert.AreEqual("AService obj is created",((service1 as ServiceImplOne)?.Rep as RepositoryImplTwo)?.Serv.TestObject());
        }

        [TestMethod]
        public void MultipleDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IRepository, RepositoryImplOne>();
            dependencies.Register<IServiceTwo, ServiceImplOne>();
            dependencies.Register<IServiceTwo, ServiceImplOne>();

            var provider = new DependencyProvider(dependencies);
            var services = provider.Resolve<IEnumerable<IServiceTwo>>().ToArray();

            Assert.AreEqual("RepositoryImpl is created",(services[0] as ServiceImplOne)?.Rep.TestObject());
            Assert.AreEqual("RepositoryImpl is created",(services[1] as ServiceImplOne)?.Rep.TestObject());
        }

        [TestMethod]
        public void MultipleDependencyTestConstructor()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IRepository, RepositoryImplOne>();
            dependencies.Register<IServiceTwo, ServiceImplThree>();

            var provider = new DependencyProvider(dependencies);
            var service3 = provider.Resolve<IServiceTwo>();

            Assert.AreEqual((service3 as ServiceImplThree)?.Rep.ToArray()[0].TestObject(), "RepositoryImpl is created");
        }

        [TestMethod]
        public void StandartGenericDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IRepository, RepositoryImplOne>();
            dependencies.Register<IService<IRepository>, ServiceImpl<IRepository>>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService<IRepository>>();

            Assert.AreEqual((service1 as ServiceImpl<IRepository>)?.TestObject(), "ServiceImpl<TRepository> with generic is created");
        }

        [TestMethod]
        public void OpenGenericDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));
            dependencies.Register<IRepository, RepositoryImplOne>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService<IRepository>>();

            Assert.AreEqual((service1 as ServiceImpl<IRepository>)?.TestObject(), "ServiceImpl<TRepository> with generic is created");
        }

        [TestMethod]
        public void LifetimeTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterSingleton<IServiceTwo, ServiceImplOne>();
            dependencies.Register<IRepository, RepositoryImplOne>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IServiceTwo>();
            var service2 = provider.Resolve<IServiceTwo>();
            var provider1 = provider;
            var service3 = Task.Run(() => provider1.Resolve<IServiceTwo>());
            Assert.IsTrue(Equals(service3.Result, service1) && Equals(service3.Result, service2) && Equals(service1, service2));

            dependencies = new DependencyConfiguration();
            dependencies.Register<IServiceTwo, ServiceImplOne>();
            dependencies.Register<IRepository, RepositoryImplOne>();

            provider = new DependencyProvider(dependencies);
            service1 = provider.Resolve<IServiceTwo>();
            service2 = provider.Resolve<IServiceTwo>();
            service3 = Task.Run(() => provider.Resolve<IServiceTwo>());
            Assert.IsFalse(Equals(service3.Result, service1) && Equals(service3.Result, service2) && Equals(service1, service2));
        }

    }
}