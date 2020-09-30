
# Referência da Api Http


## Processos API
---
<br/>

**Criar Processo**

Cadastra uma Pessoa que poderá vir a ser responsável por um ou mais Processos.

```
POST http://localhost:8080/api/processos?api-version=1.0
```

```bash
curl --location --request POST 'http://localhost:8080/api/processos?api-version=1.0' \
--header 'Content-Type: application/json' \
--data-raw '{ "processoUnificado": "3513038-00.2016.8.23.0032", "dataDistribuicao": "2020-09-01T09:34:55", "segredoJustica": false, 
"pastaFisicaCliente": "PAST-156", "responsaveis": ["c8be0171-fe22-4f05-a0f0-7482057107b0"] , "situacaoId": 1, "descricao": null, "paiId": null }'
```

| PARÂMETRO de CORPO EM JSON | TIPO          | REQUERIDO | DESCRIÇÂO                                                                                                  |
| -------------------------- | :------------ | :-------: | ---------------------------------------------------------------------------------------------------------- |
| `processoUnificado`        | string        |    Sim    | Número do processo unificado                                                                               |
| `dataDistribuicao`         | string        |    Não    | Data de distribuição, caso informada, deve ser menor ou igual a data atual e fornecida no formato ISO-8601 |
| `segredoJustica`           | boolean       |    Não    | Um valor indicando se o Processo está sob segredo de Justiça                                               |
| `pastaFisicaCliente`       | string        |    Não    | A pasta física do Processo                                                                                 |
| `responsaveis`             | array[string] |    Sim    | Uma lista de identificadores de Responsáveis                                                               |
| `situacaoId`               | int           |    Sim    | O identificador da Situação do Processo                                                                    |
| `descricao`                | string        |    Não    | Uma descrição qualquer para o Processo                                                                     |
| `paiId`                    | string        |    Não    | Um identificado único de um Processo que será considerado o Pai                                            |

<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo uma representação do Processo criado. Qualquer falha de entrada retornará um status Http 400 Bad Request e uma lista com informações obre o ocorrido.

<br/>


**Edita Processo**

Edita as informações de um processo. Note que Processo algum pode ser removido quando estiver na Situação "finalizada"

```
PUT http://localhost:8080/api/processos?api-version=1.0
```

```bash
curl --location --request PUT 'http://localhost:8080/api/processos?api-version=1.0' \
--header 'Content-Type: application/json' \
--data-raw '{
    "id": "b31b36ee-737f-422c-85f7-4393955fa03f",
    "paiId": null,
    "processoUnificado": "35130380020168230031",
    "dataDistribuicao": "2020-09-01T09:34:55",
    "segredoJustica": false,
    "pastaFisicaCliente": "PAST-159",
    "descricao": null,
    "responsaveis": [
        "c8be0171-fe22-4f05-a0f0-7482057107b0", "1664a24c-0609-4260-805b-809f7e64cc22"
    ],
    "situacaoId": 1
}'
```

| PARÂMETRO de CORPO EM JSON | TIPO          | REQUERIDO | DESCRIÇÂO                                                                                                  |
| -------------------------- | :------------ | :-------: | ---------------------------------------------------------------------------------------------------------- |
| `processoUnificado`        | string        |    Sim    | Número do processo unificado                                                                               |
| `dataDistribuicao`         | string        |    Não    | Data de distribuição, caso informada, deve ser menor ou igual a data atual e fornecida no formato ISO-8601 |
| `segredoJustica`           | boolean       |    Não    | Um valor indicando se o Processo está sob segredo de Justiça                                               |
| `pastaFisicaCliente`       | string        |    Não    | A pasta física do Processo                                                                                 |
| `responsaveis`             | array[string] |    Sim    | Uma lista de identificadores de Responsáveis                                                               |
| `situacaoId`               | int           |    Sim    | O identificador da Situação do Processo                                                                    |
| `descricao`                | string        |    Não    | Uma descrição qualquer para o Processo                                                                     |
| `paiId`                    | string        |    Não    | Um identificado único de um Processo que será considerado o Pai                                            |

<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo uma representação do Processo antes da edição.

Se o usuário informar algum dado inválido para a requisição será retorna o status 400 Bad request com uma representação de uma ou mais mensagens de erro.

**Remoção Responsável**

Remove um Processo.

