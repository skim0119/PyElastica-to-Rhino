__doc__ = """ Modules containing callback classes for Rhino visualization """
__all__ = ["ExportGeometry"]

import warnings
import os
import sys
import numpy as np
from numpy import savez

from collections import defaultdict

from elastica.callback_functions import CallBackBaseClass
from elastica_rhino.collector import RhinoExportCollector


class ExportGeometry(CallBackBaseClass):
    def __init__(
        self,
        collector: RhinoExportCollector,
        group: str,
    ):
        # Argument Parameters
        self.collector = collector
        self.step_skip = collector.step_skip

        # Data collector
        self.buffer = defaultdict(list)

        # Register
        self.registry = collector.register(group, self.buffer, step_skip)

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
            radius = system.radius.copy()
            buffer_size += sys.getsizeof(position) + sys.getsizeof(radius)

            self.buffer["time"].append(time)  # This must exist for collector
            self.buffer["position"].append(position)
            self.buffer["radius"].append(radius)

            self.collector.update(buffer_size)
