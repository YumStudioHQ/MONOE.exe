#pragma once

#include <string>
#include <vector>
#include <cstdlib>
#include <iostream>
#include <filesystem>

#if defined(_WIN32)
  #include <windows.h>
#else
  #include <unistd.h>
  #include <sys/wait.h>
#endif

namespace monoe::exe::os {
  #if defined(_WIN32)
  #include <vector>

  bool launch_godot(const std::string &exe_path,
                    const std::vector<std::string> &args,
                    const std::string &cwd) {
    std::string cmdline = "\"" + exe_path + "\"";
    for (auto &a : args) cmdline += " \"" + a + "\"";

    STARTUPINFOA si{};
    PROCESS_INFORMATION pi{};
    si.cb = sizeof(si);

    std::vector<char> env_block;
    if (!cwd.empty()) {
      std::string env_entry = "_MON_PWD=" + cwd + '\0';
      env_block.insert(env_block.end(), env_entry.begin(), env_entry.end());
    }
    env_block.push_back('\0');

    BOOL ok = CreateProcessA(
      NULL,
      cmdline.data(),
      NULL,
      NULL,
      FALSE,
      0,
      env_block.empty() ? NULL : env_block.data(),
      cwd.empty() ? NULL : cwd.c_str(),
      &si,
      &pi
    );

    if (!ok) {
      std::cerr << "Failed to launch: " << GetLastError() << "\n";
      return false;
    }

    WaitForSingleObject(pi.hProcess, INFINITE);
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
    return true;
  }

  #else
  #include <cstdlib>

  bool launch_godot(const std::string &exe_path,
                    const std::vector<std::string> &args,
                    const std::string &cwd) {
    pid_t pid = fork();
    if (pid == 0) {
      if (!cwd.empty()) {
        if (chdir(cwd.c_str()) != 0) {
          perror("chdir failed");
          _exit(1);
        }

        setenv("_MON_PWD", cwd.c_str(), 1);
      }

      std::vector<char*> argv;
      argv.push_back(const_cast<char*>(exe_path.c_str()));
      for (auto &a : args)
        argv.push_back(const_cast<char*>(a.c_str()));
      argv.push_back(nullptr);

      execv(exe_path.c_str(), argv.data());
      perror("execv failed");
      _exit(1);
    } else if (pid > 0) {
      int status = 0;
      waitpid(pid, &status, 0);
      return WIFEXITED(status) && WEXITSTATUS(status) == 0;
    } else {
      perror("fork failed");
      return false;
    }
  }
#endif
}