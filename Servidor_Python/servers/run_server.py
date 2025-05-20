import sys
from servers.server import run_server

if __name__ == "__main__":
    sid = int(sys.argv[1]) if len(sys.argv) > 1 else 1
    run_server(sid)