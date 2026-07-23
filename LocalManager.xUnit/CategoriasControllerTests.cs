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
    // Adapter "fake" en memoria — Implementación exacta de ICategoriaService.
    // ---------------------------------------------------------------------
    public class CategoriaServiceFake : ICategoriaService
    {
        private readonly List<Categoria> _categorias;

        public CategoriaServiceFake(List<Categoria> categorias) => _categorias = categorias;

        public List<Categoria> ObtenerTodas() => _categorias;

        public Categoria? ObtenerPorId(int id) => _categorias.FirstOrDefault(c => c.Id == id);

        public void Crear(Categoria categoria) => _categorias.Add(categoria);

        public void Actualizar(Categoria categoria)
        {
            var index = _categorias.FindIndex(c => c.Id == categoria.Id);
            if (index != -1) _categorias[index] = categoria;
        }

        public void Eliminar(int id) => _categorias.RemoveAll(c => c.Id == id);
    }

    // ---------------------------------------------------------------------
    // Pruebas — CategoriasController (módulo de Inventario)
    // ---------------------------------------------------------------------
    public class CategoriasControllerTests
    {
        private CategoriasController CrearControllerConDatosDePrueba(out List<Categoria> categoriasEsperadas)
        {
            // Arrange — datos de prueba en memoria
            categoriasEsperadas = new List<Categoria>
            {
                new Categoria { Id = 1, Nombre = "Bebidas" },
                new Categoria { Id = 2, Nombre = "Snacks" }
            };

            var servicioFake = new CategoriaServiceFake(categoriasEsperadas);
            return new CategoriasController(servicioFake);
        }

        [Fact]
        public void Index_DevuelveTodasLasCategoriasRegistradas()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out var categoriasEsperadas);

            // Act
            var resultado = controller.Index() as ViewResult;
            var modelo = resultado?.Model as List<Categoria>;

            // Assert
            Assert.NotNull(modelo);
            Assert.Equal(categoriasEsperadas.Count, modelo.Count);
            Assert.Equal(categoriasEsperadas, modelo);
        }

        [Fact]
        public void Create_Post_ConCategoriaValida_LaAgregaYRedirigeAIndex()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out var categoriasEsperadas);
            var nuevaCategoria = new Categoria { Id = 3, Nombre = "Limpieza" };

            // Act
            var resultado = controller.Create(nuevaCategoria) as RedirectToActionResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(nameof(CategoriasController.Index), resultado.ActionName);
            Assert.Contains(categoriasEsperadas, c => c.Id == 3 && c.Nombre == "Limpieza");
        }

        [Fact]
        public void Edit_Get_ConIdInexistente_DevuelveNotFound()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out _);

            // Act
            var resultado = controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(resultado);
        }
    }
}
