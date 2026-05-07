import os
import xml
from path import Path
from pycropml.topology import Topology
from pycropml.pparse import model_parser
from pycropml.cyml import prefix
from pycropml import render_cyml
from pycropml.transpiler.main import Main
from pycropml.transpiler import generators as generators
from openAI_interaction import create_debug_code_composite, create_debug_code_unit, create_debug_xml_composite, create_debug_xml_unit
from json2XML import format_xml

#-----------------------------------------------------------------
# Function to check if the pyx code of each model unit can be generated
#-----------------------------------------------------------------
def generate_pyx_unit(model_package, report_path, iteration):
  with open(report_path, 'a') as rf:
    rf.write(f"GENERATING PYX CODE --- ATTEMPT {iteration} ---\n")
  code_generated = False
  pkg = Path(model_package)
  output = Path(os.path.join(pkg, 'src'))
  models = model_parser(pkg)
  m2p = render_cyml.Model2Package(models, dir=output)

  try:
    for model in models:          
      m2p.generate_component(model)
    m2p.generate_package()  # generate cyml models in "pyx" directory
    with open(report_path, 'a') as rf:
      rf.write(f"Successfully generated pyx code of each model units.\n\n")
      code_generated = True
  except Exception as e:
    with open(report_path, 'a') as rf:
      rf.write(f"ERROR ModelUnit-Generation when generating pyx code --- {model.name} ---:\n{e}\n\n")
    pass
  return code_generated


#-----------------------------------------------------------------
# Function to check if the pyx code of model composite can be generated
#-----------------------------------------------------------------
def generate_pyx_composite(model_package, report_path, iteration):
  with open(report_path, 'a') as rf:
    rf.write(f"GENERATING COMPOSITE CODE --- ATTEMPT {iteration} ---\n")
  code_generated = False
  topology = Topology(model_package.split(os.path.sep)[-1], model_package)
  pkg = Path(model_package)
  output = Path(os.path.join(pkg, 'src'))
  cyml_rep = Path(os.path.join(output, 'pyx'))

  try:
    mc_name = topology.model.name
    T_pyx = topology.algo2cyml()
    fileT = Path(os.path.join(cyml_rep, f"{mc_name}Component.pyx"))
    with open(fileT, "wb") as tg_file:
      tg_file.write(T_pyx.encode('utf-8'))
    with open(report_path, 'a') as rf:
      rf.write(f"Successfully generated composite pyx code.\n\n")
    code_generated = True
  except Exception as e:
    with open(report_path, 'a') as rf:
      rf.write(f"ERROR ModelComposite-Generation when generating the pyx code of the model composite :\n{e}\n\n")
    pass

  return code_generated


