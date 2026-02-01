import bump
import os
import sys

if __name__ == '__main__':
  version = bump.get_version()
  os.system(f'gh release create v{version} --generate-notes ./build/*.zip --notes "{' '.join([note for note in sys.argv[2:]])}"')
