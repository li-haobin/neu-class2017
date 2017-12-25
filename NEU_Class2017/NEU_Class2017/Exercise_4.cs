using O2DESNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEU_Class2017
{
    class Exercise_4
    {
        public static void Main()
        {
            //Run_MMnQueue();
            Run_TandemQueue();
        }

        static void Run_MMnQueue()
        {
            // 系统配置（实例化静态属性）
            MMnQueue.Statics config = new MMnQueue.Statics
            {
                ServerCapacity = 3,
                HourlyArrivalRate = 30,
                HourlyServiceRate = 12,
            };

            // 实例化动态属性 -> 实例化仿真器
            Simulator sim = new Simulator(new MMnQueue(config, 0));
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

        static void Run_TandemQueue()
        {
            // 系统配置（实例化静态属性）
            TandemQueue.Statics config = new TandemQueue.Statics
            {
                ServerCapacity1 = 3,
                HourlyArrivalRate = 30,
                HourlyServiceRate1 = 12,

                ServerCapacity2 = 3,
                HourlyServiceRate2 = 12,

                QueueCapacity2 = 3,
            };

            // 实例化动态属性 -> 实例化仿真器
            Simulator sim = new Simulator(new TandemQueue(config, 0));
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
