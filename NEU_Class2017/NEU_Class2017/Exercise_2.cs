using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEU_Class2017
{
    class Exercise_2
    {
        public static void Main()
        {
            // 实例化 Locations
            List<Location> locs = new List<Location>
            {
                new Location{ Id = 0, Capacity = 15 },
                new Location{ Id = 1, Capacity = 15 },
                new Location{ Id = 2, Capacity = 20 },
                new Location{ Id = 3, Capacity = 20 },
                new Location{ Id = 4, Capacity = 20 },
            };

            // 添加 SKU 信息
            List<SKU> skus = new List<SKU>();
            skus.Add(new SKU(0, 0.8, new Location[] { locs[0], locs[0], locs[0], locs[1] }));
            skus.Add(new SKU(1, 0.5, new Location[] { locs[1], locs[2], locs[3], locs[4] }));
            skus.Add(new SKU(2, 1.2, new Location[] { locs[1], locs[2], locs[2], locs[4], locs[4] }));
            skus.Add(new SKU(3, 1.5, new Location[] { locs[3], locs[3], locs[3] }));


            SKU[] q1 = skus.Where(sku => sku.Storage.Distinct().Count() > 3).ToArray();
            Console.Write("Q1: ");
            foreach (SKU sku in q1) Console.Write("{0} ", sku);
            Console.WriteLine();

            Location[] q2 = locs.Where(loc => loc.Occupation > 4).ToArray();
            Console.Write("Q2: ");
            foreach (Location loc in q2) Console.Write("{0} ", loc);
            Console.WriteLine();

            Location[] q3 = locs.OrderByDescending(loc => loc.Capacity - loc.Occupation).ToArray();
            Console.Write("Q3: ");
            foreach (Location loc in q3) Console.Write("{0} ", loc);
            Console.WriteLine();

            Location[] q4 = locs.Where(loc => loc.SKUs.Distinct().Count() == 3).ToArray();
            Console.Write("Q4: ");
            foreach (Location loc in q4) Console.Write("{0} ", loc);
            Console.WriteLine();
        }
    }

    class Location // 定义 Location 类
    {
        public int Id { get; set; }
        public double Capacity { get; set; }
        public List<SKU> SKUs { get; set; } = new List<SKU>(); // 描述与 SKU 的关系
        public double Occupation { get { return SKUs.Sum(sku => sku.Size); } }
        public override string ToString() { return string.Format("Loc_{0}", Id); }
    }

    class SKU // 定义 SKU 类
    {
        public int Id { get; private set; }
        public double Size { get; private set; }
        public List<Location> Storage { get; private set; } = new List<Location>(); // 描述与 Location 的关系                                                                        
        public SKU(int id, double size, Location[] storage) // 使用构造器实现双向引用
        {
            Id = id;
            Size = size;
            Storage = storage.ToList();
            foreach (Location loc in storage) loc.SKUs.Add(this);
        }
        public override string ToString() { return string.Format("SKU_{0}", Id); }
    }
}
