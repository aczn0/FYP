import socket
import time
import numpy as np
import random
import platform

# host, port = "127.0.0.1", 25001
# sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# sock.connect((host, port))

# send = "Hello, World!"
# sock.sendall(send.encode("UTF-8"))
# receive = str(sock.recv(1024), "UTF-8")
# print("Sent:     {}".format(send))
# print("Received: {}".format(receive))

class Test:
    def test(self):
        return "Hello, World!"

print(platform.architecture())