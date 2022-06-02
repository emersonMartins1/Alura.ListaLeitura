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

## Adicionando um novo livro utilizando rotas com template

Para adicionar um novo livro a lista de livros para ler será necessário criar uma rota com 
template. Em uma rota com template é possível passar através do endereço argumentos que podem
ser acessados pelo "HttpContext" através da propriedade "GetRouteValue()".

Logo é necessário criar uma nova rota a qual atribuíra o método responsável por responder a
requisição e capturar os argumentos passados para adicionar o novo livro. Um livro precisa de nome
e autor e isso eles serão definidos como os parâmetros dessa rota entre colchetes "{}" e com o
respectivo identificador.

Será necessário criar um método que atenda a esse novo roteamento. E por isso foi criado o método
"NovoLivroParaLer". Para acessar os parâmetros de nome e autor da rota é necessário utilizar o método
"GetRouteValue()" do HttpContext com o mesmo identificador utilizado nas rotas e sem os colchetes.
O retorno desse método é um objeto e por isso será necessário converte-ló para string.

## Exibindo os detalhes de um livro e utilizando restrições nas rotas

Foi criado uma nova rota com template que recebe um id e cujo o "RequestDelegate" deve retornar
os detalhes do livro referente a aquele id. Caso não seja colocada uma restrição e seja digitado
no navegador um  valor para o id que não seja um número é exibido no navegador um código de erro
500, referente ao erro no servidor ao tentar converter algo que não é um número para um número.

Para criar uma restrição nas requisições dessa rota é possível especificar no segmento do id o tipo
de dado que se espera receber do navegador. Caso seja informado no navegador um tipo de dado diferente
do especificado o método para resposta não é chamado e é retornado no navegador um código de erro
404, informando que não há uma reposta para aquela requisição.

## Criando um formulário e cadastrando um novo livro a partir dele

Para exiber um formulário foi necessário criar uma nova rota que direciona-se a um novo 
RequestDelegate que retornasse uma string no formato HTML. Esse HTML possui a tag de formulário
com duas tags de input referentes ao título e autor do livro, e por isso nomeadas dessa forma,
e uma tag button para enviar uma nova requisição com o formulário.

Além disso um dos atributos da tag form é um action que redireciona para outro RequestDelegate 
que fará a inclusão do livro após o formulário ser enviado. Esse RequestDelegate também deve
ser mapeado no "RouteBuilder". No action deve ser utilizada a barra inversa (/) antes do endereço
mapeado do RequestDelegate para que o .NET Core entenda que essa nova requisição está no mesmo
nível de "Cadastro/NovoLivro".

Para conseguir acessar os valores passados nos inputs do formulário HTML será utilizada a propriedade
"Request" do "HttpContext" e na propriedade Request será acessada outra propriedade chamada "Query",
a qual é referente a query string enviada pelo navegador com os inputs. Na propriedade Query é dito
qual é a chave do dado que queremos ter acessso e utilizado o método "First()".

## Separando o HTML do C Sharp

Para tornar mais fácil a manutenção do HTML e do próprio C Sharp é necessário separar o HTML em
outro arquivo com a extensão ".html". Para que as páginas HTML possam ser carregadas dinamicamente
será criado um novo método chamado "CarregaArquivoHTML" que receberá o nome do arquivo HTML a ser
retornado e retornará uma string.

Para retornar a string será utilizada a classe "File" para a partir do caminho do arquivo ele poder
ser lido do início ao fim e retornado como uma string.

Obs.: Talvez o arquivo ".html" não seja salvo automaticamente na pasta de saída do projeto e o
caminho para o arquivo não seja encontrado. Para resolver isso vá no arquivo .html e clica com o
botão direito, vá em propriedades e na opção "Copy to Output Directory" selecione "Copy if newer".

## Mandando as informações do formulário via método POST e pegando as informações

Para retirar as informações enviadas pelo navegador do endereço da requisição é necessário adicionar
o atributo "method" na tag "form" do arquivo .html informando que se deseja utilizar o método "post"
para enviar o formulário.

Após fazer essa alteração não é mais possível acessar as informações do formulário pela propriedade
"Query" do "Response", já que elas não estão mais na query string. Para pegar essas informações é
utilizada a propriedade "Form" também do "Response" e o restante do código continua o mesmo.