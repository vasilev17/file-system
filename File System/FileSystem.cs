using System;
using System.IO;
using System.Text;

using static File_System.RunnerProgram;

namespace File_System
{

    class FileSystem
    {
        private FileStream containerFile;
        private readonly int baseDirOffset;
        internal DataBlock currentDirectory;
        private readonly BinaryWriter _bw;
        private readonly BinaryReader _br;

        public FileSystem(FileStream fileStream)
        {
            string basedirName = "base";

            containerFile = fileStream;
            _bw = new BinaryWriter(containerFile, Encoding.ASCII, true);
            _br = new BinaryReader(containerFile, Encoding.ASCII, true);
            currentDirectory = new DataBlock(basedirName, 0, DataBlockType.Directory, -1);
            baseDirOffset = 24;

            _bw.Write(currentDirectory.ToString());

        }

        public enum DataBlockType
        {
            Directory,
            File
        }

        public class DataBlock
        {
            public string Path { get; set; } = "";
            public long Position { get; set; }
            public long Size { get; set; }
            public DataBlockType Type { get; set; }
            public long Parent { get; set; }
            public string Content { get; set; }

            public DataBlock(string path, long position, DataBlockType type, long parent, string content = null)
            {
                Path = path;
                Position = position;
                Type = type;
                Parent = parent;
                Content = content;
                Size = this.ToString().Length;

            }

            public override string ToString()
            {
                string dataBlockString;

                dataBlockString = Path + ',' + Position + ',' + Size + ',' + Type + ',' + Parent + ',' + Content;


                return dataBlockString;
            }


        }

        private void CreateDataBlock(string directoryPath, DataBlockType type, long parent, string content = null)
        {
            DataBlock newDB = new DataBlock(directoryPath, containerFile.Length, type, parent);
            containerFile.Position = containerFile.Length;

            if (newDB.Type == DataBlockType.File)
                newDB.Content = content;
            else
                ChangeCurrentDirectory(newDB);

            _bw.Write(newDB.ToString());




        }

        public bool CheckIfDirectoryExists(string directoryName)
        {

            containerFile.Position = 0;

            while (_br.PeekChar() != -1)
            {
                string currentDataBlock = _br.ReadString();

                if (CustomContains(currentDataBlock, directoryName) && CustomContains(currentDataBlock, "Directory"))
                    return true;


            }
            return false;

        }


        private long FindDataBlockPositionByName(string directoryName, DataBlockType type)
        {

            if (directoryName == "base")
                return 0;


            long position = -1;
            string[] directories = CustomSplit(directoryName, '\\');
            string searched = directories[directories.Length - 1];

            containerFile.Position = baseDirOffset;

            while (_br.PeekChar() != -1)
            {
                string currentDataBlock = _br.ReadString();

                if (CustomContains(currentDataBlock, searched) && CustomContains(currentDataBlock, type.ToString()))
                {
                    position = (long)Convert.ToDouble(CustomSplit(currentDataBlock, ',')[1]);
                    break;
                }

            }

            return position;
        }

        private void ChangeCurrentDirectory(DataBlock dir)
        {

            currentDirectory = dir;

        }

        public void Mkdir(string directoryName)
        {

            if (CheckIfDirectoryExists(directoryName))
            {
                Console.WriteLine("Directory with that name already exists!");
            }
            else
            {

                string newDirectoryPath = CustomPathCombine(currentDirectory.Path, directoryName);
                CreateDataBlock(newDirectoryPath, DataBlockType.Directory, FindDataBlockPositionByName(currentDirectory.Path, DataBlockType.Directory));

            }

        }


        public void Cd(string directoryName)
        {
            long position = FindDataBlockPositionByName(directoryName, DataBlockType.Directory);
            if (position == -1)
                Console.WriteLine("A directory with that name wasn't found!");
            else
            {
                containerFile.Position = position;
                currentDirectory = StringToDataBlock(_br.ReadString(), DataBlockType.Directory);

            }

        }

        private DataBlock StringToDataBlock(string dataBlockInfo, DataBlockType type)
        {
            DataBlock savedDataBlock;

            string[] data = CustomSplit(dataBlockInfo, ',');

            savedDataBlock = new DataBlock(data[0], (long)Convert.ToDouble(data[1]), type, (long)Convert.ToDouble(data[4]));

            if (type == DataBlockType.File)
                savedDataBlock.Content = data[5];


            return savedDataBlock;
        }

        public void Write(string fileName, string content = null)
        {


            if (CheckIfFileExists(fileName) != null)
            {
                //Edit existing file
                Console.WriteLine("A file with that name already exists in this directory!");

            }
            else
            {
                //Create new file
                string newDirectoryPath = CustomPathCombine(currentDirectory.Path, fileName);
                CreateDataBlock(newDirectoryPath, DataBlockType.File, FindDataBlockPositionByName(currentDirectory.Path, DataBlockType.Directory), content);


            }

        }


