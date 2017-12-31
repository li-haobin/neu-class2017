using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet;
using O2DESNet.Optimizer;

namespace NEU_Class2017
{
    class Exercise_7
    {
        public static void Main()
        {
            // 实例化搜索算法引擎
            MoCompass moCompass = new MoCompass(
                new ConvexSet(dimension: 3, globalLb: 1, globalUb: 10//,
                //lowerbounds: new double[] { 1, 2, 3, }, upperbounds: new double[] { 10, 20, 30 },
                //constraints: new List<Constraint> {
                //    new ConstraintLE(new double[]{ 1, 1, 0}, 30),
                //    new ConstraintGE(new double[]{ 0, 1, 1}, 10),
                //}
                ));
            
            int i = 0;
            while (true)
            {
                Console.WriteLine("Evaluating...");
                
                var samples = moCompass.Sample(10, decimals: 0); // 通过搜索算法引擎采样
                if (samples.Length == 0) break; // 达到局部收敛
                foreach (var decs in samples)
                {
                    var sol = new StochasticSolution(decs); // 实例化随机解
                    Evaluate(sol); // 计算随机解
                    moCompass.Enter(sol); // 返回随机解
                }

                Console.Clear();
                Console.WriteLine("Iteration: {0}", i++);
                foreach (var sol in moCompass.ParetoSet.OrderBy(s => s.Objectives[0]))
                    Console.WriteLine(sol);
                Console.ReadKey();
            }
            Console.WriteLine("End.");
            Console.ReadKey();
        }

        public static void Evaluate(StochasticSolution sol, int nReps = 1)
        {
            var config = GetConfig(useORTool: false); // 配置环境参数
            config.Capacities = sol.Decisions.Select(d => (int)d).ToArray();  // 配置决策变量
            for (int i = 0; i < nReps; i++)
            {
                var state = new MultiServers(config, sol.Observations.Count); // 实例化动态属性
                Simulator sim = new Simulator(state); // 实例化仿真器            
                sim.WarmUp(TimeSpan.FromHours(1)); // 预热1小时，
                sim.Run(TimeSpan.FromHours(3)); // 运行3个小时

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
