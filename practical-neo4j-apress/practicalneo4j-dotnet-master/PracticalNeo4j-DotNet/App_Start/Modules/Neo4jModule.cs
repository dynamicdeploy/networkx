using System;
using System.Web;
using System.Configuration;
using Neo4jClient;
using Ninject.Modules;
using Ninject.Activation;

namespace PracticalNeo4j_DotNet.App_Start.Modules
{
    public class Neo4jModule : NinjectModule
    {
        /// <summary>Loads the module into the kernel.</summary>
        public override void Load()
        {
            Bind<IGraphClient>().ToMethod(InitNeo4JClient).InSingletonScope();
        }

        private static IGraphClient InitNeo4JClient(IContext context)
        {
            var neo4JUri = new Uri(ConfigurationManager.ConnectionStrings["graphStory"].ConnectionString);
            var graphClient = new GraphClient(neo4JUri);
            graphClient.Connect();

            return graphClient;
        }
    }
}
