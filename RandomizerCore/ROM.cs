using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Z2Randomizer
{

    /*
    Classes Needed:

    * Location - Represents entry from overworld
        * Terrain type
        * Requires Jump
        * Requires Fairy
        * Requires Hammer
        * X Position
        * Y Position
        * Exit Location?
        * Contains Item
        * Enter from right
        * Map Number
    
    * World - Represent a single map
        * Location Tree
            * Root of tree is starting position, children are areas directly accessible from position
        * Lists of locations broken down by terrain
        * Entry / Exit Points

    * Hyrule
        * Does the randomization
        * Checks for Sanity
        * Contains links to all World objects
    
    * Room - Palace only?

    * Palace
        * Pallette
        * Enemies?
        * Rooms
    */
    public class Rom
    {
        private readonly byte[] _romData;

        public Rom(string filename)
        {
            try
            {
                //I wonder if this will dispose weird.. if people have trouble might have to remove using
                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    var br = new BinaryReader(fs, new ASCIIEncoding());
                    _romData = br.ReadBytes(257 * 1024);
                }
            }
            catch
            {
                //MessageBox.Show("Cannot find or read file to dump.");
            }
        }

        public Rom(Stream fileStream)
        {
            try
            {
                var br = new BinaryReader(fileStream, new ASCIIEncoding());
                _romData = br.ReadBytes(257 * 1024);
            }
            catch
            {
                //MessageBox.Show("Cannot find or read file to dump.");
            }
        }

        public byte GetByte(int index)
        {
            return _romData[index];
        }

        public void Put(int index, byte data)
        {
            _romData[index] = data;
        }

        public void Dump(string filename)
        {
            File.WriteAllBytes(filename, _romData);
        }

        public Stream DumpStream()
        {
            var stream = new MemoryStream(_romData);
                return stream;
        }
    }
}
