📱 Projeto CC7261: Rede Social
👥 Integrantes
Vinícius Henrique Silva | 22.122.063-5

Pedro Henrique Almeida Leite | 22.221.003-1

Cauê Jacomini Zanatti | 22.122.024-7

Luan Petroucic Moreno | 22.122.076-7

🎯 Descrição
Projeto acadêmico desenvolvido para a disciplina CC7261 - Sistemas Distribuídos. A proposta consiste em uma rede social simulada, com suporte a criação de usuários, postagens, mensagens privadas e replicação entre servidores, utilizando conceitos como:

APIs em múltiplas linguagens (Java, C#)

Comunicação com servidores Python

Sincronização e replicação de dados

Interação via terminal Docker

🧰 Tecnologias Utilizadas
Java (Spring Boot) – Lógica de negócio e API REST

C# (.NET) – Lógica de negócio e replicação entre nós

Python – Servidores replicadores com relógios distribuídos

Docker – Interface simulada de terminal para o usuário

HTTP + JSON – Comunicação entre APIs e servidores

📂 Estrutura do Projeto
bash
Copy
Edit
rede-social/
├── API/                 # Projeto em C#
├── API_Java/            # Projeto em Java
├── Servidor_Python/     # Servidores replicadores em Python
│   └── servers/run_server.py
├── Dockerfile           # Criação da imagem terminal
├── README.md
🚀 Como Executar
🔧 Pré-requisitos
Git

Docker

Python 3.10+

Java 17+

.NET SDK 8+

1. Clone o repositório
bash
Copy
Edit
git clone git@github.com:vinihsilv/rede-social.git
cd rede-social
2. Inicie as APIs
🟡 C# (.NET)
bash
Copy
Edit
cd API/
dotnet run --urls=http://localhost:5010
🟠 Java (Spring)
bash
Copy
Edit
cd API_Java/
./mvnw spring-boot:run
3. Inicie os servidores Python (em terminais separados):
bash
Copy
Edit
cd Servidor_Python/
python3 -m servers.run_server 1
python3 -m servers.run_server 2
python3 -m servers.run_server 3
4. Inicie a simulação no terminal via Docker:
bash
Copy
Edit
docker build -t servidor_python .
docker run -it --rm servidor_python
💡 Funcionalidades
Criação de usuários

Seguir outros usuários

Postagens públicas

Feed individual

Mensagens privadas

Replicação entre múltiplos nós via Python

