

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
	///Class CalculateHourlySoilTemperature
    /// Calculate Soil temperature on a hourly basis.Parton, W.J. and Logan, J.A., 1981. A model for diurnal variation in soil and air temperature. Agric. Meteorol., 23: 205-216
    /// </summary>
	public class CalculateHourlySoilTemperature : IStrategySiriusQualitySoilT
	{

	#region Constructor

			public CalculateHourlySoilTemperature()
			{
				
				ModellingOptions mo0_0 = new ModellingOptions();
				//Parameters
				List<VarInfo> _parameters0_0 = new List<VarInfo>();
				VarInfo v1 = new VarInfo();
				 v1.DefaultValue = 0.5;
				 v1.Description = "Delay between sunset and time when maximum temperature is reached";
				 v1.Id = 0;
				 v1.MaxValue = 10;
				 v1.MinValue = 0;
				 v1.Name = "a";
				 v1.Size = 1;
				 v1.Units = "Hour";
				 v1.URL = "";
				 v1.VarType = CRA.ModelLayer.Core.VarInfo.Type.STATE;
				 v1.ValueType = VarInfoValueTypes.GetInstanceForName("Double");
				 _parameters0_0.Add(v1);
				VarInfo v2 = new VarInfo();
				 v2.DefaultValue = 1.81;
				 v2.Description = "Delay between sunrise and time when minimum temperature is reached";
				 v2.Id = 0;
				 v2.MaxValue = 10;
				 v2.MinValue = 0;
				 v2.Name = "b";
				 v2.Size = 1;
				 v2.Units = "Hour";
				 v2.URL = "";
				 v2.VarType = CRA.ModelLayer.Core.VarInfo.Type.STATE;
				 v2.ValueType = VarInfoValueTypes.GetInstanceForName("Double");
				 _parameters0_0.Add(v2);
				VarInfo v3 = new VarInfo();
				 v3.DefaultValue = 0.49;
				 v3.Description = "Nighttime temperature coefficient";
				 v3.Id = 0;
				 v3.MaxValue = 10;
				 v3.MinValue = 0;
				 v3.Name = "c";
				 v3.Size = 1;
				 v3.Units = "Dpmensionless";
				 v3.URL = "";
				 v3.VarType = CRA.ModelLayer.Core.VarInfo.Type.STATE;
				 v3.ValueType = VarInfoValueTypes.GetInstanceForName("Double");
				 _parameters0_0.Add(v3);
				mo0_0.Parameters=_parameters0_0;
				//Inputs
				List<PropertyDescription> _inputs0_0 = new List<PropertyDescription>();
				PropertyDescription pd1 = new PropertyDescription();
				pd1.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.States);
				pd1.PropertyName = "maxTSoil";
				pd1.PropertyType = (( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil)).ValueType.TypeForCurrentValue;
				pd1.PropertyVarInfo =( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil);
				_inputs0_0.Add(pd1);
				PropertyDescription pd2 = new PropertyDescription();
				pd2.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.States);
				pd2.PropertyName = "minTSoil";
				pd2.PropertyType = (( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil)).ValueType.TypeForCurrentValue;
				pd2.PropertyVarInfo =( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil);
				_inputs0_0.Add(pd2);
				PropertyDescription pd3 = new PropertyDescription();
				pd3.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.Exogenous);
				pd3.PropertyName = "dayLength";
				pd3.PropertyType = (( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.dayLength)).ValueType.TypeForCurrentValue;
				pd3.PropertyVarInfo =( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.dayLength);
				_inputs0_0.Add(pd3);
				mo0_0.Inputs=_inputs0_0;
				//Outputs
				List<PropertyDescription> _outputs0_0 = new List<PropertyDescription>();
				PropertyDescription pd4 = new PropertyDescription();
				pd4.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.States);
				pd4.PropertyName = "hourlySoilT";
				pd4.PropertyType =  (( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.hourlySoilT)).ValueType.TypeForCurrentValue;
				pd4.PropertyVarInfo =(  INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.hourlySoilT);
				_outputs0_0.Add(pd4);
				mo0_0.Outputs=_outputs0_0;
				//Associated strategies
				List<string> lAssStrat0_0 = new List<string>();
				mo0_0.AssociatedStrategies = lAssStrat0_0;
				//Adding the modeling options to the modeling options manager
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
				get { return "Calculate Soil temperature on a hourly basis.Parton, W.J. and Logan, J.A., 1981. A model for diurnal variation in soil and air temperature. Agric. Meteorol., 23: 205-216"; }
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

			
			public Double a
			{ 
				get {
						VarInfo vi= _modellingOptionsManager.GetParameterByName("a");
						if (vi != null && vi.CurrentValue!=null) return (Double)vi.CurrentValue ;
						else throw new Exception("Parameter 'a' not found (or found null) in strategy 'CalculateHourlySoilTemperature'");
				 } set {
							VarInfo vi = _modellingOptionsManager.GetParameterByName("a");
							if (vi != null)  vi.CurrentValue=value;
						else throw new Exception("Parameter 'a' not found in strategy 'CalculateHourlySoilTemperature'");
				}
			}
			public Double b
			{ 
				get {
						VarInfo vi= _modellingOptionsManager.GetParameterByName("b");
						if (vi != null && vi.CurrentValue!=null) return (Double)vi.CurrentValue ;
						else throw new Exception("Parameter 'b' not found (or found null) in strategy 'CalculateHourlySoilTemperature'");
				 } set {
							VarInfo vi = _modellingOptionsManager.GetParameterByName("b");
							if (vi != null)  vi.CurrentValue=value;
						else throw new Exception("Parameter 'b' not found in strategy 'CalculateHourlySoilTemperature'");
				}
			}
			public Double c
			{ 
				get {
						VarInfo vi= _modellingOptionsManager.GetParameterByName("c");
						if (vi != null && vi.CurrentValue!=null) return (Double)vi.CurrentValue ;
						else throw new Exception("Parameter 'c' not found (or found null) in strategy 'CalculateHourlySoilTemperature'");
				 } set {
							VarInfo vi = _modellingOptionsManager.GetParameterByName("c");
							if (vi != null)  vi.CurrentValue=value;
						else throw new Exception("Parameter 'c' not found in strategy 'CalculateHourlySoilTemperature'");
				}
			}

			// Getter and setters for the value of the parameters of a composite strategy
			

	#endregion		

	
	#region Parameters initialization method
			
            /// <summary>
            /// Set parameter(s) current values to the default value
            /// </summary>
            public void SetParametersDefaultValue()
            {
				_modellingOptionsManager.SetParametersDefaultValue();
				 

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
                aVarInfo.Name = "a";
				aVarInfo.Description =" Delay between sunset and time when maximum temperature is reached";
				aVarInfo.MaxValue = 10;
				aVarInfo.MinValue = 0;
				aVarInfo.DefaultValue = 0.5;
				aVarInfo.Units = "Hour";
				aVarInfo.ValueType = CRA.ModelLayer.Core.VarInfoValueTypes.GetInstanceForName("Double");

				bVarInfo.Name = "b";
				bVarInfo.Description =" Delay between sunrise and time when minimum temperature is reached";
				bVarInfo.MaxValue = 10;
				bVarInfo.MinValue = 0;
				bVarInfo.DefaultValue = 1.81;
				bVarInfo.Units = "Hour";
				bVarInfo.ValueType = CRA.ModelLayer.Core.VarInfoValueTypes.GetInstanceForName("Double");

				cVarInfo.Name = "c";
				cVarInfo.Description =" Nighttime temperature coefficient";
				cVarInfo.MaxValue = 10;
				cVarInfo.MinValue = 0;
				cVarInfo.DefaultValue = 0.49;
				cVarInfo.Units = "Dpmensionless";
				cVarInfo.ValueType = CRA.ModelLayer.Core.VarInfoValueTypes.GetInstanceForName("Double");

				
       
			}

			//Parameters static VarInfo list 
			
				private static VarInfo _aVarInfo= new VarInfo();
				/// <summary> 
				///a VarInfo definition
				/// </summary>
				public static VarInfo aVarInfo
				{
					get { return _aVarInfo; }
				}
				private static VarInfo _bVarInfo= new VarInfo();
				/// <summary> 
				///b VarInfo definition
				/// </summary>
				public static VarInfo bVarInfo
				{
					get { return _bVarInfo; }
				}
				private static VarInfo _cVarInfo= new VarInfo();
				/// <summary> 
				///c VarInfo definition
				/// </summary>
				public static VarInfo cVarInfo
				{
					get { return _cVarInfo; }
				}					
			
			//Parameters static VarInfo list of the composite class
						

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
					
					INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.hourlySoilT.CurrentValue=states.hourlySoilT;
					
					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();            
					
					
					RangeBasedCondition r4 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.hourlySoilT);
					if(r4.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.hourlySoilT.ValueType)){prc.AddCondition(r4);}

					

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
					
					INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil.CurrentValue=states.maxTSoil;
					INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil.CurrentValue=states.minTSoil;
					INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.dayLength.CurrentValue=exogenous.dayLength;

					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();
            
					
					RangeBasedCondition r1 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil);
					if(r1.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil.ValueType)){prc.AddCondition(r1);}
					RangeBasedCondition r2 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil);
					if(r2.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil.ValueType)){prc.AddCondition(r2);}
					RangeBasedCondition r3 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.dayLength);
					if(r3.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.dayLength.ValueType)){prc.AddCondition(r3);}
					prc.AddCondition(new RangeBasedCondition(_modellingOptionsManager.GetParameterByName("a")));
					prc.AddCondition(new RangeBasedCondition(_modellingOptionsManager.GetParameterByName("b")));
					prc.AddCondition(new RangeBasedCondition(_modellingOptionsManager.GetParameterByName("c")));

					

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


			//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section1
			//Code written below will not be overwritten by a future code generation

			if (states.maxTSoil == -999 && states.minTSoil == 999) {

				for (int i = 0; i < 12; i++) states.hourlySoilT[i] = 999;
				for (int i = 12; i < 24; i++) states.hourlySoilT[i] = -999;

			}
			else
			{

				for (int i = 0; i < 24; i++) states.hourlySoilT[i] = 0.0;

				states.hourlySoilT = getHourlySoilSurfaceTemperature(states.maxTSoil, states.minTSoil, exogenous.dayLength);

			}

				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section1 
			}

				

	#endregion


				//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section2
				//Code written below will not be overwritten by a future code generation
            public double[] getHourlySoilSurfaceTemperature(double TMax, double TMin, double ady)
            {
                double[] result = new double[24];

                double ahou = Math.PI * (ady / 24.0);
                double ani = (24 - ady);
                double bb = 12 - ady / 2 + c;
                double be = 12 + ady / 2;

                for (int i = 0; i < 24; i++)
                {
                    if (i >= bb && i <= be)
                    {
                        result[i] = (TMax - TMin) * Math.Sin(3.14 * (i - bb) / (ady + 2 * a)) + TMin;
                    }
                    else
                    {
                        double bbd;
                        if (i > be) { bbd = i - be; }
                        else //i<bb
                        {
                            bbd = 24 + be + i;
                        }
                        double ddy = ady - c;
                        double tsn = (TMax - TMin) * Math.Sin(3.14 * ddy / (ady + 2 * a)) + TMin;
                        result[i] = TMin + (tsn - TMin) * Math.Exp(-b * bbd / ani);
                    }
                }
                return result;
            }
				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section2 
	}
}
