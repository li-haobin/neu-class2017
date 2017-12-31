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
            // 实例化算例分配引擎
            MOCBA mocba = new MOCBA();

            // 创建有限解集
            var candidates = Enumerable.Range(1, 4)
                .SelectMany(i => Enumerable.Range(1, 4)
                .SelectMany(j => Enumerable.Range(1, 4)
                .Select(k => new StochasticSolution(new double[] { i, j, k }))))
                .ToDictionary(s => s.Decisions, s => s);

            // 初始测量
            foreach (var sol in candidates.Values) Exercise_7.Evaluate(sol, nReps: 5);

            int h = 0;
            while (true)
            {
                int budgetPerSolution = 2;

                // Equal Allocation - 平均分配
                //foreach (var sol in candidates.Values) Exercise_7.Evaluate(sol, budgetPerSolution);

                // MOCBA - 依据当前随机解结果分配算力，执行测量
                foreach (var alloc in
                    mocba.Alloc(candidates.Count * budgetPerSolution, candidates.Values))
                    Exercise_7.Evaluate(candidates[alloc.Key], alloc.Value);

                Console.Clear();
                Console.WriteLine("Iteration: {0}", h++);
                // 获取并显示当前帕里多集
                foreach (var sol in Pareto.GetParetoSet(candidates.Values, sol => sol.Objectives)
                    .OrderBy(sol => sol.Objectives[0]))
                    Console.WriteLine(sol);
                Console.WriteLine("Total Budget: {0}", candidates.Values.Sum(sol => sol.Observations.Count));
                // 显示当前近似正确选择概率
                Console.WriteLine("APCS-M: {0:F4}", mocba.APCS_M(candidates.Values));
                Console.ReadKey();
            }

        }        
    }

}
