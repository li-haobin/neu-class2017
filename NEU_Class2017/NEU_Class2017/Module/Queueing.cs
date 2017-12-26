using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet;

namespace NEU_Class2017.Module
{
    public class Queueing : Component<Queueing.Statics>
    {
        #region Statics
        public class Statics : Scenario
        {
            /******************************************************/
            /* All static properties shall be public,             */
            /* for both getter and setter.                        */
            /******************************************************/
            public int Capacity { get; set; }
        }
        #endregion

        #region Dynamics
        /**********************************************************/
        /* All dynamic properties shall have only public getter,  */
        /* where setter should remain as private.                 */
        /**********************************************************/
        public int QueueLength { get; private set; } = 0;
        public bool ToDequeue { get; private set; } = true;
        public int Vacancy { get { return Config.Capacity - QueueLength; } }
        #endregion

        #region Events
        private abstract class InternalEvent : Event { internal Queueing This { get; set; } } // event adapter 

        /**********************************************************/
        /* All internal events shall be private,                  */
        /* and inherite from InternalEvent as defined above       */
        /**********************************************************/
        private class EnqueueEvent : InternalEvent
        {
            public override void Invoke()
            {
                Log("Enqueue");
                This.QueueLength++;
                Execute(new DequeueEvent { This = This });
            }
        }
        private class DequeueEvent : InternalEvent
        {
            public override void Invoke()
            {
                if (This.QueueLength > 0 && This.ToDequeue)
                {
                    Log("Dequeue");
                    This.QueueLength--;
                    foreach (var evnt in This.OnDequeue) Execute(evnt());
                    Schedule(new DequeueEvent { This = This }, TimeSpan.FromSeconds(0));
                }
                foreach (var evnt in This.OnStateChg) Execute(evnt());
            }
        }
        private class UpdToDequeueEvent : InternalEvent
        {
            internal bool ToDequeue { get; set; }
            public override void Invoke()
            {
                Log("UpdToDequeue");
                This.ToDequeue = ToDequeue;
                Execute(new DequeueEvent { This = This });
            }
        }
        private class TestEvent : InternalEvent
        {
            public override void Invoke()
            {
                Schedule(This.Enqueue(), new DateTime(1, 1, 1, 0, 10, 20));
                Schedule(This.UpdToDequeue(false), new DateTime(1, 1, 1, 0, 10, 40));
                Schedule(This.Enqueue(), new DateTime(1, 1, 1, 0, 11, 20));
                Schedule(This.Enqueue(), new DateTime(1, 1, 1, 0, 12, 20));
                Schedule(This.UpdToDequeue(true), new DateTime(1, 1, 1, 0, 13, 40));
            }
        }
        #endregion

        #region Input Events - Getters
        /***************************************************************/
        /* Methods returning an InternalEvent as O2DESNet.Event,       */
        /* with parameters for the objects to be passed in.            */
        /* Note that the InternalEvent shall always carry This = this. */
        /***************************************************************/
        public Event Enqueue() { return new EnqueueEvent { This = this }; }
        public Event UpdToDequeue(bool toDequeue) { return new UpdToDequeueEvent { This = this, ToDequeue = toDequeue }; }
        #endregion

        #region Output Events - Reference to Getters
        /***********************************************************************/
        /* List of functions that maps outgoing objects to an external event.  */
        /* Note that the mapping is specified only in external structure.      */
        /***********************************************************************/
        public List<Func<Event>> OnDequeue { get; private set; } = new List<Func<Event>>();
        public List<Func<Event>> OnStateChg { get; private set; } = new List<Func<Event>>();
        #endregion

        public Queueing(Statics config, int seed, string tag = null) : base(config, seed, tag)
        {
            InitEvents.Add(new TestEvent { This = this });
        }

        public override void WarmedUp(DateTime clockTime)
        {
            base.WarmedUp(clockTime);
        }

        public override void WriteToConsole(DateTime? clockTime = null)
        {
            Console.WriteLine("Queue Length: {0}  ToDequeue: {1}",
                QueueLength, ToDequeue);
        }
    }
}