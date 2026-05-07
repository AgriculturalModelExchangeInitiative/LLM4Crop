

 //Author:Loic Manceau loic.manceau@inra.fr
 //Institution:INRA
 //Author of revision: 
 //Date first release:
 //Date of revision:

using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using CRA.ModelLayer.MetadataTypes;
using CRA.ModelLayer.Core;
using CRA.ModelLayer.Strategy;
using System.Reflection;
using VarInfo=CRA.ModelLayer.Core.VarInfo;
using Preconditions=CRA.ModelLayer.Core.Preconditions;


using INRA.SiriusQualitySoilT.Interfaces;


//To make this project compile please add the reference to assembly: SiriusQuality-SoilTemperatureDomainClasses, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//To make this project compile please add the reference to assembly: CRA.ModelLayer, Version=1.0.5212.29139, Culture=neutral, PublicKeyToken=null
//To make this project compile please add the reference to assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
//To make this project compile please add the reference to assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;

namespace SiriusQualitySoilTemperature.Strategies
{

	/// <summary>
	///Class SoilTemperature
    /// Composite Class for soil temperature
    /// </summary>
	public class SoilTemperature : IStrategySiriusQualitySoilT
	{

	#region Constructor

			public SoilTemperature()
			{
				
				ModellingOptions mo0_0 = new ModellingOptions();
				//Parameters
				List<VarInfo> _parameters0_0 = new List<VarInfo>();
				VarInfo v1 = new VarInfo();
				 v1.DefaultValue = 0;
				 v1.Description = "1 to calculate hourly temperature or 0 if not";
				 v1.Id = 0;
				 v1.MaxValue = 1;
				 v1.MinValue = 0;
				 v1.Name = "isHourlyTemperatureCalculated";
				 v1.Size = 1;
				 v1.Units = "Dimenssionless";
				 v1.URL = "";
				 v1.VarType = CRA.ModelLayer.Core.VarInfo.Type.STATE;
				 v1.ValueType = VarInfoValueTypes.GetInstanceForName("Double");
				 _parameters0_0.Add(v1);
				VarInfo v2 = new CompositeStrategyVarInfo(_calculatesoiltemperature,"lambda_");
				 _parameters0_0.Add(v2);
				VarInfo v3 = new CompositeStrategyVarInfo(_calculatehourlysoiltemperature,"a");
				 _parameters0_0.Add(v3);
				VarInfo v4 = new CompositeStrategyVarInfo(_calculatehourlysoiltemperature,"b");
				 _parameters0_0.Add(v4);
				VarInfo v5 = new CompositeStrategyVarInfo(_calculatehourlysoiltemperature,"c");
				 _parameters0_0.Add(v5);
				mo0_0.Parameters=_parameters0_0;
				//Inputs
				List<PropertyDescription> _inputs0_0 = new List<PropertyDescription>();
				mo0_0.Inputs=_inputs0_0;
				//Outputs
				List<PropertyDescription> _outputs0_0 = new List<PropertyDescription>();
				mo0_0.Outputs=_outputs0_0;
				//Associated strategies
				List<string> lAssStrat0_0 = new List<string>();
				lAssStrat0_0.Add(typeof(SiriusQualitySoilTemperature.Strategies.CalculateSoilTemperature).FullName);
				lAssStrat0_0.Add(typeof(SiriusQualitySoilTemperature.Strategies.CalculateHourlySoilTemperature).FullName);
				mo0_0.AssociatedStrategies = lAssStrat0_0;
				//Adding the modeling options to the modeling options manager

				//Creating the modeling options manager of the strategy
				_modellingOptionsManager = new ModellingOptionsManager(mo0_0);
			
				SetStaticParametersVarInfoDefinitions();
				SetPublisherData();
					
			}

	#endregion

	#region Implementation of IAnnotatable

			/// <summary>
			/// Description of the model
			/// </summary>
			public string Description
			{
				get { return "Composite Class for soil temperature"; }
			}
			
			/// <summary>
			/// URL to access the description of the model
			/// </summary>
			public string URL
			{
				get { return "http://biomamodelling.org"; }
			}
		

	#endregion
	
	#region Implementation of IStrategy

			/// <summary>
			/// Domain of the model.
			/// </summary>
			public string Domain
			{
				get {  return ""; }
			}

			/// <summary>
			/// Type of the model.
			/// </summary>
			public string ModelType
			{
				get { return ""; }
			}

			/// <summary>
			/// Declare if the strategy is a ContextStrategy, that is, it contains logic to select a strategy at run time. 
			/// </summary>
			public bool IsContext
			{
					get { return  false; }
			}

			/// <summary>
			/// Timestep to be used with this strategy
			/// </summary>
			public IList<int> TimeStep
			{
				get
				{
					IList<int> ts = new List<int>();
					
					return ts;
				}
			}
	
	
	#region Publisher Data

