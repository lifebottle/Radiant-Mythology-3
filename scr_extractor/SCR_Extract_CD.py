import os
import sys
import re
import xml.etree.ElementTree as ET

def extract_scr(in_file, out_file, encoding="euc-jp"):
    with open(in_file, "rb") as f:
        data = f.read()

    # Regex to find Japanese text sequences
    pattern = re.compile(rb'[\x80-\xFF]{2,}')

    entries = []
    for i, match in enumerate(pattern.finditer(data), start=1):
        raw = match.group()
        try:
            text = raw.decode(encoding)
        except UnicodeDecodeError:
            continue
        if not re.search(r'[\u3040-\u30FF\u4E00-\u9FFF]', text):
            continue
        offset = match.start()
        entries.append((i, offset, text))

    # Build XML
    root = ET.Element("Entries")
    for i, offset, text in entries:
        entry = ET.SubElement(root, "Entry")
        ET.SubElement(entry, "PointerOffsetDec").text = str(offset)
        ET.SubElement(entry, "PointerOffsetHex").text = hex(offset)
        ET.SubElement(entry, "JapaneseText").text = text

        # Force open+close tag by inserting a zero-width space
        eng = ET.SubElement(entry, "EnglishText")
        eng.text = "\u200b"   # invisible placeholder

        ET.SubElement(entry, "Notes")
        ET.SubElement(entry, "Id").text = str(i)
        ET.SubElement(entry, "Status")

    # Write formatted XML
    tree = ET.ElementTree(root)
    ET.indent(tree, space="  ", level=0)
    tree.write(out_file, encoding="utf-8", xml_declaration=True)

    # After writing, replace the placeholder with nothing so you see <EnglishText></EnglishText>
    with open(out_file, "r", encoding="utf-8") as f:
        content = f.read()
    content = content.replace("\u200b", "")
    with open(out_file, "w", encoding="utf-8") as f:
        f.write(content)

    print(f"✅ Extracted {len(entries)} entries from {in_file} -> {out_file}")

def process_folder(folder, encoding="euc-jp"):
    for file in os.listdir(folder):
        if file.lower().endswith(".scr"):
            in_file = os.path.join(folder, file)
            out_file = os.path.splitext(in_file)[0] + ".xml"
            extract_scr(in_file, out_file, encoding=encoding)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        folder = os.getcwd()
        print(f"ℹ️ No folder given, using current folder: {folder}")
    else:
        folder = sys.argv[1]

    if os.path.isdir(folder):
        process_folder(folder, encoding="euc-jp")
    else:
        print("❌ Not a folder:", folder)
