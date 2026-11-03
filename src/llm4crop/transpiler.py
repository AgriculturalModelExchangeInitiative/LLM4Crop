import os
import ast

from path import Path
from openAI_interaction import create_cyml_code
from pycropml.cyml import model_parser
from pycropml.cyml import render_cyml
from pycropml.cyml import Topology
import pycropml.transpiler.generators as generators
from pycropml.cyml import Main
from pycropml.cyml import nameconvention

NAMES = {
    'r': 'r',
    'cs': 'csharp',
    'cpp': 'cpp',
    "cpp2": "cpp2",
    'py': 'python',
    'f90': 'fortran',
    'java': 'java',
    'simplace': 'simplace',
    'sirius': 'sirius',
    "openalea": "openalea",
    "apsim": "apsim",
    "record": "record",
    "dssat": "dssat",
    "bioma": "bioma",
    "stics": "stics",
    "sirius2": "sirius2"
}

ext = {'r': 'r',
       'cs': 'cs',
       'cpp': 'cpp',
       "cpp2": "cpp",
       'py': 'py',
       'f90': 'f90',
       'java': 'java',
       'simplace': 'java',
       'sirius': 'cs',
       'bioma': 'cs',
       "openalea": "py",
       "apsim": "cs",
       "record": "cpp",
       "dssat": "f90",
       "stics": "f90",
       "sirius2": 'cs'
       }

#-----------------------------------------------------------------
# Function to dedent code by one level
# This function removes one level of indentation from the given code string.
#-----------------------------------------------------------------
def dedent_one_level(code):
  indent = "    "
  lines = code.splitlines()

  start_index = 0
  for i, line in enumerate(lines):
    if line.strip().startswith("def "):
      start_index = i + 1
      break
  
  body = lines[start_index:-1]
  out = []
  for line in body:
    if line.startswith(indent):
      out.append(line[len(indent):])
    else:
      out.append(line)
  result_lines = "\n".join(out).split("\n")
  return "\n".join(result_lines)


#-----------------------------------------------------------------
# Function to replace function names in the transpiled code based on the algo metadata and description metadata
# This function checks the function names in the transpiled code and replaces them with the appropriate names
#-----------------------------------------------------------------
def format(code, algo_meta, desc_meta):
  if algo_meta.get('init', {}) != '-' and algo_meta.get('init', {}) != []:
    init = algo_meta['init']
    if init.get('name', '') != '-' :
      code = code.replace(init['name'] + "(", "init_" + desc_meta.get('metadata', {}).get('Title') + "(")

  for input in algo_meta.get('inputs', []):
    if input.get('name', '') != '-' :
      for line in code.splitlines():
        if line.strip().startswith("cdef") and line.strip().endswith(input['name']):
          code = code.replace(line + '\n', '').replace(line, '')

  for output in algo_meta.get('outputs', []):
    if output.get('name', '') != '-' :
      for line in code.splitlines():
        if line.strip().startswith("cdef") and line.strip().endswith(output['name']):
          code = code.replace(line + '\n', '').replace(line, '')
  return code


