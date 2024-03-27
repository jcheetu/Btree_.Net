using BtreeDataObject.DataStruct.DataObject;

namespace BtreeDataObject.DataTypes
{
    public class Btree: IBtree
    {
        public BTreeNode root;
        public const int nodeSize = 100000;
        public Btree()
        {
            root = new BTreeNode();
        }

        public void Insert(Point pin)
        {
            Insert(root, pin);
        }

        private void Insert(BTreeNode node, Point pin)
        {
            if (node.IsLeafNode())
            {
                InsertIntoLeafNode(node, pin);
            }
            else
            {
                InsertIntoInternalNode(node, pin);
            }
        }

        private void InsertIntoLeafNode(BTreeNode node, Point pin)
        {
            if (node.Points.Count < nodeSize)
            {
                AddPointToNode(node, pin);
            }
            else
            {
                SplitLeafNode(node);
                Insert(root, pin);
            }
        }

        private void InsertIntoInternalNode(BTreeNode node, Point pin)
        {
            int childIndex = DetermineChildIndex(node, pin);
            Insert(node.Children[childIndex], pin);
        }

        private void AddPointToNode(BTreeNode node, Point pin)
        {
            node.Points.Add(pin);
        }

        private void SplitLeafNode(BTreeNode node)
        {
            int middleIndex = node.Points.Count / 2;
            Point middlePoint = node.Points[middleIndex];

            BTreeNode left = new();
            BTreeNode right = new();

            MovePointsToNodes(node, left, right, middleIndex);

            UpdateParentNode(node, left, right, middlePoint);
        }

        private void MovePointsToNodes(BTreeNode sourceNode, BTreeNode leftNode, BTreeNode rightNode, int middleIndex)
        {
            for (int i = 0; i < middleIndex; i++)
            {
                AddPointToNode(leftNode, sourceNode.Points[i]);
            }
            for (int i = middleIndex + 1; i < sourceNode.Points.Count; i++)
            {
                AddPointToNode(rightNode, sourceNode.Points[i]);
            }
        }

        private void UpdateParentNode(BTreeNode parentNode, BTreeNode leftChild, BTreeNode rightChild, Point middlePoint)
        {
            if (parentNode == root)
            {
                root = new BTreeNode();
                root.Points.Add(middlePoint);
                root.Children.Add(leftChild);
                root.Children.Add(rightChild);
            }
            else
            {
                BTreeNode parent = FindParent(root, parentNode, middlePoint);
                AddPointToNode(parent, middlePoint);
                parent.Children.Remove(parentNode);
                parent.Children.Add(leftChild);
                parent.Children.Add(rightChild);

                if (parent.Points.Count >= 10)
                {
                    SplitInternalNode(parent);
                }
            }
        }

        private BTreeNode FindParent(BTreeNode currentNode, BTreeNode childNode, Point pin)
        {
            if (currentNode.Children.Contains(childNode)) return currentNode;
            foreach (var node in currentNode.Children)
            {
                if (pin.ChannelId.CompareTo(node.Points[0].ChannelId) < 0)
                    return FindParent(node, childNode, pin);
            }
            return null;
        }

        private void SplitInternalNode(BTreeNode node)
        {
            int middleIndex = node.Points.Count / 2;
            Point middlePoint = node.Points[middleIndex];

            BTreeNode left = new BTreeNode();
            BTreeNode right = new BTreeNode();

            MovePointsToNodes(node, left, right, middleIndex);

            UpdateParentNode(node, left, right, middlePoint);
        }

        private int DetermineChildIndex(BTreeNode node, Point pin)
        {
            int i = 0;
            while (i < node.Points.Count && pin.ChannelId.CompareTo(node.Points[i].ChannelId) > 0)
            {
                i++;
            }
            return i;
        }