```
DELETE http://localhost:8080/api/processos/{id}f?api-version=1.0
```

```bash
curl --location --request DELETE 'http://localhost:8080/api/processos/b31b36ee-737f-422c-85f7-4393955fa03f?api-version=1.0'
```


| PARÂMETRO de CAMINHO | TIPO   | REQUERIDO | DESCRIÇÃO                         |
| -------------------- | :----- | :-------: | --------------------------------- |
| `{id}`               | string |    Sim    | Deve conter o identificador único |


<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo uma representação do Processo removido, caso contrário retorn Http 400 Bad request.

**Consulta Processo**

Connsulta Processos usando os filtros determinados

```
GET http://localhost:8080/api/processos?api-version=1.0&processoUnificado=35130380020168230031
```

```bash
curl --location --request GET 'http://localhost:8080/api/responsaveis?cpf=45553200008&processo=3513038-00.2016.8.23.0027&api-version=1.0&nome=ant'
```


| PARÂMETRO de QUERY        | TIPO    | REQUERIDO | DESCRIÇÃO                                                  |
| ------------------------- | :------ | :-------: | ---------------------------------------------------------- |
| `processoUnificado`       | string  |    Não    | Deve conter os 20 números que compõem o Processo Unificado |
| `dataInicialDistribuicao` | string  |    Não    | Pode conter a Data Inicial do período de busca             |
| `dataFinalDistribuicao`   | string  |    Não    | Pode conter a Data Final do período de busca               |
| `situacaoId`              | int     |    Não    | Deve conter um identificado de Situação                    |
| `parcialPastafisica`      | string  |    Não    | Pode conter uma parte ou todo o nome de uma pasta física   |
| `segredoJustica`          | boolean |    Não    | Um indicador par busca de Processos em Segredo de Justiça  |
| `parcialNomeResponsavel`  | string  |    Não    | Pode conter uma parte ou todo o nome de um Responsável     |

<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo a representação JSON da lista de Processos.

<br/>

**Situações**

Lista todas situções possíveis para os Processos

```
GET http://localhost:8080/api/processos/situacoes?api-version=1.0
```

```bash
curl --location --request GET 'http://localhost:8080/api/processos/situacoes?api-version=1.0'
```

| Nome         | Finalizado |
| ------------ | ---------- |
| Em andamento | Não        |
| Desmembrado  | Não        |
| Em recurso   | Não        |
| Finalizado   | Sim        |
| Arquivado    | Sim        |

<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo uma representação de uma lista de Situações (Veja objetos).


## Responsáveis API
---

<br/>

**Criar Responsável**

Cadastra uma Pessoa que poderá vir a ser responsável por um ou mais Processos.

```
POST http://localhost:8080/api/responsaveis?api-version=1.0
```

```bash
curl --location --request POST 'http://localhost:8080/api/responsaveis?api-version=1.0' \
--header 'Content-Type: application/json' \
--data-raw '{ "nome": "Pedro Pereira da Silva", "cpf":"51118555058", "Email": "pedro_ps1@gmail.com", "Foto": null }'
```

| PARÂMETRO de CORPO EM JSON | TIPO   | REQUERIDO | DESCRIÇÂO |
| -------------------------- | :----- | :-------: | --------- |
| `nome`                     | string |    Sim    |
| `cpf`                      | string |    Sim    |
| `email`                    | string |    Sim    |
| `foto`                     | base64 |    Não    |

<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo uma representação do Responsável criado.


```json
{
    "id": "fcd4dd9f-df00-4995-a613-be7cf4420d81",
    "nome": "Lívia Hoffmann",
    "cpf": "25556926069",
    "email": "livia_hoff@trf.gov.br",
    "foto": null
}
```

Se o usuário informar algum dado inválido para a requisição será retorna o status 400 Bad request com uma representação de uma ou mais mensagens de erro.

```json
[
    {
        "mensagemId": 5,
        "mensagem": "Já existe um Responsável registrado com esse CPF."
    }
]
```

**Edita Responsável**

Edita as informações de um Responsável.

```
PUT http://localhost:8080/api/processos?api-version=1.0
```

```bash
curl --location --request PUT 'http://localhost:8080/api/responsaveis?api-version=1.0' \
--header 'Content-Type: application/json' \
--data-raw '{
    "id": "5278a5b8-c205-40f5-acfc-c4930e8fba76",
    "nome": "Andrieli Pereira",
    "cpf": "45553200008",
    "email": "andrieli_again123@gmail.com"
}'
```

