from pathlib import Path
import xml.dom.minidom
import xml.etree.ElementTree as ET
from utilities import extract_text, log_comments

#-----------------------------------------------------------------
# Function to convert JSON data to XML format
# This function takes a file path and JSON data, then converts the data into a Crop2ML-friendly XML format.
#-----------------------------------------------------------------
def convert_unit(file_path, json_metadata, json_code):
  metadata = json_metadata['metadata']
  init = json_code['init']
  process = json_code['process']
  inputs = json_code.get('inputs',[])
  outputs = json_code.get('outputs',[])
  functions = json_code.get('functions', [])
  tests = json_code['tests']

  if init != '-' and init != []:
    init_name = init.get('name', '-')
  else:
    init_name = '-'

  # Create XML tree
  root = ET.Element('ModelUnit', {
    "modelid": Path(file_path).stem + "." + metadata['Title'],
    "name": metadata['Title'],
    "timestep": "1",
    "version":  metadata['Model version']
  })

  # Description section
  desc = ET.SubElement(root, 'Description')
  ET.SubElement(desc, 'Title').text = metadata.get('Title','')
  ET.SubElement(desc, 'Authors').text = metadata.get('Authors', '')
  ET.SubElement(desc, 'Institution').text = metadata.get('Institution', '')
  ET.SubElement(desc, 'URI').text = metadata.get('URI', '')
  ET.SubElement(desc, 'Reference').text = metadata.get('DOI', '')
  ET.SubElement(desc, 'ExtendedDescription').text = metadata.get('Extended description', '')
  ET.SubElement(desc, 'ShortDescription').text = metadata.get('Short description', '')

  # I/O section
  xml_inputs = ET.SubElement(root, 'Inputs')
  add_inputs(xml_inputs, inputs)

  xml_outputs = ET.SubElement(root, 'Outputs')
  add_outputs(xml_outputs, outputs)

  # Initialization
  if init_name != '-':
    ET.SubElement(root, 'Initialization', {
      'name': f"init_{metadata['Title']}",
      'language': 'cyml',
      'filename': f"algo/pyx/init_{metadata['Title']}.pyx"
    })

  # Functions
  if functions != '-' and functions != []:
    for func in functions:
      if func['name'] != "-" and func['name'] != init_name and func['name'] != process['name']:
        ET.SubElement(root, 'Function', {
          'name': func['name'],
          'description': func['description'],
          'language': 'cyml',
          'type': 'external',
          'filename': f"algo/pyx/{func['name']}.pyx"
        })

  # Main Algorithm
  ET.SubElement(root, 'Algorithm', {
    'language': 'cyml',
    'platform': '',
    'filename': 'algo/pyx/' + metadata['Title'] + ".pyx"
  })

  # Parametersets
  add_tests(root, tests, inputs)

  return ET.tostring(root, encoding='utf-8')


#-----------------------------------------------------------------
# Function to convert JSON data 'inputs' to Crop2ML-XML input format
# This function takes the XML element for inputs and the JSON data for inputs, then adds the inputs to the XML tree in the correct format.
#-----------------------------------------------------------------
def add_inputs(xml_inputs, json_inputs):
  for input in json_inputs:
    attrs = {
        'name': str(input['name']),
        'description': str(input.get('description', '')),
        'inputtype': str(input.get('inputtype', ''))
    }
    if input.get('inputtype') == 'parameter':
      attrs['parametercategory'] = str(input.get('category', ''))
    else:
      attrs['variablecategory'] = str(input.get('category', ''))
    attrs['datatype'] = str(input.get('datatype', ''))
    if "ARRAY" in str(input.get('datatype')).upper() or "LIST" in str(input.get('datatype')).upper():
      attrs['len'] = str(input.get('len', ''))
    attrs['max'] = str(input.get('max', ''))
    attrs['min'] = str(input.get('min', ''))
    attrs['default'] = str(input.get('default', ''))
    attrs['unit'] = str(input.get('unit', ''))
    attrs['uri'] = str(input.get('uri', ''))
    
    if str(input.get('default')) == "-" or str(input.get('default')) == "None":
      attrs['default'] = ""
    if str(input.get('max')) == "-" or str(input.get('max')) == "None":
      attrs['max'] = ""
    if str(input.get('min')) == "-" or str(input.get('min')) == "None":
      attrs['min'] = ""

    # Validate that default, max, min are numerical when datatype is numerical
    numerical_types = ['DOUBLE', 'DOUBLELIST', 'DOUBLEARRAY', 'INTEGER', 'INTEGERLIST', 'INTEGERARRAY']
    is_numerical_datatype = any(numeric_type in str(input.get('datatype', '')).upper() for numeric_type in numerical_types)
    
    if is_numerical_datatype:
      for attr_key in ['default', 'max', 'min', "len"]:
        attr_value = attrs.get(attr_key, '')
        if attr_value and attr_value != '':
          try:
            float(attr_value)
          except ValueError:
            attrs[attr_key] = ""

    ET.SubElement(xml_inputs, 'Input', attrs)