			private PublisherData _pd;
			private  void SetPublisherData()
			{
					// Set publishers' data
					
				_pd = new CRA.ModelLayer.MetadataTypes.PublisherData();
				_pd.Add("Creator", "loic.manceau@inra.fr");
				_pd.Add("Date", "");
				_pd.Add("Publisher", "INRA");
			}

			public PublisherData PublisherData
			{
				get { return _pd; }
			}

	#endregion

	#region ModellingOptionsManager

			private ModellingOptionsManager _modellingOptionsManager;
			
			public ModellingOptionsManager ModellingOptionsManager
			{
				get { return _modellingOptionsManager; }            
			}

	#endregion

			/// <summary>
			/// Return the types of the domain classes used by the strategy
			/// </summary>
			/// <returns></returns>
			public IEnumerable<Type> GetStrategyDomainClassesTypes()
			{
				return new List<Type>() {  typeof(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates),typeof(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates),typeof(INRA.SiriusQualitySoilT.Interfaces.Exogenous),typeof(INRA.SiriusQualitySoilT.Interfaces.RatesExternal),typeof(INRA.SiriusQualitySoilT.Interfaces.States) };
			}

	#endregion

    #region Instances of the parameters
			
			// Getter and setters for the value of the parameters of the strategy. The actual parameters are stored into the ModelingOptionsManager of the strategy.

			
			public Double isHourlyTemperatureCalculated
			{ 
				get {
						VarInfo vi= _modellingOptionsManager.GetParameterByName("isHourlyTemperatureCalculated");
						if (vi != null && vi.CurrentValue!=null) return (Double)vi.CurrentValue ;
						else throw new Exception("Parameter 'isHourlyTemperatureCalculated' not found (or found null) in strategy 'SoilTemperature'");
				 } set {
							VarInfo vi = _modellingOptionsManager.GetParameterByName("isHourlyTemperatureCalculated");
							if (vi != null)  vi.CurrentValue=value;
						else throw new Exception("Parameter 'isHourlyTemperatureCalculated' not found in strategy 'SoilTemperature'");
				}
			}

			// Getter and setters for the value of the parameters of a composite strategy
			
			public Double lambda_
			{ 
				get {
						return _calculatesoiltemperature.lambda_ ;
				}
				set {
						_calculatesoiltemperature.lambda_=value;
				}
			}
			public Double a
			{ 
				get {
						return _calculatehourlysoiltemperature.a ;
				}
				set {
						_calculatehourlysoiltemperature.a=value;
				}
			}
			public Double b
			{ 
				get {
						return _calculatehourlysoiltemperature.b ;
				}
				set {
						_calculatehourlysoiltemperature.b=value;
				}
			}
			public Double c
			{ 
				get {
						return _calculatehourlysoiltemperature.c ;
				}
				set {
						_calculatehourlysoiltemperature.c=value;
				}
			}

	#endregion		

	
	#region Parameters initialization method
			
            /// <summary>
            /// Set parameter(s) current values to the default value
            /// </summary>
            public void SetParametersDefaultValue()
            {
				_modellingOptionsManager.SetParametersDefaultValue();
				
					_calculatesoiltemperature.SetParametersDefaultValue();
					_calculatehourlysoiltemperature.SetParametersDefaultValue(); 

					//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section5
					//Code written below will not be overwritten by a future code generation

					//Custom initialization of the parameter. E.g. initialization of the array dimensions of array parameters

					//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
					//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section5 
            }

	#endregion		

	#region Static parameters VarInfo definition

			// Define the properties of the static VarInfo of the parameters
			private static void SetStaticParametersVarInfoDefinitions()
			{                                
                isHourlyTemperatureCalculatedVarInfo.Name = "isHourlyTemperatureCalculated";
				isHourlyTemperatureCalculatedVarInfo.Description =" 1 to calculate hourly temperature or 0 if not";
				isHourlyTemperatureCalculatedVarInfo.MaxValue = 1;
				isHourlyTemperatureCalculatedVarInfo.MinValue = 0;
				isHourlyTemperatureCalculatedVarInfo.DefaultValue = 0;
				isHourlyTemperatureCalculatedVarInfo.Units = "Dimenssionless";
				isHourlyTemperatureCalculatedVarInfo.ValueType = CRA.ModelLayer.Core.VarInfoValueTypes.GetInstanceForName("Double");

				
       
			}

			//Parameters static VarInfo list 
			
				private static VarInfo _isHourlyTemperatureCalculatedVarInfo= new VarInfo();
				/// <summary> 
				///isHourlyTemperatureCalculated VarInfo definition
				/// </summary>
				public static VarInfo isHourlyTemperatureCalculatedVarInfo
				{
					get { return _isHourlyTemperatureCalculatedVarInfo; }
				}					
			
