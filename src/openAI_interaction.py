import re
from openai import OpenAI
import anthropic
import os
from pathlib import Path
import json
from utilities import extract_text, extract_extension, language
from prompt_creation import prompt_apply_code_unit, prompt_apply_xml, prompt_choose, prompt_debug_code_unit, prompt_debug_xml_composite, prompt_debug_xml_unit, prompt_unit
from prompt_creation import prompt_composite, prompt_refactor, prompt_transpile, prompt_debug_composite, prompt_consensus_JSON, prompt_consensus_python, prompt_clean_code

#-----------------------------------------------------------------
# Function to connect to OpenAI's API
# This function reads the API key from a file and initializes the OpenAI client.
#-----------------------------------------------------------------
def extract_api_key(API_KEY_PATH):
  api_key = extract_text(API_KEY_PATH)
  try:
    OpenAI(api_key = api_key)
  except Exception as e:
    print(f"An error occurred while connecting to OpenAI: {e}")
    return None
  return api_key


#-----------------------------------------------------------------
# Function to send instructions and prompt to OpenAI's model
# This function takes instructions, a prompt, an API key, and a model name and returns the response from the model.
#-----------------------------------------------------------------
def send_to_llm(instructions, prompt, api_key, model, reasoning_effort, text_format, verbosity):
  if "gpt" in model:
    client = OpenAI(api_key = api_key)

    response = client.responses.create(
      model=model,
      reasoning={"effort": reasoning_effort},
      store=True,
      text={
        "format": {"type": text_format},
        "verbosity": verbosity,
        },
      input=[
        {"role": "developer",
          "content": [{"type": "input_text", "text": instructions}],
        },
        {
          "role": "user",
          "content": [{"type": "input_text", "text": prompt}],
        }
      ],
    )

    response = response.output_text
    response = re.sub(r'^\s*```(?:json|python|cython|cyml)?\s*$', '', response, flags=re.MULTILINE)
    response = response.lstrip()
  
  else:
    client = anthropic.Anthropic(api_key = api_key)

    response = client.messages.create(
      model = model,
      system=instructions,
      max_tokens=20000,
      messages = [
        { "role": "user", "content": prompt}
      ],
      thinking= {"type": "adaptive"},
      output_config= {"effort": reasoning_effort}
    )

    for block in response.content:
      if block.type == "text":
        response = block.text
        response = re.sub(r'^\s*```(?:json|python|cython|cyml)?\s*$', '', response, flags=re.MULTILINE)
        response = response.lstrip()
    
  return(response)


#-----------------------------------------------------------------
# Function to create metadata JSON file for a unit model 
# This function generates metadata for a given code file and saves it as a JSON file.
#-----------------------------------------------------------------
def create_unit_metadata(api_key_path, agent_descmeta, model, output_path, main_file, helper_files):
  api_key = extract_api_key(api_key_path)
  extension = extract_extension(main_file)
  language_name = language(extension)
  instructions_metadata = extract_text(agent_descmeta)

  prompt = prompt_unit(main_file, language_name, helper_files)
  response_metadata = send_to_llm(instructions_metadata, prompt, api_key, model, "medium", "json_object", "low")

  os.makedirs(output_path, exist_ok=True)
  base = Path(main_file).stem
  json_metadata_path = output_path + "/" + base + "_metadata.json"
  json_metadata = json.loads(response_metadata)

  with open(json_metadata_path, "w", encoding="utf-8") as f:
    json.dump(json_metadata, f, ensure_ascii=False, indent=4)
  return json_metadata


#-----------------------------------------------------------------
# Function to create metadata JSON file for a composite model 
# This function generates metadata for a given code file and saves it as a JSON file.
#-----------------------------------------------------------------
def create_composite_metadata(api_key_path, agent_compositemeta, model, output_path, modelunits, main_file):
  api_key = extract_api_key(api_key_path)
  instructions_metadata = extract_text(agent_compositemeta)
  prompt = prompt_composite(modelunits, main_file)
  response_metadata = send_to_llm(instructions_metadata, prompt, api_key, model, "high", "json_object", "low")

  os.makedirs(output_path, exist_ok=True)
  if (main_file is None):
    base = Path(modelunits[0]).stem
  else:
    base = Path(main_file).stem
  json_metadata_path = output_path + "/" + base.replace("unit.", "") + "_composite.json"
  json_metadata = json.loads(response_metadata)

  with open(json_metadata_path, "w", encoding="utf-8") as f:
    json.dump(json_metadata, f, ensure_ascii=False, indent=4)
  return json_metadata

