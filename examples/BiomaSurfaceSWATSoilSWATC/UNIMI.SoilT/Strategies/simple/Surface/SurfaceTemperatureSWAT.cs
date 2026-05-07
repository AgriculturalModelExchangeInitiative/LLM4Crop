

 //Author:Simone Bregaglio simone.bregaglio@unimi.it
 //Institution:University Of Milan
 //Author of revision:Nicolò Frasso nicolo.frasso@unimi.it
 //Date first release:2/1/2010
 //Date of revision:4/10/2014

using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using CRA.ModelLayer.MetadataTypes;
using CRA.ModelLayer.Core;
using CRA.ModelLayer.Strategy;
using System.Reflection;
using CRA.ModelLayer;

using UNIMI.SoilT.Interfaces;
using CRA.AgroManagement;
using CRA.AgroManagement.Impacts;
using CRA.AgroManagement.Rules;


//To make this project compile please add the reference to assembly: UNIMI.SoilT.Interfaces, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//To make this project compile please add the reference to assembly: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
//To make this project compile please add the reference to assembly: CRA.ModelLayer.Core, Version=0.8.4692.22654, Culture=neutral, PublicKeyToken=null
//To make this project compile please add the reference to assembly: CRA.ModelLayer.MetadataTypes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//To make this project compile please add the reference to assembly: CRA.ModelLayer.Strategy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//To make this project compile please add the reference to assembly: EC.JRC.MARS.ParametersManagement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//To make this project compile please add the reference to assembly: CRA.AgroManagement, Version=0.8.0.0, Culture=neutral, PublicKeyToken=null
//To make this project compile please add the reference to assembly: CRA.ModelLayer.Core, Version=0.8.4657.21311, Culture=neutral, PublicKeyToken=null;

namespace UNIMI.SoilT.Strategies.Surface
{

	/// <summary>
	///Class SurfaceTemperatureSWAT
    /// Strategy for the calculation of surface soil temperature with SWAT method. Reference: Neitsch,S.L., Arnold, J.G., Kiniry, J.R., Williams, J.R., King, K.W. Soil and Water Assessment Tool. Theoretical documentation. Version 2000. http://swatmodel.tamu.edu/media/1290/swat2000theory.pdf
    /// </summary>
	public class SurfaceTemperatureSWAT : IStrategySoilT
	{

	#region Constructor

			public SurfaceTemperatureSWAT()
			{
				
				ModellingOptions mo0_0 = new ModellingOptions();
				//Parameters
				List<VarInfo> _parameters0_0 = new List<VarInfo>();
				mo0_0.Parameters=_parameters0_0;
				//Inputs
				List<PropertyDescription> _inputs0_0 = new List<PropertyDescription>();
				PropertyDescription pd1 = new PropertyDescription();
				pd1.DomainClassType = typeof(UNIMI.SoilT.Interfaces.Exogenous);
				pd1.PropertyName = "AirTemperatureMaximum";
				pd1.PropertyType = (( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMaximum)).ValueType.TypeForCurrentValue;
				pd1.PropertyVarInfo =( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMaximum);
				_inputs0_0.Add(pd1);
				PropertyDescription pd2 = new PropertyDescription();
				pd2.DomainClassType = typeof(UNIMI.SoilT.Interfaces.Exogenous);
				pd2.PropertyName = "AirTemperatureMinimum";
				pd2.PropertyType = (( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMinimum)).ValueType.TypeForCurrentValue;
				pd2.PropertyVarInfo =( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMinimum);
				_inputs0_0.Add(pd2);
				PropertyDescription pd3 = new PropertyDescription();
				pd3.DomainClassType = typeof(UNIMI.SoilT.Interfaces.Exogenous);
				pd3.PropertyName = "GlobalSolarRadiation";
				pd3.PropertyType = (( UNIMI.SoilT.Interfaces.ExogenousVarInfo.GlobalSolarRadiation)).ValueType.TypeForCurrentValue;
				pd3.PropertyVarInfo =( UNIMI.SoilT.Interfaces.ExogenousVarInfo.GlobalSolarRadiation);
				_inputs0_0.Add(pd3);
				PropertyDescription pd4 = new PropertyDescription();
				pd4.DomainClassType = typeof(UNIMI.SoilT.Interfaces.Exogenous);
				pd4.PropertyName = "WaterEquivalentOfSnowPack";
				pd4.PropertyType = (( UNIMI.SoilT.Interfaces.ExogenousVarInfo.WaterEquivalentOfSnowPack)).ValueType.TypeForCurrentValue;
				pd4.PropertyVarInfo =( UNIMI.SoilT.Interfaces.ExogenousVarInfo.WaterEquivalentOfSnowPack);
				_inputs0_0.Add(pd4);
				PropertyDescription pd5 = new PropertyDescription();
				pd5.DomainClassType = typeof(UNIMI.SoilT.Interfaces.Exogenous);
				pd5.PropertyName = "Albedo";
				pd5.PropertyType = (( UNIMI.SoilT.Interfaces.ExogenousVarInfo.Albedo)).ValueType.TypeForCurrentValue;
				pd5.PropertyVarInfo =( UNIMI.SoilT.Interfaces.ExogenousVarInfo.Albedo);
				_inputs0_0.Add(pd5);
				PropertyDescription pd6 = new PropertyDescription();
				pd6.DomainClassType = typeof(UNIMI.SoilT.Interfaces.StatesExternal);
				pd6.PropertyName = "AboveGroundBiomass";
				pd6.PropertyType = (( UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass)).ValueType.TypeForCurrentValue;
				pd6.PropertyVarInfo =( UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass);
				_inputs0_0.Add(pd6);
				PropertyDescription pd7 = new PropertyDescription();
				pd7.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
                pd7.PropertyName = "SoilTemperatureByLayers";
                pd7.PropertyType = ((UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers)).ValueType.TypeForCurrentValue;
                pd7.PropertyVarInfo = (UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers);
				_inputs0_0.Add(pd7);
				mo0_0.Inputs=_inputs0_0;
				//Outputs
				List<PropertyDescription> _outputs0_0 = new List<PropertyDescription>();
				PropertyDescription pd8 = new PropertyDescription();
				pd8.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
				pd8.PropertyName = "SurfaceSoilTemperature";
				pd8.PropertyType =  (( UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature)).ValueType.TypeForCurrentValue;
				pd8.PropertyVarInfo =(  UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature);
				_outputs0_0.Add(pd8);
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
				get { return "Strategy for the calculation of surface soil temperature with SWAT method. Reference: Neitsch,S.L., Arnold, J.G., Kiniry, J.R., Williams, J.R., King, K.W. Soil and Water Assessment Tool. Theoretical documentation. Version 2000. http://swatmodel.tamu.edu/media/1290/swat2000theory.pdf"; }
			}
			
