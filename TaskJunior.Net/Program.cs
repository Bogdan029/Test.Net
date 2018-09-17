using System;
using System.Diagnostics;
using System.IO;

namespace TaskJunior.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            LargeFileSort hfs = new LargeFileSort();

            hfs.Split("D:\\BigFile.txt");
            hfs.SortTheChunks();
            hfs.MergeTheChunks();
            Console.WriteLine("Done");

            Console.ReadKey();

        }
    }
}
