#!/bin/bash

file_path="./hal2b/src/iac/cdk.json"
search_text="profile"

# Use grep to find the line number containing the search text
line_number=$(grep -n "${search_text}" "${file_path}" | cut -d':' -f1)

if [ -n "${line_number}" ]; then
    # Remove the line
    sed -i "${line_number}d" "${file_path}"
    echo "Line containing '${search_text}' removed from ${file_path}"
else
    echo "Text '${search_text}' not found in ${file_path}"
fi
