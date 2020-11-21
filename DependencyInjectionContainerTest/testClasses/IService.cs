namespace DependencyInjectionContainerTest.testClasses
{
    interface IService<TRepository> where TRepository : IRepository
    {
        string TestObject();
    }

}