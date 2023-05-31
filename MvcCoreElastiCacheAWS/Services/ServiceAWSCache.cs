using Microsoft.Extensions.Caching.Distributed;
using MvcCoreElastiCacheAWS.Helper;
using MvcCoreElastiCacheAWS.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MvcCoreElastiCacheAWS.Services
{
    public class ServiceAWSCache
    {
        //private IDatabase cache;
        private IDistributedCache cache;
        public ServiceAWSCache(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<List<Coche>> GetCochesFavoritosAsync()
        {
            //SE SUPONE QUE PODRIAMOS TENER COCHES ALMACENADOS
            //MEDIANTE UNA KEY
            //ALMACENAREMOS LOS COCHES UTILIZANDO JSON Y EN UNA COLECCION
            string jsonCoches =
                await this.cache.GetStringAsync("cochesfavoritos");
            if (jsonCoches == null)
            {
                return null;
            }
            else
            {
                List<Coche> cars = JsonConvert.DeserializeObject<List<Coche>>
                    (jsonCoches);
                return cars;
            }
        }

        public async Task AddCocheAsync(Coche car)
        {
            //PREGUNTAMOS SI EXISTEN COCHES O NO TODAVIA
            List<Coche> coches = await this.GetCochesFavoritosAsync();
            //SI NO DEVUELVE NADA, ES LA PRIMERA VEZ QUE ALMACENAMOS ALGO...
            //Y CREAMOS LA COLECCION
            if (coches == null)
            {
                coches = new List<Coche>();
            }
            //AÑADIMOS EL NUEVO COCHE FAVORITO
            coches.Add(car);
            //SERIALIZAMOS A JSON
            string jsonCoches = JsonConvert.SerializeObject(coches);
            //ALMACENAMOS CON LA KEY DE REDIS
            DistributedCacheEntryOptions options =
                        new DistributedCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(30),
                        };
            await this.cache.SetStringAsync
                ("cochesfavoritos", jsonCoches, options);
        }

        public async Task DeleteCocheFavoritoAsync(int idcoche)
        {
            List<Coche> cars = await this.GetCochesFavoritosAsync();
            if (cars != null)
            {
                Coche carEliminar =
                    cars.FirstOrDefault(x => x.IdCoche == idcoche);
                cars.Remove(carEliminar);
                //COMPROBAMOS SI YA NO EXISTEN COCHES FAVORITOS
                if (cars.Count == 0)
                {
                    await this.cache.RemoveAsync("cochesfavoritos");
                }
                else
                {
                    //SERIALIZAMOS Y ALMACENAMOS LA COLECCION ACTUALIZADA
                    string jsonCoches =
                        JsonConvert.SerializeObject(cars);
                    DistributedCacheEntryOptions options =
                        new DistributedCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(30),
                        };

                    await this.cache.SetStringAsync("cochesfavoritos"
                        , jsonCoches, options);
                    //await this.cache.SetStringAsync("cochesfavoritos"
                    //    , jsonCoches, TimeSpan.FromMinutes(30));
                }
            }
        }


    }
}
