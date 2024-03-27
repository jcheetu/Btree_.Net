using BtreeDataObject.DataStruct.DataObject;
using BtreeDataObject.DataTypes;

namespace BtreeDataObject
{
    class Program
    {
        // Create points with object type coordinates
        static void Main()
        {

            Point point1 = new(10);
            Point point2 = new(DateTime.Now);
            Point point3 = new(1);
            // Create the Btree
            Btree btree = new();
            btree.Insert(point1);
            btree.Insert(point2);
            btree.Insert(point3);

            btree.SearchByPointByIndex(1);
            btree.Traverse();
        }
    }
}