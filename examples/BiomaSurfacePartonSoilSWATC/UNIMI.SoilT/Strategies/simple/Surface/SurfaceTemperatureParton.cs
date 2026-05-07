

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
	///Class SurfaceTemperatureParton
    /// Strategy for the calculation of soil surface temperature with Parton's method. Reference: Parton, W. J. 1984. Predicting soil temperatures in a shortgrass steppe. Soil Science 138:93-101.
    /// </summary>
	public class SurfaceTemperatureParton : IStrategySoilT
	{

	#region Constructor

			public SurfaceTemperatureParton()
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
				pd4.PropertyName = "DayLength";
				pd4.PropertyType = (( UNIMI.SoilT.Interfaces.ExogenousVarInfo.DayLength)).ValueType.TypeForCurrentValue;
				pd4.PropertyVarInfo =( UNIMI.SoilT.Interfaces.ExogenousVarInfo.DayLength);
				_inputs0_0.Add(pd4);
				PropertyDescription pd5 = new PropertyDescription();
				pd5.DomainClassType = typeof(UNIMI.SoilT.Interfaces.StatesExternal);
				pd5.PropertyName = "AboveGroundBiomass";
				pd5.PropertyType = (( UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass)).ValueType.TypeForCurrentValue;
				pd5.PropertyVarInfo =( UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass);
				_inputs0_0.Add(pd5);
				mo0_0.Inputs=_inputs0_0;
				//Outputs
				List<PropertyDescription> _outputs0_0 = new List<PropertyDescription>();
				PropertyDescription pd6 = new PropertyDescription();
				pd6.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
				pd6.PropertyName = "SurfaceSoilTemperature";
				pd6.PropertyType =  (( UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature)).ValueType.TypeForCurrentValue;
				pd6.PropertyVarInfo =(  UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature);
				_outputs0_0.Add(pd6);
				PropertyDescription pd7 = new PropertyDescription();
				pd7.DomainClassType = typeof(UNIMI.SoilT.Interfaces.Auxiliary);
				pd7.PropertyName = "SurfaceTemperatureMaximum";
				pd7.PropertyType =  (( UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMaximum)).ValueType.TypeForCurrentValue;
				pd7.PropertyVarInfo =(  UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMaximum);
				_outputs0_0.Add(pd7);
				PropertyDescription pd8 = new PropertyDescription();
				pd8.DomainClassType = typeof(UNIMI.SoilT.Interfaces.Auxiliary);
				pd8.PropertyName = "SurfaceTemperatureMinimum";
				pd8.PropertyType =  (( UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMinimum)).ValueType.TypeForCurrentValue;
				pd8.PropertyVarInfo =(  UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMinimum);
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
				get { return "Strategy for the calculation of soil surface temperature with Parton's method. Reference: Parton, W. J. 1984. Predicting soil temperatures in a shortgrass steppe. Soil Science 138:93-101."; }
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
					UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMaximum.CurrentValue=auxiliary.SurfaceTemperatureMaximum;
					UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMinimum.CurrentValue=auxiliary.SurfaceTemperatureMinimum;
					
					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();            
					
					
					RangeBasedCondition r6 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature);
					if(r6.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature.ValueType)){prc.AddCondition(r6);}
					RangeBasedCondition r7 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMaximum);
					if(r7.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMaximum.ValueType)){prc.AddCondition(r7);}
					RangeBasedCondition r8 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMinimum);
					if(r8.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.AuxiliaryVarInfo.SurfaceTemperatureMinimum.ValueType)){prc.AddCondition(r8);}

					

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
					UNIMI.SoilT.Interfaces.ExogenousVarInfo.DayLength.CurrentValue=exogenous.DayLength;
					UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass.CurrentValue=statesexternal.AboveGroundBiomass;

					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();
            
					
					RangeBasedCondition r1 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMaximum);
					if(r1.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMaximum.ValueType)){prc.AddCondition(r1);}
					RangeBasedCondition r2 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMinimum);
					if(r2.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureMinimum.ValueType)){prc.AddCondition(r2);}
					RangeBasedCondition r3 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.GlobalSolarRadiation);
					if(r3.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.GlobalSolarRadiation.ValueType)){prc.AddCondition(r3);}
					RangeBasedCondition r4 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.DayLength);
					if(r4.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.DayLength.ValueType)){prc.AddCondition(r4);}
					RangeBasedCondition r5 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass);
					if(r5.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.StatesExternalVarInfo.AboveGroundBiomass.ValueType)){prc.AddCondition(r5);}

					

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

                double _AGB = statesexternal.AboveGroundBiomass / 10000;
                double _AirTMax = exogenous.AirTemperatureMaximum;
                double _AirTmin = exogenous.AirTemperatureMinimum;
                double _SolarRad = exogenous.GlobalSolarRadiation;
                if (_AGB > 0.15)
                {
                    auxiliary.SurfaceTemperatureMaximum = _AirTMax + (24 * (1 - Math.Exp(-0.038 * _SolarRad)) + 0.35 * _AirTMax) *
                                                   (Math.Exp(-4.8 * _AGB) - 0.13);

                    auxiliary.SurfaceTemperatureMinimum = _AirTmin + 6 * _AGB - 1.82;
                }
                else
                {
                    auxiliary.SurfaceTemperatureMaximum = exogenous.AirTemperatureMaximum;
                    auxiliary.SurfaceTemperatureMinimum = exogenous.AirTemperatureMinimum;
                }
                states.SurfaceSoilTemperature = (0.41 * auxiliary.SurfaceTemperatureMaximum) + (0.59 * auxiliary.SurfaceTemperatureMinimum);

                if (exogenous.DayLength != 0)
                {
                    states.SurfaceSoilTemperature = (exogenous.DayLength / 24) * _AirTMax + (1 - exogenous.DayLength / 24) * _AirTmin;
                }

				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section1 
			}

				

	#endregion


				//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section2
				//Code written below will not be overwritten by a future code generation

				//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
				//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section2 
	}
}