			//Parameters static VarInfo list of the composite class
			
				/// <summary> 
				///lambda_ VarInfo definition
				/// </summary>
				public static VarInfo lambda_VarInfo
				{
					get { return SiriusQualitySoilTemperature.Strategies.CalculateSoilTemperature.lambda_VarInfo; }
				}
				/// <summary> 
				///a VarInfo definition
				/// </summary>
				public static VarInfo aVarInfo
				{
					get { return SiriusQualitySoilTemperature.Strategies.CalculateHourlySoilTemperature.aVarInfo; }
				}
				/// <summary> 
				///b VarInfo definition
				/// </summary>
				public static VarInfo bVarInfo
				{
					get { return SiriusQualitySoilTemperature.Strategies.CalculateHourlySoilTemperature.bVarInfo; }
				}
				/// <summary> 
				///c VarInfo definition
				/// </summary>
				public static VarInfo cVarInfo
				{
					get { return SiriusQualitySoilTemperature.Strategies.CalculateHourlySoilTemperature.cVarInfo; }
				}			

	#endregion
	
	#region pre/post conditions management		

		    /// <summary>
			/// Test to verify the postconditions
			/// </summary>
			public string TestPostConditions(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates,INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates1,INRA.SiriusQualitySoilT.Interfaces.Exogenous exogenous,INRA.SiriusQualitySoilT.Interfaces.RatesExternal ratesexternal,INRA.SiriusQualitySoilT.Interfaces.States states, string callID)
			{
				try
				{
					//Set current values of the outputs to the static VarInfo representing the output properties of the domain classes				
					
					
					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();            
					
					

					
					string ret = "";
					 ret += _calculatesoiltemperature.TestPostConditions(deeplayerstates,deeplayerstates1,exogenous,ratesexternal,states, "strategy SiriusQualitySoilTemperature.Strategies.CalculateSoilTemperature");
					 ret += _calculatehourlysoiltemperature.TestPostConditions(deeplayerstates,deeplayerstates1,exogenous,ratesexternal,states, "strategy SiriusQualitySoilTemperature.Strategies.CalculateHourlySoilTemperature");
					if (ret != "") { pre.TestsOut(ret, true, "   postconditions tests of associated classes"); }

					//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section4
					//Code written below will not be overwritten by a future code generation

        

					//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
					//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section4 

					//Get the evaluation of postconditions
					string postConditionsResult =pre.VerifyPostconditions(prc, callID);
					//if we have errors, send it to the configured output 
					if(!string.IsNullOrEmpty(postConditionsResult)) { pre.TestsOut(postConditionsResult, true, "PostConditions errors in component SiriusQualitySoilTemperature.Strategies, strategy " + this.GetType().Name ); }
					return postConditionsResult;
				}
				catch (Exception exception)
				{
					//Uncomment the next line to use the trace
					//TraceStrategies.TraceEvent(System.Diagnostics.TraceEventType.Error, 1001,	"Strategy: " + this.GetType().Name + " - Unhandled exception running post-conditions");

					string msg = "Component SiriusQualitySoilTemperature.Strategies, " + this.GetType().Name + ": Unhandled exception running post-condition test. ";
					throw new Exception(msg, exception);
				}
			}

			/// <summary>
			/// Test to verify the preconditions
			/// </summary>
			public string TestPreConditions(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates,INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates1,INRA.SiriusQualitySoilT.Interfaces.Exogenous exogenous,INRA.SiriusQualitySoilT.Interfaces.RatesExternal ratesexternal,INRA.SiriusQualitySoilT.Interfaces.States states, string callID)
			{
				try
				{
					//Set current values of the inputs to the static VarInfo representing the input properties of the domain classes				
					

					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();
            
					
					prc.AddCondition(new RangeBasedCondition(_modellingOptionsManager.GetParameterByName("isHourlyTemperatureCalculated")));

					
					string ret = "";
					 ret += _calculatesoiltemperature.TestPreConditions(deeplayerstates,deeplayerstates1,exogenous,ratesexternal,states, "strategy SiriusQualitySoilTemperature.Strategies.CalculateSoilTemperature");
					 ret += _calculatehourlysoiltemperature.TestPreConditions(deeplayerstates,deeplayerstates1,exogenous,ratesexternal,states, "strategy SiriusQualitySoilTemperature.Strategies.CalculateHourlySoilTemperature");
					if (ret != "") { pre.TestsOut(ret, true, "   preconditions tests of associated classes"); }

					//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section3
					//Code written below will not be overwritten by a future code generation

        

					//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
					//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section3 
								
					//Get the evaluation of preconditions;					
					string preConditionsResult =pre.VerifyPreconditions(prc, callID);
					//if we have errors, send it to the configured output 
					if(!string.IsNullOrEmpty(preConditionsResult)) { pre.TestsOut(preConditionsResult, true, "PreConditions errors in component SiriusQualitySoilTemperature.Strategies, strategy " + this.GetType().Name ); }
					return preConditionsResult;
				}
				catch (Exception exception)
				{
					//Uncomment the next line to use the trace
					//	TraceStrategies.TraceEvent(System.Diagnostics.TraceEventType.Error, 1002,"Strategy: " + this.GetType().Name + " - Unhandled exception running pre-conditions");

					string msg = "Component SiriusQualitySoilTemperature.Strategies, " + this.GetType().Name + ": Unhandled exception running pre-condition test. ";
					throw new Exception(msg, exception);
				}
			}

		
	#endregion
		


