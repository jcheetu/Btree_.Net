

namespace BtreeDataObject.DataStruct.DataObject
{
    public class BTreeNode
    {
        public List<BTreeNode> Children { get; set; }
        public List<Point> Points { get; set; }

        public bool IsLeafNode()
        {
            return Children.Count == 0;
        }
        public BTreeNode()
        {   
            Points = new List<Point>();
            Children = new List<BTreeNode>();
        }
    }
}