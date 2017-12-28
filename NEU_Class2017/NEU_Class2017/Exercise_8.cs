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
                .ToDictionary(s => s.Decisions, s => s);

            // 初始测量
            foreach (var sol in candidates.Values) Exercise_7.Evaluate(sol, nReps: 5);

            int h = 0;
            while (true)
            {
                int budgetPerSolution = 2;

                // Equal Allocation
                foreach (var sol in candidates.Values) Exercise_7.Evaluate(sol, budgetPerSolution);

                // MOCBA
                //foreach (var alloc in
                //    mocba.Alloc(candidates.Count * budgetPerSolution, candidates.Values))
                //    Exercise_7.Evaluate(candidates[alloc.Key], alloc.Value);

                Console.Clear();
                Console.WriteLine("Iteration: {0}", h++);
                foreach (var sol in Pareto.GetParetoSet(candidates.Values, sol => sol.Objectives)
                    .OrderBy(sol => sol.Objectives[0]))
                    Console.WriteLine(sol);
                Console.WriteLine("Total Budget: {0}", candidates.Values.Sum(sol => sol.Observations.Count));
                Console.WriteLine("APCS-M: {0:F4}", mocba.APCS_M(candidates.Values));
                Console.ReadKey();
            }

        }        
    }

}
