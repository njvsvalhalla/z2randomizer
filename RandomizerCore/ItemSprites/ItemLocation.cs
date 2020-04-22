using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerCore.ItemSprites
{
    public class ItemLocation
    {
        public string Name { get; set; }
        public List<int> StartingAddresses { get; set; }
        public int[] Sprite { get; set; }
    }
}
