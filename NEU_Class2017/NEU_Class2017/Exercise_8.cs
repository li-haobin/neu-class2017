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
    class Exercise_8
    {
        public static void Main()
        {
            MOCBA mocba = new MOCBA();

            var candidates = Enumerable.Range(1, 4)
                .SelectMany(i => Enumerable.Range(1, 4)
                .SelectMany(j => Enumerable.Range(1, 4)
                .Select(k => new StochasticSolution(new double[] { i, j, k }))))
                .ToList();

            
        }
        
        static void Evaluate(StochasticSolution sol, bool useORTool = false)
        {
            // 配置决策变量
            var config = GetConfig(useORTool);
            config.Capacities = sol.Decisions.Select(d => (int)d).ToArray();
            // 实例化仿真器
            var state = new MultiServers(config, sol.Observations.Count);
            Simulator sim = new Simulator(state);
            // 一次性运行3个小时
            sim.Run(TimeSpan.FromHours(3));

            // 添加目标值测量结果
            sol.Evaluate(new double[] {
                state.CycleTime_Average.TotalMinutes,
                config.Capacities.Sum(),
            });
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
