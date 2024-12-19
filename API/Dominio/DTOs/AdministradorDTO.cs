using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FUNDAMENTOS5_API.Dominio.Enuns;

namespace FUNDAMENTOS5_API.Dominio.DTOs
{
    public class AdministradorDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public Perfil? Perfil { get; set; } = default!;
    }
}