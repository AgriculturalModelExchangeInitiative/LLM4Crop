from path import Path
from cookiecutter.main import cookiecutter
import shutil
from openAI_interaction import create_composite_metadata, create_unit_metadata, create_python_code, create_algo_metadata, create_consensus_python, create_clean_code
from json2XML import json_to_XML_composite, json_to_XML_unit
from transpiler import transpile_functions
import concurrent.futures
from pycropml.cyml import transpile_package
import xml.etree.ElementTree as ET
from textwrap import dedent

#-----------------------------------------------------------------
# Function to transform a modelUnit in Crop2ML
#-----------------------------------------------------------------
def process_unit(api_key, unit_meta, py_refactor, algo_meta, cyml_transpile, py_consensus, 
                 small_model, big_model, number_candidates, log_file, group, model_composite, output_folder):
  main_file = group[0]
  helper_files = group[1:]
  model_unit_name = Path(main_file).stem
  codes = []

  print(f"Processing descriptive metadata of the model {model_unit_name}...")
  metadata = create_unit_metadata(api_key, unit_meta, small_model, output_folder, main_file, helper_files)

  print(f"Creating {number_candidates} candidates refactored python versions of the model {model_unit_name}...")
  with concurrent.futures.ThreadPoolExecutor() as executor:
    futures = []
    for _ in range(number_candidates):
      futures.append(
        executor.submit(create_python_code, api_key, py_refactor, big_model, main_file, helper_files)
        )
    for fut in concurrent.futures.as_completed(futures):
      code = fut.result()
      codes.append(code)

  print(f"Selecting the best candidate for the model {model_unit_name}...")
  code = create_consensus_python(api_key, py_consensus, big_model, codes, main_file, helper_files, output_folder)
  algo = create_algo_metadata(api_key, algo_meta, small_model, code)

  print(f"Transpiling each function into CyML of the model {model_unit_name}...")
  functions = transpile_functions(code, algo, metadata, api_key, big_model, cyml_transpile, output_folder)
  
  if model_composite is None:
    xml = json_to_XML_unit(main_file, output_folder, metadata, algo, log_file)
  else:
    xml = json_to_XML_unit(model_composite, output_folder, metadata, algo, log_file)

  print(f"{model_unit_name} generated successfully !")

  return xml, functions


#-----------------------------------------------------------------
# Function to create a modelComposite in Crop2ML
#-----------------------------------------------------------------
def process_composite(api_key, composite_meta, small_model, output_folder, xml_units, model_composite, log_file, first_file):
  composite_metadata = create_composite_metadata(api_key, composite_meta, small_model, output_folder, xml_units, model_composite)
  if model_composite is None :
    model_composite = first_file
  xml_composite = json_to_XML_composite(model_composite, output_folder, composite_metadata, xml_units, log_file)

  return composite_metadata, xml_composite, model_composite


#-----------------------------------------------------------------
# Function to create a Crop2ML package
#-----------------------------------------------------------------
def create_crop2ml_package(cookiecutter_template, output_folder, model_composite, composite_metadata, XML_units, xml_composite, functions_transpiled, log_file):
  metadata = composite_metadata['metadata']
  project_dir = f"{output_folder}/{Path(model_composite).stem}"
  if Path(project_dir).exists():
    shutil.rmtree(project_dir)

  cookiecutter(cookiecutter_template, 
    no_input=True,
    extra_context={'project_name':Path(model_composite).stem, 
                  'repo_name':Path(model_composite).stem,
                  'author_name': metadata['Authors'],
                  'description': metadata['Extended description'], 
                  'open_source_license':"MIT"},
    output_dir=output_folder)
  
  # Move generated files to the cookiecutter project directory
  for xml_file in XML_units:
    shutil.copy(xml_file, f"{output_folder}/{Path(model_composite).stem}/crop2ml/")
  shutil.copy(xml_composite, f"{output_folder}/{Path(model_composite).stem}/crop2ml/")

  for function_transpiled in functions_transpiled:
    for function in function_transpiled:
      shutil.copy(function, f"{output_folder}/{Path(model_composite).stem}/crop2ml/algo/pyx/")

  shutil.copy(f"{output_folder}/{log_file}", f"{output_folder}/{Path(model_composite).stem}/")

  return project_dir


#-----------------------------------------------------------------
# Function to clean generated pyx
#-----------------------------------------------------------------
def clean_pyx(model_package, api_key, agent_cleaner, model, max_parallel):
  pyx_dir = Path(model_package) / 'src' / 'pyx'
  skip_file  = pyx_dir / (pyx_dir.name + 'Component.pyx')

  with concurrent.futures.ThreadPoolExecutor(max_workers=max_parallel) as executor:
    futures = []
    for pyx_path in pyx_dir.glob('*.pyx'):
      if pyx_path.name == skip_file.name:
        continue
      futures.append(
        executor.submit(create_clean_code, api_key, agent_cleaner, model, pyx_path)
        )
    for fut in concurrent.futures.as_completed(futures):
      clean_code, path = fut.result()
      path.write_text(clean_code, encoding='utf-8')


