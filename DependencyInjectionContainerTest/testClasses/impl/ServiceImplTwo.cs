namespace DependencyInjectionContainerTest.testClasses.impl
{
    public class ServiceImplTwo : IServiceTwo
    {
        public readonly IRepository Rep;

        public ServiceImplTwo(IRepository rep)
        {
            this.Rep = rep;
        }
    }
}