#-----------------------------------------------------------------
# Function to check the syntax of the generated code files and the CROP2ML -> language/platform transformation
#-----------------------------------------------------------------
def check_code_unit(model_package, report_path, iteration):
  with open(report_path, 'a') as rf:
    rf.write(f"CHECKING CODE GENERATED --- ATTEMPT {iteration} ---\n")
  verif_result = False
  topology = Topology(model_package.split(os.path.sep)[-1], model_package)
  namep = model_package.split(os.path.sep)[-1]
  mc_name = topology.model.name
  pkg = Path(model_package)
  output = Path(os.path.join(pkg, 'src'))
  models = model_parser(pkg)
  cyml_rep = Path(os.path.join(output, 'pyx'))
  dir_test = Path(os.path.join(pkg, 'test'))

  tg_rep1 = Path(os.path.join(output, 'cs'))  # target language models  directory in output
  dir_test_lang = Path(os.path.join(dir_test, 'cs'))

  namep_ = namep.replace("-", "_")
  tg_rep = Path(os.path.join(tg_rep1, namep_))
  if not Path.is_dir(tg_rep):
    tg_rep.mkdir()

  if not Path.is_dir(dir_test_lang):  #Create if it doesn't exist
    dir_test_lang.mkdir()

  getattr(getattr(generators, 'csharpGenerator'), 'to_struct_cs')([topology.model], tg_rep, mc_name)
  getattr(getattr(generators, 'csharpGenerator'), 'to_wrapper_cs')(topology.model, tg_rep, mc_name)
  
  # Check each modelUnit
  for k, file in enumerate(cyml_rep.files()):
    with open(file, 'r') as fi:
      source = fi.read()
    name = os.path.split(file)[1].split(".")[0]
    for model in models:
      if name.lower() == model.name.lower() and prefix(model) != "function":
        test = Main(file, 'cs', model, topology.model.name)

        try:
          test.parse()
          with open(report_path, 'a') as rf:
            rf.write(f"Successfully parsed {os.path.basename(file)}\n")
        except Exception as e:
          with open(report_path, 'a') as rf:
            rf.write(f"ERROR ModelUnit when parsing --- {os.path.basename(file)} ---\n{e}\n\n")
          raise

        try:
          test.to_ast(source)
          with open(report_path, 'a') as rf:
            rf.write(f"Successfully generated AST for {os.path.basename(file)}\n")
        except Exception as e:
          with open(report_path, 'a') as rf:
            rf.write(f"ERROR ModelUnit when generating AST --- {os.path.basename(file)} ---\n{e}\n\n")
          raise

        try:
          test.to_source()
          with open(report_path, 'a') as rf:
            rf.write(f"Successfully transformed {os.path.basename(file)}\n\n")
        except Exception as e:
          with open(report_path, 'a') as rf:
            rf.write(f"ERROR ModelUnit when transpiling --- {os.path.basename(file)} ---\n{e}\n\n")
          raise
  verif_result = True
  return verif_result


#-----------------------------------------------------------------
# Function to check the syntax of the generated composite and the CROP2ML -> language/platform transformation
#-----------------------------------------------------------------
def check_code_composite(model_package, report_path, iteration):
  with open(report_path, 'a') as rf:
    rf.write(f"CHECKING CODE COMPOSITE GENERATED --- ATTEMPT {iteration} ---\n")
  verif_result = False
  pkg = Path(model_package)
  output = Path(os.path.join(pkg, 'src'))
  cyml_rep = Path(os.path.join(output, 'pyx'))
  topology = Topology(model_package.split(os.path.sep)[-1], model_package)
  mc_name = topology.model.name
  compoPath = Path(os.path.join(cyml_rep, f"{mc_name}Component.pyx"))
  with open(compoPath, 'r') as fi:
    source = fi.read()
  test = Main(source, 'cs', topology.model, topology.model.name)

  try:
    test.parse()
    with open(report_path, 'a') as rf:
      rf.write(f"Successfully parsed composite model --- {mc_name}Component.pyx ---\n")
  except Exception as e:
    with open(report_path, 'a') as rf:
      rf.write(f"ERROR ModelComposite when parsing --- {mc_name}Component.pyx --- :\n{e}\n\n")
    raise

  try:
    test.to_ast(source)
    with open(report_path, 'a') as rf:
      rf.write(f"Successfully generated the AST for the composite model --- {mc_name}Component.pyx ---\n\n")
  except Exception as e:
    with open(report_path, 'a') as rf:
      rf.write(f"ERROR ModelComposite when generating the AST --- {mc_name}Component.pyx --- :\n{e}\n\n")
    raise

  with open(report_path, 'a') as rf:
    rf.write("All files parsed and AST generated successfully.\n\n")
    
  verif_result = True
  return verif_result