        public bool Search(Point pin)
        {
            return Search(root, pin);
        }
        public Point SearchByChannelId(int ChannelId)
        {
            return SearchByChannelId(root, ChannelId);
        }
        #region search by ChannelId
        private Point SearchByChannelId(BTreeNode node, int ChannelId)
        {
            foreach (var pin in node.Points)
            {
                if (pin.ChannelId == ChannelId)
                {
                    Console.WriteLine($"Point with ChannelId '{ChannelId}' found.");
                    return pin;
                }
            }

            if (!node.IsLeafNode())
            {
                int childIndex = FindChildIndex(node.Points, ChannelId);
                if (childIndex >= 0)
                {
                    return SearchByChannelId(node.Children[childIndex], ChannelId);
                }
            }

            Console.WriteLine($"Point with ChannelId '{ChannelId}' not found.");
            return null; // Point not found
        }
        private int FindChildIndex(List<Point> list, int ChannelId)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (ChannelId.CompareTo(list[i].ChannelId) < 0)
                {
                    return i;
                }
            }
            return list.Count;
        }
        #endregion

        #region search by PointByIndex
        public Point SearchByPointByIndex(int targetPointByIndex)
        {
            if (root == null)
            {
                Console.WriteLine("B-tree is empty.");
            }

            return SearchByPointByIndex(root, targetPointByIndex);
        }
        private Point SearchByPointByIndex(BTreeNode node, int targetPointByIndex)
        {
            if (node == null)
            {
                return null;
            }

            Point foundPoint = null; // Initialize foundPoint to null

            // Traverse the rightmost subtree (maximum PointByIndex will be on the right)
            if (!node.IsLeafNode())
            {
                foundPoint = SearchByPointByIndex(node.Children.Last(), targetPointByIndex);
            }

            // Check each Point in the current node
            foreach (var pin in node.Points)
            {
                if (pin.PointByIndex == targetPointByIndex)
                {
                    Console.WriteLine($"Point with PointByIndex '{targetPointByIndex}' found. with ChannelId {pin.ChannelId}");
                    return pin;
                }
            }

            // If a match was found in the right subtree, return it
            if (foundPoint != null)
            {
                return foundPoint;
            }

            // Continue to traverse the remaining subtrees
            if (!node.IsLeafNode())
            {
                for (int i = node.Children.Count - 2; i >= 0; i--)
                {
                    foundPoint = SearchByPointByIndex(node.Children[i], targetPointByIndex);

                    // If a match was found in a subtree, return it
                    if (foundPoint != null)
                    {
                        return foundPoint;
                    }
                }
            }

            // If no match is found in any subtree, foundPoint remains null
            return foundPoint;
        }


        #endregion

        private bool Search(BTreeNode node, Point pin)
        {
            int index = FindIndex(node.Points, pin);
            if (index >= 0)
            {
                Console.WriteLine($"Point with ChannelId '{pin.ChannelId}' found.");
                return true;
            }
            else if (!node.IsLeafNode())
            {
                int childIndex = FindChildIndex(node.Points, pin);
                return Search(node.Children[childIndex], pin);
            }
            else
            {
                Console.WriteLine($"Point with ChannelId '{pin.ChannelId}' not found.");
                return false;
            }
        }

        private int FindIndex(List<Point> list, Point targetPoint)
        {
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ChannelId == targetPoint.ChannelId)
                {
                    return count;
                }
                count++;
            }
            return -1; // Point not found
        }

        private int FindChildIndex(List<Point> list, Point pin)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (pin.ChannelId.CompareTo(list[i].ChannelId) < 0)
                {
                    return i;
                }
            }
            return list.Count;
        }


        #region Traverse
        public List<Point> Traverse()
        {
            List<Point> pinList = new List<Point>();
            if (root == null)
            {
                Console.WriteLine("B-tree is empty.");
                return pinList;
            }

            int maxPointByIndex = int.MinValue;

            Traverse(root, ref maxPointByIndex, ref pinList);
            return pinList;
        }

        private void Traverse(BTreeNode node, ref int maxPointByIndex, ref List<Point> pinList)
        {
            if (node == null)
            {
                return;
            }

            if (!node.IsLeafNode())
            {
                Traverse(node.Children.Last(), ref maxPointByIndex, ref pinList);
            }

            foreach (var pin in node.Points)
            {
                if (pin.PointByIndex > maxPointByIndex)
                {
                    maxPointByIndex = pin.PointByIndex;
                }
                pinList.Add(pin);
            }

            if (!node.IsLeafNode())
            {
                for (int i = node.Children.Count - 2; i >= 0; i--)
                {
                    Traverse(node.Children[i], ref maxPointByIndex, ref pinList);
                }
            }
        }

        #endregion
    }
}