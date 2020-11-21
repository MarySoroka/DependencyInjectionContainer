using System.Collections.Generic;

namespace DependencyInjectionContainerTest.testClasses.impl
{
    public class ServiceImplThree: IServiceTwo
    {
        public readonly IEnumerable<IRepository> Rep;

        public ServiceImplThree(IEnumerable<IRepository> rep)
        {
            this.Rep = rep;
        }
    }
}