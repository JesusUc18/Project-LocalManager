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
    // Adapter "fake" en memoria — Implementación exacta de IClienteService.
    // ---------------------------------------------------------------------
    public class ClienteServiceFake : IClienteService
    {
        private readonly List<Cliente> _clientes;

        public ClienteServiceFake(List<Cliente> clientes) => _clientes = clientes;

        public List<Cliente> ObtenerTodos() => _clientes;

        public Cliente? ObtenerPorId(int id) => _clientes.FirstOrDefault(c => c.Id == id);

        public void Crear(Cliente cliente) => _clientes.Add(cliente);

        public void Actualizar(Cliente cliente)
        {
            var index = _clientes.FindIndex(c => c.Id == cliente.Id);
            if (index != -1) _clientes[index] = cliente;
        }

        public void Eliminar(int id) => _clientes.RemoveAll(c => c.Id == id);
    }

    // ---------------------------------------------------------------------
    // Pruebas — ClientesController (módulo de Clientes)
    // ---------------------------------------------------------------------
    public class ClientesControllerTests
    {
        private ClientesController CrearControllerConDatosDePrueba(out List<Cliente> clientesEsperados)
        {
            // Arrange — datos de prueba en memoria
            clientesEsperados = new List<Cliente>
            {
                new Cliente { Id = 1, Nombre = "Ana López" },
                new Cliente { Id = 2, Nombre = "Luis Pérez" }
            };

            var servicioFake = new ClienteServiceFake(clientesEsperados);
            return new ClientesController(servicioFake);
        }

        [Fact]
        public void Index_DevuelveTodosLosClientesRegistrados()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out var clientesEsperados);

            // Act
            var resultado = controller.Index() as ViewResult;
            var modelo = resultado?.Model as List<Cliente>;

            // Assert
            Assert.NotNull(modelo);
            Assert.Equal(clientesEsperados.Count, modelo.Count);
            Assert.Equal(clientesEsperados, modelo);
        }

        [Fact]
        public void Edit_Get_ConIdExistente_DevuelveElClienteCorrectoEnElModelo()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out _);

            // Act
            var resultado = controller.Edit(2) as ViewResult;
            var modelo = resultado?.Model as Cliente;

            // Assert
            Assert.NotNull(modelo);
            Assert.Equal(2, modelo.Id);
            Assert.Equal("Luis Pérez", modelo.Nombre);
        }

        [Fact]
        public void DeleteConfirmed_EliminaElClienteYRedirigeAIndex()
        {
            // Arrange
            var controller = CrearControllerConDatosDePrueba(out var clientesEsperados);

            // Act
            var resultado = controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(nameof(ClientesController.Index), resultado.ActionName);
            Assert.DoesNotContain(clientesEsperados, c => c.Id == 1);
        }
    }
}
