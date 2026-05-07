import os
import ast
from openAI_interaction import create_cyml_code

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