//%%CyML Description Begin%%
/*
     - Name: SurfaceTemperatureParton -Version: 001, -Time step: 1
     - Description:
                 * Title: SurfaceTemperatureParton model
                 * Authors: simone.bregaglio@unimi.it
                 * Reference: ('http://bioma.jrc.ec.europa.eu/ontology/JRC_MARS_biophysical_domain.owl',)
                 * Institution: University Of Milan
                 * ExtendedDescription: Strategy for the calculation of soil surface temperature with Parton's method. Reference: Parton, W. J. 1984. Predicting soil temperatures in a shortgrass steppe. Soil Science 138:93-101.
                 * ShortDescription: None
     - inputs:
                 * name: DayLength
                               ** description : Length of the day
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLE
                               ** max : 24
                               ** min : 0
                               ** default : 10
                               ** unit : h
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
                 * name: AboveGroundBiomass
                               ** description : Above ground biomass
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLE
                               ** max : 60
                               ** min : 0
                               ** default : 3
                               ** unit : Kg ha-1
                 * name: GlobalSolarRadiation
                               ** description : Daily global solar radiation
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLE
                               ** max : 50
                               ** min : 0
                               ** default : 15
                               ** unit : Mj m-2 d-1
     - outputs:
                 * name: SurfaceTemperatureMinimum
                               ** description : Minimum surface soil temperature
                               ** datatype : DOUBLE
                               ** variablecategory : auxiliary
                               ** max : 60
                               ** min : -60
                               ** unit : degC
                 * name: SurfaceTemperatureMaximum
                               ** description : Maximum surface soil temperature
                               ** datatype : DOUBLE
                               ** variablecategory : auxiliary
                               ** max : 60
                               ** min : -60
                               ** unit : degC
                 * name: SurfaceSoilTemperature
                               ** description : Average surface soil temperature
                               ** datatype : DOUBLE
                               ** variablecategory : auxiliary
                               ** max : 60
                               ** min : -60
                               ** unit : degC

*/
//%%CyML Description End%%