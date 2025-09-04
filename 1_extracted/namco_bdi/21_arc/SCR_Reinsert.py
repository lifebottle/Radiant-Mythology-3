import sys
import xml.etree.ElementTree as ET

def reinsert_text(scr_file, xml_file, out_file, encoding="utf-8"):
    # Load XML
    tree = ET.parse(xml_file)
    root = tree.getroot()

    with open(scr_file, "rb") as f:
        data = bytearray(f.read())  # mutable for editing

    for entry in root.findall("Entry"):
        offset_dec = int(entry.find("PointerOffsetDecimal").text)
        jp_text = entry.find("JapaneseText").text or ""
        en_text = entry.find("EnglishText").text or ""

        # Encode Japanese to know original space length
        jp_bytes = jp_text.encode(encoding, errors="ignore")
        en_bytes = en_text.encode(encoding, errors="ignore")

        # Truncate or pad English to fit Japanese size
        if len(en_bytes) > len(jp_bytes):
            print(f"⚠️ Warning: Entry {entry.find('Id').text} - "
                  f"English text too long ({len(en_bytes)} > {len(jp_bytes)}). Truncating.")
            en_bytes = en_bytes[:len(jp_bytes)]
        else:
            en_bytes = en_bytes.ljust(len(jp_bytes), b"\x00")

        # Insert into file data
        for i, b in enumerate(en_bytes):
            if offset_dec + i < len(data):
                data[offset_dec + i] = b

    # Save modified file
    with open(out_file, "wb") as f:
        f.write(data)

    print(f"✅ Reinserted translations into {out_file}")

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: python scr_reinsert.py <input.scr> <input.xml> [output.scr]")
        sys.exit(1)

    scr_in = sys.argv[1]
    xml_in = sys.argv[2]
    scr_out = sys.argv[3] if len(sys.argv) > 3 else scr_in.replace(".scr", "_patched.scr")

    reinsert_text(scr_in, xml_in, scr_out)
