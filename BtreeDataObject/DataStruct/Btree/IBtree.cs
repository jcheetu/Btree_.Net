using System.Collections.Generic;
using BtreeDataObject.DataStruct.DataObject;

namespace BtreeDataObject.DataTypes
{
    public interface IBtree
    {
        void Insert(Point point);

        bool Search(Point point);

        Point SearchByChannelId(int ChannelId);

        Point SearchByPointByIndex(int targetPointByIndex);

        List<Point> Traverse();
    }
}
