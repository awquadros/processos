## Processos


### Como rodar a aplicação

A aplicação foi contairizada usando Docker, por isso desse ponto em diante vamos 
assumir que você tenha o Docker instalado na sua máquina. Caso você ainda não tenha o Docker
instalado (are you from the past? [Trenneman, Roy - IT Crowd]), por favor, visite o site 
[https://docs.docker.com/get-docker/]("https://docs.docker.com/get-docker/")

Na raíz do projeto, onde o Dockerfile se encontra, execute o seguinte comando num terminal:

docker build -f Dockerfile.ubuntu-x64.focal -t awfq.processos.api .


sudo docker run -d -p 8080:80 --name processos-api awfq.processos.api


* Talvez você precise de privilegios elevados para executar o docker no seu sistema operacional.

## Mongo Shell

docker exec -it mongoContainer mongo

## Implementação

**Processos**

http://localhost:8080/api/processos/situacoes?api-version=1.0

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