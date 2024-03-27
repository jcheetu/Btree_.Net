using System.Collections.Generic;
using BtreeDataObject.DataStruct.DataObject;

namespace BtreeDataObject.DataTypes
{
    public interface IBtree
    {
        void Insert(Point pin);

        bool Search(Point pin);

        Point SearchByChannelId(int ChannelId);

        Point SearchByPointByIndex(int targetPointByIndex);

        List<Point> Traverse();
    }
}
