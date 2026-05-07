import os
import json
from utilities import extract_text

#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-DescMeta/Agent-PyRefactor
# This function constructs a prompt based on the main file and the programming language.
#-----------------------------------------------------------------
def prompt_unit(main_file, language_name, helper_files):
  main_file_name = os.path.basename(main_file)
  
  prompt = f"Analyze the following crop model source code, written in {language_name}. The main file to process is {main_file_name}.\n"
  prompt += f"The content is marked clearly with --- START MAIN FILE --- at the start and --- END MAIN FILE --- at the end.\n"
  if helper_files:
    prompt += f"Additional files are provided to give more context about the model.\n"
    prompt += f"Each helper file is marked clearly with --- START HELPER FILE N--- at the start and --- END HELPER FILE N --- at the end.\n"
  prompt += f"Follow the system instructions.\n\n"
  prompt += f"--- START MAIN FILE ---\n{extract_text(main_file)}\n--- END MAIN FILE ---"
  for i, helper_file in enumerate(helper_files):
    prompt += f"\n\n--- START HELPER FILE {i+1} ---\n{extract_text(helper_file)}\n--- END HELPER FILE {i+1} ---"
  
  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-AlgoMeta
# This function constructs a prompt based on the refactored code.
#-----------------------------------------------------------------
def prompt_refactor(code_refactored):
  prompt = f"Analyze the following Python crop model source code and follow the system instructions.\n"
  prompt += f"The Python code is marked clearly with --- START FILE --- at the start and --- END FILE --- at the end.\n\n"
  prompt += f"--- START FILE ---\n{code_refactored}\n--- END FILE ---"
  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-AlgoConsensus
# This function constructs a prompt based on the JSON candidates.
#-----------------------------------------------------------------
def prompt_consensus_JSON(jsons, main_file, language_name):
  main_file_name = os.path.basename(main_file)
  prompt = f"You will be provided {len(jsons)} JSON files, corresponding to interfaces of the source code file {main_file} written in {language_name}.\n"
  prompt += f"The source file is marked clearly with --- SOURCE CODE FILE: filename --- at the start and --- END SOURCE CODE FILE --- at the end.\n"
  prompt += f"Each JSON file is marked clearly with --- JSON FILE --- at the start and --- END JSON FILE --- at the end.\n"
  prompt += f"Follow the system instructions.\n\n"
  prompt += f"--- SOURCE CODE FILE: {main_file_name} ---\n{extract_text(main_file)}\n--- END FILE ---"
 
  for i, candidate in enumerate(jsons):
    prompt += f"\n\n--- JSON FILE {i+1} ---\n{candidate}\n--- END FILE ---"
  
  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-MetaMerge
# This function constructs a prompt based on the k candidates.
#-----------------------------------------------------------------
def prompt_consensus_python(codes, main_file, language_name, helper_files):
  main_file_name = os.path.basename(main_file)
  
  prompt = f"You will be provided {len(codes)} python module, corresponding to interfaces of the source code file {main_file_name} written in {language_name}.\n"
  prompt += f"The source file is marked clearly with --- SOURCE CODE FILE: filename --- at the start and --- END SOURCE CODE FILE --- at the end.\n"
  if helper_files:
    prompt += f"Additional files are provided to give more context about the model.\n"
    prompt += f"Each helper file is marked clearly with --- START HELPER FILE N--- at the start and --- END HELPER FILE N --- at the end.\n"
  prompt += f"Each python module is marked clearly with --- PYTHON MODULE N --- at the start and --- END PYTHON MODULE N --- at the end.\n"
  prompt += f"Follow the system instructions.\n\n"
  prompt += f"--- SOURCE CODE FILE: {main_file_name} ---\n{extract_text(main_file)}\n--- END SOURCE CODE FILE ---"
 
  for i, helper_file in enumerate(helper_files):
    prompt += f"\n\n--- START HELPER FILE {i+1} ---\n{extract_text(helper_file)}\n--- END HELPER FILE {i+1} ---"
  for i, code in enumerate(codes):
    prompt += f"\n\n--- PYTHON MODULE {i+1} ---\n{code}\n--- END PYTHON MODULE {i+1} ---"
  
  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-CyMLTranspile
