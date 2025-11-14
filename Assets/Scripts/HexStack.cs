using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace
{
    public class HexStack
    {
        public List<StackItem> Items = new ();

        public HexStack(List<StackItem> items)
        {
            Items = items;
        }
        
        public void Add(StackItem stackItem)
        {
            Items.Add(stackItem);
        }

        public StackItem Pop()
        {
            StackItem stackItem = Items.Last();
            
            Items.RemoveAt(Items.Count - 1);
            
            return stackItem;
        }
    }
}