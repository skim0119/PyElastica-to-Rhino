__all__ = ["CallbackCollection"]

import os
import sys
import numpy as np


class RhinoExportCollector:
    """

    Class Attributes
    ----------------
    FILE_SIZE_CUTOFF
        Maximum buffer size for each file. If the buffer
        size exceed, new file is created. Actual size of
        the file is expected to be marginally larger.

    """

    FILE_SIZE_CUTOFF = 512 * 1e6  # mB

    def __init__(self, save_path, fps):
        self.save_path = save_path
        self.fps = fps

        os.path.makedirs(save_path, exist_ok=true)