	#region Model

		 	/// <summary>
			/// Run the strategy to calculate the outputs. In case of error during the execution, the preconditions tests are executed.
			/// </summary>
			public void Estimate(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates,INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates1,INRA.SiriusQualitySoilT.Interfaces.Exogenous exogenous,INRA.SiriusQualitySoilT.Interfaces.RatesExternal ratesexternal,INRA.SiriusQualitySoilT.Interfaces.States states)
			{
				try
				{
					CalculateModel(deeplayerstates,deeplayerstates1,exogenous,ratesexternal,states);

					//Uncomment the next line to use the trace
					//TraceStrategies.TraceEvent(System.Diagnostics.TraceEventType.Verbose, 1005,"Strategy: " + this.GetType().Name + " - Model executed");
				}
				catch (Exception exception)
				{
					//Uncomment the next line to use the trace
					//TraceStrategies.TraceEvent(System.Diagnostics.TraceEventType.Error, 1003,		"Strategy: " + this.GetType().Name + " - Unhandled exception running model");

					string msg = "Error in component SiriusQualitySoilTemperature.Strategies, strategy: " + this.GetType().Name + ": Unhandled exception running model. "+exception.GetType().FullName+" - "+exception.Message;				
					throw new Exception(msg, exception);
				}
			}

		

			private void CalculateModel(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates,INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates1,INRA.SiriusQualitySoilT.Interfaces.Exogenous exogenous,INRA.SiriusQualitySoilT.Interfaces.RatesExternal ratesexternal,INRA.SiriusQualitySoilT.Interfaces.States states)
			{				
				
					EstimateOfAssociatedClasses(deeplayerstates,deeplayerstates1,exogenous,ratesexternal,states);

				//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section1
				//Code written below will not be overwritten by a future code generation

        

				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section1 
			}

				
			#region Composite class: associations

			//Declaration of the associated strategies
			SiriusQualitySoilTemperature.Strategies.CalculateSoilTemperature _calculatesoiltemperature = new SiriusQualitySoilTemperature.Strategies.CalculateSoilTemperature();
			SiriusQualitySoilTemperature.Strategies.CalculateHourlySoilTemperature _calculatehourlysoiltemperature = new SiriusQualitySoilTemperature.Strategies.CalculateHourlySoilTemperature();

			//Call of the associated strategies
			private void EstimateOfAssociatedClasses(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates,INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates1,INRA.SiriusQualitySoilT.Interfaces.Exogenous exogenous,INRA.SiriusQualitySoilT.Interfaces.RatesExternal ratesexternal,INRA.SiriusQualitySoilT.Interfaces.States states){
				_calculatesoiltemperature.Estimate(deeplayerstates,deeplayerstates1,exogenous,ratesexternal,states);
                _calculatehourlysoiltemperature.Estimate(deeplayerstates, deeplayerstates1, exogenous, ratesexternal, states);
			}

			#endregion


	#endregion


				//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section2
				//Code written below will not be overwritten by a future code generation

                  public void Init(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates, INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates1, INRA.SiriusQualitySoilT.Interfaces.Exogenous exogenous, INRA.SiriusQualitySoilT.Interfaces.RatesExternal ratesexternal, INRA.SiriusQualitySoilT.Interfaces.States states)
                  {
					//Nothing to do here, Init Day Step reported to Wrapper Soil T, beware the DeepLayer T initilization

					}

        public void InitDayStep()
                  {
                      //Nothing to do here, Init Day Step reported to Wrapper Soil T, beware the DeepLayer T initilization

                  }

                  public SoilTemperature(SoilTemperature toCopy)
                      : this()
                  {
                      a = toCopy.a;
                      b = toCopy.b;
                      c = toCopy.c;
                      lambda_ = toCopy.lambda_;
                      isHourlyTemperatureCalculated = toCopy.isHourlyTemperatureCalculated;
                  }

				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section2 
	}
}
