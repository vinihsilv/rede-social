import time
import random

class Clock:
    def __init__(self):
        self.logical = 0
        self.offset = random.uniform(-1.0, 1.0)

    def now_physical(self):
        return time.time() + self.offset

    def tick(self, received=None):
        if received is not None:
            self.logical = max(self.logical, received) + 1
        else:
            self.logical += 1
        return self.logical

    def get_logical(self):
        return self.logical