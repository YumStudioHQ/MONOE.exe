#pragma once

#include <string>

namespace monoe::exe::os {  
  class Platform {
    public:
    enum class OS {
      Windows,
      Linux,
      MacOS,
      Unknown
    };
    
    enum class Arch {
      x86_32,
      x86_64,
      ARM32,
      ARM64,
      Unknown
    };
    
    Platform() {
      detect_os();
      detect_arch();
    }
    
    OS get_os() const { return os; }
    Arch get_arch() const { return arch; }
    
    std::string os_str() const {
      switch (os) {
        case OS::Windows: return "Windows";
        case OS::Linux:   return "Linux";
        case OS::MacOS:   return "macOS";
        default:          return "UnknownOS";
      }
    }
    
    std::string arch_str() const {
      switch (arch) {
        case Arch::x86_32: return "x86_32";
        case Arch::x86_64: return "x86_64";
        case Arch::ARM32:  return "ARM32";
        case Arch::ARM64:  return "ARM64";
        default:           return "UnknownArch";
      }
    }
    
    std::string get_binary_name() const {
      if (this->os == OS::MacOS) return "monoe.exe";
      else if (this->os == OS::Windows) return "monoe.console.exe";
      return "monoe";
    }

    private:
    OS os = OS::Unknown;
    Arch arch = Arch::Unknown;
    
    void detect_os() {
      #if defined(_WIN32)
      os = OS::Windows;
      #elif defined(__APPLE__) && defined(__MACH__)
      os = OS::MacOS;
      #elif defined(__linux__)
      os = OS::Linux;
      #else
      os = OS::Unknown;
      #endif
    }
    
    void detect_arch() {
      #if defined(__x86_64__) || defined(_M_X64)
      arch = Arch::x86_64;
      #elif defined(__i386__) || defined(_M_IX86)
      arch = Arch::x86_32;
      #elif defined(__aarch64__) || defined(_M_ARM64)
      arch = Arch::ARM64;
      #elif defined(__arm__) || defined(_M_ARM)
      arch = Arch::ARM32;
      #else
      arch = Arch::Unknown;
      #endif
    }
  };
}