        private DataBlock CheckIfFileExists(string fileName, bool everywhere = false)
        {

            containerFile.Position = 0;

            long newFileParent = FindDataBlockPositionByName(CustomSplit(currentDirectory.Path, '\\')[CustomSplit(currentDirectory.Path, '\\').Length - 1], DataBlockType.Directory);

            while (_br.PeekChar() != -1)
            {
                string currentDataBlock = _br.ReadString();
                long currentParent = (long)Convert.ToDouble(CustomSplit(currentDataBlock, ',')[4]);

                if (everywhere)
                {
                    if (CustomContains(currentDataBlock, fileName) && CustomContains(currentDataBlock, "File"))
                        return StringToDataBlock(currentDataBlock, DataBlockType.File);
                }
                else
                {
                    if (CustomContains(currentDataBlock, fileName) && currentParent == newFileParent && CustomContains(currentDataBlock, "File"))
                        return StringToDataBlock(currentDataBlock, DataBlockType.File);
                }

            }

            return null;

        }

        public void Ls(string directoryName)
        {

            containerFile.Position = 0;

            long searchedPosition = FindDataBlockPositionByName(directoryName, DataBlockType.Directory);

            containerFile.Position = 0;

            while (_br.PeekChar() != -1)
            {
                string currentDataBlock = _br.ReadString();
                string[] data = CustomSplit(currentDataBlock, ',');

                if (searchedPosition != -1)
                {
                    if ((long)Convert.ToDouble(data[4]) == searchedPosition)
                        if (data[3].Equals("Directory"))
                            Console.WriteLine(CustomSplit(data[0], '\\')[CustomSplit(data[0], '\\').Length - 1] + '\\');
                        else
                            Console.WriteLine(CustomSplit(data[0], '\\')[CustomSplit(data[0], '\\').Length - 1]);


                }
            }

        }

        public void Cat(string fileName)
        {

            long searched = FindDataBlockPositionByName(fileName, DataBlockType.File);

            containerFile.Position = searched;

            DataBlock db = StringToDataBlock(_br.ReadString(), DataBlockType.File);

            Console.WriteLine(db.Content);

        }

        public void Cp(string fileName, string directoryName)
        {
            if (CheckIfDirectoryExists(directoryName) && CheckIfFileExists(fileName, true) != null)
            {

                long searchedFile = FindDataBlockPositionByName(fileName, DataBlockType.File);
                long searchedDirectory = FindDataBlockPositionByName(directoryName, DataBlockType.Directory);


                containerFile.Position = searchedDirectory;
                DataBlock newDir = StringToDataBlock(_br.ReadString(), DataBlockType.Directory);

                ChangeCurrentDirectory(newDir);

                if (CheckIfFileExists(fileName, false) == null)
                {

                    containerFile.Position = searchedFile;

                    string[] data = CustomSplit(_br.ReadString(), ',');

                    string fileContent = data[data.Length - 1];


                    string newFilePath = CustomPathCombine(newDir.Path, fileName);
                    DataBlock newFile = new DataBlock(newFilePath, containerFile.Length, DataBlockType.File, searchedDirectory, fileContent);


                    containerFile.Position = containerFile.Length;
                    _bw.Write(newFile.ToString());
                }
                else
                    Console.WriteLine("A file with that name already exists there!");

            }
            else
                Console.WriteLine("The File or Directory specified not found!");
        }

        public void Import(string sourcePath, string destinationDirectoryName, string content = null)
        {
            if (CheckIfDirectoryExists(destinationDirectoryName) && File.Exists(sourcePath))
            {

                string fileContent = File.ReadAllText(sourcePath);

                if (content != null)
                    fileContent += content;

                sourcePath = CustomTrimEnd(sourcePath, '\\');

                string fileName = CustomSplit(sourcePath, '\\')[CustomSplit(sourcePath, '\\').Length - 1];


                long searchedDirectory = FindDataBlockPositionByName(destinationDirectoryName, DataBlockType.Directory);


                containerFile.Position = searchedDirectory;
                DataBlock newDir = StringToDataBlock(_br.ReadString(), DataBlockType.Directory);

                ChangeCurrentDirectory(newDir);

                if (CheckIfFileExists(fileName, false) == null)
                {


                    string newFilePath = CustomPathCombine(newDir.Path, fileName);
                    DataBlock newFile = new DataBlock(newFilePath, containerFile.Length, DataBlockType.File, searchedDirectory, fileContent);


                    containerFile.Position = containerFile.Length;
                    _bw.Write(newFile.ToString());
                }
                else
                    Console.WriteLine("A file with that name already exists there!");

            }
            else
                Console.WriteLine("The File or Directory specified not found!");
        }

        public void Export(string fileName, string destinationDirectoryPath)
        {
            if (CheckIfFileExists(fileName, true) != null)
            {

                long searchedFile = FindDataBlockPositionByName(fileName, DataBlockType.File);

                containerFile.Position = searchedFile;

                string[] data = CustomSplit(_br.ReadString(), ',');


                string newFilePath = CustomPathCombine(destinationDirectoryPath, CustomSplit(data[0], '\\')[CustomSplit(data[0], '\\').Length - 1]);

                File.WriteAllText(newFilePath, data[data.Length - 1]);

            }
            else
                Console.WriteLine("File or destination Directory not found!");
        }

    }
}