# This function constructs a prompt based on the refactored code.
#-----------------------------------------------------------------
def prompt_transpile(code_refactored, algo_meta):
  prompt = f"Analyze the following Python crop model source code along with its JSON documentation and follow the system instructions.\n"
  prompt += f"The JSON documentation is marked clearly with --- START DOCUMENTATION --- at the start and --- END DOCUMENTATION --- at the end.\n"
  prompt += f"The Python code is marked clearly with --- START FILE --- at the start and --- END FILE --- at the end.\n\n"
  prompt += f"--- START DOCUMENTATION ---\n{json.dumps(algo_meta, indent=4)}\n--- END DOCUMENTATION ---\n\n"
  prompt += f"--- START FILE ---\n{code_refactored}\n--- END FILE ---"
  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-CompositeMeta
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_composite(XML_files, composite):
  prompt = ""
  if composite is not None:
    prompt += f"Analyze the following file representing the composite crop model and follow the system instructions.\n"
    prompt += f"The composite is marked clearly with --- START COMPOSITE --- at the start and --- END COMPOSITE --- at the end.\n\n"
    prompt += f"--- START COMPOSITE ---\n{extract_text(composite)}\n--- END COMPOSITE ---\n\n"

  prompt += f"Analyze the following XML representing the model units of a composite crop model and follow the system instructions.\n"
  prompt += f"Each file is marked clearly with --- START FILE --- at the start and --- END FILE --- at the end.\n\n"

  for file in XML_files:
    prompt += f"--- START FILE ---\n{extract_text(file)}\n--- END FILE ---\n\n"
  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-Debug for modelUnit
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_clean_code(cyml_module):
  prompt = ""
  prompt += f"Analyze and clean (if needed) the following codebase and follow the system instructions.\n"
  prompt += f"The code is marked clearly with --- START CODE --- at the start and --- END CODE --- at the end.\n"
  prompt += f"--- START CODE ---\n{extract_text(cyml_module)}\n--- END CODE ---\n\n"

  return prompt

#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-Debug for modelUnit
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_debug_code_unit(cyml_module, algo_meta, error_msg):
  prompt = ""
  prompt += f"Analyze and debug the following codebase representing a crop model component and follow the system instructions.\n"
  prompt += f"The code is marked clearly with --- START CODE --- at the start and --- END CODE --- at the end.\n"
  prompt += f"The XML documentation associated is marked clearly with --- START XML --- at the start and --- END XML --- at the end.\n"
  prompt += f"The error message associated is marked clearly with --- START ERROR --- at the start and --- END ERROR --- at the end.\n"
  prompt += f"--- START CODE ---\n{extract_text(cyml_module)}\n--- END CODE ---\n\n"
  prompt += f"--- START XML ---\n{extract_text(algo_meta)}\n--- END XML ---\n\n"
  prompt += f"--- START ERROR ---\n{error_msg}\n--- END ERROR ---\n\n"

  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-Debug for modelUnit
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_choose(proposed_correction):
  prompt = ""
  prompt += f"Follow the system instructions with the proposed correction.\n"
  prompt += f"The proposed correction is marked clearly with --- START PROPOSED CORRECTION --- at the start and --- END PROPOSED CORRECTION --- at the end.\n"
  prompt += f"--- START PROPOSED CORRECTION ---\n{proposed_correction}\n--- END PROPOSED CORRECTION ---\n\n"

  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-Debug for modelUnit
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_apply_code_unit(cyml_module, error_msg, proposed_correction):
  prompt = ""
  prompt += f"Produce a corrected codebase using the proposed correction and follow the system instructions.\n"
  prompt += f"The codebase is marked clearly with --- START CODE --- at the start and --- END CODE --- at the end.\n"
  prompt += f"The error message is marked clearly with --- START ERROR --- at the start and --- END ERROR --- at the end.\n"
  prompt += f"The proposed correction is marked clearly with --- START PROPOSED CORRECTION --- at the start and --- END PROPOSED CORRECTION --- at the end.\n"
  prompt += f"--- START CODE ---\n{extract_text(cyml_module)}\n--- END CODE ---\n\n"
  prompt += f"--- START ERROR ---\n{error_msg}\n--- END ERROR ---\n\n"
  prompt += f"--- START PROPOSED CORRECTION ---\n{proposed_correction}\n--- END PROPOSED CORRECTION ---\n\n"

  return prompt

