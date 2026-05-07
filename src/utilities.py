import os
from pathlib import Path

#-----------------------------------------------------------------
# Function to extract text from a file
# This function reads the content of a file and returns it as a string.
#-----------------------------------------------------------------
def extract_text(file_path):
  with open(file_path, "r", encoding="utf-8", errors="replace") as file:
    return file.read()


#-----------------------------------------------------------------
# Function to extract the file extension from a file path
#-----------------------------------------------------------------
def extract_extension(file_path):
  fichier = Path(file_path)
  return fichier.suffix


#-----------------------------------------------------------------
# Function to determine the programming language based on file extension
# This function checks the file extension and returns the corresponding language name.
#-----------------------------------------------------------------
def language(extension):
  if extension == ".py":
    return "Python"
  elif extension == ".java":
    return "Java"
  elif extension == ".cs":
    return "C#"
  elif extension == ".cpp":
    return "C++"
  elif extension == ".for":
    return "Fortran"
  elif extension == ".f90":
    return "Fortran90"
  elif extension == ".pyx":
    return "Cython"
  elif extension == ".vba":
    return "Visual Basic"
  else:
    return "Unknown"

#-----------------------------------------------------------------
# Function to check if all files exist and are correctly read
#-----------------------------------------------------------------
def check_files(*args, comp, config_files, log_file, output_folder):
  # Flatten incoming args (they may be individual file paths or lists/tuples of paths)
  files_to_check = []
  for a in args:
    if isinstance(a, (list, tuple)):
      files_to_check.extend(a)
    else:
      files_to_check.append(a)

  # Validate each provided file path
  for file_path in files_to_check:
    if not os.path.exists(file_path):
      raise FileNotFoundError(f"File {file_path} does not exist")
    try:
      with open(file_path, 'r', encoding='utf-8', errors="replace") as f:
        f.read()
    except Exception as e:
      raise ValueError(f"Cannot read file {file_path}: {e}")

  # Validate composite file if provided
  if comp is not None:
    if not os.path.exists(comp):
      raise FileNotFoundError(f"File {comp} does not exist")
    try:
      with open(comp, 'r', encoding='utf-8') as f:
        f.read()
    except Exception as e:
      raise ValueError(f"Cannot read file {comp}: {e}")
  
  for file_path in config_files:
    if not os.path.exists(file_path):
      raise FileNotFoundError(f"Configuration file {file_path} does not exist")
    try:
      with open(file_path, 'r', encoding='utf-8') as f:
        f.read()
    except Exception as e:
      raise ValueError(f"Cannot read configuration file {file_path}: {e}")
    
  # Create or clear log file
  log_file_path = os.path.join(output_folder, log_file)
  with open(log_file_path, 'w', encoding='utf-8') as f:
    f.write("")
    pass


#-----------------------------------------------------------------
# Function to log comments from the JSON metadata into a log file
#-----------------------------------------------------------------
def log_comments(json, output_path, log_file, model_name):
  comments = json.get('comments', [])
  log_path = os.path.join(output_path, log_file)
  with open(log_path, 'a', encoding='utf-8') as lf:
    lf.write(f"--- {model_name} ---\n")
    for c in comments:
      if isinstance(c, dict):
        comment_text = c.get('comment', str(c))
        lf.write(comment_text + "\n")
      else:
        lf.write(str(c) + "\n")
    lf.write("\n")