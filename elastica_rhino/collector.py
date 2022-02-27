__all__ = ["CallbackCollection"]

import os
import sys
import numpy as np

from collections import defaultdict


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

    def __init__(self, save_path: str, step_skip: int, file_name: str = "data"):
        # Save arguments
        self.step_skip = step_skip

        # Create path
        self.save_path = save_path
        self.file_name = file_name
        os.makedirs(save_path, exist_ok=true)

        self.num_buffers = 0
        self.buffer_collection = {}  # key: registry, value: callback buffer
        self.buffer_groups = defaultdict(list)  # key: group, value: list of registry
        self.buffer_size = 0

        self.file_count = 0

    def register(self, group: str, buffer: dict[list], step_skip: int) -> int:
        assert (
            step_skip == self.step_skip
        ), "Step-skip for the callback must equal to the step-skip of the collector. Otherwise, different collector must be used."
        registry = self.num_buffers
        self.num_buffers += 1

        self.buffer_collection[registry] = buffer
        self.buffer_groups[group].append(registry)

        return registry

    def update(self, buffer_size: int) -> None:
        self.buffer_size += buffer_size
        if self.buffer_size > ExportCallBack.FILE_SIZE_CUTOFF:
            self.save()

    def save(self) -> None:
        # File path
        file_path = os.path.join(
            self.save_path, f"{file_name}_{self.file_count:03d}.npz"
        )

        # Data length
        length = min([len(v["time"]) for _, v in self.buffer_collection.items()])
        data_collection = defaultdict(list)

        # Prep data
        for group, registry_list in self.buffer_groups.items():
            rod_data_collection = defaultdict(list)
            for registry in registry_list:
                for field_name, data_list in self.buffer_collection[registry].items():
                    if field_name == "time":
                        continue
                    rod_data_collection[field_name].append(
                        data_list[:length]
                    )  # query data
                    data_list[:] = data_list[length:]  # purge data
            for field_name, history in rod_data_collection.items():
                name = f"{group}_{field_name}_history"
                data_collection[name] = np.array(history)

        np.savez(file_path, **data_collection)

        self.file_count += 1
        self.buffer_size = 0