#-----------------------------------------------------------------
# Function to check if the code generated in the output folder is correct by verifying the syntax and AST of the generated files
#-----------------------------------------------------------------
def debug_code(api_key, debug_cyml, apply_xml, apply_code, code_or_xml, model, model_package, report_path, apply_correction):
  with open(report_path, 'r') as f:
    lines = f.readlines()
  for i, line in enumerate(reversed(lines)):

    if "ERROR ModelUnit" in line:
      parts = line.split('---')
      filename = parts[1].strip()
      cyml_path = os.path.join(model_package, 'src', 'pyx', filename)
      xml_path = os.path.join(model_package, 'crop2ml', f"unit.{filename.split('.')[0]}.xml")
      error_msg = "".join(lines[len(lines)-i-1:])
      response, response_xml, response_code, file_to_modify = create_debug_code_unit(
                                                              api_key, debug_cyml, code_or_xml, apply_code,
                                                              apply_xml, model, cyml_path, xml_path, 
                                                              error_msg, apply_correction)
      break

    elif "ERROR ModelComposite" in line:
      parts = line.split('---')
      filename = parts[1].strip()
      cyml_path = os.path.join(model_package, 'src', 'pyx', filename)
      base = filename.split('.')[0].replace("Component", "", 1)
      xml_path = os.path.join(model_package, 'crop2ml', f"composition.{base}.xml")
      algo_metas = [os.path.join(model_package, 'crop2ml', f) for f in os.listdir(os.path.join(model_package, 'crop2ml')) if f.startswith("unit") and f.endswith(".xml")]
      error_msg = "".join(lines[lines.index(line)+1:])
      response, response_xml, response_code, file_to_modify = create_debug_code_composite(
                                                              api_key, debug_cyml, code_or_xml, apply_code,
                                                              apply_xml, model, cyml_path, xml_path, algo_metas,
                                                              error_msg, apply_correction)
      break

  if apply_correction:
    if file_to_modify == "XML" or file_to_modify == "BOTH":
      dom = format_xml(response_xml)
      dom = xml.dom.minidom.parseString(dom.decode('utf-8') if isinstance(dom, bytes) else dom)
      with open(xml_path, 'w', encoding='utf-8') as f:
        f.write(dom.toprettyxml())
    if file_to_modify == "CODEBASE" or file_to_modify == "BOTH":
      with open(cyml_path, 'w', encoding='utf-8') as rf:
        rf.write(response_code)
    
  else :
    with open(report_path, 'a') as rf:
      rf.write(f"To debug this error, try :\n\n {response}\n")


#-----------------------------------------------------------------
# Function to check if the code generated in the output folder is correct by verifying the syntax and AST of the generated files
#-----------------------------------------------------------------
def debug_xml(api_key, debug_xml, apply_xml, model, model_package, report_path, apply_correction):
  with open(report_path, 'r') as f:
    lines = f.readlines()
  for line in reversed(lines):

    if "ERROR ModelUnit-Generation" in line:
      parts = line.split('---')
      filename = parts[1].strip()
      xml_path = os.path.join(model_package, 'crop2ml', f"unit.{filename.split('.')[0]}.xml")
      error_msg = "".join(lines[lines.index(line)+1:])
      response = create_debug_xml_unit(api_key, debug_xml, apply_xml, model, xml_path, error_msg, apply_correction)
      break

    elif "ERROR ModelComposite-Generation" in line:
      parts = line.split('---')
      filename = parts[1].strip()
      base = filename.split('.')[0].replace("Component", "", 1)
      xml_path = os.path.join(model_package, 'crop2ml', f"composition.{base}.xml")
      algo_metas = [os.path.join(model_package, 'crop2ml', f) for f in os.listdir(os.path.join(model_package, 'crop2ml')) if f.startswith("unit") and f.endswith(".xml")]
      error_msg = "".join(lines[lines.index(line)+1:])
      response = create_debug_xml_composite(api_key, debug_xml, apply_xml, model, xml_path, algo_metas, error_msg, apply_correction)
      break

  if apply_correction:
    dom = format_xml(response)
    dom = xml.dom.minidom.parseString(dom.decode('utf-8') if isinstance(dom, bytes) else dom)
    with open(xml_path, 'w', encoding='utf-8') as f:
      f.write(dom.toprettyxml())
  else:
    with open(report_path, 'a') as rf:
      rf.write(f"To debug this error, try :\n\n {response}\n")