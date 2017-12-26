using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using O2DESNet;

namespace NEU_Class2017
{
    public class MultiServers : Component<MultiServers.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public double[] HourlyArrivalRates { get; set; }
            public double[,] HourlyServiceRates { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        public Generator<Customer>[] Generators { get; private set; }
        public Server<Customer>[] Servers { get; private set; }
        public List<Customer> Waiting { get; private set; } = new List<Customer>();
        public List<Customer> Completed { get; private set; } = new List<Customer>();
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal MultiServers This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        private class ArriveEvent : InternalEvent
        {
            internal Customer Customer { get; set; }
            public override void Invoke()
            {
                Log("Arrive");
                This.Waiting.Add(Customer);
                Execute(Customer.Arrive());
                Execute(new StartServiceEvent { This = This });
            }
        }
        private class StartServiceEvent : InternalEvent
        {
            public override void Invoke()
            {
                var idleServers = This.Servers.Where(s => s.Vacancy > 0).ToList();
                if (idleServers.Count > 0 && This.Waiting.Count > 0)
                {
                    Log("Start Service");
                    // 选中首个客户
                    var customer = This.Waiting.First();
                    This.Waiting.RemoveAt(0);
                    // 随机选择任意空闲服务器
                    var server = idleServers[This.DefaultRS.Next(idleServers.Count)];
                    // 记录开始服务时间戳
                    Execute(customer.StartService());
                    // 开始服务
                    Execute(server.Start(customer));
                }
            }
        }
        private class DepartEvent : InternalEvent
        {
            internal Customer Customer { get; set; }
            public override void Invoke()
            {
                Log("Depart");
                This.Completed.Add(Customer);
                Execute(Customer.Depart());
                Execute(new StartServiceEvent { This = This });
            }
        }
        #endregion

        #region Input Events - Getters
        /***************************************************************/
        /* Methods returning an InternalEvent as O2DESNet.Event,       */
        /* with parameters for the objects to be passed in.            */
        /* Note that the InternalEvent shall always carry This = this. */
        /***************************************************************/
        //public Event Input(TLoad load) { return new InternalEvent { This = this, Load = load }; }
        #endregion

        #region Output Events - Reference to Getters
        /***********************************************************************/
        /* List of functions that maps outgoing objects to an external event.  */
        /* Note that the mapping is specified only in external structure.      */
        /***********************************************************************/
        //public List<Func<TLoad, Event>> OnOutput { get; private set; } = new List<Func<TLoad, Event>>();
        #endregion

        public MultiServers(Statics config, int seed, string tag = null) : base(config, seed, tag)
        {
            int nTypes = Config.HourlyArrivalRates.Length;
            int nServers = Config.HourlyServiceRates.GetLength(0);
            // 根据每一种类型客户的到达率创建随即发生器
            Generators = Enumerable.Range(0, nTypes)
                .Select(type => new Generator<Customer>(
                    new Generator<Customer>.Statics {
                        Create = rs => new Customer(type),
                        InterArrivalTime = rs => TimeSpan.FromHours(
                            Exponential.Sample(rs, Config.HourlyArrivalRates[type])),
                        SkipFirst = true
                    }, DefaultRS.Next())).ToArray();
            // 创建服务器，并根据不同类型的客户设置服务事件
            Servers = Enumerable.Range(0, nServers)
                .Select(i => new Server<Customer>(
                    new Server<Customer>.Statics {
                        Capacity = 1,
                        ServiceTime = (cstm, rs) => TimeSpan.FromHours(
                            Exponential.Sample(rs, Config.HourlyServiceRates[i, cstm.Config.Type])),
                    }, DefaultRS.Next())).ToArray();
            // 将发生器输出事件连接至内部事件
            foreach (var g in Generators) g.OnArrive.Add(c => new ArriveEvent { This = this, Customer = c });
            // 将服务器输出事件连接至内部事件
            foreach (var s in Servers) s.OnDepart.Add(c => new DepartEvent { This = this, Customer = c });
            // 初始化事件
            InitEvents.AddRange(Generators.Select(g => g.Start()));
        }

        public override void WarmedUp(DateTime clockTime)
        {
            base.WarmedUp(clockTime);
        }

        public override void WriteToConsole(DateTime? clockTime = null)
        {
            Console.Write("# Waiting: {0}  # in Servers: ", Waiting.Count);
            foreach (var s in Servers) Console.Write("{0} ", s.Occupancy);
            if (Completed.Count > 0)
                Console.WriteLine("Average Cycle Time: {0:F2} mins",
                    Completed.Average(c => (c.DepartTime - c.ArriveTime).TotalMinutes));
            else Console.WriteLine();
        }
    }
}