			/// <summary>
			/// URL to access the description of the model
			/// </summary>
			public string URL
			{
				get { return "http://bioma.jrc.ec.europa.eu/ontology/JRC_MARS_biophysical_domain.owl"; }
			}
		

	#endregion
	
	#region Implementation of IStrategy

			/// <summary>
			/// Domain of the model.
			/// </summary>
			public string Domain
			{
				get {  return "Soil"; }
			}

			/// <summary>
			/// Type of the model.
			/// </summary>
			public string ModelType
			{
				get { return "Surface"; }
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
				_pd.Add("Creator", "simone.bregaglio@unimi.it");
				_pd.Add("Date", "2/1/2010");
				_pd.Add("Publisher", "University Of Milan");
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
				return new List<Type>() {  typeof(UNIMI.SoilT.Interfaces.Rates),typeof(UNIMI.SoilT.Interfaces.States),typeof(UNIMI.SoilT.Interfaces.Auxiliary),typeof(UNIMI.SoilT.Interfaces.States),typeof(UNIMI.SoilT.Interfaces.Exogenous),typeof(UNIMI.SoilT.Interfaces.StatesExternal),typeof(CRA.AgroManagement.ActEvents) };
			}

	#endregion

    #region Instances of the parameters
			
			// Getter and setters for the value of the parameters of the strategy. The actual parameters are stored into the ModelingOptionsManager of the strategy.

			

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
                
       
			}

			//Parameters static VarInfo list 
								
			
			//Parameters static VarInfo list of the composite class
						

	#endregion
	
	#region pre/post conditions management		

		    /// <summary>
			/// Test to verify the postconditions
			/// </summary>
			public string TestPostConditions(UNIMI.SoilT.Interfaces.Rates rates,UNIMI.SoilT.Interfaces.States states,UNIMI.SoilT.Interfaces.Auxiliary auxiliary,UNIMI.SoilT.Interfaces.States states1,UNIMI.SoilT.Interfaces.Exogenous exogenous,UNIMI.SoilT.Interfaces.StatesExternal statesexternal, string callID)
			{
				try
				{
					//Set current values of the outputs to the static VarInfo representing the output properties of the domain classes				
					
					UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature.CurrentValue=states.SurfaceSoilTemperature;
					
					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();            
					
					
					RangeBasedCondition r8 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature);
					if(r8.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature.ValueType)){prc.AddCondition(r8);}

					

					//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section4
					//Code written below will not be overwritten by a future code generation

        

					//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
					//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section4 

