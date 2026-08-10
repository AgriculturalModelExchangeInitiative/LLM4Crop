import argparse
import os
import sys
from utilities import check_files
from generation import maj_component, process_unit, process_composite, create_crop2ml_package, clean_pyx, generate_component
from verification import check_code_composite, debug_code, debug_xml, generate_pyx_composite, generate_pyx_unit, check_code_unit
import concurrent.futures

#-----------------------------------------------------------------
# CONFIGURATION
#-----------------------------------------------------------------
API_KEY_PATH = "./config/api_key.txt"
BIG_MODEL = "gpt-5.3-codex"
SMALL_MODEL = "gpt-5.4-mini"

'''API_KEY_PATH = "./config/api_key_claude.txt"
BIG_MODEL = "claude-opus-4-6"
SMALL_MODEL = "claude-sonnet-4-6"'''
COOKIE_CUTTER_TEMPLATE = "./config/cookiecutter-crop2ml/"
LOG_FILE = "Crop2LLM_report.txt"
REPORT_FILE = "Transformation_report.txt"
LANGUAGES = ['r', 'cs', 'py', 'f90', 'apsim', 'dssat', 'stics', 'bioma', 'sirius', 'java', 'openalea', 'simplace','cpp']
NUMBER_CANDIDATES = 3
MAX_PARALLEL_UNITS = 5
NUMBER_ITERATIONS = 50

UNIT_META = "./config/Agents/Agent-UnitMeta.txt"
COMPOSITE_META = "./config/Agents/Agent-CompositeMeta.txt"
PY_REFACTOR = "./config/Agents/Agent-PyRefactor.txt"
PY_CONSENSUS = "./config/Agents/Agent-PyConsensus.txt"
CYML_TRANSPILE = "./config/Agents/Agent-CyMLTranspile.txt"
ALGO_META = "./config/Agents/Agent-AlgoMeta.txt"
DEBUG_CYML = "./config/Agents/Agent-DebugCode.txt"
DEBUG_XML = "./config/Agents/Agent-DebugXML.txt"
APPLY_CODE = "./config/Agents/Agent-ApplyCode.txt"
APPLY_XML = "./config/Agents/Agent-ApplyXML.txt"
CODE_OR_XML = "./config/Agents/Agent-CodeOrXML.txt"
CLEANER = "./config/Agents/Agent-CodeCleaner.txt"
CONFIG_FILES = [
  API_KEY_PATH,
  UNIT_META,
  COMPOSITE_META,
  PY_REFACTOR,
  PY_CONSENSUS,
  CYML_TRANSPILE,
  ALGO_META,
  DEBUG_CYML,
  DEBUG_XML,
  APPLY_CODE,
  APPLY_XML,
  CODE_OR_XML,
  CLEANER
]

