namespace DependencyInjectionContainerTest.testClasses.impl
{
    internal class ServiceImpl<TRepository> : IService<TRepository>
        where TRepository : IRepository
    {
        private IRepository _rep;
        public ServiceImpl(TRepository rep)
        {
            this._rep = rep;
        }

        public string TestObject()
        {
            return "ServiceImpl<TRepository> with generic is created";
        }
    }
}