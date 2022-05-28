# Alura.ListaLeitura

O objetivo desse projeto para o curso é criar uma WebApp Asp.NET CORE de uma lista de livros para ler, lendo e lidos.

O projeto entregue foi uma aplicação console que no main lia um arquivo CSV com as listas citadas e imprimia no terminal.

## Commit: f44189d885be9ed4ea10b601d3086b6cf0b349d7

Para criar uma página da web que apresente as listas de leitura é necessário primeiro criar um servidor HTTP, por isso
precisamos transformar o projeto de uma aplicação console para uma aplicação servidor que receba chamadas HTTP que retorne
essas listas ao ser consultada no navegador.

Para isso foi necessário utilizar um objeto host do tipo WebHost, mas utilizando a interface IWebHost. Depois de declarar o objeto
é necessário inicializa-lo com WebHostBuilder() e construir o host com o método Build().

Porém ainda é necessário definir algumas configurações para o host ficar pronto. Uma delas é definir qual será o servidor web que
utilizado pela aplicação e nesse caso foi o Kestrel, utilizando o método UseKestrel().

Por fim é necessário implementar definições de inicialização do servidor através do método UseStartup<nome_classe_com_definicoes>,
que são definidas em uma outra classe que pode ter qualquer nome, mas por convenção é chamada de Startup.

Ao chamar o método Run() do objeto inicializado na variável "host" e executar o código é possível verificar que o servidor já está
rodando, mas por enquanto suas chamadas não retornam nenhum conteúdo.
