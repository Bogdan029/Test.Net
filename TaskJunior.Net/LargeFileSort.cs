using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TaskJunior.Net
{
    class LargeFileSort
    {
        public void MergeTheChunks()
        {

            string[] paths = Directory.GetFiles("D:\\", "sorted*.dat");
            int chunks = paths.Length; // Number of chunks
            int recordsize = 100; // estimated record size
            int records = 1000000000; // estimated total # records
            int maxusage = 500000000; // max memory usage
            int buffersize = maxusage / chunks; // size in bytes of each buffer
            double recordoverhead = 7.5; // The overhead of using Queue<>
            int bufferlen = (int)(buffersize / recordsize / recordoverhead); // number of records in each buffer

            StreamReader[] readers = new StreamReader[chunks];
            for (int i = 0; i < chunks; i++)
                readers[i] = new StreamReader(paths[i]);

            Queue<string>[] queues = new Queue<string>[chunks];
            for (int i = 0; i < chunks; i++)
                queues[i] = new Queue<string>(bufferlen);

            for (int i = 0; i < chunks; i++)
                LoadQueue(queues[i], readers[i], bufferlen);

            // Merge
            StreamWriter sw = new StreamWriter("D:\\BigFileSorted.txt");
            bool done = false;
            int lowest_index, j, progress = 0;
            string lowest_value;
            while (!done)
            {
                // Report the progress
                if (++progress % 5000 == 0)
                    Console.Write("{0:f2}%   \r",
                      100.0 * progress / records);

                // Find the chunk with the lowest value
                lowest_index = -1;
                lowest_value = "";
                for (j = 0; j < chunks; j++)
                {
                    if (queues[j] != null)
                    {
                        if (lowest_index < 0 || String.CompareOrdinal(queues[j].Peek(), lowest_value) < 0)
                        {
                            lowest_index = j;
                            lowest_value = queues[j].Peek();
                        }
                    }
                }

                if (lowest_index == -1) { done = true; break; }

                sw.WriteLine(lowest_value);

                queues[lowest_index].Dequeue();
                if (queues[lowest_index].Count == 0)
                {
                    LoadQueue(queues[lowest_index], readers[lowest_index], bufferlen);
                    if (queues[lowest_index].Count == 0)
                    {
                        queues[lowest_index] = null;
                    }
                }
            }
            sw.Close();

            // Close and delete the files
            for (int i = 0; i < chunks; i++)
            {
                readers[i].Close();
                File.Delete(paths[i]);
            }

        }

 
        // Loads up to a number of records into a queue

        public void LoadQueue(Queue<string> queue, StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }

        public void SortTheChunks()
        {
            foreach (string path in Directory.GetFiles("D:\\", "split*.dat"))
            {
                Console.WriteLine("{0}     \r", path);

                string[] contents = File.ReadAllLines(path);
                Array.Sort(contents);
                string newpath = path.Replace("split", "sorted");
                File.WriteAllLines(newpath, contents);
                File.Delete(path);
                contents = null;
                GC.Collect();
            }
        }

        public void Split(string file)
        {
            int split_num = 1;
            StreamWriter sw = new StreamWriter(string.Format("d:\\split{0:d5}.dat", split_num));
            long read_line = 0;
            using (StreamReader sr = new StreamReader(file))
            {
                while (sr.Peek() >= 0)
                {
                    // Progress reporting
                    if (++read_line % 5000 == 0)
                        Console.Write("{0:f2}%   \r",
                          100.0 * sr.BaseStream.Position / sr.BaseStream.Length);

                    // Copy a line
                    sw.WriteLine(sr.ReadLine());

                    if (sw.BaseStream.Length > 100000000 && sr.Peek() >= 0)
                    {
                        sw.Close();
                        split_num++;
                        sw = new StreamWriter(string.Format("d:\\split{0:d5}.dat", split_num));
                    }
                }
            }
            sw.Close();
        }

    }
}
