namespace DependencyInjectionContainerTest.testClasses.impl
{
    public class ServiceImplOne: IServiceTwo
    {
        public readonly IRepository Rep;

        public ServiceImplOne(IRepository rep)
        {
            this.Rep = rep;
        }
    }
}