using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEU_Class2017
{
    class Exercise_3
    {
        public static void Main()
        {
            //Analyze(); // 思考题：为什么选择算 24 点，而不是其它？

            while (true)
            {
                Console.WriteLine("========== GET 24 ==========");
                Console.WriteLine("Input any 4 integers:");
                Console.WriteLine("Or press Enter to Terminate");
                Console.WriteLine("============================");
                try
                {
                    var exprs = GetExpr(Console.ReadLine().Split(' ')
                        .Select(s => Convert.ToInt32(s)).Take(4).ToArray());
                    foreach (var expr in exprs) if (expr.Value == 24) Console.WriteLine(expr);
                    Console.WriteLine();
                }
                catch { break; }
            }
        }

        // 思考题：为什么选择算 24 点，而不是其它？
        static void Analyze()
        {
            Dictionary<int, int> counts = Enumerable.Range(1, 100).ToDictionary(k => k, k => 0);
            Dictionary<int, List<int>> countsCombs = Enumerable.Range(1, 100).ToDictionary(k => k, k => new List<int>());

            for (int n1 = 0; n1 < 14; n1++)
            {
                for (int n2 = 0; n2 < 14; n2++)
                    for (int n3 = 0; n3 < 14; n3++)
                        for (int n4 = 0; n4 < 14; n4++)
                        {
                            var allIntValues = GetExpr(new int[] { n1, n2, n3, n4 }).Select(expr => expr.Value)
                                .Where(v => v <= counts.Keys.Max() && (int)v == v);
                            var hashSet = new HashSet<int>(
                                allIntValues.Select(v => (int)v).Distinct());
                            var countCombs = allIntValues.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
                            foreach (var k in counts.Keys.ToArray())
                            {
                                if (hashSet.Contains(k)) counts[k]++;
                                if (countCombs.ContainsKey(k)) countsCombs[k].Add(countCombs[k]);
                                else countsCombs[k].Add(0);
                            }
                        }
                Console.Clear();
                foreach (var i in counts.OrderBy(c => c.Key))
                {
                    Console.WriteLine("{0}\t{1}\t{2:F4}", i.Key, i.Value, countsCombs[i.Key].Average());
                }
                Console.WriteLine("已完成 {0:F2}%...", 100.0 * (n1 + 1) / 13);
            }
        }

        static Dictionary<Tuple<int, int>, List<Expr>> _exprs2 = new Dictionary<Tuple<int, int>, List<Expr>>();
        static Dictionary<Tuple<int, int, int>, List<Expr>> _exprs3 = new Dictionary<Tuple<int, int, int>, List<Expr>>();

        // 给定运算对象数组，返回所有可能的表达式
        static List<Expr> GetExpr(int[] arr)
        {
            // 初始化返回值
            var exprs = new List<Expr>();

            // 将运算对象切分成左右两部分
            var divs = new List<Tuple<int[], int[]>>();

            // 寻找所有可能的两分方式，存储至 divs
            switch (arr.Length)
            {
                case 1: return new List<Expr> { new Expr { Value = arr[0] } }; // 如果只有一个运算对象直接返回数值表达式
                case 2:
                    divs = new List<Tuple<int[], int[]>> { new Tuple<int[], int[]>(new int[] { arr[0] }, new int[] { arr[1] }) };
                    break;
                case 3:
                    divs = new List<Tuple<int[], int[]>> {
                        new Tuple<int[], int[]>(new int[]{ arr[0] }, new int[]{ arr[1], arr[2] }),
                        new Tuple<int[], int[]>(new int[]{ arr[1] }, new int[]{ arr[0], arr[2] }),
                        new Tuple<int[], int[]>(new int[]{ arr[2] }, new int[]{ arr[0], arr[1] }),
                    };
                    break;
                case 4:
                    divs = new List<Tuple<int[], int[]>> {
                        new Tuple<int[], int[]>(new int[]{ arr[0] }, new int[]{ arr[1], arr[2],  arr[3] }),
                        new Tuple<int[], int[]>(new int[]{ arr[1] }, new int[]{ arr[0], arr[2],  arr[3] }),
                        new Tuple<int[], int[]>(new int[]{ arr[2] }, new int[]{ arr[0], arr[1],  arr[3] }),
                        new Tuple<int[], int[]>(new int[]{ arr[3] }, new int[]{ arr[0], arr[1],  arr[2] }),
                        new Tuple<int[], int[]>(new int[]{ arr[0], arr[1] }, new int[]{ arr[2], arr[3] }),
                        new Tuple<int[], int[]>(new int[]{ arr[0], arr[2] }, new int[]{ arr[1], arr[3] }),
                        new Tuple<int[], int[]>(new int[]{ arr[0], arr[3] }, new int[]{ arr[1], arr[2] }),
                    };
                    break;
            }

            // 对应于每一种两分结果，使用递归方法，生成左右表达式组配对
            var pairs = divs.Select(t => new Tuple<List<Expr>, List<Expr>>(GetExpr(t.Item1), GetExpr(t.Item2))).ToList();
            // 遍历配对
            foreach (Tuple<List<Expr>, List<Expr>> pair in pairs)
            {
                foreach (Expr exp1 in pair.Item1)
                    foreach (Expr exp2 in pair.Item2)
                    {
                        // 遍历运算符，生成表达式
                        exprs.Add(new Expr { Operand1 = exp1, Operand2 = exp2, Operator = "+" });
                        exprs.Add(new Expr { Operand1 = exp1, Operand2 = exp2, Operator = "*" });
                        exprs.Add(new Expr { Operand1 = exp1, Operand2 = exp2, Operator = "-" });
                        exprs.Add(new Expr { Operand1 = exp2, Operand2 = exp1, Operator = "-" });
                        exprs.Add(new Expr { Operand1 = exp1, Operand2 = exp2, Operator = "/" });
                        exprs.Add(new Expr { Operand1 = exp2, Operand2 = exp1, Operator = "/" });
                    }
            }
            // 返回结果
            return exprs;
        }

        public class Expr
        {
            public Expr Operand1 { get; set; }
            public Expr Operand2 { get; set; }
            public string Operator { get; set; }
            private double? _value;
            public double Value
            {
                set { _value = value; } // 赋值
                get // 取值
                {
                    double value = 0;
                    if (_value is null)
                    {
                        switch (Operator)
                        {
                            case "+": value = Operand1.Value + Operand2.Value; break;
                            case "-": value = Operand1.Value - Operand2.Value; break;
                            case "*": value = Operand1.Value * Operand2.Value; break;
                            case "/": value = Operand1.Value / Operand2.Value; break;
                        }
                        if (value < 0) return double.NaN;
                        return value;
                    }
                    return _value.Value;
                }
            }
            public override string ToString()
            {
                if (_value is null) return string.Format("({0}{1}{2})", Operand1, Operator, Operand2);
                return _value.Value.ToString();
            }
        }
    }
}
