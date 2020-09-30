## Processos


### Como rodar a aplicação

A aplicação foi desenvolvida usando Docker, por isso desse ponto em diante vamos 
assumir que você tenha o Docker instalado na sua máquina. Caso você ainda não tenha o Docker
instalado (are you from the past? [Trenneman, Roy - IT Crowd]), por favor, visite o site 
[https://docs.docker.com/get-docker/]("https://docs.docker.com/get-docker/")

Abra um terminal (Bash, CMD, Powershell) e navegue até o diretório onde o você clonou o projeto.
Na raíz do projeto, onde o arquivo docker-compose.yaml se encontra, execute o seguinte comando:

docker-compose build --no-cache

Em seguida, execute:

docker-compose up

Pronto! A aplicação está pronta para receber requisições usando a porta http 8080.


* Talvez você precise de privilegios elevados para executar o docker no seu sistema operacional.

## Mongo Shell

Caso queira executar alguma query diretamente no MongoDB

docker exec -it nome_mongo_container mongo

O nome do container aparece logo após executar "docker-compose up", mas você pode verificar listando 
os contaiiner ativos na sua máquina. Use o seguinte comando para ver os container ativos:

docker ps

## Envio de Emails

O envio de emails foi implementado usando a API do site MailJet. É preciso se cadastrar para obter uma cahve de API.

Visite o site [MailJet]("https://www.mailjet.com/")

Após ter a chave em mãos inseri-las no arquivo appsettings.json

```json
  "ConfiguracoesNotificadorSmtp": {
    "ChaveApi": "sua chave",
    "Segredo": "seu segredo"
  }
```

### Persistência

**Ref.:**
1. https://kevsoft.net/2020/06/25/storing-guids-as-strings-in-mongodb-with-csharp.html
2. https://www.mongodb.com/blog/post/generating-globally-unique-identifiers-for-use-with-mongodb
   
### Testes de Integração

**Ref.:**
1. https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1

### Bibliotecas

* [Simple Injector]("https://simpleinjector.org/")
* [LanguageExt]("https://github.com/louthy/language-ext")
* [xUnit]("https://xunit.net/")
* [Fluent Assertions]("https://fluentassertions.com/")
* [MongoDB.Driver]("https://docs.mongodb.com/drivers/csharp")
* [Microsoft.Extensions.Logging]("https://github.com/dotnet/extensions")
* [Mailjet.Api]("https://github.com/mailjet/mailjet-apiv3-dotnet")