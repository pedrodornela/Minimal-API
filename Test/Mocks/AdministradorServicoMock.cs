using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FUNDAMENTOS5_API.Dominio.DTOs;
using FUNDAMENTOS5_API.Dominio.Entidades;
using FUNDAMENTOS5_API.Dominio.Servicos;
using FUNDAMENTOS5_API.Infraestrutura.Interfaces;

namespace Test.Mocks
{
    public class AdministradorServicoMock : IAdministradorServico
    {

        private static List<Administrador> administradores = new List<Administrador>(){
            new () {
                Id = 1,
                Email = "adm@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            },
            new () {
                Id = 2,
                Email = "editor@teste.com",
                Senha = "123456",
                Perfil = "Editor"
            },
        };

        public Administrador? BuscaPorId(int id)
        {
            return administradores.Find(a => a.Id == id);
        }

        public Administrador Incluir(Administrador administrador)
        {
            administrador.Id = administradores.Count() + 1;
            administradores.Add(administrador);
            return administrador;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            return administradores.Find(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
        }

        public List<Administrador> Todos(int? pagina)
        {
            return administradores;
        }
    }
}