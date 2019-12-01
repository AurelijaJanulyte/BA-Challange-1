using System;
using System.IO;
using System.Collections;

namespace PirmaUzduotisBA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input directory path for searching comments");
            string path = Console.ReadLine();

            if (!Directory.Exists(path))
            {
                Console.WriteLine("This directory not exists");
            }
            else
            {
                Console.WriteLine("Input results file path");
                string resultsFile = Console.ReadLine();
                ProcessDirectory(path, resultsFile);
            }
        }

        public static void ProcessDirectory(string targetDirectory, string resultsFile)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                string comments = ExtractComments(fileName);
                File.AppendAllText(resultsFile, comments);
            }

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory,resultsFile);
            }
        }

        public static string ExtractComments(string filename)
        {
            string text = File.ReadAllText(filename);

            text = text.Replace("\\\"", "");

            string comments = "";
            while (text.Length > 0)
            {
                int stringPos = text.IndexOf("\"");
                int singleLinePos = text.IndexOf("//");
                int multiLinePos = text.IndexOf("/*");

                if ((stringPos < 0) &&
                    (singleLinePos < 0) &&
                    (multiLinePos < 0)) break;

                if (stringPos < 0) stringPos = text.Length;
                if (singleLinePos < 0) singleLinePos = text.Length;
                if (multiLinePos < 0) multiLinePos = text.Length;

                if ((stringPos < singleLinePos) &&
                    (stringPos < multiLinePos))
                {
                    int stringEndPos = text.IndexOf("\"", stringPos + 1);

                    if (stringEndPos < 0)
                    {
                        text = "";
                    }
                    else
                    {
                        text = text.Substring(stringEndPos + 1);
                    }
                }
                else if (singleLinePos < multiLinePos)
                {
                    int singleLineEndPos =
                        text.IndexOf("\n", singleLinePos + 1);

                    if (singleLineEndPos < 0)
                    {
                        comments +=
                            text.Substring(singleLinePos) + "\n";
                        text = "";
                    }
                    else
                    {
                        comments += text.Substring(
                            singleLinePos, singleLineEndPos - singleLinePos) + "\n";
                        text = text.Substring(singleLineEndPos + 1);
                    }
                }
                else
                {
                    int multiLineEndPos = text.IndexOf(
                        "*/", multiLinePos + 2);

                    if (multiLineEndPos < 0)
                    {
                        comments +=
                            text.Substring(multiLinePos) + "\n";
                        text = "";
                    }
                    else
                    {
                        comments += text.Substring(multiLinePos,
                            multiLineEndPos - multiLinePos + 2) + "\n";
                        text = text.Substring(multiLineEndPos + 2);
                    }
                }
            }

            return comments;
        }
    }
}
