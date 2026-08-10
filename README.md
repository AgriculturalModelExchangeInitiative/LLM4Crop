# LLM4Crop - Documentation

## Overview
We developed **LLM4Crop**, a workflow as a Python script that automates the transformation of crop model source code from any languages/modelling platforms to an other language or platform. LLM4Crop uses the **Crop2ML** standardized format as an intermediate step. It leverages LLMs to refactor, analyze, and document source code through an agent-based workflow. 
LLM4Crop uses **CyMLTh**, a process that verifies if a Crop2ML package is in the correct format and if all CyML code are correctly constructed following Crop2ML rules. If not, CyMLTh leverages LLMs to autonomously correct them.

## Purpose
The script facilitates the transformation of crop model implementations, written in various programming languages and software architectures into Crop2ML, therefore **facilitates crop model component exchange**. This process includes:

- **Code Analysis & Documentation**: Extract metadata and create comprehensive descriptions.
- **Code Refactoring**: Convert source code to standardized Python modules, in a functional structure.
- **Algorithmic Metadata Generation**: Extract algorithm inputs, outputs and parameters.
- **XML Generation**: Create Crop2ML-compliant XML model descriptions.
- **Code Transpilation**: Convert Python modules to CyML (Crop2ML Language).
- **Project Generation**: Build complete Crop2ML project structures using cookiecutter.
- **Hybrid veryfier**: Correct automatically CyML code if not following Crop2ML rules.
- **Transpiler**: Transpile Crop2ML component into a broad range of languages/platforms

## Usage

### Command Line Interface

**From platform to Crop2ML**
```bash
python main.py -u <unit_file> <helper_file> ... [-u <unit_file2> ...] [-c <composite_file>] -o <output_folder>
```

- **`-u, --unit`** (required, multiple): Model unit source file(s) to process
- **`-c, --composite`** (optional): Composite model file (defines how units connect)
- **`-o, --output`** (required): Output folder where results will be saved



*Examples*

#### Single Model Unit Processing
```bash
python main.py -u soil_temperature.java -o ./output
```
Processes a single soil temperature model file and generates a Crop2ML component.

#### Multiple Model Units (Composite Model)
```bash
python main.py -u growth.py -u stress.py -u weather.py -c composite.json -o ./output
```
Processes three model units and combines them into a composite model using the composite.json configuration.

#### Multiple Model Units separated in different files
```bash
python main.py -u surface_temperature.cs surface_temperature_info.txt -u soil_layers_temeprature.cs soil_layers_temperature_structure.json -o ./output
```
Generates a soil temperature model combining surface and soil layers temperature modules.




**From Crop2ML to platform**
```bash
python main.py -p <Crop2ML package>
```
- **`-p, --package`** (required): The Crop2ML package to transform in all languages/platforms supported

*Examples*

#### Single Model Unit Processing
```bash
python main.py -p SoilTemperature
```
Transform the Crop2MLpackage "SoilTemperature" into component compatible with a broad range of languages and platforms.


## Configuration Files Required
- **API_KEY_PATH**: The path of a OpenAi or Claude API's key.
