using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using O2DESNet;

namespace NEU_Class2017.Module
{
    public class Server : Component<Server.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public int Capacity { get; set; }
            public double HourlyServiceRate { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        public int NInService { get; private set; } = 0;
        public int NCompletedInServer { get; private set; } = 0;
        public bool ToDepart { get; private set; } = true;
        public int Vacancy { get { return Config.Capacity - NInService; } }
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal Server This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        private class StartEvent : InternalEvent
        {
            public override void Invoke()
            {
                Log("Start");
                This.NInService++;
                Schedule(new CompleteEvent { This = This }, TimeSpan.FromHours(
                    Exponential.Sample(This.DefaultRS, This.Config.HourlyServiceRate)));
                foreach (var evnt in This.OnStateChg) Execute(evnt());
            }
        }
        private class CompleteEvent : InternalEvent
        {
            public override void Invoke()
            {
                Log("Complete");
                This.NCompletedInServer++;
                Execute(new DepartEvent { This = This });
            }
        }
        private class DepartEvent : InternalEvent
        {
            public override void Invoke()
            {
                if (This.NCompletedInServer > 0 && This.ToDepart)
                {
                    Log("Depart");
                    This.NInService--;
                    This.NCompletedInServer--;
                    foreach (var evnt in This.OnDepart) Execute(evnt());
                    foreach (var evnt in This.OnStateChg) Execute(evnt());
                    Schedule(new DepartEvent { This = This }, TimeSpan.FromSeconds(0));
                }
            }
        }
        private class UpdToDepartEvent : InternalEvent
        {
            internal bool ToDepart { get; set; }
            public override void Invoke()
            {
                Log("UpdToDepart");
                This.ToDepart = ToDepart;
                Execute(new DepartEvent { This = This });
            }
        }
        private class TestEvent : InternalEvent
        {
            public override void Invoke()
            {
                Schedule(This.Start(), new DateTime(1, 1, 1, 0, 10, 20));
                Schedule(This.UpdToDepart(false), new DateTime(1, 1, 1, 0, 10, 40));
                Schedule(This.Start(), new DateTime(1, 1, 1, 0, 11, 20));
                Schedule(This.Start(), new DateTime(1, 1, 1, 0, 12, 20));
                Schedule(This.UpdToDepart(true), new DateTime(1, 1, 1, 0, 13, 40));
            }
        }
        #endregion

        #region Input Events - Getters
        /***************************************************************/
        /* Methods returning an InternalEvent as O2DESNet.Event,       */
        /* with parameters for the objects to be passed in.            */
        /* Note that the InternalEvent shall always carry This = this. */
        /***************************************************************/
        public Event Start() { return new StartEvent { This = this }; }
        public Event UpdToDepart(bool toDepart) { return new UpdToDepartEvent { This = this, ToDepart = toDepart }; }
        #endregion

        #region Output Events - Reference to Getters
        /***********************************************************************/
        /* List of functions that maps outgoing objects to an external event.  */
        /* Note that the mapping is specified only in external structure.      */
        /***********************************************************************/
        public List<Func<Event>> OnDepart { get; private set; } = new List<Func<Event>>();
        public List<Func<Event>> OnStateChg { get; private set; } = new List<Func<Event>>();
        #endregion

        public Server(Statics config, int seed, string tag = null) : base(config, seed, tag)
        {
            InitEvents.Add(new TestEvent { This = this });
        }

        public override void WarmedUp(DateTime clockTime)
        {
            base.WarmedUp(clockTime);
        }

        public override void WriteToConsole(DateTime? clockTime = null)
        {
            Console.WriteLine("NInService: {0}  NCompleted: {1}  ToDepart: {2}",
                NInService, NCompletedInServer, ToDepart);
        }
    }
}