using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRA.AgroManagement;
using CRA.ModelLayer;
using CRA.ModelLayer.Core;
using UNIMI.SoilT.Interfaces;

/*
namespace UNIMI.SoilT.Interfaces
{
    /// <summary>
    /// UNIMI.SoilTAPI
    /// </summary>
    public class SoilTAPI : ISoilT
    {
        private string preconditionsResult;
        private string postconditionsResult;

        #region ISoilT Members

        /// <summary>
        /// Info
        /// </summary>
        public void Info()
        {
            //FrmAbout a = new FrmAbout();
            //a.ShowDialog();
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="st"></param>
        /// <param name="r"></param>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <param name="s1"></param>
        /// <param name="ex"></param>
        /// <param name="se"></param>
        /// <param name="ae"></param>
        /// <param name="saveLog"></param>
        /// <param name="callID"></param>
        public void Estimate(IStrategySoilT st, Rates r, States s, Auxiliary a, States s1,
                           Exogenous ex, StatesExternal se, ActEvents ae, bool saveLog, string callID)
        {
            Preconditions prc = new Preconditions();
            preconditionsResult = st.TestPreConditions(r, s, a, s1, ex, se, callID);
            st.Estimate(r, s, a, s1, ex, se, ae);
            postconditionsResult = st.TestPostConditions(r, s, a, s1, ex, se, callID);
            if (preconditionsResult != "" || postconditionsResult != "")
            {
                prc.TestsOut(preconditionsResult + postconditionsResult, saveLog, "SoilT component, class " + st.ToString());
            }
            prc = null;
        }
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="st"></param>
        /// <param name="r"></param>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <param name="s1"></param>
        /// <param name="ex"></param>
        /// <param name="se"></param>
        /// <param name="ae"></param>
        public void Estimate(IStrategySoilT st, Rates r, States s, Auxiliary a, States s1,
                           Exogenous ex, StatesExternal se, ActEvents ae)
        {
            st.Estimate(r, s, a, s1, ex, se, ae);
        }

        #endregion

        #region - Methods -
                

        /// <summary>
        /// Sets default values of data structures.
        /// </summary>
        /// <param name="r">Rates.</param>
        /// <param name="s">States.</param>
        /// <param name="a">Auxiliary variables.</param>
        /// <param name="ex">Exogenous variables.</param>
        /// <param name="re">External rates.</param>
        /// <param name="se">External states.</param>
        /// <param name="allValues"><c>true</c> if all values have to be set,
        /// <c>false</c> if only <c>NaN</c> or <c>null</c> values have to be set.</param>
        public static void SetDefaulValues(Rates r, States s,
            Auxiliary a, Exogenous ex,
            StatesExternal se, bool allValues)
        {
            r.SetDefaultValues(allValues);
            s.SetDefaultValues(allValues);
            a.SetDefaultValues(allValues);
            ex.SetDefaultValues(allValues);
            se.SetDefaultValues(allValues);
        }

        #endregion

        #region Trace class
        internal static class TraceSoilT
        {
            static private System.Diagnostics.TraceSource Source = new System.Diagnostics.TraceSource(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

            /// <summary>
            ///     Writes a trace event message to the trace listeners in the System.Diagnostics.TraceSource.Listeners
            ///     collection using the specified event type, event identifier, and message.
            /// </summary>
            /// <param Name="eventType">One of the System.Diagnostics.TraceEventType values that specifies the event type of the trace data.</param>
            /// <param Name="id">A numeric identifier for the event.</param>
            /// <param Name="message">The trace message to write.</param>
            [System.Diagnostics.Conditional("TRACE")]
            static public void TraceEvent(System.Diagnostics.TraceEventType eventType, int id, string message) { Source.TraceEvent(eventType, id, message); Source.Flush(); }
        }
        #endregion
    }
}
*/