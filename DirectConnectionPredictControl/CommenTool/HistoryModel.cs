using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    public class HistoryModel
    {
        private int count;
        private long fileLength;
        private List<MainDevDataContains> containers_1;
        private List<SliverDataContainer> containers_2;
        private List<SliverDataContainer> containers_3;
        private List<SliverDataContainer> containers_4;
        private List<SliverDataContainer> containers_5;
        private List<MainDevDataContains> containers_6;
        private List<double> x;

        public List<MainDevDataContains> Containers_1 { get => containers_1; set => containers_1 = value; }
        public List<SliverDataContainer> Containers_2 { get => containers_2; set => containers_2 = value; }
        public List<SliverDataContainer> Containers_3 { get => containers_3; set => containers_3 = value; }
        public List<SliverDataContainer> Containers_4 { get => containers_4; set => containers_4 = value; }
        public List<SliverDataContainer> Containers_5 { get => containers_5; set => containers_5 = value; }
        public List<MainDevDataContains> Containers_6 { get => containers_6; set => containers_6 = value; }
        public int Count { get => count; set => count = value; }
        public List<double> X { get => x; set => x = value; }
        public long FileLength { get => fileLength; set => fileLength = value; }

        public HistoryModel()
        {
            Containers_1 = new List<MainDevDataContains>();
            Containers_2 = new List<SliverDataContainer>();
            Containers_3 = new List<SliverDataContainer>();
            Containers_4 = new List<SliverDataContainer>();
            Containers_5 = new List<SliverDataContainer>();
            Containers_6 = new List<MainDevDataContains>();
            Count = 0;
            X = new List<double>();

            
        }
    }
}
