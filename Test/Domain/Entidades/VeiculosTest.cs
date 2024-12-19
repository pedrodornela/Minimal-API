using FUNDAMENTOS5_API.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class VeiculosTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            // Arrange -> Variáveis que serão testadas/validadas
            var veiculo = new Veiculo();

            // Act -> Ação executada
            veiculo.Id = 1;
            veiculo.Nome = "Picasso";
            veiculo.Marca = "Citroen";
            veiculo.Ano = 2015;

            var validaAno = veiculo.Ano > 1950;

            // Assert -> Validação
            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("Picasso", veiculo.Nome);
            Assert.AreEqual("Citroen", veiculo.Marca);
            Assert.IsTrue(validaAno);

        }
    }
}