#-----------------------------------------------------------------
# Function to extract functions from a Python code string and transpile each to a separate file
# This function parses the Python code string, detects each function definition, and transpiles them in a new file containing only that function.
#-----------------------------------------------------------------
def transpile_functions(python_code, algo_meta, desc_meta, api_key_path, model, agent_cymltranspile, output_folder):
  try:
    tree = ast.parse(python_code)
  except SyntaxError as e:
    print(f"Syntax error in code: {e}")
    return
  
  functions_transpiled = []
  functions = []
  functions.append(algo_meta.get('process', {}).get('name'))
  if algo_meta.get('init', {}) != '-' and algo_meta.get('init', {}) != []:
    functions.append(algo_meta.get('init', {}).get('name'))
  if algo_meta.get('functions', {}) != '-' and algo_meta.get('functions', {}) != []:
    for func in algo_meta.get('functions', {}):
      functions.append(func.get('name'))

  lines = python_code.splitlines()
  for node in ast.walk(tree):
    if isinstance(node, ast.FunctionDef):
      function_name = node.name
      start_line = node.lineno - 1
      end_line = node.end_lineno

      if function_name in functions:
        function_code = '\n'.join(lines[start_line:end_line])
        cyml = create_cyml_code(api_key_path, agent_cymltranspile, model, function_code, algo_meta)
        
        if algo_meta.get('init', {}) != '-' and algo_meta.get('init', {}) != [] and function_name == algo_meta.get('init', {}).get('name') :
          file_name = f"init_{desc_meta.get('metadata', {}).get('Title')}"
          cyml = dedent_one_level(cyml)
        elif function_name == algo_meta.get('process', {}).get('name'):
          file_name = desc_meta.get('metadata', {}).get('Title')
          cyml = dedent_one_level(cyml)
        else:
          file_name = function_name

        if cyml and cyml.strip() and any(line.strip() and not line.strip().startswith('#') for line in cyml.split('\n')):
          cyml = format(cyml, algo_meta, desc_meta)
          file_path = os.path.join(output_folder, f"{file_name}.pyx")
          functions_transpiled.append(file_path)
          with open(file_path, 'w', encoding='utf-8') as f:
            f.write(cyml)
        else:
          if function_name == algo_meta.get('init', {}).get('name'):
            algo_meta['init'] = '-'
          else:
            algo_meta['functions'] = [f for f in algo_meta['functions'] if f.get('name') != function_name]
            
  return functions_transpiled





def transformation(package, language):
    domain_class = ["cs", "java", "sirius", "cpp", "cpp2", "bioma", "sirius2", "apsim"]
    wrapper=["cs", "sirius", "bioma", "sirius2", "apsim"]
    platform = ["simplace","sirius","openalea","apsim","bioma","record","dssat", "stics", "sirius2"]

    namep = package.split(os.path.sep)[-1]
    pkg = Path(package)
    models = model_parser(pkg)  # parse xml files and create python model object

    output = Path(os.path.join(pkg, 'src'))
    dir_test = Path(os.path.join(pkg, 'test'))
    dir_doc = Path(os.path.join(pkg, 'doc'))

    m2p = render_cyml.Model2Package(models, dir=output)
    tg_rep1 = Path(os.path.join(output, language))  # target language models  directory in output
    dir_test_lang = Path(os.path.join(dir_test, language))

    namep_ = namep.replace("-", "_")
    tg_rep = Path(os.path.join(tg_rep1, namep_))

    # generate cyml functions
    cyml_rep = Path(os.path.join(output, 'pyx'))  # cyml model directory in output

    # create topology of composite model
    T = Topology(namep, package)
    mc_name = T.model.name

    # domain class
    if language in domain_class:
      getattr(getattr(generators, f'{NAMES[language]}Generator'), f'to_struct_{language}')([T.model], tg_rep, mc_name)
    # wrapper
    if language in wrapper:
      getattr(getattr(generators, f'{NAMES[language]}Generator'), f'to_wrapper_{language}')(T.model, tg_rep, mc_name)

    # Transform model unit to languages and platforms
    for k, file in enumerate(cyml_rep.files()):
      with open(file, 'r') as fi:
        source = fi.read()
      name = os.path.split(file)[1].split(".")[0]
      for model in models:  # in the case we haven't the same order
        if name.lower() == model.name.lower() and model.modelid.split(".")[0] != "function":
          test = Main(file, language, model, T.model.name)
          test.parse()
          test.to_ast(source)
          code = test.to_source()
          filename = Path(
            os.path.join(tg_rep, f"{nameconvention.signature(model, ext[language])}.{ext[language]}"))
          with open(filename, "wb") as tg_file:
            tg_file.write(code.encode('utf-8'))

    # Create Cyml Composite model
    filename = Path(os.path.join(tg_rep, f"{mc_name}Component.{ext[language]}"))
    code = T.compotranslate(language).encode('utf-8')
    if code:
      with open(filename, "wb") as tg_file:
        tg_file.write(code)