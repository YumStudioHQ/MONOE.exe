#include <string>
#include <vector>
#include <iostream>
#include <filesystem>

#include "os/launch.hpp"
#include "os/platform.hpp"

namespace monoe::exe {
  void parse_arguments(std::vector<std::string> &args) {
    for (size_t i = 0; i < args.size(); i++) {
      if (args[i] == "-c") {
        args[i] = "--headless";
        args.push_back("-nr");
      }
    }
  }

  int main(const std::filesystem::path &basedir, std::vector<std::string> &args) {
    os::Platform platform = os::Platform();

    std::string appname = platform.get_binary_name();
    auto bin = basedir / appname;
    auto cwd = std::filesystem::current_path().string();

    parse_arguments(args);

    if (!os::launch_godot(bin.string(), args, cwd)) {
      std::cerr << "\n[@monoe/bootloader] monoe.exe did not exit properly!" << std::endl;
      return 1;
    }

    return 0;
  }
}

int main(int argc, char *const argv[]) {
  std::cout << "[@monoe/bootloader] launching monoe.exe ..." << std::endl;

  std::vector<std::string> args = {};
  for (int i = 1; i < argc; i++) {
    args.push_back(argv[i]);
  }

  auto appdir = std::filesystem::path(argv[0]);
  auto basedir = appdir.parent_path();
  return monoe::exe::main(basedir, args);
}