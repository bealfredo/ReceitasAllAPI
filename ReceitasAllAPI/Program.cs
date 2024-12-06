using Microsoft.EntityFrameworkCore;
using ReceitasAllAPI.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configurando o Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "ReceitasAllApi",
            Description = "A aplica��o consiste em um sistema que simula um blog de receitas. O sistema permite que o usu�rio crie uma conta, fa�a login, crie, edite e exclua suas receitas. Al�m disso, o usu�rio pode visualizar as receitas p�blicas de outros usu�rios e favoritar essas receitas. Al�m disso, o usu�rio pode visualizar e desfavoritar receitas favoritas.\r\n\r\nTamb�m � poss�vel criar, editar e excluir livro de receitas, que s�o uma cole��o de receitas do usu�rio, permitindo que um autor crie v�rias cole��es de receitas, organizando suas cria��es em diferentes livros. O usu�rio pode adicionar e remover receitas de um livro de receitas, al�m de visualizar as receitas de um livro de receitas seu ou de outro usu�rio.\r\n\r\nLink para o reposit�rio: [ReceitasAllAPI](https://github.com/bealfredo/ReceitasAllAPI)",
            Contact = new OpenApiContact
            {
                Name = "ReceitasAllAPI",
                Email = "aguiaralfredo@unitins.br",
                Url = new Uri("https://www.unitins.br/")
            }
        });



    // pegamos o nome no nosso arquivo assembly e armazenamos na vari�vel
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    // pega o diret�rio base da aplica��o e concatena com o nome do arquivo xml
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);


    // seguran�a
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o Token JWT no campo abaixo"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});


// Para adicionar o contexto do banco de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReceitasAllAPI")));

// Configurar autentica��o usando JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;  // Permite HTTP para desenvolvimento
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "localhost",  // Este deve ser o seu emissor, aqui pode ser algo como "localhost" ou o nome do seu servi�o
            ValidateAudience = true,
            ValidAudience = "receitasallapi",  // Deve ser o mesmo valor que voc� define ao criar o JWT
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("chave-secreta-super-segura-256bits"))  // Chave para validar o token
        };
    });

builder.Services.AddAuthorization();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Isso configura o Swagger para armazenar o token no `localStorage`
        options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    }); // � a interface gr�fica do Swagger
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
