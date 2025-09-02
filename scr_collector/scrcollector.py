import os
import shutil

def collect_scr_files(source_folder, target_folder):
    # Make sure target folder exists
    os.makedirs(target_folder, exist_ok=True)

    count = 0
    for root, dirs, files in os.walk(source_folder):
        for file in files:
            if file.lower().endswith(".scr"):
                src_path = os.path.join(root, file)
                dst_path = os.path.join(target_folder, file)

                # If file with same name already exists, rename it
                if os.path.exists(dst_path):
                    base, ext = os.path.splitext(file)
                    i = 1
                    while os.path.exists(dst_path):
                        dst_path = os.path.join(target_folder, f"{base}_{i}{ext}")
                        i += 1

                shutil.copy2(src_path, dst_path)  # copy so originals remain
                print(f"Copied: {src_path} -> {dst_path}")
                count += 1

    print(f"\n✅ Done! Copied {count} .scr files to {target_folder}")

if __name__ == "__main__":
    source = r"C:\RM3\namco_bdi"     # Parent folder with .scr files in subdirs
    target = r"C:\RM3\SCR_Files"     # Destination folder
    collect_scr_files(source, target)
