using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl.CommenTool
{
    public class HistoryModel
    {
        public List<MainDevDataContains> Containers_1 { get; set; }
        public List<SliverDataContainer> Containers_2 { get; set; }
        public List<SliverDataContainer> Containers_3 { get; set; }
        public List<SliverDataContainer> Containers_4 { get; set; }
        public List<SliverDataContainer> Containers_5 { get; set; }
        public List<MainDevDataContains> Containers_6 { get; set; }

        public List<int> ListID { get; set; }
        public int Count { get; set; }
        public List<double> X { get; set; }
        public long FileLength { get; set; }

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
            ListID = new List<int>();            
        }
    }
}
