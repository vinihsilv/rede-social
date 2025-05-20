from flask import Flask, request, jsonify
import requests
import time
import json
import sys
import os

sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), "..")))
from logic.clock import Clock
from logic.logger import log

app = Flask(__name__)

clock = Clock()
messages = []
server_id = 1
peers = []

@app.route('/post', methods=['POST'])
def receive_post():
    data = request.get_json()
    ts_received = data.get("timestamp")
    logical = clock.tick(ts_received)

    action = data.get("action", "post")  # padrão é 'post'

    if action == "post":
        messages.append({
            "user": data["user"],
            "content": data["content"],
            "timestamp": logical
        })
        log(server_id, f"POST {data['user']}: {data['content']} (logical={logical})")

    elif action == "follow":
        log(server_id, f"FOLLOW user {data['userId']} → {data['follows']} (logical={logical})")

    elif action == "create_user":
        log(server_id, f"CREATE_USER {data['userId']}: {data['username']} (logical={logical})")

    else:
        log(server_id, f"UNKNOWN ACTION: {action} → {data} (logical={logical})")

    replicate_to_peers(data)
    return jsonify({"status": "ok", "clock": logical})

@app.route('/replica', methods=['POST'])
def receive_replica():
    data = request.get_json()
    ts_received = data.get("timestamp")
    logical = clock.tick(ts_received)

    action = data.get("action", "post")

    if action == "post":
        messages.append({
            "user": data["user"],
            "content": data["content"],
            "timestamp": logical
        })
        log(server_id, f"REPLICA {data['user']}: {data['content']} (logical={logical})")

    elif action == "follow":
        log(server_id, f"REPLICA FOLLOW user {data['userId']} → {data['follows']} (logical={logical})")

    elif action == "create_user":
        log(server_id, f"REPLICA CREATE_USER {data['userId']}: {data['username']} (logical={logical})")

    else:
        log(server_id, f"REPLICA UNKNOWN ACTION: {action} → {data} (logical={logical})")

    return jsonify({"status": "replica-ok"})

@app.route('/feed', methods=['GET'])
def get_feed():
    return jsonify(messages)

def replicate_to_peers(post_data):
    for peer in peers:
        try:
            requests.post(f"{peer}/replica", json=post_data, timeout=1)
        except Exception as e:
            log(server_id, f"Erro ao replicar para {peer}: {e}")

def run_server(sid):
    global server_id, peers
    server_id = sid
    with open("config/config.json") as f:
        config = json.load(f)
    all_servers = config["servers"]
    peers = [url for k, url in all_servers.items() if int(k) != server_id]
    app.run(port=5000 + server_id)

