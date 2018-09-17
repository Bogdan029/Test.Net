using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TaskJunior.Net
{
    class CreateLargeFile
    {
        public void Create(string fileName, int length)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fileStream.SetLength(length);
            }
        }
    }
}
