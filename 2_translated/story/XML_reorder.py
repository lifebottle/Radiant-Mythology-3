import os
import shutil
import xml.etree.ElementTree as ET
from xml.dom import minidom
import re

# Desired order of elements inside <Entry>
ORDER = [
    "PointerOffset",
    "JapaneseText",
    "EnglishText",
    "Notes",
    "Id",
    "Status",
]

def remove_whitespace_nodes(node):
    """Remove extraneous whitespace text nodes from a DOM tree."""
    for child in list(node.childNodes):
        if child.nodeType == child.TEXT_NODE and child.data.strip() == "":
            node.removeChild(child)
        elif child.hasChildNodes():
            remove_whitespace_nodes(child)

def prettify(root):
    """Pretty-print XML with controlled empty tag formatting."""
    rough_string = ET.tostring(root, encoding="utf-8")
    reparsed = minidom.parseString(rough_string)

    # Clean out extra whitespace nodes
    remove_whitespace_nodes(reparsed)

    pretty_xml = reparsed.toprettyxml(indent="  ")

    # <Notes/> stays self-closing
    pretty_xml = pretty_xml.replace("<Notes></Notes>", "<Notes/>")

    # Force all other empty tags into expanded form
    pretty_xml = re.sub(r"<(\w+)\s*/>", r"<\1></\1>", pretty_xml)
    # Restore Notes to self-closing
    pretty_xml = pretty_xml.replace("<Notes></Notes>", "<Notes/>")

    return pretty_xml.encode("utf-8")

def process_xml(file_path, output_path):
    tree = ET.parse(file_path)
    root = tree.getroot()

    for entry in root.findall("Entry"):
        # Rename <PointerOffsetDec> → <PointerOffset>
        for child in list(entry):
            if child.tag.lower() == "pointeroffsetdec":  # case-insensitive
                child.tag = "PointerOffset"

        # Remove <PointerOffsetHex>
        for child in list(entry):
            if child.tag == "PointerOffsetHex":
                entry.remove(child)

        # Reorder according to ORDER
        reordered = []
        existing = {child.tag: child for child in list(entry)}
        for tag in ORDER:
            if tag in existing:
                reordered.append(existing[tag])
            else:
                reordered.append(ET.Element(tag))

        entry.clear()
        for elem in reordered:
            entry.append(elem)

    # Write cleaned XML
    pretty_xml = prettify(root)
    with open(output_path, "wb") as f:
        f.write(pretty_xml)

def main():
    backup_folder = "backup"
    os.makedirs(backup_folder, exist_ok=True)

    processed, skipped = 0, 0
    for filename in os.listdir("."):
        if filename.lower().endswith(".xml"):
            if filename.lower().endswith("_clean.xml"):
                print(f"Skipping already cleaned file: {filename}")
                skipped += 1
                continue

            input_path = os.path.join(".", filename)
            temp_output = os.path.splitext(filename)[0] + "_clean.xml"

            print(f"Processing {input_path} → {filename}")
            process_xml(input_path, temp_output)

            # Move original file to backup folder
            backup_path = os.path.join(backup_folder, filename)
            shutil.move(input_path, backup_path)

            # Rename cleaned file back to original name
            os.rename(temp_output, input_path)

            processed += 1

    print(f"\nDone! {processed} file(s) processed, {skipped} skipped.")
    print(f"Original files moved to '{backup_folder}/'.")

if __name__ == "__main__":
    main()
