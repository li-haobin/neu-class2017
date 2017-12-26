using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using O2DESNet;

namespace NEU_Class2017.Module
{
    public class Generator : Component<Generator.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public double HourlyArrivalRate { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        //public HourCounter HourCounter { get; private set; }
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal Generator This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        private class ArriveEvent : InternalEvent
        {
            public override void Invoke()
            {
                Log("Arrive");
                foreach (var evnt in This.OnArrive) Execute(evnt()); // 内部事件中连接输出事件
                Schedule(new ArriveEvent { This = This },
                    TimeSpan.FromHours(Exponential.Sample(This.DefaultRS, This.Config.HourlyArrivalRate))
                    );
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
        public List<Func<Event>> OnArrive { get; private set; } = new List<Func<Event>>();
        #endregion

        public Generator(Statics config, int seed, string tag = null) : base(config, seed, tag)
        {
            InitEvents.Add(new ArriveEvent { This = this });
        }

        public override void WarmedUp(DateTime clockTime)
        {
            base.WarmedUp(clockTime);
        }

        public override void WriteToConsole(DateTime? clockTime = null)
        {
            //base.WriteToConsole(clockTime);
        }
    }
}