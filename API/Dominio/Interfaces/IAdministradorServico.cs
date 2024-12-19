using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FUNDAMENTOS5_API.Dominio.DTOs;
using FUNDAMENTOS5_API.Dominio.Entidades;

namespace FUNDAMENTOS5_API.Infraestrutura.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);
        Administrador Incluir(Administrador administrador);
        Administrador? BuscaPorId(int id);
        List<Administrador> Todos(int? pagina);

    }
}