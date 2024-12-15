using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FUNDAMENTOS5_API.Dominio.DTOs;
using FUNDAMENTOS5_API.Dominio.Entidades;

namespace FUNDAMENTOS5_API.Infraestrutura.Interfaces
{
    public interface IVeiculoServico
    {
        List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null);
        Veiculo? BuscaPorId(int id);
        void Incluir(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Apagar(Veiculo veiculo);

    }
}