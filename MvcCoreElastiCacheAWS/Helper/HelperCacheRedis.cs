using StackExchange.Redis;

namespace MvcCoreElastiCacheAWS.Helper
{
    public class HelperCacheRedis
    {
        private static Lazy<ConnectionMultiplexer> CreateConnection =
        new Lazy<ConnectionMultiplexer>(() =>
        {
            //AQUI ES DONDE IRA LA CADENA DE CONEXION
            return ConnectionMultiplexer.Connect("cahce-coches.cq5yxu.ng.0001.use1.cache.amazonaws.com:6379");
        });

        public static ConnectionMultiplexer Connection
        {
            get { return CreateConnection.Value; }
        }
    }
}
