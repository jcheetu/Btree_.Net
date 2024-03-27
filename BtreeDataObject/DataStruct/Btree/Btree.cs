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

        public void Insert(Point point)
        {
            Insert(root, point);
        }

        private void Insert(BTreeNode node, Point point)
        {
            if (node.IsLeafNode())
            {
                InsertIntoLeafNode(node, point);
            }
            else
            {
                InsertIntoInternalNode(node, point);
            }
        }

        private void InsertIntoLeafNode(BTreeNode node, Point point)
        {
            if (node.Points.Count < nodeSize)
            {
                AddPointToNode(node, point);
            }
            else
            {
                SplitLeafNode(node);
                Insert(root, point);
            }
        }

        private void InsertIntoInternalNode(BTreeNode node, Point point)
        {
            int childIndex = DetermineChildIndex(node, point);
            Insert(node.Children[childIndex], point);
        }

        private void AddPointToNode(BTreeNode node, Point point)
        {
            node.Points.Add(point);
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

        private BTreeNode FindParent(BTreeNode currentNode, BTreeNode childNode, Point point)
        {
            if (currentNode.Children.Contains(childNode)) return currentNode;
            foreach (var node in currentNode.Children)
            {
                if (point.ChannelId.CompareTo(node.Points[0].ChannelId) < 0)
                    return FindParent(node, childNode, point);
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

        private int DetermineChildIndex(BTreeNode node, Point point)
        {
            int i = 0;
            while (i < node.Points.Count && point.ChannelId.CompareTo(node.Points[i].ChannelId) > 0)
            {
                i++;
            }
            return i;
        }

        public bool Search(Point point)
        {
            return Search(root, point);
        }
        public Point SearchByChannelId(int ChannelId)
        {
            return SearchByChannelId(root, ChannelId);
        }
        #region search by ChannelId
        private Point SearchByChannelId(BTreeNode node, int ChannelId)
        {
            foreach (var point in node.Points)
            {
                if (point.ChannelId == ChannelId)
                {
                    Console.WriteLine($"Point with ChannelId '{ChannelId}' found.");
                    return point;
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
            foreach (var point in node.Points)
            {
                if (point.PointByIndex == targetPointByIndex)
                {
                    Console.WriteLine($"Point with PointByIndex '{targetPointByIndex}' found. with ChannelId {point.ChannelId}");
                    return point;
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

        private bool Search(BTreeNode node, Point point)
        {
            int index = FindIndex(node.Points, point);
            if (index >= 0)
            {
                Console.WriteLine($"Point with ChannelId '{point.ChannelId}' found.");
                return true;
            }
            else if (!node.IsLeafNode())
            {
                int childIndex = FindChildIndex(node.Points, point);
                return Search(node.Children[childIndex], point);
            }
            else
            {
                Console.WriteLine($"Point with ChannelId '{point.ChannelId}' not found.");
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

        private int FindChildIndex(List<Point> list, Point point)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (point.ChannelId.CompareTo(list[i].ChannelId) < 0)
                {
                    return i;
                }
            }
            return list.Count;
        }


        #region Traverse
        public List<Point> Traverse()
        {
            List<Point> pointList = new List<Point>();
            if (root == null)
            {
                Console.WriteLine("B-tree is empty.");
                return pointList;
            }

            int maxPointByIndex = int.MinValue;

            Traverse(root, ref maxPointByIndex, ref pointList);
            return pointList;
        }

        private void Traverse(BTreeNode node, ref int maxPointByIndex, ref List<Point> pointList)
        {
            if (node == null)
            {
                return;
            }

            if (!node.IsLeafNode())
            {
                Traverse(node.Children.Last(), ref maxPointByIndex, ref pointList);
            }

            foreach (var point in node.Points)
            {
                if (point.PointByIndex > maxPointByIndex)
                {
                    maxPointByIndex = point.PointByIndex;
                }
                pointList.Add(point);
            }

            if (!node.IsLeafNode())
            {
                for (int i = node.Children.Count - 2; i >= 0; i--)
                {
                    Traverse(node.Children[i], ref maxPointByIndex, ref pointList);
                }
            }
        }

        #endregion
    }
}