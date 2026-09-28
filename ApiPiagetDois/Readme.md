



# Como rodar (Passo a Passo DevOps) docker-compose.yml
Abra o terminal na pasta onde está o docker-compose.yml e execute:

# Sobe o banco de dados e a API juntos
docker-compose up -d --build

# Para ver os logs e garantir que o banco inicializou
docker-compose logs -f api























# Passo a Passo para Executar no Docker
Abra o Terminal (PowerShell, CMD ou Terminal do VS Code) e siga os passos:
Passo 1: Navegue até a pasta raiz do projeto
Certifique-se de estar na pasta que contém a pasta ApiPiagetDois e o arquivo Dockerfile (geralmente a pasta da Solution).

cd C:\caminho\para\o\seu\projeto\ApiPiagetDois

Passo 2: Construa (Build) a imagem Docker
Este comando cria a imagem chamada apipiaget-api. O ponto . no final é importante, pois indica o diretório atual.

docker build -t apipiaget-api .

(Na primeira vez, isso pode levar alguns minutos para baixar as imagens base do .NET 8).
Passo 3: Execute o Container
Este comando roda a imagem em segundo plano (-d), mapeia a porta 8080 do seu computador para a porta 8080 do container (-p), e dá um nome ao container.

docker run -d -p 8080:8080 --name apipiaget_container apipiaget-api

Passo 4: Verifique se está rodando

docker ps


Você deve ver o apipiaget_container na lista, com a porta 0.0.0.0:8080->8080/tcp.
Passo 5: Teste a API
Abra seu navegador e acesse:
👉 http://localhost:8080/swagger
Se o Swagger abrir, sua API está rodando perfeitamente no Docker e conectada ao seu banco de dados local!
4. Ajuste Necessário no Frontend (script.js)
Como o Docker local geralmente roda em HTTP (porta 8080) para evitar a complexidade de configurar certificados SSL/HTTPS no container, você precisa atualizar a URL base no seu script.js para evitar erros de CORS ou de certificado inválido:

// Altere esta linha no topo do script.js:
const API_BASE = 'http://localhost:8080/api';


(Remova o "s" do "https" e mude a porta para 8080).

## Comandos Úteis para o Dia a Dia
Parar o container:
docker stop apipiaget_container
Iniciar o container novamente:
docker start apipiaget_container
Ver os logs da API (útil para debugar erros de banco de dados):
docker logs -f apipiaget_container
Reconstruir a imagem após alterar o código C#:
docker stop apipiaget_container
docker rm apipiaget_container
docker build -t apipiaget-api .
docker run -d -p 8080:8080 --name apipiaget_container apipiaget-api
Se aparecer algum erro de conexão com o banco no docker logs, é quase 100% de certeza que é a Connection String (verifique o host.docker.internal e as credenciais).