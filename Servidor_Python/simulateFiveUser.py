import requests
import time
import random

API_URL = input("Digite a URL base da API (ex: http://172.17.0.1:5010): ").strip()

def create_users():
    qtd = int(input("Quantos usuários deseja criar? "))
    for i in range(1, qtd + 1):
        user_id = int(input(f"User {i} - ID: "))
        username = input(f"User {i} - Username: ")
        user = {
            "userId": user_id,
            "username": username,
            "followers": [],
            "following": []
        }
        r = requests.post(f"{API_URL}/api/User", json=user)
        print(f"[CREATE] User {user_id} → {r.status_code}")

def follow_users():
    qtd = int(input("Quantas conexões de follow deseja criar? "))
    for _ in range(qtd):
        id_user = int(input("ID do usuário que vai seguir: "))
        id_following = int(input("ID do usuário a ser seguido: "))
        r = requests.post(f"{API_URL}/NewFollow", params={
            "idUser": id_user,
            "idFollowing": id_following
        })
        print(f"[FOLLOW] User {id_user} → User {id_following} → {r.status_code}")

def send_post():
    user_id = int(input("ID do usuário que está postando: "))
    message = input("Texto do post: ")
    r = requests.post(f"{API_URL}/api/Post", params={
        "userId": user_id,
        "postText": message
    })
    print(f"[POST] User {user_id}: '{message}' → {r.status_code}")
    time.sleep(random.uniform(0.5, 1.0))  # atraso opcional

def post_messages():
    qtd = int(input("Quantos posts deseja criar? "))
    for _ in range(qtd):
        send_post()

def private_messages():
    qtd = int(input("Quantas mensagens privadas deseja enviar? "))
    for _ in range(qtd):
        sender_id = int(input("ID do remetente: "))
        receiver_id = int(input("ID do destinatário: "))
        message = input("Mensagem: ")
        params = {
            "senderId": sender_id,
            "receiverId": receiver_id,
            "messageText": message
        }
        response = requests.post(f"{API_URL}/api/Post/sendMessage", params=params)
        print(f"[DM] User {sender_id} → {receiver_id}: '{message}' → {response.status_code}")
        time.sleep(random.uniform(0.5, 1.0))

def get_all_posts():
    response = requests.get(f"{API_URL}/api/Post")
    print("\n=== [GET] All Posts ===")
    if response.ok:
        data = response.json().get("value", [])
        for post in data:
            print(post)
    else:
        print(f"[ERROR] GET posts → {response.status_code}")


if __name__ == "__main__":
    print("Ações disponíveis:")
    print("1 - Criar usuários")
    print("2 - Seguir usuários")
    print("3 - Criar posts")
    print("4 - Enviar mensagens privadas")
    print("5 - Ver todos os posts")
    print("0 - Sair")

    while True:
        action = input("\nEscolha uma ação (0-5): ").strip()
        if action == "1":
            create_users()
        elif action == "2":
            follow_users()
        elif action == "3":
            post_messages()
        elif action == "4":
            private_messages()
        elif action == "5":
            get_all_posts()
        elif action == "0":
            print("Encerrando.")
            break
        else:
            print("Opção inválida.")
