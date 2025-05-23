import requests
import time
import random


API_URL = "http://172.17.0.1:5010" # Porta da sua API .NET

# 1. Criação de 5 usuários
def create_users():
    for i in range(1, 6):
        user = {
            "userId": i,
            "username": f"user{i}",
            "followers": [],
            "following": []
        }
        r = requests.post(f"{API_URL}/api/User", json=user)
        print(f"Create User {i} → {r.status_code}")

# 2. Todos seguem o próximo (1→2, 2→3, ...)
def follow_users():
    for i in range(1, 5):
        r = requests.post(f"{API_URL}/NewFollow", params={
            "idUser": i,
            "idFollowing": i+1
        })
        print(f"User {i} followed User {i+1} → {r.status_code}")

# 3. Cada usuário posta uma mensagem
def post_messages():
    for i in range(1, 6):
        post = f"Olá, eu sou o user{i}!"
        r = requests.post(f"{API_URL}/api/Post", params={
            "userId": i,
            "postText": post
        })
        print(f"User {i} posted: '{post}' → {r.status_code}")
        time.sleep(random.uniform(0.5, 1.0))  # pausa entre posts
def private_messages():
    for i in range(1, 6):
        message = f"olá user {i+1}, sou o user {i}"
        params = {
            "senderId": i,
            "receiverId": i+1,
            "messageText": message
        }
        response = requests.post(f"{API_URL}/api/Post/sendMessage", params=params)
        print(f"User {i} DMs: '{message}' → {response.status_code}")
        time.sleep(random.uniform(0.5, 1.0))

if __name__ == "__main__":
    create_users()
    follow_users()
    post_messages()
    private_messages()
