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
"Query" do "Request", já que elas não estão mais na query string. Para pegar essas informações é
utilizada a propriedade "Form" também do "Request" e o restante do código continua o mesmo.

## Gerando HTML dinamicante para lista de livros para ler

Para exibir uma página HTML com uma lista de livros foi criado um novo arquivo .html e adicionado
a tag de lista "ul", porém ao invés de adicionar os itens de lista (li) foi deixada uma string
com o nome "#NOVO-ITEM#".

No método "LivrosParaLer" é carregadado o conteúdo HTML porém antes dele ser retornado o repositório
de livros para ler é percorrido e para cada livro no repositório é substituída a string "#NOVO-ITEM#"
pelass informações do livro e a string é colocada novamente para o próximo item.

Ao final do foreach a última string "#NOVO-ITEM#" é removido do conteúdo do arquivo HTML e ele pode
ser retornado. O resultado é a geração de um HTML dinâmica para a lista de livros para ler.

## Isolando as responsabilidades em classes distintas

Dado o acúmulo de responsabilidades na classe "Startup" que deveria ter o objetivo apenas de 
prover as configurações de inicialização do servidor se fez necessário isolar os seus métodos
em classes distintas que recebessem essas responsabilidades tornando a classe "Startup" menos
dependente.

Por isso foram criadas as classes "LivrosLogica", para se responsabilzar pelos métodos de exibição
dos livros, "CadastroLogica", para os métodos de cadastro de livros nas listas, e também a classe
"HtmlUtils" que contêm o método para carregar a string html que é utilizada pelas duas classes de
lógica.

Foram necessários fazer alguns ajustes no mapeamento das rotas utilizando os métodos das classes
de lógica como estáticos.

## Utilizando o framework Asp.NET Core Mvc para Roteamento

Como pode ser visto nos últimos commits foi retirado parte das responsabilidades da classe "Startup"
para que ela deixasse de ser uma classe "faz tudo" e passasse a ser mais focada no seu papel que
é o dar as configurações de inicialização do servidor. Contudo ao olhar a classe é possível ver
que ela ainda possui o papel fazer o tratamento do roteamento, pois é no método "Configure" que
temos que definir cada nova funcionalidade que for criada na aplicação.

O ideal é que possuímos outra classe que pudesse fazer o roteamento para que não seja preciso alterar
a classe "Startup" sempre que adicionar uma nova funcionalidade.

O .NET já possui um pacote que faz esse tratamento das rotas encaminhando para as devidas funcionalidades
dependendo do endereço acessado. Esse é o Asp.NET Core Mvc.

Para utilizar o Asp.NET Core Mvc é necessário fazer a instalação do pacote e adicionar os serviços
do framework no método "ConfigureServices" da classe "Startup". Depois no método "Configure" nós
dizemos que queremos utilizar o roteamento padrão do Asp.NET Core Mvc.

Depois é preciso adequar as classes que chamamos "Logica" para o padrão do Asp.NET Core Mvc. Nele
as classes que fazem o tratamento das requisições são nomeadas pelo sufixo "Controller", ou seja,
são controllers. E nos controllers nós temos as "Actions" que são os métodos dessas classes.

Da mesma forma é possível adequar os métodos para o padrão das actions do framework. Os métodos não
precisam mais ser RequestDelegates pois o framework já fará esse tratamento, dessa forma os métodos
podem ter os retornos não do tipo "Task" mas dos tipos que eles querem retornar, como "string" por
exemplo. 

Logo também não é mais preciso do parâmetro "HttpContext" nas actions. Para enviar o retorno das
requisoções podesse simplesmente retornar o tipo definido no método sem a necessidade do "HttpContext".
E para pegar os dados da requisição o framework adicionou um estágio no request pipeline após o roteamento
e antes da execução do método que é chamado de "Model Binding" em que o Asp.NET Core Mvc busca atender
a um modelo definido no método chamado pelo navegador. Esse modelo é definido nos parâmetros do método
e o framework busca uma forma de atender esse modelo antes da execução do método.

