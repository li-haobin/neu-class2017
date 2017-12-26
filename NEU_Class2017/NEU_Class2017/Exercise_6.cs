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
            //// 实例化优化环境
            //Solver solver = Solver.CreateSolver(name: "MySolver", type: "GLOP_LINEAR_PROGRAMMING");

            //// 定义决策变量
            //Variable x = solver.MakeNumVar(0.0, double.PositiveInfinity, "x");
            //Variable y = solver.MakeNumVar(0.0, double.PositiveInfinity, "y");

            //// 定义约束条件
            //solver.Add(x + 2 * y <= 14);
            //solver.Add(3 * x - y >= 0);
            //solver.Add(x - y <= 2);
            
            //solver.Maximize(3 * x + 4 * y); // 定义目标函数        
            //solver.Solve(); // 求解

            //// 显示优化结果
            //Console.WriteLine("x = {0}", x.SolutionValue()); // 显示决策变量值
            //Console.WriteLine("y = {0}", y.SolutionValue()); // 显示决策变量值
            //Console.WriteLine("Obj = {0}", solver.Objective().Value()); // 显示目标值

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
            };

            // 实例化动态属性 -> 实例化仿真器
            Simulator sim = new Simulator(new MultiServers(config, 0));
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
