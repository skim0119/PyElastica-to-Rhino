__doc__ = """ Modules containing callback classes for Rhino visualization """
__all__ = ['ExportGeometry']

import warnings
import os
import sys
import numpy as np
from numpy import savez

from elastica_rhino.collector import RhinoExportCollector


class ExportGeometry(CallBackBaseClass):

    def __init__(
        self,
        collector: RhinoExportCollector,
        step_skip: int,
        save_every: int = 1e8,
    ):
        """
        Parameters
        ----------
        step_skip : int
            Collect data at each step_skip steps.
        save_every : int
            Save the file every save_every steps. (default=1e8)
        """

        # Argument Parameters
        self.collector = collector
        self.step_skip = step_skip
        self.save_every = save_every

        # Data collector
        from collections import defaultdict

        self.buffer = defaultdict(list)
        self.buffer_size = 0

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
            radius = system.radius.copy()

            self.buffer["time"].append(time)
            self.buffer["step"].append(current_step)
            self.buffer["position"].append(position)
            self.buffer["directors"].append(director)
            self.buffer["velocity"].append(velocity)
            self.buffer["radius"].append(radius)

            self.buffer_size += (
                sys.getsizeof(position)
                + sys.getsizeof(velocity)
                + sys.getsizeof(director)
            )
            if (
                self.buffer_size > ExportCallBack.FILE_SIZE_CUTOFF
                or (current_step + 1) % self.save_every == 0
            ):
                self._flush()

    def _flush(self, **kwargs):
        file_path = f"{self.save_path}_{self.file_count}.dat"
        data = {k: np.array(v) for k, v in self.buffer.items()}
        savez(file_path, **data)

        self.file_count += 1
        self.buffer_size = 0
        self.buffer.clear()