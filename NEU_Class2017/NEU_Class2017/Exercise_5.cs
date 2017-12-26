using NEU_Class2017.Module;
using O2DESNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEU_Class2017
{
    class Exercise_5
    {
        public static void Main()
        {
            Run_ModularMMnQ();
            //Run_ModularTandemQ();
        }

        static void Run_ModularMMnQ()
        {
            // 系统配置（实例化静态属性）
            Modular_MMnQ.Statics config = new Modular_MMnQ.Statics
            {
                Generator = new Generator.Statics { HourlyArrivalRate = 30 },
                Queueing = new Queueing.Statics { Capacity = int.MaxValue },
                Server = new Server.Statics { Capacity = 3, HourlyServiceRate = 12 },
            };

            // 实例化动态属性 -> 实例化仿真器
            Simulator sim = new Simulator(new Modular_MMnQ(config, 0));
            // 设置控制台调试显示，即 Log 方法
            sim.State.Display = true;
            // 循环运行
            while (true)
            {
                sim.Run(1); // 每次运行一个事件
                sim.WriteToConsole(); // 控制台输出系统状态（动态属性）
                Console.ReadKey();
            }
        }

        static void Run_ModularTandemQ()
        {
            // 系统配置（实例化静态属性）
            Modular_TandemQ.Statics config = new Modular_TandemQ.Statics
            {
                Generator = new Generator.Statics { HourlyArrivalRate = 30 },
                Queueing1 = new Queueing.Statics { Capacity = int.MaxValue },
                Server1 = new Server.Statics { Capacity = 3, HourlyServiceRate = 12 },
                Queueing2 = new Queueing.Statics { Capacity = 3 },
                Server2 = new Server.Statics { Capacity = 3, HourlyServiceRate = 10 }
            };

            // 实例化动态属性 -> 实例化仿真器
            Simulator sim = new Simulator(new Modular_TandemQ(config, 0));
            // 设置控制台调试显示，即 Log 方法
            sim.State.Display = true;
            // 循环运行
            while (true)
            {
                sim.Run(1); // 每次运行一个事件
                sim.WriteToConsole(); // 控制台输出系统状态（动态属性）
                Console.ReadKey();
            }
        }
    }    
}
