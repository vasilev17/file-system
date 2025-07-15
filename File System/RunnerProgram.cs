using System;
using System.IO;

namespace File_System
{
    class RunnerProgram
    {

        public static string CustomPathCombine(string path1, string path2)
        {
            if (path2[0] == '\\')
                return path1 + path2;

            else
                return path1 + '\\' + path2;
        }

        public static bool CustomContains(string source, string subString)
        {
            for (int i = 0; i <= source.Length - subString.Length; i++)
            {
                bool match = true;

                for (int j = 0; j < subString.Length; j++)
                {
                    if (source[i + j] != subString[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return true;
            }

            return false;
        }

        internal static string[] CustomSplit(string input, char delimiter)
        {
            int count = CountOccurrences(input, delimiter) + 1;
            string[] result = new string[count];

            int startIndex = 0;
            int resultIndex = 0;

            for (int i = 0; i < input.Length; i++)
                if (input[i] == delimiter)
                {
                    result[resultIndex++] = input.Substring(startIndex, i - startIndex);
                    startIndex = i + 1;
                }


            result[resultIndex] = GetStringSlice(input, startIndex);

            return result;
        }

        internal static string GetStringSlice(string input, int startIndex, int length = -1)
        {
            if (length == -1)
            {
                length = input.Length - startIndex;
            }

            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = input[startIndex + i];
            }

            return new string(result);
        }

        internal static int CountOccurrences(string input, char target)
        {
            int count = 0;

            foreach (char c in input)
                if (c == target)
                    count++;

            return count;
        }

        public static string CustomTrim(string input)
        {
            int startIndex = 0;
            int endIndex = input.Length - 1;

            while (startIndex <= endIndex && input[startIndex] == ' ')
                startIndex++;


            while (endIndex >= startIndex && input[endIndex] == ' ')
                endIndex--;


            if (startIndex > endIndex)
                return string.Empty;

            else
            {
                int length = endIndex - startIndex + 1;
                char[] trimmedArray = new char[length];

                for (int i = 0; i < length; i++)
                    trimmedArray[i] = input[startIndex + i];

                return new string(trimmedArray);
            }
        }

        internal static string StringToBinary(string input)
        {
            char[] binaryResult = new char[input.Length * 8];

            int index = 0;

            foreach (char c in input)
                for (int i = 7; i >= 0; i--)
                    binaryResult[index++] = ((c & (1 << i)) != 0) ? '1' : '0';

            return new string(binaryResult, 0, index);
        }

        internal static string BinaryToString(string binaryInput)
        {
            int length = binaryInput.Length;
            char[] result = new char[length / 8];

            for (int i = 0; i < length; i += 8)
            {
                int decimalValue = 0;

                for (int j = 0; j < 8; j++)
                {
                    int binaryDigitIndex = i + j;
                    if (binaryDigitIndex < length && binaryInput[binaryDigitIndex] == '1')
                    {
                        decimalValue += (1 << (7 - j));
                    }
                }

                result[i / 8] = (char)decimalValue;
            }

            return new string(result);
        }

        public static string CustomTrimEnd(string input, char trimChar)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            int length = input.Length;
            int trimLength = 0;

            for (int i = length - 1; i >= 0; i--)
            {
                if (input[i] != trimChar)
                {
                    break;
                }

                trimLength++;
            }

            return trimLength == 0 ? input : input.Substring(0, length - trimLength);
        }

        static void Main(string[] args)
        {

            string pathName = @".\fileSystem.txt";

            FileStream fs = new FileStream(pathName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            FileSystem fileSystemSimulator = new FileSystem(fs);

            while (true)
            {
                Console.WriteLine($"\nCurrent dir: {fileSystemSimulator.currentDirectory.Path}\\");
                Console.Write("Enter command: ");
                string userInput = Console.ReadLine();

                if (string.IsNullOrEmpty(userInput))
                    continue; // Skip empty input
                

                string[] commandArgs = CustomSplit(userInput, ' ');
                string command = commandArgs[0].ToLower();

                switch (command)
                {
                    case "mkdir":

                        if (commandArgs.Length == 2)
                            fileSystemSimulator.Mkdir(commandArgs[1]);

                        else
                            Console.WriteLine("Usage: mkdir <directory_name>");

                        break;

                    case "cd":

                        if (commandArgs.Length == 2)
                            fileSystemSimulator.Cd(commandArgs[1]);

                        else
                            Console.WriteLine("Usage: cd <directory_name>");

                        break;

                    case "write":

                        if (commandArgs.Length == 2)
                            fileSystemSimulator.Write(commandArgs[1]);

                        else if (commandArgs.Length >= 4)
                        {
                            string textContent = CustomSplit(userInput, '\"')[1];
                            fileSystemSimulator.Write(commandArgs[1], textContent);
                        }

                        else
                        {
                            Console.WriteLine("Usage: write <file_name>");
                            Console.WriteLine("or");
                            Console.WriteLine("Usage: write <file_name> +append \"<file_content>\"");
                        }
                        break;

                    case "ls":

                        if (commandArgs.Length == 2)
                            fileSystemSimulator.Ls(commandArgs[1]);

                        else
                            Console.WriteLine("Usage: ls <directory_name>");

                        break;

                    case "cat":

                        if (commandArgs.Length == 2)
                            fileSystemSimulator.Cat(commandArgs[1]);

                        else
                            Console.WriteLine("Usage: cat <file_name>");

                        break;

                    case "cp":

                        if (commandArgs.Length == 3)
                            fileSystemSimulator.Cp(commandArgs[1], commandArgs[2]);

                        else
                            Console.WriteLine("Usage: cp <file_name> <directory_name>");

                        break;

                    case "import":

                        if (commandArgs.Length >= 3)
                        {

                            string[] commands = CustomSplit(userInput, '\"');

                            if (!userInput.Contains("+append"))
                                fileSystemSimulator.Import(commands[1], CustomTrim(commandArgs[2]));

                            else
                                fileSystemSimulator.Import(commands[1], CustomTrim(commandArgs[2]), commands[3]);

                        }

                        else
                        {
                            Console.WriteLine("Usage: import \"<file_path>\" <directory_name>");
                            Console.WriteLine("or");
                            Console.WriteLine("Usage: import \"<file_path>\" <directory_name> +append \"<extra_file_content>\"");
                        }

                        break;

                    case "export":

                        if (commandArgs.Length >= 3)
                        {

                            string[] commands = CustomSplit(userInput, '\"');
                            fileSystemSimulator.Export(commandArgs[1], commands[1]);

                        }

                        else
                            Console.WriteLine("Usage: export <file_name> \"<directory_path>\"");


                        break;

                    case "exit":
                        return;

                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
            }

        }

    }



}
