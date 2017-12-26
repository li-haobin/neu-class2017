using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools;
using Google.OrTools.LinearSolver;
using O2DESNet;

namespace NEU_Class2017
{
    class Exercise_6
    {
        public static void Main()
        {
            Run_MultiServer();
        }

        static void Run_MultiServer()
        {
            // 系统配置（实例化静态属性）
            MultiServers.Statics config = new MultiServers.Statics
            {
                HourlyArrivalRates = new double[] { 10, 20, 30, 40 },
                HourlyServiceRates = new double[3, 4] {
                    { 25, 34, 28, 34 },
                    { 22, 34, 40, 60 },
                    { 19, 25, 28, 45 }
                }, 
                UseORTool = true,
            };

            // 实例化动态属性 -> 实例化仿真器
            Simulator sim = new Simulator(new MultiServers(config, 0));
            // 设置控制台调试显示，即 Log 方法
            sim.State.Display = true;
            // 循环运行
            while (true)
            {
                //sim.Run(1); // 每次运行一个事件
                sim.Run(TimeSpan.FromHours(3)); // 每次运行一个事件
                sim.WriteToConsole(); // 控制台输出系统状态（动态属性）
                Console.ReadKey();
            }
        }
    }

}