#-----------------------------------------------------------------
# Function to create algorithm metadata JSON file
# This function generates a algorithm metadata for a given code file and saves it as a JSON file.
#-----------------------------------------------------------------
def create_algo_metadata(api_key_path, agent_algometa, model, python_code):
  api_key = extract_api_key(api_key_path)
  instructions_json = extract_text(agent_algometa)

  prompt = prompt_refactor(python_code)
  response = send_to_llm(instructions_json, prompt, api_key, model, "high", "json_object", "low")
  json_code = json.loads(response)

  return json_code


#-----------------------------------------------------------------
# Function to create a consensus algorithm metadata JSON file from different candidates
# This function generates a consensus algorithm metadata for a given code file and saves it as a JSON file.
#-----------------------------------------------------------------
def create_consensus_JSON(api_key_path, agent_algo_consensus, model, jsons, main_file, output_path):
  api_key = extract_api_key(api_key_path)
  extension = extract_extension(main_file)
  language_name = language(extension)

  instructions_algo_consensus = extract_text(agent_algo_consensus)

  prompt = prompt_consensus_JSON(jsons, main_file, language_name)
  response = send_to_llm(instructions_algo_consensus, prompt, api_key, model, "high", "json_object", "low")

  os.makedirs(output_path, exist_ok=True)
  base = Path(main_file).stem
  json_code_path = output_path + "/" + base + "_code.json"
  json_code = json.loads(response)

  with open(json_code_path, "w", encoding="utf-8") as f:
    json.dump(json_code, f, ensure_ascii=False, indent=4)

  return json_code


#-----------------------------------------------------------------
# Function to create python code
# This function generates a refactored python module for a given code file and saves it.
#-----------------------------------------------------------------
def create_python_code(api_key_path, agent_pyrefactor, model, main_file, helper_files):
  api_key = extract_api_key(api_key_path)
  extension = extract_extension(main_file)
  language_name = language(extension)
  instructions_refactor = extract_text(agent_pyrefactor)

  prompt = prompt_unit(main_file, language_name, helper_files)
  response_refactored = send_to_llm(instructions_refactor, prompt, api_key, model, "high", "text", "low")

  return response_refactored


#-----------------------------------------------------------------
# Function to create a consensus python code from different candidates
# This function generates a consensus python code for a given code file and saves it as a py file.
#-----------------------------------------------------------------
def create_consensus_python(api_key_path, agent_py_consensus, model, codes, main_file, helper_files, output_path):
  api_key = extract_api_key(api_key_path)
  extension = extract_extension(main_file)
  language_name = language(extension)

  instructions_py_consensus = extract_text(agent_py_consensus)

  prompt = prompt_consensus_python(codes, main_file, language_name, helper_files)
  response = send_to_llm(instructions_py_consensus, prompt, api_key, model, "high", "text", "low")

  os.makedirs(output_path, exist_ok=True)
  base = Path(main_file).stem
  python_code_path = output_path + "/" + base + "_code.py"

  with open(python_code_path, "w", encoding="utf-8") as f:
    f.write(response)
  return response


#-----------------------------------------------------------------
# Function to transpile code to CyML
# This function generates a CyML module for a given python module and saves it.
#-----------------------------------------------------------------
def create_cyml_code(api_key_path, agent_cymltranspile, model, python_module, algo_meta):
  api_key = extract_api_key(api_key_path)
  instructions_transpile = extract_text(agent_cymltranspile)

  prompt_transpiled = prompt_transpile(python_module, algo_meta)
  response_cyml = send_to_llm(instructions_transpile, prompt_transpiled, api_key, model, "high", "text", "low")

  return response_cyml


#-----------------------------------------------------------------
# Function to clean CyML code
# This function generates a cleaned Cython module for a given code file and saves it.
#-----------------------------------------------------------------
def create_clean_code(api_key_path, agent_cleaner, model, pyx_path):
  api_key = extract_api_key(api_key_path)
  instructions_cleaner = extract_text(agent_cleaner)

  prompt = prompt_clean_code(pyx_path)
  response_cleaned = send_to_llm(instructions_cleaner, prompt, api_key, model, "medium", "text", "low")

  return response_cleaned, pyx_path


