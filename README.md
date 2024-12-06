# ReceitasAllAPI

# Tópicos em Programação III - Trabalho A2

**Aluno**: Alfredo de Souza Aguiar Neto

## Descrição da Aplicação: ReceitasAllAPI

<!-- API Web do Asp.Net Core com Entity Framework-->

A aplicação consiste em um sistema que simula um blog de receitas. O sistema permite que o usuário crie uma conta, faça login, crie, edite e exclua suas receitas. Além disso, o usuário pode visualizar as receitas públicas de outros usuários e favoritar essas receitas. Além disso, o usuário pode visualizar e desfavoritar receitas favoritas.

Também é possível criar, editar e excluir livro de receitas, que são uma coleção de receitas do usuário, permitindo que um autor crie várias coleções de receitas, organizando suas criações em diferentes livros. O usuário pode adicionar e remover receitas de um livro de receitas, além de visualizar as receitas de um livro de receitas seu ou de outro usuário.

Link para o repositório: [ReceitasAllAPI](https://github.com/bealfredo/ReceitasAllAPI)

A aplicação foi desenvolvida utilizando API Web do Asp.Net Core com Entity Framework. O Sistema possui autenticação com token JWT a fim de ser possível discriminar as áreas públicas e privadas da aplicação, assim como trabalhar com recursos relacionados ao usuário logado. A aplicação de dados foi gerada pelo ORM Entity Framework utilizando code-first.


## Passos para Execução

1. Clone o repositório para sua máquina local: [ReceitasAllAPI](https://github.com/bealfredo/ReceitasAllAPI)
2. Abra o projeto no Visual Studio
3. Abra o arquivo `appsettings.json` e altere a string de conexão para o seu banco de dados.
4. Abra o Package Manager Console e execute `cd .\ReceitasAllAPI` para navegar até o diretório do projeto
5. No Package Manager Console, execute `dotnet add package Microsoft.EntityFrameworkCore.SqlServer` para instalar o pacote do Entity Framework
6. Agora execute `dotnet add package Microsoft.EntityFrameworkCore.Tools` para instalar as ferramentas do Entity Framework
7. Execute `dotnet tool install --global dotnet-ef` para instalar a ferramenta do Entity Framework
8. Em seguida, execute `dotnet build` para compilar o projeto
9. Execute `dotnet ef database update` para criar o banco de dados e as tabelas, assim como popular o banco de dados com dados de exemplo.
10. Na interface do Visual Studio, execute o projeto. Pode ser necessário mudar para http ao invés de https para que o Swagger funcione corretamente.

A partir desse ponto, a aplicação estará rodando e você poderá acessar o Swagger para testar as rotas da API. 


### Seed do Banco de Dados

Ao executar o comando `dotnet ef database update`, o banco de dados será criado e populado com dados de exemplo. Os dados de exemplo incluem usuários, receitas, ingredientes, passos de preparo, livros de receitas e relações entre essas entidades.


- Os usuários criados no seed são:
  - Usuário 1:
      - Username: `maria`
      - Senha: `#Maria5000`

   - Usuário 2:
      - Username: `jose`
      - Senha: `#Jose5000`

- O usuário administrador criado no seed é:
  - Username: `admin`
   - Senha: `#Admin5000`

O usuário administrador pode excluir qualquer receita ou livro de receitas, tendo acesso a todas as receitas e livros de receitas, independente de serem públicos ou privados, de forma a simular um moderador do sistema.


## Funcionalidades

- Login de usuário
- Cadastro, edição, exclusão e visualização de receitas, com ingredientes e passos de preparo
- Visualização de receitas públicas de outros usuários
- Cadastro, edição, exclusão e visualização de livros de receitas
- Adição e remoção de receitas de um livro de receitas
- Visualização de livros de receitas públicos de outros usuários
- Favoritar receitas públicas de outros usuários e visualização de receitas favoritas
- Visualização de receitas favoritas

## Entidades

- **Author**: Entidade que representa um autor, usuário do sistema
- **Recipe**: Receitas cadastradas pelos usuários
- **Ingredient**: Ingredientes de uma receita
- **Step**: Passos de preparo de uma receita
- **Cookbook**: Livros de receitas cadastrados pelos usuários
- **RecipeCookbook**: Relacionamento entre receitas e livros de receitas
- **FavoriteRecipe**: Receitas favoritas de um usuário

## Relacionamentos

- Uma receita está associada a um autor
- Um ingrediente está associado a uma receita
- Um passo de preparo está associado a uma receita
- Um livro de receitas está associado a um autor
- Uma receita pode estar associada a vários livros de receitas
- Uma receita pode ser favoritada por vários usuários


## Algumas validações

- **Acesso a Receitas Privadas**
   - Usuários não podem acessar uma receita de outro usuário se ela for privada.

- **Acesso a Livros de Receitas Privados**
   - Usuários não podem acessar um livro de receitas de outro usuário se ele for privado.

- **Favoritar Receitas**
   - Não é possível favoritar receitas privadas.

- **Edição de Receitas**
   - Somente o dono da receita pode editar, adicionar e remover ingredientes e passos, além de deletar a receita.

- **Edição de Livros de Receitas**
   - Somente o dono de um livro de receitas pode editar, adicionar e remover receitas desse livro, além de deletar o livro de receitas.

- **Visibilidade de Receitas em Livros de Outros Usuários**
   - Usuários só conseguem ver receitas públicas nos livros de receitas de outros usuários.


## Autenticação

A aplicação utiliza autenticação JWT para proteger as rotas que requerem autenticação. O usuário pode se autenticar com username e senha, e receber um token JWT que deve ser enviado no cabeçalho das requisições para acessar as rotas protegidas.

O token JWT é gerado ao fazer login, no header da resposta da requisição de login (`POST /api/auth/login`) e tem duração de 24 horas.

No Swagger, é possível fazer login e obter o token JWT para testar as rotas protegidas. Para isso, clique no botão `Authorize` no canto superior direito da página do Swagger, e insira o token JWT no campo `Value`. Apenas o token JWT é necessário, sem o prefixo `Bearer`.

Todas as rotas do Swagger estão protegidas por autenticação JWT, exceto as seguintes rotas:
- `POST /api/auth/login`
- `POST /api/authors` (Cadastro de usuário)
- `GET /api/recipes` (Listar receitas públicas)
- `GET /api/cookbooks` (Listar livros de receitas públicos)


## Area do Administrador

Para moderação, foi criado um usuário administrador que pode excluir qualquer receita ou livro de receitas, tendo acesso a todas as receitas e livros de receitas, independente de serem públicos ou privados.

A área no Swagger para as rotas do administrador estão reunidas em um grupo chamado `Admin`. São protegidas por autenticação JWT, e somente o usuário administrador pode acessá-las. O usuário administrador é criado no seed do banco de dados e posui uma role `Admin`.

- **Usuário**:
  - **Admin**: admin
  - **Senha**: #Admin5000