					//Get the evaluation of postconditions
					string postConditionsResult =pre.VerifyPostconditions(prc, callID);
					//if we have errors, send it to the configured output 
					if(!string.IsNullOrEmpty(postConditionsResult)) { pre.TestsOut(postConditionsResult, true, "PostConditions errors in component UNIMI.SoilT.Strategies.Surface, strategy " + this.GetType().Name ); }
					return postConditionsResult;
				}
				catch (Exception exception)
				{
					//Uncomment the next line to use the trace
					//TraceStrategies.TraceEvent(System.Diagnostics.TraceEventType.Error, 1001,	"Strategy: " + this.GetType().Name + " - Unhandled exception running post-conditions");

					string msg = "Component UNIMI.SoilT.Strategies.Surface, " + this.GetType().Name + ": Unhandled exception running post-condition test. ";
					throw new Exception(msg, exception);
				}
			}

			/// <summary>
			/// Test to verify the preconditions
			/// </summary>
			public string TestPreConditions(UNIMI.SoilT.Interfaces.Rates rates,UNIMI.SoilT.Interfaces.States states,UNIMI.SoilT.Interfaces.Auxiliary auxiliary,UNIMI.SoilT.Interfaces.States states1,UNIMI.SoilT.Interfaces.Exogenous exogenous,UNIMI.SoilT.Interfaces.StatesExternal statesexternal, string callID)
			{
				try
				{
					//Set current values of the inputs to the static VarInfo representing the input properties of the domain classes				
					
					UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMaximum.CurrentValue=exogenous.AirTemperatureMaximum;
					UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMinimum.CurrentValue=exogenous.AirTemperatureMinimum;
					UNIMI.SoilT.Interfaces.ExogenousVarInfo.GlobalSolarRadiation.CurrentValue=exogenous.GlobalSolarRadiation;
					UNIMI.SoilT.Interfaces.ExogenousVarInfo.WaterEquivalentOfSnowPack.CurrentValue=exogenous.WaterEquivalentOfSnowPack;
					UNIMI.SoilT.Interfaces.ExogenousVarInfo.Albedo.CurrentValue=exogenous.Albedo;
					UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass.CurrentValue=statesexternal.AboveGroundBiomass;
					

					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();

				    int numberOfLayers = states.HeatCapacity.Length;

                    for (int i = 0; i < numberOfLayers; i++)
				    {
                        UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers.CurrentValue = states.SoilTemperatureByLayers;
                        RangeBasedCondition r7 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers);
                        if (r7.ApplicableVarInfoValueTypes.Contains(UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers.ValueType)) { prc.AddCondition(r7); }
				    }
            
					
					RangeBasedCondition r1 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMaximum);
					if(r1.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMaximum.ValueType)){prc.AddCondition(r1);}
					RangeBasedCondition r2 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMinimum);
					if(r2.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMinimum.ValueType)){prc.AddCondition(r2);}
					RangeBasedCondition r3 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.GlobalSolarRadiation);
					if(r3.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.GlobalSolarRadiation.ValueType)){prc.AddCondition(r3);}
					RangeBasedCondition r4 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.WaterEquivalentOfSnowPack);
					if(r4.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.WaterEquivalentOfSnowPack.ValueType)){prc.AddCondition(r4);}
					RangeBasedCondition r5 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.Albedo);
					if(r5.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.Albedo.ValueType)){prc.AddCondition(r5);}
					RangeBasedCondition r6 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass);
					if(r6.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass.ValueType)){prc.AddCondition(r6);}
					

					

					//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section3
					//Code written below will not be overwritten by a future code generation

        

					//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
					//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section3 
								
					//Get the evaluation of preconditions;					
					string preConditionsResult =pre.VerifyPreconditions(prc, callID);
					//if we have errors, send it to the configured output 
					if(!string.IsNullOrEmpty(preConditionsResult)) { pre.TestsOut(preConditionsResult, true, "PreConditions errors in component UNIMI.SoilT.Strategies.Surface, strategy " + this.GetType().Name ); }
					return preConditionsResult;
				}
				catch (Exception exception)
				{
					//Uncomment the next line to use the trace
					//	TraceStrategies.TraceEvent(System.Diagnostics.TraceEventType.Error, 1002,"Strategy: " + this.GetType().Name + " - Unhandled exception running pre-conditions");

					string msg = "Component UNIMI.SoilT.Strategies.Surface, " + this.GetType().Name + ": Unhandled exception running pre-condition test. ";
					throw new Exception(msg, exception);
				}
			}

		
	#endregion
		


	#region Model

		 	/// <summary>
			/// Run the strategy to calculate the outputs. In case of error during the execution, the preconditions tests are executed.
			/// </summary>
			public void Estimate(UNIMI.SoilT.Interfaces.Rates rates,UNIMI.SoilT.Interfaces.States states,UNIMI.SoilT.Interfaces.Auxiliary auxiliary,UNIMI.SoilT.Interfaces.States states1,UNIMI.SoilT.Interfaces.Exogenous exogenous,UNIMI.SoilT.Interfaces.StatesExternal statesexternal,CRA.AgroManagement.ActEvents actevents)
			{



			//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section1
			//Code written below will not be overwritten by a future code generation

			double _Tavg = (exogenous.AirTemperatureMaximum + exogenous.AirTemperatureMinimum) / 2;

			double _Hterm = (exogenous.GlobalSolarRadiation * (1 - exogenous.Albedo) - 14) / 20;
			double _Tbare = _Tavg + _Hterm * (exogenous.AirTemperatureMaximum - exogenous.AirTemperatureMinimum) / 2;
			double _WeightingCover = statesexternal.AboveGroundBiomass /
									 (statesexternal.AboveGroundBiomass + Math.Exp(7.563 - 0.0001297 * statesexternal.AboveGroundBiomass));
			double _WeightingSnow = exogenous.WaterEquivalentOfSnowPack /
									(exogenous.WaterEquivalentOfSnowPack +
									 Math.Exp(6.055 - 0.3002 * exogenous.WaterEquivalentOfSnowPack));
			double _WeightingActual = Math.Max(_WeightingCover, _WeightingSnow);

			states.SurfaceSoilTemperature = _WeightingActual * states.SoilTemperatureByLayers[0] +
										(1 - _WeightingActual) * _Tbare;

			//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
			//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section1 
		}



		#endregion


		//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section2
		//Code written below will not be overwritten by a future code generation

		

        
        #region - Bibliography -
        // Neitsch,S.L., Arnold, J.G., Kiniry, J.R., Williams, J.R., King, K.W. Soil and Water Assessment Tool. Theoretical documentation. Version 2000.
        // http://swatmodel.tamu.edu/media/1290/swat2000theory.pdf
        #endregion

				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section2 
	}
}

