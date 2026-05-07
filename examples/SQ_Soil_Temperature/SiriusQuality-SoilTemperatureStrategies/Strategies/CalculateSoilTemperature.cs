

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
	///Class CalculateSoilTemperature
    /// Calculation of minimum and maximum Soil temperature, Further used in shoot temperature estimate.
    /// </summary>
	public class CalculateSoilTemperature : IStrategySiriusQualitySoilT
	{

	#region Constructor

			public CalculateSoilTemperature()
			{
				
				ModellingOptions mo0_0 = new ModellingOptions();
				//Parameters
				List<VarInfo> _parameters0_0 = new List<VarInfo>();
				VarInfo v1 = new VarInfo();
				 v1.DefaultValue = 2.454;
				 v1.Description = "Latente heat of water vaporization at 20°C";
				 v1.Id = 0;
				 v1.MaxValue = 10;
				 v1.MinValue = 0;
				 v1.Name = "lambda_";
				 v1.Size = 1;
				 v1.Units = "MJ.kg-1";
				 v1.URL = "";
				 v1.VarType = CRA.ModelLayer.Core.VarInfo.Type.STATE;
				 v1.ValueType = VarInfoValueTypes.GetInstanceForName("Double");
				 _parameters0_0.Add(v1);
				mo0_0.Parameters=_parameters0_0;
				//Inputs
				List<PropertyDescription> _inputs0_0 = new List<PropertyDescription>();
				PropertyDescription pd1 = new PropertyDescription();
				pd1.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates);
				pd1.PropertyName = "deepLayerT";
				pd1.PropertyType = (( INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT)).ValueType.TypeForCurrentValue;
				pd1.PropertyVarInfo =( INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT);
				_inputs0_0.Add(pd1);
				PropertyDescription pd2 = new PropertyDescription();
				pd2.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.Exogenous);
				pd2.PropertyName = "maxTAir";
				pd2.PropertyType = (( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.maxTAir)).ValueType.TypeForCurrentValue;
				pd2.PropertyVarInfo =( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.maxTAir);
				_inputs0_0.Add(pd2);
				PropertyDescription pd3 = new PropertyDescription();
				pd3.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.Exogenous);
				pd3.PropertyName = "meanTAir";
				pd3.PropertyType = (( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanTAir)).ValueType.TypeForCurrentValue;
				pd3.PropertyVarInfo =( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanTAir);
				_inputs0_0.Add(pd3);
				PropertyDescription pd4 = new PropertyDescription();
				pd4.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.Exogenous);
				pd4.PropertyName = "minTAir";
				pd4.PropertyType = (( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.minTAir)).ValueType.TypeForCurrentValue;
				pd4.PropertyVarInfo =( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.minTAir);
				_inputs0_0.Add(pd4);
				PropertyDescription pd5 = new PropertyDescription();
				pd5.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.RatesExternal);
				pd5.PropertyName = "heatFlux";
				pd5.PropertyType = (( INRA.SiriusQualitySoilT.Interfaces.RatesExternalVarInfo.heatFlux)).ValueType.TypeForCurrentValue;
				pd5.PropertyVarInfo =( INRA.SiriusQualitySoilT.Interfaces.RatesExternalVarInfo.heatFlux);
				_inputs0_0.Add(pd5);
				PropertyDescription pd6 = new PropertyDescription();
				pd6.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.Exogenous);
				pd6.PropertyName = "meanAnnualAirTemp";
				pd6.PropertyType = ((INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanAnnualAirTemp)).ValueType.TypeForCurrentValue;
				pd6.PropertyVarInfo = (INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanAnnualAirTemp);
				_inputs0_0.Add(pd6);
				mo0_0.Inputs=_inputs0_0;
            //Outputs
            List<PropertyDescription> _outputs0_0 = new List<PropertyDescription>();
				PropertyDescription pd7 = new PropertyDescription();
				pd7.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates);
				pd7.PropertyName = "deepLayerT";
				pd7.PropertyType =  (( INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT)).ValueType.TypeForCurrentValue;
				pd7.PropertyVarInfo =(  INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT);
				_outputs0_0.Add(pd7);
				PropertyDescription pd8 = new PropertyDescription();
				pd8.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.States);
				pd8.PropertyName = "maxTSoil";
				pd8.PropertyType =  (( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil)).ValueType.TypeForCurrentValue;
				pd8.PropertyVarInfo =(  INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil);
				_outputs0_0.Add(pd8);
				PropertyDescription pd9 = new PropertyDescription();
				pd9.DomainClassType = typeof(INRA.SiriusQualitySoilT.Interfaces.States);
				pd9.PropertyName = "minTSoil";
				pd9.PropertyType =  (( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil)).ValueType.TypeForCurrentValue;
				pd9.PropertyVarInfo =(  INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil);
				_outputs0_0.Add(pd9);
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
				get { return "Calculation of minimum and maximum Soil temperature, Further used in shoot temperature estimate."; }
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

			
			public Double lambda_
			{ 
				get {
						VarInfo vi= _modellingOptionsManager.GetParameterByName("lambda_");
						if (vi != null && vi.CurrentValue!=null) return (Double)vi.CurrentValue ;
						else throw new Exception("Parameter 'lambda_' not found (or found null) in strategy 'CalculateSoilTemperature'");
				 } set {
							VarInfo vi = _modellingOptionsManager.GetParameterByName("lambda_");
							if (vi != null)  vi.CurrentValue=value;
						else throw new Exception("Parameter 'lambda_' not found in strategy 'CalculateSoilTemperature'");
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
                lambda_VarInfo.Name = "lambda_";
				lambda_VarInfo.Description =" Latente heat of water vaporization at 20°C";
				lambda_VarInfo.MaxValue = 10;
				lambda_VarInfo.MinValue = 0;
				lambda_VarInfo.DefaultValue = 2.454;
				lambda_VarInfo.Units = "MJ.kg-1";
				lambda_VarInfo.ValueType = CRA.ModelLayer.Core.VarInfoValueTypes.GetInstanceForName("Double");

				
       
			}

			//Parameters static VarInfo list 
			
				private static VarInfo _lambda_VarInfo= new VarInfo();
				/// <summary> 
				///lambda_ VarInfo definition
				/// </summary>
				public static VarInfo lambda_VarInfo
				{
					get { return _lambda_VarInfo; }
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
					
					INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT.CurrentValue=deeplayerstates.deepLayerT;
					INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil.CurrentValue=states.maxTSoil;
					INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil.CurrentValue=states.minTSoil;
					
					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();            
					
					
					RangeBasedCondition r6 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT);
					if(r6.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT.ValueType)){prc.AddCondition(r6);}
					RangeBasedCondition r7 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil);
					if(r7.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.maxTSoil.ValueType)){prc.AddCondition(r7);}
					RangeBasedCondition r8 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil);
					if(r8.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.StatesVarInfo.minTSoil.ValueType)){prc.AddCondition(r8);}

					

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
					
					INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT.CurrentValue=deeplayerstates.deepLayerT;
					INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.maxTAir.CurrentValue=exogenous.maxTAir;
					INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanTAir.CurrentValue=exogenous.meanTAir;
					INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.minTAir.CurrentValue=exogenous.minTAir;
					INRA.SiriusQualitySoilT.Interfaces.RatesExternalVarInfo.heatFlux.CurrentValue=ratesexternal.heatFlux; 
					INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanAnnualAirTemp.CurrentValue = exogenous.meanAnnualAirTemp;

                //Create the collection of the conditions to test
                ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();

                    //CRA.ModelLayer.Core.AtLeastOneDifferentFromZeroCondition
					RangeBasedCondition r1 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT);
					if(r1.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.DeepLayerStatesVarInfo.deepLayerT.ValueType)){prc.AddCondition(r1);}
					RangeBasedCondition r2 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.maxTAir);
					if(r2.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.maxTAir.ValueType)){prc.AddCondition(r2);}
					RangeBasedCondition r3 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanTAir);
					if(r3.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanTAir.ValueType)){prc.AddCondition(r3);}
					RangeBasedCondition r4 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.minTAir);
					if(r4.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.minTAir.ValueType)){prc.AddCondition(r4);}
					RangeBasedCondition r5 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.RatesExternalVarInfo.heatFlux);
					if(r5.ApplicableVarInfoValueTypes.Contains( INRA.SiriusQualitySoilT.Interfaces.RatesExternalVarInfo.heatFlux.ValueType)){prc.AddCondition(r5);}
					prc.AddCondition(new RangeBasedCondition(_modellingOptionsManager.GetParameterByName("lambda_"))); 
				RangeBasedCondition r6 = new RangeBasedCondition(INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanAnnualAirTemp);
                if (r6.ApplicableVarInfoValueTypes.Contains(INRA.SiriusQualitySoilT.Interfaces.ExogenousVarInfo.meanAnnualAirTemp.ValueType)) { prc.AddCondition(r6); }



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

			double tmp = exogenous.meanAnnualAirTemp;
			if(exogenous.maxTAir==-999 && exogenous.minTAir == 999)
            {
				states.minTSoil = 999;
				states.maxTSoil = -999;
				deeplayerstates.deepLayerT = 0.0;


			}
            else
            {
				states.minTSoil = SoilMinimumTemperature(exogenous.maxTAir, exogenous.meanTAir, exogenous.minTAir, ratesexternal.heatFlux, lambda_, deeplayerstates.deepLayerT);
				states.maxTSoil = SoilMaximumTemperature(exogenous.maxTAir, exogenous.meanTAir, exogenous.minTAir, ratesexternal.heatFlux, lambda_, deeplayerstates.deepLayerT);
				deeplayerstates.deepLayerT = UpdateTemperature(states.minTSoil, states.maxTSoil, deeplayerstates.deepLayerT);
			}



				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section1 
			}
			public void Init(INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates, INRA.SiriusQualitySoilT.Interfaces.DeepLayerStates deeplayerstates1, INRA.SiriusQualitySoilT.Interfaces.Exogenous exogenous, INRA.SiriusQualitySoilT.Interfaces.RatesExternal ratesexternal, INRA.SiriusQualitySoilT.Interfaces.States states)
				{
					deeplayerstates.deepLayerT = exogenous.meanAnnualAirTemp;
			}
				

	#endregion


				//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section2
				//Code written below will not be overwritten by a future code generation
            public double SoilMinimumTemperature(double weatherMaxTemp, double weatherMeanTemp, double weatherMinTemp, double soilHeatFlux, double lambda_, double deepTemperature)
            {
                return Math.Min(SoilTempA(weatherMaxTemp, weatherMeanTemp, soilHeatFlux, lambda_),
                                SoilTempB(weatherMinTemp, deepTemperature));
            }

            ///<summary>
            ///Soil maximum temperature
            ///OUTPUT UNITS: °C
            ///</summary>
            ///<param name="weatherMaxTemp"></param>
            ///<param name="weatherMeanTemp"></param>
            ///<param name="weatherMinTemp"></param>
            ///<param name="soilHeatFlux"></param>
            ///<param name="lambda_"></param>
            ///<param name="deepTemperature"></param>
            ///<returns></returns>
            public double SoilMaximumTemperature(double weatherMaxTemp, double weatherMeanTemp, double weatherMinTemp, double soilHeatFlux, double lambda_, double deepTemperature)
            {
                return Math.Max(SoilTempA(weatherMaxTemp, weatherMeanTemp, soilHeatFlux, lambda_),
                                SoilTempB(weatherMinTemp, deepTemperature));
            }


            ///<summary>
            ///Soil temperature A
            ///</summary>
            ///<param name="weatherMaxTemp"></param>
            ///<param name="weatherMeanTemp"></param>
            ///<param name="soilHeatFlux"></param>
            ///<param name="lambda_"></param>
            ///<returns></returns>
            private double SoilTempA(double weatherMaxTemp, double weatherMeanTemp, double soilHeatFlux, double lambda_)
            {
                double TempAdjustment = (weatherMeanTemp < 8.0) ? -0.5 * weatherMeanTemp + 4.0 : 0;
                double SoilAvailableEnergy = soilHeatFlux * lambda_ / 1000;

                return weatherMaxTemp + 11.2 * (1.0 - Math.Exp(-0.07 * (SoilAvailableEnergy - 5.5))) + TempAdjustment;
            }

            ///<summary>
            ///Soil temperature B
            ///</summary>
            ///<param name="weatherMinTemp"></param>
            ///<param name="deepTemperature"></param>
            ///<returns></returns>
            private double SoilTempB(double weatherMinTemp, double deepTemperature)
            {
                return (weatherMinTemp + deepTemperature) / 2.0;
            }

            ///<summary>Update the temperature from the soil</summary>
            ///<param name="soil">The soil</param>
            public double UpdateTemperature(double minSoilTemp, double maxSoilTemp, double Temperature)
            {

                var meanTemp = (minSoilTemp + maxSoilTemp) / 2.0;
                Temperature = (9.0 * Temperature + meanTemp) / 10.0;

                return Temperature;
            }

				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section2 
	}
}
