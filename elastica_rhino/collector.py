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

    def __init__(self, save_path):
        self.save_path = save_path

        os.path.makedirs(save_path, exist_ok=true)

        self.buffer_size = 0

    def register(self, group:str, buffer:dict[list]) -> int:
        pass

    def update_size(self, buffer_size: int) -> None:
        pass

    def save(self) -> bool:
        if self.buffer_size > ExportCallBack.FILE_SIZE_CUTOFF:
            self.flush()

    def flush(self, **kwargs):
        file_path = f"{self.save_path}_{self.file_count}.dat"
        data = {k: np.array(v) for k, v in self.buffer.items()}
        savez(file_path, **data)

        self.file_count += 1
        self.buffer_size = 0
        self.buffer.clear()