//%%CyML Description Begin%%
/*

     - Name: SurfaceTemperatureSWAT -Version: 001, -Time step: 1
     - Description:
                 * Title: SurfaceTemperatureSWAT model
                 * Authors: simone.bregaglio@unimi.it
                 * Reference: http://bioma.jrc.ec.europa.eu/ontology/JRC_MARS_biophysical_domain.owl
                 * Institution: University Of Milan
                 * ExtendedDescription: Strategy for the calculation of surface soil temperature with SWAT method. Reference: Neitsch,S.L., Arnold, J.G., Kiniry, J.R., Williams, J.R., King, K.W. Soil and Water Assessment Tool. Theoretical documentation. Version 2000. http://swatmodel.tamu.edu/media/1290/swat2000theory.pdf
                 * ShortDescription: Strategy for the calculation of surface soil temperature with SWAT method
     - inputs:
                 * name: GlobalSolarRadiation
                               ** description : Daily global solar radiation
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLE
                               ** max : 50
                               ** min : 0
                               ** default : 15
                               ** unit : Mj m-2 d-1
                 * name: SoilTemperatureByLayers
                               ** description : Soil temperature of each layer
                               ** inputtype : variable
                               ** variablecategory : auxiliary
                               ** datatype : DOUBLEARRAY
                               ** len : 
                               ** max : 60
                               ** min : -60
                               ** default : 15
                               ** unit : Â°C
                 * name: AirTemperatureMaximum
                               ** description : Maximum daily air temperature
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLE
                               ** max : 60
                               ** min : -40
                               ** default : 15
                               ** unit : Â°C
                 * name: AirTemperatureMinimum
                               ** description : Minimum daily air temperature
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLE
                               ** max : 50
                               ** min : -60
                               ** default : 5
                               ** unit : Â°C
                 * name: Albedo
                               ** description : Albedo of soil
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLE
                               ** max : 1
                               ** min : 0
                               ** default : 0.2
                               ** unit : unitless
                 * name: AboveGroundBiomass
                               ** description : Above ground biomass
                               ** inputtype : variable
                               ** variablecategory : auxiliary
                               ** datatype : DOUBLE
                               ** max : 60
                               ** min : 0
                               ** default : 3
                               ** unit : Kg ha-1
                 * name: WaterEquivalentOfSnowPack
                               ** description : Water equivalent of snow pack
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLE
                               ** max : 1000
                               ** min : 0
                               ** default : 10
                               ** unit : mm
     - outputs:
                 * name: SurfaceSoilTemperature
                               ** description : Average surface soil temperature
                               ** datatype : DOUBLE
                               ** variablecategory : auxiliary
                               ** max : 60
                               ** min : -60
                               ** unit : degC

*/
//%%CyML Description End%%