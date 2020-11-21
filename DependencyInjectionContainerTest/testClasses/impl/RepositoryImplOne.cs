namespace DependencyInjectionContainerTest.testClasses.impl
{
    public class RepositoryImplOne : IRepository
    {
        public RepositoryImplOne() { }
       
        public string TestObject()
        {
            return "RepositoryImpl is created";
        }
    }
}