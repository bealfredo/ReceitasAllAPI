using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceitasAllAPI.Migrations
{
    /// <inheritdoc />
    public partial class seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseId = 100;

            migrationBuilder.Sql($@"DELETE FROM Cookbooks WHERE ID = {baseId + 6}");
            migrationBuilder.Sql($@"DELETE FROM Cookbooks WHERE ID = {baseId + 7}");

            // Remova as receitas inseridas na migração
            migrationBuilder.Sql($@"DELETE FROM Recipes WHERE ID = {baseId + 1}");
            migrationBuilder.Sql($@"DELETE FROM Recipes WHERE ID = {baseId + 2}");
            migrationBuilder.Sql($@"DELETE FROM Recipes WHERE ID = {baseId + 3}");
            migrationBuilder.Sql($@"DELETE FROM Recipes WHERE ID = {baseId + 4}");
            migrationBuilder.Sql($@"DELETE FROM Recipes WHERE ID = {baseId + 5}");

            // Remova os ingredientes inseridos na migração
            migrationBuilder.Sql($@"DELETE FROM Ingredients WHERE RecipeId = {baseId + 1}");
            migrationBuilder.Sql($@"DELETE FROM Ingredients WHERE RecipeId = {baseId + 2}");
            migrationBuilder.Sql($@"DELETE FROM Ingredients WHERE RecipeId = {baseId + 3}");
            migrationBuilder.Sql($@"DELETE FROM Ingredients WHERE RecipeId = {baseId + 4}");
            migrationBuilder.Sql($@"DELETE FROM Ingredients WHERE RecipeId = {baseId + 5}");

            // Remova os passos inseridos na migração
            migrationBuilder.Sql($@"DELETE FROM Steps WHERE RecipeId = {baseId + 1}");
            migrationBuilder.Sql($@"DELETE FROM Steps WHERE RecipeId = {baseId + 2}");
            migrationBuilder.Sql($@"DELETE FROM Steps WHERE RecipeId = {baseId + 3}");
            migrationBuilder.Sql($@"DELETE FROM Steps WHERE RecipeId = {baseId + 4}");
            migrationBuilder.Sql($@"DELETE FROM Steps WHERE RecipeId = {baseId + 5}");

            // Remova o autor padrão se necessário
            migrationBuilder.Sql("DELETE FROM Authors WHERE EmailContact = 'maria3@gmail.com'");
            migrationBuilder.Sql("DELETE FROM Authors WHERE EmailContact = 'jose3@gmail.com'");

            //// Remova o usuário padrão se necessário
            //migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE UserName = 'maria3@gmail.com'");
            //migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE UserName = 'jose3@gmail.com'");

            // Usuario 1 - Administrador
            var userId1 = 1;
            migrationBuilder.Sql($@"
                SET IDENTITY_INSERT Authors ON;
                INSERT INTO Authors (ID, UserName, PasswordHash, Admin, FirstName, LastName, Nacionality, Image, Bibliography, Pseudonym, EmailContact)
                VALUES (
                    {userId1},
                    'admin',
                    '#Admin5000',
                    'true',
                    'Admin', 
                    'Administrador', 
                    'Brasileiro', 
                    'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSg_H-BZgmjZYpmp3QDlppUbkyUX2OBbpG0Ug&s', 
                    'Administrador do sistema',
                    'admin', 
                    'admin@gmail.com'
                );
                SET IDENTITY_INSERT Authors OFF;
            ");

            // Usuario 2
            var userId2 = 2;
            migrationBuilder.Sql($@"
                SET IDENTITY_INSERT Authors ON;
                INSERT INTO Authors (ID, UserName, PasswordHash, Admin, FirstName, LastName, Nacionality, Image, Bibliography, Pseudonym, EmailContact)
                VALUES (
                    {userId2},
                    'mary',
                    '#Maria5000',
                    'false',
                    'Maria', 
                    'da Silva', 
                    'Brasileira', 
                    'https://img.freepik.com/fotos-premium/cozinheira-muito-feliz-mulher-com-fundo-branco_1042814-56143.jpg', 
                    'Uma mulher que adora cozinhar e compartilhar suas receitas',
                    'mary', 
                    'maria@gmail.com'
                );
                SET IDENTITY_INSERT Authors OFF;
            ");

            // Usuario 3
            var userId3 = 3;
            migrationBuilder.Sql($@"
                SET IDENTITY_INSERT Authors ON;
                INSERT INTO Authors (ID, UserName, PasswordHash, Admin, FirstName, LastName, Nacionality, Image, Bibliography, Pseudonym, EmailContact)
                VALUES (
                    {userId3},
                    'jota',
                    '#Jose5000',
                    'false',
                    'Jose', 
                    'Aguiar', 
                    'Brasileiro', 
                    'https://img.freepik.com/fotos-premium/um-homem-cozinhando-comida-em-uma-frigideira-em-uma-cozinha_1072138-193393.jpg', 
                    'Um homem que descobriu o prazer de cozinhar e compartilhar suas receitas',
                    'jota', 
                    'jose@gmail.com'
                );
                SET IDENTITY_INSERT Authors OFF;
            ");

            // Receita 1 - Bolo de Chocolate
            migrationBuilder.Sql($@"
                SET IDENTITY_INSERT Recipes ON;

                INSERT INTO Recipes (ID, Title, Description, Image, Difficulty, IsPrivate, PreparationTimeInMinutes, Rendimento, AuthorId, DateAdded, DateUpdated, AccentColor)
                VALUES (
                    {baseId + 1},
                    'Bolo de Chocolate', 
                    'Como não pensar em um bolo de chocolate com morango e não salivar, certo? O bolo sensação é o doce que conquista o paladar de muita gente, já que o morango quebra o gosto adocicado do chocolate.', 
                    'https://www.estadao.com.br/resizer/v2/FIVYQFU6J5ND3PYRA6XQHR4NW4.jpg?quality=80&auth=04a93b8f4c288302da64fd8a96da7bb7cc11dff70430e4ba66587218d5b6011f&width=720&height=503&focal=0,0', 
                    1,  -- Dificuldade: Fácil
                    0,  -- Público
                    45,  -- Tempo de preparação
                    '8 porções', 
                    {userId2},
                    '2024-10-03 08:15:33', 
                    '2024-10-04 09:12:33', 
                    '#8B4513'
                );

                INSERT INTO Ingredients ([Order], [Value], [RecipeId]) VALUES
                (1, '2 xícaras de farinha de trigo', {baseId + 1}),
                (2, '2 xícaras de açúcar', {baseId + 1}),
                (3, '1 xícara de leite', {baseId + 1}),
                (4, '6 colheres(sopa) de chocolate em pó', {baseId + 1}),
                (5, '1 colher(sopa) de fermento em pó', {baseId + 1}),
                (6, '6 ovos', {baseId + 1}),
                (7, 'Cobertura', {baseId + 1}),
                (8, '2 colheres(sopa) de manteiga', {baseId + 1}),
                (9, '2 xícaras de leite', {baseId + 1}),
                (10, '1 xícara de chocolate em pó', {baseId + 1});

                INSERT INTO Steps ([Order], [Value], [RecipeId]) VALUES
                (1, 'Em uma batedeira, bata as claras em neve.', {baseId + 1}),
                (2, 'Acrescente as gemas, o açúcar e bata novamente.', {baseId + 1}),
                (3, 'Adicione a farinha, o chocolate em pó, o fermento, o leite e bata por mais alguns minutos.', {baseId + 1}),
                (4, 'Despeje a massa em uma forma untada e leve para assar em forno médio(180° C), preaquecido, por 40 minutos.', {baseId + 1}),
                (5, 'Cobertura: Em uma panela, leve a fogo médio o chocolate em pó, a manteiga e o leite, deixe até ferver.', {baseId + 1}),
                (6, 'Despeje quente sobre o bolo já assado.', {baseId + 1});

                SET IDENTITY_INSERT Recipes OFF;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