#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-Debug for modelUnit
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_debug_xml_unit(algo_meta, error_msg):
  prompt = ""
  prompt += f"Analyze and debug the following XML documentation of a crop model component and follow the system instructions.\n"
  prompt += f"The XML documentation is marked clearly with --- START XML --- at the start and --- END XML --- at the end.\n"
  prompt += f"The error message associated is marked clearly with --- START ERROR --- at the start and --- END ERROR --- at the end.\n"
  prompt += f"--- START XML ---\n{extract_text(algo_meta)}\n--- END XML ---\n\n"
  prompt += f"--- START ERROR ---\n{error_msg}\n--- END ERROR ---\n\n"

  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-Debug for modelUnit
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_debug_xml_composite(algo_meta, algo_metas, error_msg):
  prompt = ""
  prompt += f"Analyze and debug the following XML documentation of a composition of different crop model components and follow the system instructions.\n"
  prompt += f"The XML documentation is marked clearly with --- START XML --- at the start and --- END XML --- at the end.\n"
  prompt += f"The error message associated is marked clearly with --- START ERROR --- at the start and --- END ERROR --- at the end.\n"
  prompt += f"The XML documentation of the other units are marked clearly with --- START XML UNIT : XXX --- at the start and --- END XML UNIT : XXX--- at the end.\n"

  prompt += f"--- START XML ---\n{extract_text(algo_meta)}\n--- END XML ---\n\n"
  prompt += f"--- START ERROR ---\n{error_msg}\n--- END ERROR ---\n\n"

  for algo_meta in algo_metas:
    base = os.path.basename(algo_meta)
    prompt += f"--- START XML UNIT : {base} ---\n{extract_text(algo_meta)}\n--- END XML UNIT : {base} ---\n\n"

  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-Debug for modelUnit
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_apply_xml(algo_meta, proposed_correction):
  prompt = ""
  prompt += f"Produce a corrected XML documentation using the proposed correction and follow the system instructions.\n"
  prompt += f"The XML documentation is marked clearly with --- START XML --- at the start and --- END XML --- at the end.\n"
  prompt += f"The proposed correction is marked clearly with --- START PROPOSED CORRECTION --- at the start and --- END PROPOSED CORRECTION --- at the end.\n"
  prompt += f"--- START XML ---\n{extract_text(algo_meta)}\n--- END XML ---\n\n"
  prompt += f"--- START PROPOSED CORRECTION ---\n{proposed_correction}\n--- END PROPOSED CORRECTION ---\n\n"

  return prompt


#-----------------------------------------------------------------
# Function to create a prompt adapted to Agent-Debug for modelComposite
# This function constructs a prompt based on the XML files of each model units.
#-----------------------------------------------------------------
def prompt_debug_composite(cyml_module, composite_meta, algo_metas, error_msg):
  prompt = ""
  prompt += f"Analyze and debug the following composition codebase representing a crop model component and follow the system instructions.\n"
  prompt += f"The composite code is marked clearly with --- START CODE --- at the start and --- END CODE --- at the end.\n"
  prompt += f"The XML documentation associated is marked clearly with --- START XML COMPOSITE --- at the start and --- END XML COMPOSITE --- at the end.\n"
  prompt += f"The XML documentation of the other units are marked clearly with --- START XML UNIT : XXX --- at the start and --- END XML UNIT : XXX--- at the end.\n"
  prompt += f"The error message associated is marked clearly with --- START ERROR --- at the start and --- END ERROR --- at the end.\n"
  prompt += f"--- START CODE ---\n{extract_text(cyml_module)}\n--- END CODE ---\n\n"
  prompt += f"--- START XML COMPOSITE ---\n{extract_text(composite_meta)}\n--- END XML COMPOSITE ---\n\n"
  prompt += f"--- START ERROR ---\n{error_msg}\n--- END ERROR ---\n\n"

  for algo_meta in algo_metas:
    base = os.path.basename(algo_meta)
    prompt += f"--- START XML UNIT : {base} ---\n{extract_text(algo_meta)}\n--- END XML UNIT : {base} ---\n\n"

  return prompt