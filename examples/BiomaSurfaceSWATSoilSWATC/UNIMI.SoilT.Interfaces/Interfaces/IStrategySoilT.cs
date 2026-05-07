using CRA.AgroManagement;
using CRA.ModelLayer.Strategy;


namespace UNIMI.SoilT.Interfaces
{

    /// <summary>
    /// Methods implemented by all models (strategies) of the component SoilTemperature. 
    /// </summary>
    public interface IStrategySoilT : IStrategy
    {

        /// <summary>
        /// Set paramareters to the default value.
        /// </summary>
        void SetParametersDefaultValue();

        /// <summary>
        /// Test of pre-conditions.
        /// </summary>
        /// <param name="r">Rate variables.</param>
        /// <param name="s1">State variables at time t.</param>
        /// <param name="a">Auxiliary variables.</param>
        /// <param name="s">State variables at time t-1.</param>
        /// <param name="ex">Exogenous variables.</param>
        /// <param name="se">External to the domain modelled state variables at time t-1.</param>
        /// <param name="callID">User defined call ID.</param>
        /// <returns></returns>
        string TestPreConditions(
            Rates r,
            States s,
            Auxiliary a,
            States s1,
            Exogenous ex,
            StatesExternal se,
            string callID);

        /// <summary>
        /// Test of post-conditions.
        /// </summary>
        /// <param name="r">Rate variables.</param>
        /// <param name="s">State variables at time t-1.</param>
        /// <param name="a">Auxiliary variables.</param>
        /// <param name="callID">User defined call ID.</param>
        /// <returns></returns>
        string TestPostConditions(
            Rates r,
            States s,
            Auxiliary a,
            States s1,
            Exogenous ex,
            StatesExternal se,
            string callID);

        /// <summary>
        /// Estimate of rates at the current time step as stateless models.
        /// </summary>
        /// <param name="r">Rate variables.</param>
        /// <param name="s1">State variables at time t.</param>
        /// <param name="a">Auxiliary variables.</param>
        /// <param name="s">State variables at time t-1.</param>
        /// <param name="ex">Exogenous variables.</param>
        /// <param name="se">External to the domain modelled state variables at time t-1.</param>
        /// <param name="ae"></param>
        void Estimate(
            Rates r,
            States s,
            Auxiliary a,
            States s1,
            Exogenous ex,
            StatesExternal se,
            ActEvents ae);

    }
}