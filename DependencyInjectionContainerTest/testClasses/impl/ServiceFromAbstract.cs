namespace DependencyInjectionContainerTest.testClasses.impl
{
    public class ServiceFromAbstract : AbstractService
    {
        public override string TestObject()
        {
            return "AService obj is created";
        }
    }
}