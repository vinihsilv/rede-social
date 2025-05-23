import requests
import time
import random
import platform

def get_api_url():
    sistema = platform.system()
    if sistema == "Windows":
        return "http://localhost:5010"
    else:
        return input("Digite a URL base da API (ex: http://172.17.0.1:5010): ").strip()

API_URL = get_api_url()

LOGGED_USER = None  # será preenchido após o login/criação

def create_logged_user():
    global LOGGED_USER
    print("=== Criar usuário logado ===")
    user_id = int(input("ID do usuário: "))
    username = input("Nome de usuário: ")

    user = {
        "userId": user_id,
        "username": username,
        "followers": [],
        "following": []
    }

    r = requests.post(f"{API_URL}/api/User", json=user)
    if r.status_code == 200:
        LOGGED_USER = user
        print(f"[LOGIN] Usuário '{username}' criado e logado com sucesso.")
    else:
        print(f"[ERRO] Falha ao criar usuário: {r.status_code} - {r.text}")

def follow_users():
    qtd = int(input("Quantos usuários deseja seguir? "))
    for _ in range(qtd):
        id_following = int(input("ID do usuário a ser seguido: "))
        r = requests.post(f"{API_URL}/NewFollow", params={
            "idUser": LOGGED_USER["userId"],
            "idFollowing": id_following
        })
        print(f"[FOLLOW] {LOGGED_USER['userId']} → {id_following} → {r.status_code}")

def send_post():
    message = input("Texto do post: ")
    r = requests.post(f"{API_URL}/api/Post", params={
        "userId": LOGGED_USER["userId"],
        "postText": message
    })
    print(f"[POST] '{message}' → {r.status_code}")
    time.sleep(random.uniform(0.5, 1.0))
    view_feed()  # mostra o feed automaticamente depois de postar


def view_feed():
    r = requests.get(f"{API_URL}/getUserFeed", params={"userId": LOGGED_USER["userId"]})
    if r.ok:
        feed = r.json().get("value", [])
        if not feed:
            print("\n[INFO] Feed vazio. Siga usuários ou poste algo.")
            return
        print("\n=== Feed ===")
        for post in feed:
            user = post["user"]["username"]
            text = post["postText"]
            timestamp = post["postTime"]  # <-- CORRIGIDO AQUI
            print(f"[{timestamp}] @{user}: {text}")
    else:
        print(f"[ERRO] Falha ao carregar feed → {r.status_code}")


def send_private_message():
    receiver_id = int(input("ID do destinatário: "))
    message = input("Mensagem: ")
    params = {
        "senderId": LOGGED_USER["userId"],
        "receiverId": receiver_id,
        "messageText": message
    }
    r = requests.post(f"{API_URL}/api/Post/sendMessage", params=params)
    print(f"[DM] → {receiver_id}: '{message}' → {r.status_code}")
    time.sleep(random.uniform(0.5, 1.0))

def view_private_conversation():
    user2_id = int(input("ID do outro usuário: "))
    r = requests.get(f"{API_URL}/api/Post/getMessagesBetween", params={
        "user1Id": LOGGED_USER["userId"],
        "user2Id": user2_id
    })
    if r.ok:
        messages = r.json().get("value", [])
        print("\n=== Conversa ===")
        for msg in messages:
            sender = "Você" if msg["senderId"] == LOGGED_USER["userId"] else f"User {msg['senderId']}"
            print(f"[{msg['timestamp']}] {sender}: {msg['messageText']}")
    else:
        print(f"[ERRO] Falha ao carregar conversa → {r.status_code}")

if __name__ == "__main__":
    print("Bem-vindo à Simulação da Rede Social via Terminal!\n")
    create_logged_user()

    while True:
        print("\nAções disponíveis:")
        print("1 - Seguir usuários")
        print("2 - Criar post")
        print("3 - Ver feed")
        print("4 - Enviar mensagem privada")
        print("5 - Ver conversa privada")
        print("0 - Sair")

        action = input("\nEscolha uma ação (0-5): ").strip()
        if action == "1":
            follow_users()
        elif action == "2":
            send_post()
        elif action == "3":
            view_feed()
        elif action == "4":
            send_private_message()
        elif action == "5":
            view_private_conversation()
        elif action == "0":
            print("Encerrando.")
            break
        else:
            print("Opção inválida.")
