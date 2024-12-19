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
    public class VeiculoServicoTest
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
        public void TestSalvarVeiculo()
        {
            // Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

            var veiculo = new Veiculo();
            veiculo.Nome = "Uno";
            veiculo.Marca = "Fiat";
            veiculo.Ano = 2020;

            
            var veiculoServico = new VeiculoServico(context);

            // Act
            veiculoServico.Incluir(veiculo);
        
            // Assert
            Assert.AreEqual(1, veiculoServico.Todos(1).Count());

        }

        [TestMethod]
        public void TestBuscaPorId()
        {
            // Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

            var veiculo = new Veiculo();
            veiculo.Nome = "Uno";
            veiculo.Marca = "Fiat";
            veiculo.Ano = 2020;

            
            var veiculoServico = new VeiculoServico(context);

            // Act
            veiculoServico.Incluir(veiculo);
            var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id);
        

            // Assert
            Assert.AreEqual(1, veiculoDoBanco?.Id);

        }

    }
}