# Alura.ListaLeitura

O objetivo desse projeto para o curso é criar uma WebApp Asp.NET CORE de uma lista de livros para ler, lendo e lidos.

O projeto entregue foi uma aplicação console que no main lia um arquivo CSV com as listas citadas e imprimia no terminal.

## Criando um Servidor HTTP

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

## Retornando uma Lista de Livros para Ler na Requisição

Nesse commit o objetivo era fazer com que o servidor retornasse a lista de livros para ler sempre que recebesse uma requisição.
Para isso foi criado o método "LivrosParaLer", no corpo do método é instanciada o objeto de uma classe que possui as listas de livros
e então é chamado o método que retorna a lista de livros para ler. 

Essa lista precisa ser encapsulada por um objeto do tipo "HttpContext". Para delegar a atividade de instanciar esse objeto para o dotnet 
o objeto do tipo "HttpContext" é colocado como parâmetro do método "LivrosParaLer", tendo o objeto "context" colocado como argumento é 
possível acessar suas propriedades e uma delas é o "Response" que dá acesso ao método "WriteAsync" que pode receber como argumento uma String.

Para que o método "LivrosParaLer" seja acionado quando o navegador fizer a requisição do servidor é necessário colocar essa configuração no
método "Configure". Para isso o método "Configure" deve ter como parâmetro um objeto do tipo "IApplicationBuilder" que da acesso ao método
"Run()" onde é possível colocar o método "LivrosParaLer" como argumento para que sempre que o navegador faça a requição do servidor esse método
seja acionado.

O método "Run()" recebe como parâmetro um "RequestDelegate", o qual deve retornar um objeto do tipo "Task", por isso o método "LivrosParaLer"
deve ter o retorno do tipo "Task". O método "WriteAsync" mencionado anteriormente já retorna um objeto do tipo "Task" então basta informar o retorno.
Após isso basta executar a aplicação novamente e acessar o endereço do servidor que a lista de livros para ler será retornada.

## Criando um Roteamento Simples Para os Repositórios de Livros

Foi criado um novo método Task na classe Startup a qual será responsável por identificar qual o
recurso deve ser retornado ao navegador dependendo do path inserido no navegador.

Para isso foi criado uma variável do tipo Dictionary com as chaves do tipo string, referente ao
path inserido no navegador, e os values do tipo RequestDelegate, referente aos métodos Task.

Para identificar o path inserido no navegador é utilizada a propriedade Request do parâmetro
HttpContext do método Roteamento. Ao verificar se o path contido no navegador é igual a uma
chave do dicionário é chamado o RequestDelegate contido através do método "Invoke()" passando
o context do Roteamento como argumento.

Caso o path não esteja contido no dicionário é retornando uma mensagem de rota não encotrada
e o StatusCode 404.

Para que o servidor seja inicializado com as opções de roteamento o método "Roteamento" é passado
como argumento no app.Run() do método Configure.

## Substituindo o método de roteamento pelo serviço de roteamento do Asp.NET Core

Para utilizar o serviço de roteamento do Asp.NET Core foi necessário no método adicionar o serviço
de roteamento na classe "Startup" através do método "ConfigureServices" fazendo a injeção de dependência
do variável "services" do tipo "IServicesCollection", no corpo do método é chamado o método
"AddRouting" do "services" para adicionar o serviço de roteamento.

Já no método "Configure" foi necessário criar uma variável "builder" do tipo "RouteBuilder"
na qual é passado o "app" como argumento. Com a variável "builder" é possível configurar de forma
parecida com a que foi feito com a variável do tipo "Dictionary" chamando o método "MapRoute"
do "builder".

Depois de configuradas as rotas é chamado o método "Build()" para construir um objeto do tipo
"IRouter" que será usado como argumento em "app.UserRouter()" para passar as rotas. Ao testar
as requisições é percebido que o servidor continua atendendo as requisições.
