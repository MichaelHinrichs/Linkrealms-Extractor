using System;
using System.IO;

namespace Linkrealms_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.GetDirectoryName(args[0]);
            path = path.Remove(path.IndexOf("\\resources"));
            BinaryReader br = new(File.OpenRead(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + ".idx"));

            System.Collections.Generic.List<Subfile> data = new();

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                string name = br.ReadString();
                br.BaseStream.Position += 0xA4 - name.Length - 1;
                data.Add(new()
                {
                    name = name,
                    start = br.ReadInt32(),
                    size = br.ReadInt32()
                });
            }

            foreach (Subfile file in data)
            {
                br.BaseStream.Position = file.start;
                try
                {
                    Directory.CreateDirectory(path + "//" + Path.GetDirectoryName(file.name));
                    using FileStream FS = File.Create(path + "//" + file.name);
                    BinaryWriter bw = new(FS);
                    bw.Write(br.ReadBytes(file.size));
                    bw.Close();
                }
                catch(DirectoryNotFoundException)//"particles.idx" has an empty spot where a file's data should be at 0x84FD4
                {
                    continue;
                }
            }
        }

        class Subfile
        {
            public string name;
            public int start;
            public int size;
        }
    }
}