#-----------------------------------------------------------------
# Function to convert JSON data 'outputs' to Crop2ML-XML output format
# This function takes the XML element for outputs and the JSON data for outputs, then adds the outputs to the XML tree in the correct format.
#-----------------------------------------------------------------
def add_outputs(xml_outputs, json_outputs):
  for output in json_outputs:
    attrs = {
      'name': str(output['name']),
      'description': str(output.get('description', '')),
      'variablecategory': str(output.get('category', '')),
      'datatype': str(output.get('datatype', ''))
    }
    if "ARRAY" in str(output.get('datatype')).upper() or "LIST" in str(output.get('datatype')).upper():
      attrs['len'] = str(output.get('len', ''))
    attrs['max'] = str(output.get('max', ''))
    attrs['min'] = str(output.get('min', ''))
    attrs['unit'] = str(output.get('unit', ''))
    attrs['uri'] = str(output.get('uri', ''))

    if str(output.get('max')) == "-" or str(output.get('max')) == "None":
      attrs['max'] = ""
    if str(output.get('min')) == "-" or str(output.get('min')) == "None":
      attrs['min'] = ""

    # Validate that default, max, min are numerical when datatype is numerical
    numerical_types = ['DOUBLE', 'DOUBLELIST', 'DOUBLEARRAY', 'INTEGER', 'INTEGERLIST', 'INTEGERARRAY']
    is_numerical_datatype = any(numeric_type in str(output.get('datatype', '')).upper() for numeric_type in numerical_types)
    
    if is_numerical_datatype:
      for attr_key in ['max', 'min', "len"]:
        attr_value = attrs.get(attr_key, '')
        if attr_value and attr_value != '':
          try:
            float(attr_value)
          except ValueError:
            attrs[attr_key] = ""

    ET.SubElement(xml_outputs, 'Output', attrs)


#-----------------------------------------------------------------
# Function to convert JSON data 'tests' to Crop2ML-XML test format
# This function takes the XML element for tests and the JSON data for tests, then adds them to the XML tree in the correct format.
#-----------------------------------------------------------------
def add_tests(root_XML, json_tests, json_inputs):
  parametersSets = ET.SubElement(root_XML, 'Parametersets')
  testSets = ET.SubElement(root_XML, 'Testsets')

  if json_tests == [] or json_tests[0] == "-" or json_tests[0] == "" or json_tests[0].get('name') == "-":
    return
  
  return

  #TO-DO : add the tests detected in the XML
  parameter_inputs = []
  variable_inputs = []
  inputtype_by_name = {inp['name']: inp.get('inputtype', '') for inp in json_inputs}

  for test in json_tests:
    test_inputs = test['inputs']
    test_outputs = test['outputs']

    for test_input in test_inputs:
      name = test_input['name']
      if inputtype_by_name.get(name) is not None:
        if inputtype_by_name.get(name) == 'parameter':
          parameter_inputs.append(test_input)
        else:
          variable_inputs.append(test_input)

    if len(parameter_inputs) > 0:
      parameterset = ET.SubElement(parametersSets, 'Parameterset', {
        'name': "p_" + test.get('name'),
        'description': test.get('description', '')
      })
      for parameter_input in parameter_inputs:
        ET.SubElement(parameterset, 'Param', name=parameter_input.get('name')).text = str(parameter_input.get('value'))

    if len(variable_inputs) > 0 or len(test_outputs) > 0:
      testset = ET.SubElement(testSets, 'Testset', {
        'name': "t_" + test.get('name'),
        'description': test.get('description', '')
      })
      if len(parameter_inputs) > 0:
        testset.set('parameterset', "p_" + test.get('name'))
      test = ET.SubElement(testset, 'Test', name=testset.get('name'))
      for variable_input in variable_inputs:
        ET.SubElement(test, 'InputValue', name=variable_input.get('name')).text = str(variable_input.get('value'))
      for test_output in test_outputs:
        ET.SubElement(test, 'OutputValue', name=test_output.get('name')).text = str(test_output.get('value'))


