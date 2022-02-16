__doc__ = """ Modules containing callback classes for Rhino visualization """

import warnings
import os
import sys
import numpy as np

from elastica.callback_functions import CallBackBaseClass


class CallbackCollection:
    pass


class RodCallback(CallBackBaseClass):
    """
    ExportCallback is an example callback class to demonstrate
    how to export rod-data into data file.
    If one wants to customize the saving data, we recommend to
    override `make_callback` method.
        Class Attributes
        ----------------
        AVAILABLE_METHOD
            Supported method to save the file. We recommend
            binary save to maintain the tensor structure of
            data.
        FILE_SIZE_CUTOFF
            Maximum buffer size for each file. If the buffer
            size exceed, new file is created. Actual size of
            the file is expected to be marginally larger.
    """

    AVAILABLE_METHOD = ["pickle", "npz", "tempfile"]
    FILE_SIZE_CUTOFF = 32 * 1e6  # mB

    def __init__(
        self,
        step_skip: int,
        path: str,
        method: str,
        initial_file_count: int = 0,
        save_every: int = 1e8,
    ):
        """
        Parameters
        ----------
        step_skip : int
            Collect data at each step_skip steps.
        path : str
            Path to save the file. If directories are prepended,
            they must exist. The filename depends on the method.
            The path is not expected to include extension.
        method : str
            Method name. Only the name in AVAILABLE_METHOD is
            allowed.
        initial_file_count : int
            Initial file count index that will be appended
        save_every : int
            Save the file every save_every steps. (default=1e8)
        """
        # Assertions
        MIN_STEP_SKIP = 100
        if step_skip <= MIN_STEP_SKIP:
            warnings.warn(f"We recommend step_skip at least {MIN_STEP_SKIP}")
        assert (
            method in ExportCallBack.AVAILABLE_METHOD
        ), f"The exporting method ({method}) is not supported. Please use one of {ExportCallBack.AVAILABLE_METHOD}."
        assert os.path.exists(path), "The export path does not exist."

        # Argument Parameters
        self.step_skip = step_skip
        self.save_path = path
        self.method = method
        self.file_count = initial_file_count
        self.save_every = save_every

        # Data collector
        from collections import defaultdict

        self.buffer = defaultdict(list)
        self.buffer_size = 0

        # Module
        if method == ExportCallBack.AVAILABLE_METHOD[0]:
            import pickle

            self._pickle = pickle
        elif method == ExportCallBack.AVAILABLE_METHOD[1]:
            from numpy import savez

            self._savez = savez
        elif method == ExportCallBack.AVAILABLE_METHOD[2]:
            import tempfile
            import pickle

            self._tempfile = tempfile.NamedTemporaryFile(delete=False)
            self._pickle = pickle

    def make_callback(self, system, time, current_step: int):
        """
        Parameters
        ----------
        system :
            Each part of the system (i.e. rod, rigid body, etc)
        time :
            simulation time unit
        current_step : int
            simulation step
        """
        if current_step % self.step_skip == 0:
            position = system.position_collection.copy()
            velocity = system.velocity_collection.copy()
            director = system.director_collection.copy()

            self.buffer["time"].append(time)
            self.buffer["step"].append(current_step)
            self.buffer["position"].append(position)
            self.buffer["directors"].append(director)
            self.buffer["velocity"].append(velocity)

            self.buffer_size += (
                sys.getsizeof(position)
                + sys.getsizeof(velocity)
                + sys.getsizeof(director)
            )
            if (
                self.buffer_size > ExportCallBack.FILE_SIZE_CUTOFF
                or (current_step + 1) % self.save_every == 0
            ):
                self._dump()

    def _dump(self, **kwargs):
        file_path = f"{self.save_path}_{self.file_count}.dat"
        data = {k: np.array(v) for k, v in self.buffer.items()}
        if self.method == ExportCallBack.AVAILABLE_METHOD[0]:
            # pickle
            with open(file_path, "wb") as file:
                self._pickle.dump(data, file)
        elif self.method == ExportCallBack.AVAILABLE_METHOD[1]:
            # npz
            self._savez(file_path, **data)
        elif self.method == ExportCallBack.AVAILABLE_METHOD[2]:
            # tempfile
            file = open(self._tempfile.name, "wb")
            self._pickle.dump(data, file)

        self.file_count += 1
        self.buffer_size = 0
        self.buffer.clear()