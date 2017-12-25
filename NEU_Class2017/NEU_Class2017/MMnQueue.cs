using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using O2DESNet;

namespace NEU_Class2017
{
    public class MMnQueue : Component<MMnQueue.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public int ServerCapacity { get; set; }
            public double HourlyArrivalRate { get; set; }
            public double HourlyServiceRate { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        public int QueueLength { get; private set; } = 0;
        public int NInService { get; private set; } = 0;
        public HourCounter HourCounter_QueueLength { get; private set; } = new HourCounter();
        public HourCounter HourCounter_NInService { get; private set; } = new HourCounter();
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal MMnQueue This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        private class ArriveEvent : InternalEvent
        {
            public override void Invoke()
            {
                Log("Arrive.");
                This.QueueLength++;
                Execute(new StartServiceEvent { This = This });
                Schedule(new ArriveEvent { This = This }, TimeSpan.FromHours(
                    Exponential.Sample(This.DefaultRS, This.Config.HourlyArrivalRate)));
            }
        }
        private class StartServiceEvent : InternalEvent
        {
            public override void Invoke()
            {
                if (This.QueueLength > 0 && This.NInService < This.Config.ServerCapacity)
                {
                    Log("Start Service.");
                    This.QueueLength--;
                    This.NInService++;
                    Schedule(new DepartEvent { This = This }, TimeSpan.FromHours(
                        Exponential.Sample(This.DefaultRS, This.Config.HourlyServiceRate)));
                }
                This.HourCounter_NInService.ObserveCount(This.NInService, ClockTime);
                This.HourCounter_QueueLength.ObserveCount(This.QueueLength, ClockTime);
            }
        }
        private class DepartEvent : InternalEvent
        {
            public override void Invoke()
            {
                Log("Depart.");
                This.NInService--;
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

        public MMnQueue(Statics config, int seed, string tag = null) : base(config, seed, tag)
        {
            InitEvents.Add(new ArriveEvent { This = this });
        }

        public override void WarmedUp(DateTime clockTime)
        {
            HourCounter_QueueLength.WarmedUp(clockTime);
            HourCounter_NInService.WarmedUp(clockTime);
        }

        public override void WriteToConsole(DateTime? clockTime = null)
        {
            Console.WriteLine("Snapshot - Queue Length: {0}  # in Service: {1}", QueueLength, NInService);
            Console.WriteLine("Average - Queue Length: {0:F4}  # in Service: {1:F4}\n", 
                HourCounter_QueueLength.AverageCount, 
                HourCounter_NInService.AverageCount);
        }
    }
}
