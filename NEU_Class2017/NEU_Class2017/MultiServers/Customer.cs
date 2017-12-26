using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet;

namespace NEU_Class2017
{
    public class Customer : Component<Customer.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public int Type { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        public DateTime ArriveTime { get; private set; }
        public DateTime StartServiceTime { get; private set; }
        public DateTime DepartTime { get; private set; }
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal Customer This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        private class ArriveEvent : InternalEvent { public override void Invoke() { This.ArriveTime = ClockTime; } }
        private class StartServiceEvent : InternalEvent { public override void Invoke() { This.StartServiceTime = ClockTime; } }
        private class DepartEvent : InternalEvent { public override void Invoke() { This.DepartTime = ClockTime; } }
        #endregion

        #region Input Events - Getters
        /***************************************************************/
        /* Methods returning an InternalEvent as O2DESNet.Event,       */
        /* with parameters for the objects to be passed in.            */
        /* Note that the InternalEvent shall always carry This = this. */
        /***************************************************************/
        public Event Arrive() { return new ArriveEvent { This = this }; }
        public Event StartService() { return new StartServiceEvent { This = this }; }
        public Event Depart() { return new DepartEvent { This = this }; }
        #endregion

        #region Output Events - Reference to Getters
        /***********************************************************************/
        /* List of functions that maps outgoing objects to an external event.  */
        /* Note that the mapping is specified only in external structure.      */
        /***********************************************************************/
        //public List<Func<TLoad, Event>> OnOutput { get; private set; } = new List<Func<TLoad, Event>>();
        #endregion

        public Customer(int type) : base(new Statics { Type = type }) { }
        public Customer(Statics config, int seed, string tag = null) : base(config, seed, tag) { }

        public override void WarmedUp(DateTime clockTime)
        {
            base.WarmedUp(clockTime);
        }

        public override void WriteToConsole(DateTime? clockTime = null)
        {
            base.WriteToConsole(clockTime);
        }
    }
}
