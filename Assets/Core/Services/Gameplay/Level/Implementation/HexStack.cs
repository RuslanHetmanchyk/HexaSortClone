using System.Collections.Generic;
using System.Linq;

namespace Core.Services.Gameplay.Level.Implementation
{
    public class HexStack
    {
        public List<HexItem> Items;

        public HexStack(List<HexItem> items)
        {
            Items = items;
        }
        
        public void Add(HexItem hexItem)
        {
            Items.Add(hexItem);
        }

        public HexItem Pop()
        {
            var hexItem = Items.Last();
            
            Items.RemoveAt(Items.Count - 1);
            
            return hexItem;
        }
    }
}