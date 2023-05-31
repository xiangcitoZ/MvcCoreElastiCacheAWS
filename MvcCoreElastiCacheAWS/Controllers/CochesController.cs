﻿using Microsoft.AspNetCore.Mvc;
using MvcCoreElastiCacheAWS.Models;
using MvcCoreElastiCacheAWS.Repositories;
using MvcCoreElastiCacheAWS.Services;

namespace MvcCoreElastiCacheAWS.Controllers
{
    public class CochesController : Controller
    {
        private RepositoryCoches repo;
        private ServiceAWSCache serviceCache;
        public CochesController(RepositoryCoches repo, 
            ServiceAWSCache service)
        {
            this.repo = repo;
            this.serviceCache = service;
        }

        public IActionResult Index()
        {
            List<Coche> coches = this.repo.GetCoches();
            return View(coches);
        }

        public IActionResult Details(int id)
        {
            Coche car = this.repo.FindCoche(id);
            return View(car);
        }

        //METODOS PARA CACHE REDIS
        public async Task<IActionResult> SeleccionarFavorito
            (int idcoche)
        {
            //DEBEMOS BUSCAR EL COCHE A ALMACENAR DENTRO DEL XML
            Coche car = this.repo.FindCoche(idcoche);
            await this.serviceCache.AddCocheAsync(car);
            return RedirectToAction("Favoritos");
        }

        public async Task<IActionResult> Favoritos()
        {
            List<Coche> coches = await this.serviceCache.GetCochesFavoritosAsync();
            return View(coches);
        }

        public async Task<IActionResult> EliminarFavorito(int idcoche)
        {
            await this.serviceCache.DeleteCocheFavoritoAsync(idcoche);
            return RedirectToAction("Favoritos");
        }




    }
}