#-----------------------------------------------------------------
# Function to convert JSON data to XML format
# This function takes a file path and JSON data, then converts the data into a Crop2ML-friendly XML format.
#-----------------------------------------------------------------
def convert_composite(file_path, json_metadata, XML_units):
  metadata = json_metadata['metadata']
  link_data = json_metadata.get('links', [])
  name = Path(file_path).stem

  # Create XML tree
  root = ET.Element('ModelComposition', {
    "name": name,
    "id": name + "." + name,
    "version": metadata['Model version'],
    "timestep": "1",
  })

  # Description section
  desc = ET.SubElement(root, 'Description')
  ET.SubElement(desc, 'Title').text = name
  ET.SubElement(desc, 'Authors').text = metadata.get('Authors', '')
  ET.SubElement(desc, 'Institution').text = metadata.get('Institution', '')
  ET.SubElement(desc, 'Reference').text = metadata.get('DOI', '')
  ET.SubElement(desc, 'ExtendedDescription').text = metadata.get('Extended description', '')
  ET.SubElement(desc, 'ShortDescription').text = metadata.get('Short description', '')

  # Composition section
  composition = ET.SubElement(root, 'Composition')
  for unit_path in XML_units:
    unit = extract_text(unit_path)
    root_unit = ET.fromstring(unit)
    ET.SubElement(composition, 'Model', {
      'name': root_unit.attrib.get("name"),
      'id': root_unit.attrib.get("modelid"),
      'filename': f"unit.{root_unit.attrib.get('name')}.xml"
    })

  links_elem = ET.SubElement(composition, 'Links')

  internal_sources = set()
  internal_targets = set()
  for link in link_data:
    internal_sources.add(link['Source variable name'])
    internal_targets.add(link['Target variable name'])

  for unit_path in XML_units:
    unit = extract_text(unit_path)
    root_unit = ET.fromstring(unit)
    unit_name = root_unit.attrib.get("name")
    for input_elem in root_unit.findall('.//Input'):
      #is_state = False
      input_name = input_elem.attrib.get('name')
      #input_type = input_elem.attrib.get('inputtype')
      #if input_type == "variable":
      #  input_category = input_elem.attrib.get('variablecategory')
      #  if input_category == "state":
      #    is_state = True
      #if input_name not in internal_sources and not is_state:
      if input_name not in internal_sources:
        ET.SubElement(links_elem, 'InputLink', {
          'target': f"{unit_name}.{input_name}", 
          'source': input_name
        })

  for link in link_data:
    ET.SubElement(links_elem, 'InternalLink', {
      'target': f"{link['Target model unit']}.{link['Target variable name']}", 
      'source': f"{link['Source model unit']}.{link['Source variable name']}" 
    })
  
  for unit_path in XML_units:
    unit = extract_text(unit_path)
    root_unit = ET.fromstring(unit)
    unit_name = root_unit.attrib.get("name")
    for output_elem in root_unit.findall('.//Output'):
      output_name = output_elem.attrib.get('name')
      ET.SubElement(links_elem, 'OutputLink', {
        'target': output_name,
        'source': f"{unit_name}.{output_name}"
      })

  for link in link_data:
    for unit_path in XML_units:
      if Path(unit_path).stem == f"unit.{link['Target model unit']}":
        unit = extract_text(unit_path)
        root_unit = ET.fromstring(unit)
        for input_elem in root_unit.findall('.//Input'):
          input_name = input_elem.attrib.get('name')
          if input_name == link['Target variable name']:
            input_elem.attrib['variablecategory'] = 'auxiliary'
            with open(unit_path, 'wb') as f:
              f.write(ET.tostring(root_unit, encoding='utf-8'))
  
  return ET.tostring(root, encoding='utf-8')


