using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet;

namespace NEU_Class2017.Module
{
    public class Modular_MMnQ : Component<Modular_MMnQ.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public Generator.Statics Generator { get; set; }
            public Queueing.Statics Queueing { get; set; }
            public Server.Statics Server { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        public Generator Generator { get; private set; }
        public Queueing Queueing { get; private set; }
        public Server Server { get; private set; }
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal Modular_MMnQ This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        //private class InitEvent : InternalEvent
        //{
        //    public override void Invoke()
        //    {
        //        throw new NotImplementedException();
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

        public Modular_MMnQ(Statics config, int seed, string tag = null) : base(config, seed, tag)
        {
            // 子模块（动态属性）的初始化
            Generator = new Generator(Config.Generator, DefaultRS.Next());
            Queueing = new Queueing(Config.Queueing, DefaultRS.Next());
            Server = new Server(Config.Server, DefaultRS.Next());

            // 连接子模块之间的输出和输入事件
            Generator.OnArrive.Add(Queueing.Enqueue);
            //Generator.OnArrive.Add(() => Queueing.Enqueue());
            Queueing.OnDequeue.Add(Server.Start);         
            Server.OnStateChg.Add(() => Queueing.UpdToDequeue(Server.Vacancy > 0));

            // 初始化事件
            InitEvents.AddRange(Generator.InitEvents);
        }

        public override void WarmedUp(DateTime clockTime)
        {
            base.WarmedUp(clockTime);
        }

        public override void WriteToConsole(DateTime? clockTime = null)
        {
            Queueing.WriteToConsole(clockTime);
            Server.WriteToConsole(clockTime);
        }
    }
}
