<div align="center">
<h1> PyElastica to Rhino </h1>
<img src="https://img.shields.io/badge/Python-3776AB?style=flat&logo=Python&logoColor=white"/>
<img src="https://img.shields.io/badge/Rhino-801010?style=flat&logo=rhinoceros&logoColor=white"/>
</div>

PyElastica extension and Rhino plugin for Cosserat Rod (+ Rigid Body) visualization.

## Requirements

The Grasshopper plugin does not have any additional requirement, but we recommend using [`anemone`](https://www.food4rhino.com/en/app/anemone) together. This plugin provide loop function that can be used to generate animated video.

## Grasshopper Plugin

<!--
 ![diagram](https://github.com/skim0119/PyElastica-to-Rhino/blob/assets/assets/diagram.png)
 -->

- PyElastica exports `npz` file which can directly be imported using `NpzImport` module.
    - For now, `NpzImport(Legacy)` is used. More generalized version is still under development.
- Provides `CosseratRod` and `CosseratRodPeriodic` that constructs rods directly.
    - Shape of position: `(num_rod, timesteps, 3, num_nodes)`
    - Shape of radius: `(num_rod, timesteps, 3, num_elements)`
    - If `periodic` is set to true, the trapazoidal averaging step is skipped for the radius.
- All modules include `C (switch)` input. The purpose is to serialize the execution.
- All modules include `D (debug)` output which returns the debugging text to inform progression or runtime error.

### Legacy Import Mode

The grasshopper module import `npz` file. In the legacy import module, we expect the `npz` file to have the key `<group>_position_history` and `<group>_radius_history` for each group.

- `<group>_position_history` should have the shape `<number of rod, timesteps, 3 (dimension), number of nodes>`.
- `<group>_radius_history` should have the shape `<number of rod, timesteps, number of elements>`.
>The `number of nodes` is expected to be one more than the `number of elements`. If the periodic is set to `True`, the `number of nodes` can be equal to the `number of elements`.

> For sphere, the `number of elements` and the `number of nodes` are both `1`. In such case, a `sphere module` must be used in grasshopper.

## PyElastica Extension

We provide a data collector extension to PyElastica to easily collect the batch of data. 

```sh
pip install elastica-rhino
```



### Example Usage

```py
import elastica
import elastica_rhino as er

... <simulation setup>

data_collector = er.RhinoExportCollector(
    save_path="data",
    step_skip=int((1/fps)/dt)  # Collect data every 'step_skip' iteration 
)
for arm in arm_list:
    simulation.collect_diagnostics(arm).using(
        er.ExportGeometry,
        data_collector,
        group="arm"
)
for muscle in muscle_list:
    simulation.collect_diagnostics(muscle).using(
        er.ExportGeometry,
        data_collector,
        group="muscle"
)

... <simulation setup>

simulation.finalize()

... <simulation run>

data_collector.save()
```

In Rhino, objects in the same group will be collected by layers. We recommend setting the parameter `group` for the same material or color of the rods.

### Included Callbacks

- ExportGeometry
- ExportStrain (work in progress)
- ExportInternalStress (work in progress)
- ExportExternalStress (work in progress)
