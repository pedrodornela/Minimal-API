using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FUNDAMENTOS5_API.Dominio.DTOs;
using FUNDAMENTOS5_API.Dominio.Entidades;
using FUNDAMENTOS5_API.Dominio.Enuns;
using FUNDAMENTOS5_API.Dominio.ModelViews;
using FUNDAMENTOS5_API.Dominio.Servicos;
using FUNDAMENTOS5_API.Infraestrutura.Db;
using FUNDAMENTOS5_API.Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {   
                Configuration = configuration;
                key = Configuration?.GetSection("Jwt")?.ToString() ?? "";            
        }

        private string key = "";

        public IConfiguration Configuration { get; set; } = default!;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization();

            services.AddScoped<IAdministradorServico, AdministradorServico>();
            services.AddScoped<IVeiculoServico, VeiculoServico>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT aqui:"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme 
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string [] {}
                    }
                    
                });
            });


            services.AddDbContext<DbContexto>(options =>
            {
                options.UseMySql(
                    Configuration.GetConnectionString("MySql"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthentication();            

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoint => { 
                #region Home
                endpoint.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                #endregion


                #region Administradores

                string GerarTokenJwt(Administrador administrador)
                {
                    if(string.IsNullOrEmpty(key)) return string.Empty;

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>(){
                        new ("Email", administrador.Email),        
                        new ("Perfil", administrador.Perfil),
                        new (ClaimTypes.Role, administrador.Perfil)
                    };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);

                }


                endpoint.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
                {
                    var adm = administradorServico.Login(loginDTO);

                    if (adm != null)
                    {
                        string token = GerarTokenJwt(adm);
                        return Results.Ok(new AdministradorLogado{
                            Email = adm.Email,
                            Perfil = adm.Perfil,
                            Token = token
                        });
                    }
                    return Results.Unauthorized();
                }).AllowAnonymous().WithTags("Administradores");

                endpoint.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
                {
                    var adms = new List<AdministradorModelView>();
                    var administradores = administradorServico.Todos(pagina);

                    foreach (var adm in administradores)
                    {
                        adms.Add(new AdministradorModelView
                        {
                            Id = adm.Id,
                            Email = adm.Email,
                            Perfil = adm.Perfil
                        });
                    }

                    return Results.Ok(adms);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm" })
                .WithTags("Administradores");

                endpoint.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
                {

                    var administrador = administradorServico.BuscaPorId(id);

                    if (administrador == null)
                        return Results.NotFound("Administrador não encontrado!");

                    return Results.Ok(new AdministradorModelView
                    {
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                    });
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm" })
                .WithTags("Administradores");

                endpoint.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
                {
                    var validacao = new ErrosDeValidacao
                    {
                        Mensagens = new List<string>()
                    };

                    if (string.IsNullOrEmpty(administradorDTO.Email))
                        validacao.Mensagens.Add("Email não pode ser Vazio!");

                    if (string.IsNullOrEmpty(administradorDTO.Senha))
                        validacao.Mensagens.Add("Senha não pode ser Vazia!");

                    if (administradorDTO.Perfil == null)
                        validacao.Mensagens.Add("Perfil não pode ser Vazio!");

                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);

                    var administrador = new Administrador
                    {
                        Email = administradorDTO.Email,
                        Senha = administradorDTO.Senha,
                        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
                    };

                    administradorServico.Incluir(administrador);

                    return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView
                    {
                        Id = administrador.Id,
                        Email = administrador.Email,
                        Perfil = administrador.Perfil
                    });

                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm" })
                .WithTags("Administradores");

                #endregion

                #region Veículos

                static ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
                {
                    var validacao = new ErrosDeValidacao
                    {
                        Mensagens = []
                    };

                    if (string.IsNullOrEmpty(veiculoDTO.Nome))
                        validacao.Mensagens.Add("O nome não pode ser vazio!");

                    if (string.IsNullOrEmpty(veiculoDTO.Marca))
                        validacao.Mensagens.Add("A marca não pode ficar em branco!");

                    if (veiculoDTO.Ano < 1950)
                        validacao.Mensagens.Add("Veículo muito antigo, aceita somente anos posteriores a 1950!");

                    return validacao;
                }


                endpoint.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
                {

                    var validacao = validaDTO(veiculoDTO);
                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);

                    var veiculo = new Veiculo
                    {
                        Nome = veiculoDTO.Nome,
                        Marca = veiculoDTO.Marca,
                        Ano = veiculoDTO.Ano,
                    };
                    veiculoServico.Incluir(veiculo);

                    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm,Editor" })
                .WithTags("Veículos");


                endpoint.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
                {
                    var veiculos = veiculoServico.Todos(pagina);
                    return Results.Ok(veiculos);
                }).RequireAuthorization().WithTags("Veículos");


                endpoint.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
                {

                    var veiculo = veiculoServico.BuscaPorId(id);

                    if (veiculo == null)
                        return Results.NotFound("Veículo não encontrado!");

                    return Results.Ok(veiculo);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm,Editor" })
                .WithTags("Veículos");


                endpoint.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
                {

                    var veiculo = veiculoServico.BuscaPorId(id);

                    if (veiculo == null)
                        return Results.NotFound("Veículo não encontrado!");

                    var validacao = validaDTO(veiculoDTO);

                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);

                    veiculo.Nome = veiculoDTO.Nome;
                    veiculo.Marca = veiculoDTO.Marca;
                    veiculo.Ano = veiculoDTO.Ano;

                    veiculoServico.Atualizar(veiculo);

                    return Results.Ok(veiculo);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm" })
                .WithTags("Veículos");


                endpoint.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
                {

                    var veiculo = veiculoServico.BuscaPorId(id);

                    if (veiculo == null)
                        return Results.NotFound("Veículo não encontrado!");

                    veiculoServico.Apagar(veiculo);

                    return Results.NoContent();
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm" })
                .WithTags("Veículos");

                #endregion
            });
        }


    }
}