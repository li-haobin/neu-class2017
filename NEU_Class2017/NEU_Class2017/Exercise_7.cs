using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools;
using Google.OrTools.LinearSolver;
using O2DESNet;
using O2DESNet.Optimizer;

namespace NEU_Class2017
{
    class Exercise_7
    {
        public static void Main()
        {

            MoCompass moCompass = new MoCompass(new ConvexSet(dimension: 3, globalLb: 1));

            int i = 0;
            while (true)
            {
                Console.WriteLine("Evaluating...");
                var samples = moCompass.Sample(10, decimals: 0);
                if (samples.Length == 0) break;
                foreach (var decs in samples)
                {
                    var sol = new StochasticSolution(decs);
                    Evaluate(sol);
                    moCompass.Enter(sol);
                }

                Console.Clear();
                Console.WriteLine("Iteration: {0}", i++);
                foreach (var sol in moCompass.ParetoSet.OrderBy(s => s.Objectives[0])) Console.WriteLine(sol);
                Console.ReadKey();
            }
            Console.WriteLine("End.");
            Console.ReadKey();
        }

        public static void Evaluate(StochasticSolution sol, int nReps = 1)
        {
            // 配置决策变量
            var config = GetConfig(useORTool: false);
            config.Capacities = sol.Decisions.Select(d => (int)d).ToArray();
            for (int i = 0; i < nReps; i++)
            {
                // 实例化仿真器
                var state = new MultiServers(config, sol.Observations.Count);
                Simulator sim = new Simulator(state);
                // 预热1小时，一次性运行3个小时
                sim.WarmUp(TimeSpan.FromHours(1));
                sim.Run(TimeSpan.FromHours(3));

                // 添加目标值测量结果
                sol.Evaluate(new double[] {
                    state.CycleTime_Average.TotalMinutes,
                    config.Capacities.Sum(),
                });
            }
        }

        static MultiServers.Statics GetConfig(bool useORTool)
        {
            return new MultiServers.Statics
            {
                HourlyArrivalRates = new double[] { 10, 20, 30, 40 },
                HourlyServiceRates = new double[3, 4] {
                    { 25, 34, 28, 34 },
                    { 22, 30, 25, 40 },
                    { 19, 25, 30, 30 }
                },
                UseORTool = useORTool,
            };
        }
    }

}