Ao observar os métodos é possível ver que eles não possuem mais nenhuma dependência com a tecnologia
de desenvolvimento web e parecessem simplesmente métodos comuns de um classe. E isso traz uma das
vantagens de utilizar o Asp.NET Core Mvc que é poder testar os métodos sem precisar subir um servidor.

## Retornado o HTML com Asp.NET Core Mvc

Foi necessário fazer algumas alterações nos métodos que retornam HTML pois agora é o framework que
trata dos retorno dos métodos e não mais o código próprio, o método anterior agora retorna texto 
puro. Isso acontece pois o framework adiciona um estágio após o processamento da action que é 
chamado de "ExecuteResult" o qual trata o retorno dos dados dependendo do tipo informado no 
retorno.

O framework encapsula o resultado das actions em uma interface chamada "IActionResult". Cada tipo
de dado possui uma implementação diferente do "IActionResult" que é o tipo de dado retornado pela
requisição. A implementação que retorna conteúdo HTML é chamada de "ViewResult". Ao instanciar um
objeto do tipo "ViewResult" é possível utilizar a propriedade "ViewName" e dizer qual a view que
deve ser carregada para retorno pela action.

Porém apenas fazer essas alterações ainda não vai retornar o resultado esperado e o retorno será
o status code 500. Para verificar qual a causa desse código de erro no servidor é possível habilitar
uma configuração, a qual só é recomendado ser usado em ambientes de testes e desenvolvimento, chamada
"UseDeveloperExceptionPage()", que é um método que é acessado através do objeto app do "Configure"
da classe "Startup".

Ao tentar rodar o código novamente é recebido uma informação de erro mais detalhada a qual diz
que o arquivo não foi encontrado e lista as pastas em que ele foi procurado, entre elas está o
caminho "Views/PrefixoControlador" e é possível ver também que o arquivo o qual o framework procura
tem a extensão "cshtml" e não "html".

Após fazer esses ajustes o erro será resolvido, mas é provável que seja exibido outro erro na página
referente a falta de uma configuração no arquivo "csproj". Para resolver esse erro é necessário 
ir no arquivo "csproj" na tag "PropertyGroup" e adicionar a tag "PreserveCompilationContext" com 
o valor true, a tag mencionada é exigida para compilar as views, quando está habilitada é indicado
ao .NET que as views serão compiladas em tempo de execução. Também é possível trabalhar com ela 
desabilitada usando ela em conjunto com outras opções para pré-compilar as views. 

Depois é necessário limpar a solução e fazer o rebuild. Ao subir o servidor novamente e requisitar
a action que devolve o conteúdo html será visto o conteúdo esperado.

## O motor de views Razor

Ao tentar executar a action "ParaLer" do controller "Livros" era possível ver que o HTML para a lista
não estava mais sendo gerado dinamicanete após as alterações que precisarem ser feitas no método
para se adequar as convenções do Asp.NET Core Mvc, já que agora não temos mais o controle do arquivo
carregado.

Para gerar HTML dinâmico para uma lista como estava sendo feito o framework possui um motor de views
chamado Razor em que é possível utilizar código C Sharp para executar dentro do arquivo da view, por
isso o arquivo não possui mais a extensão "html" e sim "cshtml", porque o Razor faz a compilação do
"cshtml" para gerar o HTML da página, tendo assim um HTML dinâmico.

Para utilizar o código C Sharp dentro da view devesse indicar ao Razor utilizando o caractere "@",
porém ainda há a necessidade de passarmos o conteúdo que será trabalhado pelo Razor da action para
a view. Por isso o framework possui uma classe chamada "Controller" da qual podesse fazer a herança
para o nosso controller.

Essa classe "Controller" possui a propriedade "ViewBag" a qual pode ser utilizada para carregar
variáveis da action para a  view. Para que a "ViewBag" seja inicializada é necessário utilizar o
método "View" da classe pai "Controller". E com isso podemos acessar o conteúdo da "ViewBag" 
no "cshtml".