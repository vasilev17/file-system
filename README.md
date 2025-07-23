
<div align="center">
  
<h1>💾📁 File System — File & Directory Manager</h1>
  
<p>This project is a single file virtual file system implemented in C#. It stores an entire directory tree plus file contents in one backing file <code>fileSystem.txt</code> and exposes a mini shell with Unix-like commands for interaction.</p>

<div>
  <img src="https://img.shields.io/badge/Solo-Project-gray?logo=codecrafters&labelColor=cyan&logoColor=%23323232" style="height: 30px; width: auto;">
</div>

</div>

---

## 📦 Features

- **Single-File Persistence** – The entire virtual file-system tree, including directories, file metadata and raw contents is serialised into one human-readable backing file (fileSystem.txt), keeping the project self-contained and easy to reset or share.

- **Mini Shell** – Built-in commands (mkdir, ls, cd, write, cat, cp, import, export, exit, …) give a Unix-like workflow without leaving the program.

- **Cross-Platform & Zero-Dependency** – Targets .NET 5+ and needs nothing beyond the standard runtime, so it runs the same on Windows, macOS, and Linux.

- **Session Durability** – The working directory and files survive application restarts.

- **Real OS and Virtual FS Bridging** – Import and export commands let the user copy files back and forth between the real OS and the simulated FS.

- **Path-Safe Navigation** – Internally normalises and validates paths to keep the user from escaping above the virtual root, protecting the host machine.

- **Easy to Expand** – Commands are implemented with a simple switch dispatcher; adding new operations (e.g., rm, move, find) is straightforward.

---

## 📋 Command List
| Command                                        | Example                                      | Effect                                 |
|-----------------------------------------------|----------------------------------------------|-----------------------------------------|
| `mkdir <dir>`                                  | `mkdir docs`                                 | Create sub-directory in current dir     |
| `cd <dir>`                                     | `cd docs`                                    | Change current virtual directory        |
| `ls <dir>`                                     | `ls base`                                    | List contents (dirs end with `\`)       |
| `write <file> ["text"]`                        | `write notes.txt "hello"`                    | Create file (or append with `+append`)  |
| `cat <file>`                                   | `cat notes.txt`                              | Print file contents                     |
| `cp <file> <dir>`                              | `cp notes.txt backup`                        | Copy file to another virtual dir        |
| `import "<host\path>" <dir> [+append "extra"]` | `import "C:\readme.md" docs`                 | Pull real OS file into container        |
| `export <vfile> "<host\path>"`                 | `export notes.txt "D:\dump\notes.txt"`       | Write virtual file out to host FS       |
| `exit`                                         | –                                            | Quit                                    |


---

## 🏁 Getting Started

### Prerequisites
- **.NET ≥ 5.0**

### Clone & Run
```bash
git clone https://github.com/vasilev17/file-system.git
cd file-system
dotnet run --project "File System/File System.csproj"
```

---

## 🎬 Showcase

https://github.com/user-attachments/assets/05c0b55e-fc05-4976-9f30-65ab6bdd08cd


