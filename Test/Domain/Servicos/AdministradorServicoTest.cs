using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FUNDAMENTOS5_API.Dominio.Entidades;
using FUNDAMENTOS5_API.Dominio.Servicos;
using FUNDAMENTOS5_API.Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Domain.Servicos
{

    [TestClass]
    public class AdministradorServicoTest
    {
        private DbContexto CriarContextoDeTeste()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Configurar o ConfigurationBuilder
            var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new DbContexto(configuration);

        }

        [TestMethod]
        public void TestSalvarAdministrador()
        {
            // Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador();
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";

            
            var administradorServico = new AdministradorServico(context);

            // Act
            administradorServico.Incluir(adm);
        
            // Assert
            Assert.AreEqual(1, administradorServico.Todos(1).Count());

        }

        [TestMethod]
        public void TestBuscaPorId()
        {
            // Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador();
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "Adm";

            
            var administradorServico = new AdministradorServico(context);

            // Act
            administradorServico.Incluir(adm);
            var admDoBanco = administradorServico.BuscaPorId(adm.Id);
        

            // Assert
            Assert.AreEqual(1, admDoBanco?.Id);

        }
    }
}