#-----------------------------------------------------------------
# Function to create Crop2ML XML file from JSON metadata and algorithm
# This function generates a Crop2ML XML file from given JSON metadata and algorithm.
#-----------------------------------------------------------------
def json_to_XML_unit(model_composite, output_path, json_metadata, json_algo, log_file):
  metadata = json_metadata['metadata']
  xml_path = output_path + "/" + "unit." + metadata['Title'] + ".xml"
  xml_data = convert_unit(model_composite, json_metadata, json_algo)
  dom = xml.dom.minidom.parseString(xml_data.decode('utf-8') if isinstance(xml_data, bytes) else xml_data)
  with open(xml_path, 'w', encoding='utf-8') as f:
    f.write(dom.toprettyxml())

  log_comments(json_algo, output_path, log_file, metadata['Title'])

  return xml_path


#-----------------------------------------------------------------
# Function to create Crop2ML XML file from JSON metadata and algorithm
# This function generates a Crop2ML XML file from given JSON metadata and algorithm.
#-----------------------------------------------------------------
def json_to_XML_composite(model_composite, output_path, json_metadata, XML_units, log_file):
  base = Path(model_composite).stem
  xml_path = output_path + "/" + "composition." + base + ".xml"
  xml_data = convert_composite(model_composite, json_metadata, XML_units)
  dom = xml.dom.minidom.parseString(xml_data.decode('utf-8') if isinstance(xml_data, bytes) else xml_data)
  with open(xml_path, 'w', encoding='utf-8') as f:
    f.write(dom.toprettyxml())

  log_comments(json_metadata, output_path, log_file, "Composite model")

  return xml_path


#-----------------------------------------------------------------
# Function to format a Crop2ML XML file
#-----------------------------------------------------------------
def format_xml(xml_data):
  root = ET.fromstring(xml_data)
  
  for input_elem in root.findall('.//Input'):
    attrs = input_elem.attrib
    
    if str(attrs.get('default', '')) == "-" or str(attrs.get('default', '')) == "None":
      attrs['default'] = ""
    if str(attrs.get('max', '')) == "-" or str(attrs.get('max', '')) == "None":
      attrs['max'] = ""
    if str(attrs.get('min', '')) == "-" or str(attrs.get('min', '')) == "None":
      attrs['min'] = ""

    # Validate that default, max, min are numerical when datatype is numerical
    numerical_types = ['DOUBLE', 'DOUBLELIST', 'DOUBLEARRAY', 'INTEGER', 'INTEGERLIST', 'INTEGERARRAY']
    is_numerical_datatype = any(numeric_type in str(attrs.get('datatype', '')).upper() for numeric_type in numerical_types)
    
    if is_numerical_datatype:
      for attr_key in ['default', 'max', 'min', "len"]:
        attr_value = attrs.get(attr_key, '')
        if attr_value and attr_value != '':
          try:
            float(attr_value)
          except ValueError:
            attrs[attr_key] = ""

  for output_elem in root.findall('.//Output'):
    attrs = output_elem.attrib
    
    if str(attrs.get('max', '')) == "-" or str(attrs.get('max', '')) == "None":
      attrs['max'] = ""
    if str(attrs.get('min', '')) == "-" or str(attrs.get('min', '')) == "None":
      attrs['min'] = ""

    # Validate that default, max, min are numerical when datatype is numerical
    numerical_types = ['DOUBLE', 'DOUBLELIST', 'DOUBLEARRAY', 'INTEGER', 'INTEGERLIST', 'INTEGERARRAY']
    is_numerical_datatype = any(numeric_type in str(attrs.get('datatype', '')).upper() for numeric_type in numerical_types)
    
    if is_numerical_datatype:
      for attr_key in ['max', 'min', "len"]:
        attr_value = attrs.get(attr_key, '')
        if attr_value and attr_value != '':
          try:
            float(attr_value)
          except ValueError:
            attrs[attr_key] = ""
  
  # Return the formatted XML as string
  return ET.tostring(root, encoding='utf-8')