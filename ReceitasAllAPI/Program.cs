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
            Description = "API para gerenciamento de receitas",
            Contact = new OpenApiContact
            {
                Name = "ReceitasAllAPI",
                Email = "aguiaralfredo@unitins.br",
                Url = new Uri("https://www.unitins.br/")
            }
        });



    // pegamos o nome no nosso arquivo assembly e armazenamos na variável
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    // pega o diretório base da aplicação e concatena com o nome do arquivo xml
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);


    // segurança
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

// Configurar autenticação usando JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;  // Permite HTTP para desenvolvimento
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "localhost",  // Este deve ser o seu emissor, aqui pode ser algo como "localhost" ou o nome do seu serviço
            ValidateAudience = true,
            ValidAudience = "receitasallapi",  // Deve ser o mesmo valor que você define ao criar o JWT
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
    }); // é a interface gráfica do Swagger
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