#-----------------------------------------------------------------
# Simulation section
# Generate a complete Crop2ML component from model units and composite in the output folder defined
#-----------------------------------------------------------------
if __name__ == "__main__":
  parser = argparse.ArgumentParser(description="Transform crop model components into Crop2ML component and vice-versa.")
  parser.add_argument('-u', '--unit', action='append', nargs='+', required=False, help='Model unit files (can be specified multiple times)')
  parser.add_argument('-c', '--composite', required=False, help='Model composite file')
  parser.add_argument('-o', '--output', required=False, help='Output folder')
  parser.add_argument('-p', '--package', required=False, help='Model package directory')
  args = parser.parse_args()

  if args.unit is None and args.package is None:
    parser.error("At least one of --unit or --package must be provided.")

  elif args.unit is not None :
    if args.package is not None :
      parser.error("You must choose between --unit and --package, not both.")

    elif args.output is None:
      parser.error("Output folder must be specified when using --unit.")

    #-----------------------------------------------------------------
    # SECTION : From crop model component to Crop2ML (LLM4Crop)
    else :
      model_units = args.unit
      model_composite = args.composite
      output_folder = args.output
      XML_units = []
      functions_transpiled = []

      check_files(*model_units, comp=model_composite, config_files=CONFIG_FILES, log_file=LOG_FILE, output_folder=output_folder)

      # Process each model unit concurrently
      print("Generating modelunits...")
      with concurrent.futures.ThreadPoolExecutor(max_workers=MAX_PARALLEL_UNITS) as executor:
        futures = [
          executor.submit(process_unit, API_KEY_PATH, UNIT_META, PY_REFACTOR, ALGO_META, CYML_TRANSPILE, PY_CONSENSUS,
                          SMALL_MODEL, BIG_MODEL, NUMBER_CANDIDATES, LOG_FILE, grp, model_composite, output_folder)
          for idx, grp in enumerate(model_units)
        ]
        for fut in concurrent.futures.as_completed(futures):
          xml, functions = fut.result()
          XML_units.append(xml)
          functions_transpiled.append(functions)

      # Process model composite
      print(f"Generating the composite model...")
      composite_metadata, xml_composite, model_composite = process_composite(API_KEY_PATH, COMPOSITE_META, SMALL_MODEL, output_folder, XML_units, model_composite, LOG_FILE, model_units[0][0])
      
      # Create cookiecutter project
      print(f"Generating Crop2ML project for the model component...")
      project_dir = create_crop2ml_package(COOKIE_CUTTER_TEMPLATE, output_folder, model_composite, composite_metadata, XML_units, xml_composite, functions_transpiled, LOG_FILE)

      print(f"Crop2ML package generated successfully in {project_dir} !\nCheck {LOG_FILE} for more details during the automatic transformation !")

  #-----------------------------------------------------------------
  # SECTION : From Crop2ML to crop model component (CyMLTh)
  elif args.package is not None:
    package = args.package
    verif_result = False
    code_generated = False
    iteration = 0
    report_path = os.path.join(package, REPORT_FILE)

    check_files([], comp=None, config_files=CONFIG_FILES, log_file=REPORT_FILE, output_folder=package)
    print("Checking if the Crop2ML package is correct...")

    # Trying to generate the pyx code of each model units
    while not code_generated and iteration < NUMBER_ITERATIONS:
      iteration += 1
      try:
        code_generated = generate_pyx_unit(package, report_path, iteration)
      except Exception as e:
        debug_xml(API_KEY_PATH, DEBUG_XML, APPLY_XML, BIG_MODEL, package, report_path, iteration < NUMBER_ITERATIONS)

    if not code_generated:
      print("Code generation for model units failed. Please check the report for details.")
      sys.exit()

    # Clean pyx generated
    clean_pyx(package, API_KEY_PATH, CLEANER, SMALL_MODEL, MAX_PARALLEL_UNITS)
    
    # Verifying each pyx code of each model units are correct
    iteration = 0
    while not verif_result and iteration < NUMBER_ITERATIONS:
      iteration += 1
      try:
        verif_result = check_code_unit(package, report_path, iteration)
      except Exception as e:
        debug_code(API_KEY_PATH, DEBUG_CYML, APPLY_XML, APPLY_CODE, CODE_OR_XML, BIG_MODEL, package, report_path, iteration < NUMBER_ITERATIONS)
      
    if not verif_result:
      print("Code verification for model units failed. Please check the report for details.")
      sys.exit()

    # Trying to generate pyx code of model composite
    code_generated = False
    iteration = 0
    while not code_generated and iteration < NUMBER_ITERATIONS:
      iteration += 1
      try:
        code_generated = generate_pyx_composite(package, report_path, iteration)
      except Exception as e:
        debug_xml(API_KEY_PATH, DEBUG_CYML, DEBUG_XML, APPLY_XML, APPLY_CODE, CODE_OR_XML, BIG_MODEL, package, report_path, iteration < NUMBER_ITERATIONS)
      
    if not code_generated:
      print("Code generation for model composite failed. Please check the report for details.")
      sys.exit()

    # Verifying the pyx code of model component is correct
    verif_result = False
    iteration = 0
    while not verif_result and iteration < NUMBER_ITERATIONS:
      iteration += 1
      try:
        verif_result = check_code_composite(package, report_path, iteration)
      except Exception as e:
        debug_code(API_KEY_PATH, DEBUG_CYML, APPLY_XML, APPLY_CODE, CODE_OR_XML, BIG_MODEL, package, report_path, iteration < NUMBER_ITERATIONS)
      
    if not verif_result:
      print("Code verification for model composite failed. Please check the report for details.")
      sys.exit()

    else:
      # Trying to transpile the component in each language/platform
      maj_component(package, os.path.join(package, 'src', 'pyx'), os.path.join(package, 'crop2ml'))
      for language in LANGUAGES:
        generate_component(package, language, report_path)