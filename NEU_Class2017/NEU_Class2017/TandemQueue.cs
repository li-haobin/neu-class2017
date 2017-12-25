using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using O2DESNet;

namespace NEU_Class2017
{
    public class TandemQueue : Component<TandemQueue.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public double HourlyArrivalRate { get; set; }
            public int ServerCapacity1 { get; set; }
            public double HourlyServiceRate1 { get; set; }
            public int ServerCapacity2 { get; set; }
            public double HourlyServiceRate2 { get; set; }
            public int QueueCapacity2 { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        public int QueueLength1 { get; private set; } = 0;
        public int NInService1 { get; private set; } = 0;
        public int NCompletedInServer1 { get; private set; } = 0;
        public int QueueLength2 { get; private set; } = 0;
        public int NInService2 { get; private set; } = 0;
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal TandemQueue This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        private class Arrive1Event : InternalEvent
        {
            public override void Invoke()
            {
                Log("Arrive 1");
                This.QueueLength1++;
                Execute(new StartService1Event { This = This });
                Schedule(new Arrive1Event { This = This },
                    TimeSpan.FromHours(Exponential.Sample(This.DefaultRS, This.Config.HourlyArrivalRate))
                    );
            }
        }
        private class StartService1Event : InternalEvent
        {
            public override void Invoke()
            {
                if (This.QueueLength1 > 0 && This.NInService1 < This.Config.ServerCapacity1)
                {
                    Log("Start Service 1.");
                    This.QueueLength1--;
                    This.NInService1++;
                    Schedule(new Complete1Event { This = This }, TimeSpan.FromHours(
                        Exponential.Sample(This.DefaultRS, This.Config.HourlyServiceRate1)));
                }

            }
        }
        private class Complete1Event : InternalEvent
        {
            public override void Invoke()
            {
                Log("Complete 1.");
                This.NCompletedInServer1++;
                Execute(new Depart1Event { This = This });

            }
        }
        private class Depart1Event : InternalEvent
        {
            public override void Invoke()
            {
                if (This.NCompletedInServer1 > 0 &&
                    This.QueueLength2 < This.Config.QueueCapacity2)
                {
                    Log("Depart 1.");
                    This.NInService1--;
                    This.NCompletedInServer1--;
                    Execute(new StartService1Event { This = This });
                    Execute(new Arrive2Event { This = This });
                }
            }
        }

        private class Arrive2Event : InternalEvent
        {
            public override void Invoke()
            {
                Log("Arrive 2");
                This.QueueLength2++;
                Execute(new StartService2Event { This = This });
            }
        }
        private class StartService2Event : InternalEvent
        {
            public override void Invoke()
            {
                if (This.QueueLength2 > 0 && This.NInService2 < This.Config.ServerCapacity2)
                {
                    Log("Start Service 2.");
                    This.QueueLength2--;
                    This.NInService2++;
                    Schedule(new Depart2Event { This = This }, TimeSpan.FromHours(
                        Exponential.Sample(This.DefaultRS, This.Config.HourlyServiceRate2)));
                }

            }
        }
        private class Depart2Event : InternalEvent
        {
            public override void Invoke()
            {
                Log("Depart 2.");
                This.NInService2--;
                Execute(new StartService2Event { This = This });
                Execute(new Depart1Event { This = This });
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

        public TandemQueue(Statics config, int seed, string tag = null) : base(config, seed, tag)
        {
            InitEvents.Add(new Arrive1Event { This = this });
        }

        public override void WarmedUp(DateTime clockTime)
        {
            base.WarmedUp(clockTime);
        }
        public override void WriteToConsole(DateTime? clockTime = null)
        {
            Console.WriteLine("Snapshot - Queue Length 1: {0}  # in Service 1: {1}", QueueLength1, NInService1);
            Console.WriteLine("Snapshot - Queue Length 2: {0}  # in Service 2: {1}", QueueLength2, NInService2);
        }
    }
}
