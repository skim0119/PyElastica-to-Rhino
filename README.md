<div align="center">
<h1> PyElastica to Rhino </h1>
<img src="https://img.shields.io/badge/Python-3776AB?style=flat&logo=Python&logoColor=white"/>
<img src="https://img.shields.io/badge/Rhino-801010?style=flat&logo=rhinoceros&logoColor=white"/>
</div>

PyElastica extension and Rhino plugin for Cosserat Rod (+ Rigid Body) visualization.

## Grasshopper Plugin


![diagram](https://github.com/skim0119/PyElastica-to-Rhino/blob/assets/assets/diagram.png)

- PyElastica exports `npz` file which can directly be imported using `NpzImport` module.
    - For now, `NpzImport(Legacy)` is used. More generalized version is still under development.
- Provides `CosseratRod` and `CosseratRodPeriodic` that constructs rods directly.
    - Shape of position: `(num_rod, timesteps, 3, num_nodes)`
    - Shape of radius: `(num_rod, timesteps, 3, num_elements)`
    - If `periodic` is set to true, the trapazoidal averaging step is skipped for the radius.
- All modules include `C (switch)` input. The purpose is to serialize the execution.
- All modules include `D (debug)` output which returns the debugging text to inform progression or runtime error.

## PyElastica Extension

### CollectiveExport 

```py
data_collector = RhinoExportCollector(directory, fps)
```

### Export Callbacks

```py
simulation.collect_diagnostics(rod).using(
    ExportGeometry,
    data_collector
)
```

- ExportGeometry
- ExportStrain (work in progress)
- ExportInternalStress (work in progress)
- ExportExternalStress (work in progress)
