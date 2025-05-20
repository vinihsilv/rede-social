def log(server_id, content):
    with open(f"server_{server_id}.log", "a") as f:
        f.write(content + "\n")