
using CRA.AgroManagement;


namespace UNIMI.SoilT.Interfaces
{
    public interface ISoilT
    {
        /// <summary>
        /// 
        /// </summary>
        void Estimate(IStrategySoilT st, Rates r, States s, Auxiliary a, States s1,
                    Exogenous ex, StatesExternal se, ActEvents ae);
        /// <summary>
        /// 
        /// </summary>
        void Estimate(IStrategySoilT st, Rates r, States s, Auxiliary a, States s1,
                    Exogenous ex, StatesExternal se, ActEvents ae, bool saveLog, string callID);
        /// <summary>
        /// 
        /// </summary>
        void Info();
    }
}