#-----------------------------------------------------------------
# Function to debug CyML code for modelUnit
# This function proposes corrections for a CyML module modelUnit.
#-----------------------------------------------------------------
def create_debug_code_unit(api_key_path, agent_debug_code, agent_choose, agent_apply_code, agent_apply_xml, 
                    model, cyml_module, algo_meta, error_msg, apply_correction):
  
  api_key = extract_api_key(api_key_path)
  instructions_debug = extract_text(agent_debug_code)

  prompt_debug = prompt_debug_code_unit(cyml_module, algo_meta, error_msg)
  response = send_to_llm(instructions_debug, prompt_debug, api_key, model, "high", "text", "medium")
  file_to_modify = ""
  response_xml = ""
  response_code = ""

  if apply_correction:
    instructions_choose = extract_text(agent_choose)
    prompt_code_or_xml = prompt_choose(response)
    response_choose = send_to_llm(instructions_choose, prompt_code_or_xml, api_key, model, "medium", "json_object", "low")
    json_response = json.loads(response_choose)

    file_to_modify = json_response.get("modifs").get("type", "")

    if file_to_modify == "XML" or file_to_modify == "BOTH":
      instructions_apply = extract_text(agent_apply_xml)
      prompt_apply = prompt_apply_xml(algo_meta, response)
      response_xml = send_to_llm(instructions_apply, prompt_apply, api_key, model, "medium", "text", "low")

    if file_to_modify == "CODEBASE" or file_to_modify == "BOTH":
      instructions_apply = extract_text(agent_apply_code)
      prompt_apply = prompt_apply_code_unit(cyml_module, error_msg, response)
      response_code = send_to_llm(instructions_apply, prompt_apply, api_key, model, "medium", "text", "low") 

  return response, response_xml, response_code, file_to_modify


#-----------------------------------------------------------------
# Function to debug XML documentation for modelUnit
# This function proposes corrections for a XML documentation modelUnit.
#-----------------------------------------------------------------
def create_debug_xml_unit(api_key_path, agent_debug, agent_apply, model, algo_meta, error_msg, apply_correction):
  api_key = extract_api_key(api_key_path)
  instructions_debug = extract_text(agent_debug)

  prompt_debug = prompt_debug_xml_unit(algo_meta, error_msg)
  response = send_to_llm(instructions_debug, prompt_debug, api_key, model, "high", "text", "medium")

  if apply_correction:
    instructions_apply = extract_text(agent_apply)
    prompt_apply = prompt_apply_xml(algo_meta, response)
    response = send_to_llm(instructions_apply, prompt_apply, api_key, model, "medium", "text", "low")

  return response


#-----------------------------------------------------------------
# Function to debug XML documentation for modelComposite
# This function proposes corrections for a XML documentation modelComposite.
#-----------------------------------------------------------------
def create_debug_xml_composite(api_key_path, agent_debug, agent_apply, model, algo_meta, algo_metas, error_msg, apply_correction):
  api_key = extract_api_key(api_key_path)
  instructions_debug = extract_text(agent_debug)

  prompt_debug = prompt_debug_xml_composite(algo_meta, algo_metas, error_msg)
  response = send_to_llm(instructions_debug, prompt_debug, api_key, model, "high", "text", "medium")

  if apply_correction:
    instructions_apply = extract_text(agent_apply)
    prompt_apply = prompt_apply_xml(algo_meta, response)
    response = send_to_llm(instructions_apply, prompt_apply, api_key, model, "medium", "text", "low")

  return response


#-----------------------------------------------------------------
# Function to debug CyML code for ModelComposite
# This function proposes corrections for a CyML module modelComposite.
#-----------------------------------------------------------------
def create_debug_code_composite(api_key_path, agent_debug_code, agent_choose, agent_apply_code, agent_apply_xml, 
                    model, cyml_module, composite_meta, algo_metas, error_msg, apply_correction):
  api_key = extract_api_key(api_key_path)
  instructions_debug = extract_text(agent_debug_code)

  prompt_debug = prompt_debug_composite(cyml_module, composite_meta, algo_metas, error_msg)
  response = send_to_llm(instructions_debug, prompt_debug, api_key, model, "high", "text", "medium")

  file_to_modify = ""
  response_xml = ""
  response_code = ""

  if apply_correction:
    instructions_choose = extract_text(agent_choose)
    prompt_code_or_xml = prompt_choose(response)
    response_choose = send_to_llm(instructions_choose, prompt_code_or_xml, api_key, model, "medium", "json_object", "low")
    json_response = json.loads(response_choose)

    file_to_modify = json_response.get("modifs").get("type", "")

    if file_to_modify == "XML" or file_to_modify == "BOTH":
      instructions_apply = extract_text(agent_apply_xml)
      prompt_apply = prompt_apply_xml(composite_meta, response)
      response_xml = send_to_llm(instructions_apply, prompt_apply, api_key, model, "medium", "text", "low")
      
    if file_to_modify == "CODEBASE" or file_to_modify == "BOTH":
      instructions_apply = extract_text(agent_apply_code)
      prompt_apply = prompt_apply_code_unit(cyml_module, error_msg, response)
      response_code = send_to_llm(instructions_apply, prompt_apply, api_key, model, "medium", "text", "low")

  return response, response_xml, response_code, file_to_modify


  
