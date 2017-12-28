using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Google.OrTools;
using Google.OrTools.LinearSolver;
using O2DESNet;
using O2DESNet.Optimizer;

namespace NEU_Class2017
{
    public class Exercise_9
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

            // 实例化 Scenario
            Scenario scenario = new Scenario { Locations = locs, SKUs = skus };
            scenario.ToXML("1.xml");
            var scenario2 = Scenario.FromXML("1.xml");
        }        

        public class Scenario
        {
            public List<Location> Locations { get; set; }            
            public List<SKU> SKUs { get; set; }

            // 生成 XML 并写入文件
            public void ToXML(string file)
            {
                using (var sw = new StreamWriter(string.Format(file)))
                    new XmlSerializer(typeof(Scenario)).Serialize(XmlWriter.Create(sw), this);
            }
            // 读取 XML 文件还原成类
            public static Scenario FromXML(string file)
            {
                var scenario = (Scenario)new XmlSerializer(typeof(Scenario)).Deserialize(new StreamReader(file));
                // 重新构建双引用
                var locs = scenario.Locations.ToDictionary(loc => loc.Id, loc => loc);
                var skus = scenario.SKUs.ToDictionary(sku => sku.Id, sku => sku);
                foreach (var loc in scenario.Locations)
                {
                    loc.SKUs = loc.SKUs.Select(sku => skus[sku.Id]).ToList();
                    foreach (var sku in loc.SKUs) sku.Storage.Add(loc);
                }
                return scenario;
            }
        }

        public class Location // 定义 Location 类
        {
            public int Id { get; set; }
            public double Capacity { get; set; }
            public List<SKU> SKUs { get; set; } = new List<SKU>(); // 描述与 SKU 的关系
            public double Occupation { get { return SKUs.Sum(sku => sku.Size); } }
            public override string ToString() { return string.Format("Loc_{0}", Id); }
        }

        public class SKU // 定义 SKU 类
        {
            public int Id { get; set; }
            public double Size { get; set; }
            [XmlIgnore]
            public List<Location> Storage { get; set; } = new List<Location>(); // 描述与 Location 的关系                                                                        
            public SKU() { }
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

}