#-----------------------------------------------------------------
# Transpile a Crop2ML component in the output folder for a specific language or  platform
#-----------------------------------------------------------------
def generate_component(model_package, language, report_path):
  with open(report_path, 'a') as rf:
    rf.write(f"--- TRANSPILING COMPONENT {language} ---\n")
  try:
    transpile_package(model_package, language)
    with open(report_path, 'a') as rf:
      rf.write(f"Component generated successfully.\n\n")

  except Exception as e:
    with open(report_path, 'a') as rf:
      rf.write(f"Error occurred while generating component for {language}: \n{e}\n\n")
    pass


#-----------------------------------------------------------------
# Update a Crop2ML component in the output folder
#-----------------------------------------------------------------
def maj_component(package: str, pyx_folder: str, crop2ml_folder: str):
  pyx_dir    = Path(pyx_folder)
  crop2ml    = Path(crop2ml_folder)
  algo_pyx   = crop2ml / 'algo' / 'pyx'
  skip_file  = Path(package).stem + 'Component.pyx'

  for pyx_path in pyx_dir.glob('*.pyx'):
    if pyx_path.name == skip_file:
      continue

    content = pyx_path.read_text()
    xml_path = crop2ml / f'unit.{pyx_path.stem}.xml'
    inputs, outputs = read_xml_vars(xml_path)

    # Load XML tree once if it exists (needed for attribute patching)
    xml_tree = ET.parse(xml_path)
    xml_root = xml_tree.getroot()

    for func_name, body in extract_functions(content):
      if func_name.startswith('init_'):
        out_name = func_name
        cleaned  = filter_and_clean_body(body, inputs, outputs)
        (algo_pyx / f'{out_name}.pyx').write_text(cleaned)
        if xml_root is not None:
          for el in xml_root.findall('.//Initialization'):
            el.set('filename', f'algo/pyx/{out_name}.pyx')
            el.set('name', func_name)

      elif func_name.startswith('model_'):
        out_name = func_name[len('model_'):]
        cleaned  = filter_and_clean_body(body, inputs, outputs)
        (algo_pyx / f'{out_name}.pyx').write_text(cleaned)
        if xml_root is not None:
          for el in xml_root.findall('.//Algorithm'):
            el.set('filename', f'algo/pyx/{out_name}.pyx')

      else:
        start = content.find(f'\ndef {func_name}') or \
                content.find(f'\ncdef {func_name}') or \
                content.find(f'\ncpdef {func_name}')
        if start != -1:
          func_lines = []
          recording  = False
          for line in content.splitlines():
            if line.startswith(('def ', 'cdef ', 'cpdef ')) and func_name in line:
              recording = True
            elif recording and line.startswith(('def ', 'cdef ', 'cpdef ')):
              break
            if recording:
              func_lines.append(line)
          (algo_pyx / f'{func_name}.pyx').write_text('\n'.join(func_lines))

      if xml_tree is not None:
        xml_tree.write(xml_path)


#-----------------------------------------------------------------
# Extract all functions from a pyx file
#-----------------------------------------------------------------
def extract_functions(content: str) -> list[tuple[str, str]]:
  functions = []
  lines = content.splitlines()
  i = 0

  while i < len(lines):
    line = lines[i]
    if not (line.startswith('def') and '(' in line):
      i += 1
      continue

    func_name = line.split('(')[0].split()[-1]

    while i < len(lines) and not lines[i].rstrip().endswith(':'):
      i += 1
    i += 1

    # Collect indented body
    body = []
    while i < len(lines) and (not lines[i] or lines[i][0] in (' ', '\t')):
      body.append(lines[i])
      i += 1

    functions.append((func_name, '\n'.join(body)))

  return functions


#-----------------------------------------------------------------
# Dedent and clean variables already declared
#-----------------------------------------------------------------
def filter_and_clean_body(body: str, inputs: set, outputs: set) -> str:
  known_vars = inputs | outputs
  lines = []
  in_docstring = False

  for line in body.splitlines():
    stripped = line.strip()

    if stripped.startswith('"""'):
      in_docstring = not in_docstring
      continue
    if in_docstring:
      continue

    if stripped.startswith('cdef'):
      var = stripped.split()[2] if len(stripped.split()) >= 3 else ''
      var = var.split('[')[0]
      if var in known_vars:
        continue

    lines.append(line)

  # Drop last return statement
  for i in range(len(lines) - 1, -1, -1):
    if lines[i].strip().startswith('return'):
      lines.pop(i)
      break

  return dedent('\n'.join(lines)).strip()


#-----------------------------------------------------------------
# Get all variable from XML file
#-----------------------------------------------------------------
def read_xml_vars(xml_path: Path) -> tuple[set, set]:
  root = ET.parse(xml_path).getroot()
  inputs  = {el.get('name') for el in root.findall('.//Input')}
  outputs = {el.get('name') for el in root.findall('.//Output')}
  return inputs, outputs