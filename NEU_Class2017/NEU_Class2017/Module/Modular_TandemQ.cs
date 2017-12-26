using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet;

namespace NEU_Class2017.Module
{
    public class Modular_TandemQ : Component<Modular_TandemQ.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public Generator.Statics Generator { get; set; }
            public Queueing.Statics Queueing1 { get; set; }
            public Server.Statics Server1 { get; set; }
            public Queueing.Statics Queueing2 { get; set; }
            public Server.Statics Server2 { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        public Generator Generator { get; private set; }
        public Queueing Queueing1 { get; private set; }
        public Server Server1 { get; private set; }
        public Queueing Queueing2 { get; private set; }
        public Server Server2 { get; private set; }
        //public int TotalNCompleted { get; private set; }
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal Modular_TandemQ This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        //private class InitEvent : InternalEvent
        //{
        //    public override void Invoke()
        //    {
        //        Schedule(This.Queueing1.Enqueue(), TimeSpan.FromMinutes(5));
        //    }
        //}

        //private class FinishEvent : InternalEvent
        //{
        //    public override void Invoke()
        //    {
        //        This.TotalNCompleted++;
        //    }
        //}
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

        public Modular_TandemQ(Statics config, int seed, string tag = null) : base(config, seed, tag)
        {
            // 子模块（动态属性）的初始化
            Generator = new Generator(Config.Generator, DefaultRS.Next());
            Queueing1 = new Queueing(Config.Queueing1, DefaultRS.Next());
            Server1 = new Server(Config.Server1, DefaultRS.Next());
            Queueing2 = new Queueing(Config.Queueing2, DefaultRS.Next());
            Server2 = new Server(Config.Server2, DefaultRS.Next());

            // 连接子模块之间的输出和输入事件
            Generator.OnArrive.Add(Queueing1.Enqueue);
            //Generator.OnArrive.Add(() => Queueing.Enqueue());
            Queueing1.OnDequeue.Add(Server1.Start);
            Server1.OnDepart.Add(Queueing2.Enqueue);
            Queueing2.OnDequeue.Add(Server2.Start);

            Server2.OnStateChg.Add(() => Queueing2.UpdToDequeue(Server2.Vacancy > 0));
            Queueing2.OnStateChg.Add(() => Server1.UpdToDepart(Queueing2.Vacancy > 0));
            Server1.OnStateChg.Add(() => Queueing1.UpdToDequeue(Server1.Vacancy > 0));
            //Server1.OnDepart.Add(() => new FinishEvent { This = this });

            // 初始化事件
            InitEvents.AddRange(Generator.InitEvents);
            //InitEvents.Add(new InitEvent { This = this });
        }

        public override void WarmedUp(DateTime clockTime)
        {
            base.WarmedUp(clockTime);
        }

        public override void WriteToConsole(DateTime? clockTime = null)
        {
            Queueing1.WriteToConsole(clockTime);
            Server1.WriteToConsole(clockTime);
            //Console.WriteLine("Total NComplete: {0}", TotalNCompleted);
            Queueing2.WriteToConsole(clockTime);
            Server2.WriteToConsole(clockTime);
        }
    }
}
