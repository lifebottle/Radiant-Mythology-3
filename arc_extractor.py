import os
import sys
import glob
import gzip
import fnmatch
import struct
import tempfile
from io import BytesIO

GZ_HEADER = b"\x1F\x8B"

def parse(data, out_path="."):
    # short cut
    def _G(offset, fmt):
        fmt_size = struct.calcsize(fmt)
        return struct.unpack(fmt, data[offset: offset + fmt_size])
        
    # check header
    MAGIC_CODE = b"EZBIND\x00\x00"
    magic_code = data[0: len(MAGIC_CODE)]
    if magic_code != MAGIC_CODE:
        raise Exception("unknown file format!")
    
    file_count, = _G(0x8, "<I")
    
    # file info blocks
    for base_offset in range(0x10, 0x10 + file_count * 0x10, 0x10):
        # filename offset
        filename_offset, = _G(base_offset + 0x0, "<I")
        # filename
        end_offset = data.index(b"\x00", filename_offset)
        filename = data[filename_offset: end_offset].decode("utf-8")
        
        # There is a "*.tmp" special file which can't be created in file system.
        if filename == "*.tmp":
            filename = "dummy.tmp"
            
        out_file = os.path.join(out_path, filename)
        
        if os.path.exists(out_file):
            continue
        
        # file size        
        file_size, = _G(base_offset + 0x4, "<I")
        
        # file offset
        file_offset, = _G(base_offset + 0x8, "<I")
        
        print("name = %s, offset = 0x%x, size = 0x%x" % (filename, file_offset, file_size))
        
        file_content = data[file_offset: file_offset + file_size]
        file_content = decompress(file_content)
        
        with open(out_file, "wb") as fp:
            fp.write(file_content)

def decompress(data):
    while data.startswith(GZ_HEADER):
        string_io = BytesIO(data)
        with gzip.GzipFile(mode="rb", fileobj=string_io) as fp:
            data = fp.read()
    return data

# extract a single archive file.
def do_file(arc_file, out_path):
    print("do file %s %s" % (arc_file, out_path))
    
    with open(arc_file, "rb") as fp:
        data = fp.read()
    
    data = decompress(data)
    
    if not os.path.exists(out_path):
        os.makedirs(out_path)
    
    parse(data, out_path)

if __name__ == '__main__':
    if len(sys.argv) == 1:
        print("No input file(*.arc)!")
    else:
        arc_file = sys.argv[1]
        if len(sys.argv) > 2:
            out_path = sys.argv[2]
        else:
            out_path = os.path.splitext(arc_file)[0] + "_arc"
        
        if os.path.isfile(arc_file):
            do_file(arc_file, out_path)
        elif os.path.isdir(arc_file):
            top_folder = arc_file
            out_top_folder = out_path
            
            for root, dirnames, filenames in os.walk(top_folder):
                for filename in fnmatch.filter(filenames, "*.arc"):
                    arc_file = os.path.join(root, filename)
                    arc_file_relative = os.path.relpath(arc_file, top_folder)
                    out_path_relative = os.path.splitext(arc_file_relative)[0] + "_arc"
                    out_path = os.path.join(out_top_folder, out_path_relative)
                    do_file(arc_file, out_path)
        else:
            print("can't extract %s" % arc_file)
