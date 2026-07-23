using System.Collections.Generic;
using System.Linq;
using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using LocalManager.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace LocalManager.Tests.Controllers
{
    // ---------------------------------------------------------------------
    // Adapter "fake" en memoria — Implementación exacta de ICajaService.
    // ---------------------------------------------------------------------
    public class CajaServiceFake : ICajaService
    {
        private readonly List<Caja> _cajas;

        public CajaServiceFake(List<Caja> cajas) => _cajas = cajas;

        public List<Caja> ObtenerTodas() => _cajas;

        public Caja? ObtenerPorId(int id) => _cajas.FirstOrDefault(c => c.Id == id);

        public List<Caja> ObtenerAbiertas() => _cajas.Where(c => c.Estado == "Abierta").ToList();

        public void Abrir(Caja caja) => _cajas.Add(caja);

        public void Cerrar(int id, decimal montoCierre)
        {
            var caja = _cajas.FirstOrDefault(c => c.Id == id);
            if (caja is null) return;
            caja.Estado = "Cerrada";
            caja.MontoCierre = montoCierre;
        }
    }

    // ---------------------------------------------------------------------
    // Pruebas — CajaController (módulo de Caja)
    // ---------------------------------------------------------------------
    public class CajaControllerTests
    {
        private CajaController CrearControllerConDatosDePrueba(out List<Caja> cajasEsperadas)
        {
            // Arrange — datos de prueba en memoria
            cajasEsperadas = new List<Caja>
            {
                new Caja { Id = 1, MontoInicial = 500m, Estado = "Abierta" },
                new Caja { Id = 2, MontoInicial = 300m, Estado = "Cerrada", MontoCierre = 320m }
            };

            var servicioFake = new CajaServiceFake(cajasEsperadas);
            return new CajaController(servicioFake);
        }

        [Fact]
        public void Index_DevuelveTodasLasCajasRegistradas()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out var cajasEsperadas);

            // Act
            var resultado = controller.Index() as ViewResult;
            var modelo = resultado?.Model as List<Caja>;

            // Assert
            Assert.NotNull(modelo);
            Assert.Equal(cajasEsperadas.Count, modelo.Count);
            Assert.Equal(cajasEsperadas, modelo);
        }

        [Fact]
        public void Abrir_Post_ConCajaValida_LaAgregaYRedirigeAIndex()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out var cajasEsperadas);
            var nuevaCaja = new Caja { Id = 3, MontoInicial = 100m, Estado = "Abierta" };

            // Act
            var resultado = controller.Abrir(nuevaCaja) as RedirectToActionResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(nameof(CajaController.Index), resultado.ActionName);
            Assert.Contains(cajasEsperadas, c => c.Id == 3 && c.MontoInicial == 100m);
        }

        [Fact]
        public void Cerrar_Get_ConIdInexistente_DevuelveNotFound()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out _);

            // Act
            var resultado = controller.Cerrar(999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }
    }
}
