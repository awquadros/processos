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

## Implementação

### Testes de Integração

**Ref.:**
1. https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1