| PARÂMETRO de CORPO EM JSON | TIPO   | REQUERIDO |
| -------------------------- | :----- | :-------: |
| `nome`                     | string |    Sim    |
| `cpf`                      | string |    Sim    |
| `email`                    | string |    Sim    |
| `foto`                     | base64 |    Não    |

<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo uma representação do Responsável editado.


```json
{
    "id": "fcd4dd9f-df00-4995-a613-be7cf4420d81",
    "nome": "Lívia Hoffmann",
    "cpf": "25556926069",
    "email": "livia_hoff@trf.gov.br",
    "foto": null
}
```

Se o usuário informar algum dado inválido para a requisição será retorna o status 400 Bad request com uma representação de uma ou mais mensagens de erro.

```json
[
    {
        "mensagemId": 5,
        "mensagem": "Já existe um Responsável registrado com esse CPF."
    }
]
```

**Remoção Responsável**

Remove um Responśavel contanto que não faça parte de um Processo.

```
DELETE http://localhost:8080/api/responsaveis/8b137bbc-cc8e-44c0-99c2-bf95734924b5?api-version=1.0
```

```bash
curl --location --request DELETE 'http://localhost:8080/api/responsaveis/8b137bbc-cc8e-44c0-99c2-bf95734924b5?api-version=1.0'
```


| PARÂMETRO de CAMINHO | TIPO   | REQUERIDO | DESCRIÇÃO                         |
| -------------------- | :----- | :-------: | --------------------------------- |
| `{id}`               | string |    Sim    | Deve conter o identificador único |


<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo uma representação do Responsável removido, caso contrário retorn Http 400 Bad request.


**Consulta Responsável**

Consulta um ou mais Responsáveis

```
GET http://localhost:8080/api/responsaveis?api-version=1.0
```

```bash
curl --location --request GET 'http://localhost:8080/api/responsaveis?cpf=45553200008&processo=3513038-00.2016.8.23.0027&api-version=1.0&nome=ant'
```


| PARÂMETRO de QUERY | TIPO   | REQUERIDO | DESCRIÇÃO                                                  |
| ------------------ | :----- | :-------: | ---------------------------------------------------------- |
| `cpf`              | string |    Não    | Deve conter os 11 números que compõem o CPF                |
| `processo`         | string |    Não    | Deve conter os 20 números que compõem o Processo Unificado |
| `nome`             | string |    Não    | Deve conter ao menos uma parte do nome                     |

<br/>

Response

Uma requisição realizada com sucesso deve retornar um status Http 200 OK e trazer no corpo uma representação JSON da lista de Responsáveis.

<br/>

## Objetos
---

<br/>

### Processo

| PROPRIEDADE          | TIPO          | DESCRIÇÃO                                                    |
| -------------------- | :------------ | ------------------------------------------------------------ |
| `id`                 | string        | O identificador único                                        |
| `processoUnificado`  | string        | O número do Processo Unificado                               |
| `dataDistribuicao`   | string        | A data de distribuição do Processo em formato ISO-8601       |
| `segredoJustica`     | boolean       | Um valor indicando se o Processo está sob segredo de Justiça |
| `pastaFisicaCliente` | string        | A pasta física do Processo                                   |
| `descricao`          | string        | Uma descrição                                                |
| `responsaveis`       | array[string] | Uma lista de Nomes dos Responnsáveis                         |
| `situacaoId`         | int           | Um identificador da situação atual                           |
    

<br/>

### Responsável

| PROPRIEDADE | TIPO   | DESCRIÇÃO                                            |
| ----------- | :----- | ---------------------------------------------------- |
| `id`        | string | O identificador único                                |
| `cpf`       | string | O número de CPF sem formatação                       |
| `email`     | string | O E-mail                                             |
| `nome`      | string | O Nome completo                                      |
| `foto`      | string | Uma representação em Base64 da foto de identificação |

<br/>

### Situação

| PROPRIEDADE  | TIPO   | DESCRIÇÃO                                               |
| ------------ | :----- | ------------------------------------------------------- |
| `id`         | string | O identificador único                                   |
| `nome`       | string | O nome da Situação                                      |
| `finalizado` | string | Um valor indicando se o processo está finalizado ou não |