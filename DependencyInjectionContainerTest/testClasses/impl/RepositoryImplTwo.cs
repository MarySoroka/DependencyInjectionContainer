namespace DependencyInjectionContainerTest.testClasses.impl
{
    public class RepositoryImplTwo: IRepository
    {
        public AbstractService Serv { get; }

        public RepositoryImplTwo(AbstractService serv) {
            this.Serv = serv;
        }

        public string TestObject()
        {
            return "RepositoryImp2 obj is created";
        }
    }
}