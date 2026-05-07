

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

namespace UNIMI.SoilT.Strategies.Soil
{

	/// <summary>
	///Class SoilTemperatureSWAT
    /// Strategy for the calculation of soil temperature with SWAT method. Reference: Neitsch,S.L., Arnold, J.G., Kiniry, J.R., Williams, J.R., King, K.W. Soil and Water Assessment Tool. Theoretical documentation. Version 2000. http://swatmodel.tamu.edu/media/1290/swat2000theory.pdf
    /// </summary>
	public class SoilTemperatureSWAT : IStrategySoilT
	{

	#region Constructor

			public SoilTemperatureSWAT()
			{
				
				ModellingOptions mo0_0 = new ModellingOptions();
				//Parameters
				List<VarInfo> _parameters0_0 = new List<VarInfo>();
				VarInfo v1 = new VarInfo();
				 v1.DefaultValue = 0.8;
				 v1.Description = "Lag coefficient that controls the influence of the previous day's temperature on the current day's temperature";
				 v1.Id = 0;
				 v1.MaxValue = 1;
				 v1.MinValue = 0;
				 v1.Name = "LagCoefficient";
				 v1.Size = 1;
				 v1.Units = "dimensionless";
				 v1.URL = "";
				 v1.VarType = CRA.ModelLayer.Core.VarInfo.Type.PARAMETER;
				 v1.ValueType = VarInfoValueTypes.GetInstanceForName("Double");
				 _parameters0_0.Add(v1);
				mo0_0.Parameters=_parameters0_0;
				//Inputs
				List<PropertyDescription> _inputs0_0 = new List<PropertyDescription>();
				PropertyDescription pd1 = new PropertyDescription();
				pd1.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
				pd1.PropertyName = "SoilProfileDepth";
				pd1.PropertyType = (( UNIMI.SoilT.Interfaces.StatesVarInfo.SoilProfileDepth)).ValueType.TypeForCurrentValue;
				pd1.PropertyVarInfo =( UNIMI.SoilT.Interfaces.StatesVarInfo.SoilProfileDepth);
				_inputs0_0.Add(pd1);
				PropertyDescription pd2 = new PropertyDescription();
				pd2.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
				pd2.PropertyName = "SurfaceSoilTemperature";
				pd2.PropertyType = (( UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature)).ValueType.TypeForCurrentValue;
				pd2.PropertyVarInfo =( UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature);
				_inputs0_0.Add(pd2);
				PropertyDescription pd3 = new PropertyDescription();
				pd3.DomainClassType = typeof(UNIMI.SoilT.Interfaces.Exogenous);
				pd3.PropertyName = "AirTemperatureAnnualAverage";
				pd3.PropertyType = (( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureAnnualAverage)).ValueType.TypeForCurrentValue;
				pd3.PropertyVarInfo =( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureAnnualAverage);
				_inputs0_0.Add(pd3);
                PropertyDescription pd4 = new PropertyDescription();
                pd4.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
                pd4.PropertyName = "SoilTemperatureByLayers";
                pd4.PropertyType = ((UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers)).ValueType.TypeForCurrentValue;
                pd4.PropertyVarInfo = (UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers);
                _inputs0_0.Add(pd4);
                PropertyDescription pd5 = new PropertyDescription();
                pd5.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
                pd5.PropertyName = "BulkDensity";
                pd5.PropertyType = ((UNIMI.SoilT.Interfaces.StatesVarInfo.BulkDensity)).ValueType.TypeForCurrentValue;
                pd5.PropertyVarInfo = (UNIMI.SoilT.Interfaces.StatesVarInfo.BulkDensity);
                _inputs0_0.Add(pd5);
                PropertyDescription pd6 = new PropertyDescription();
                pd6.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
                pd6.PropertyName = "VolumetricWaterContent";
                pd6.PropertyType = ((UNIMI.SoilT.Interfaces.StatesVarInfo.VolumetricWaterContent)).ValueType.TypeForCurrentValue;
                pd6.PropertyVarInfo = (UNIMI.SoilT.Interfaces.StatesVarInfo.VolumetricWaterContent);
                _inputs0_0.Add(pd6);
                PropertyDescription pd7 = new PropertyDescription();
                pd7.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
                pd7.PropertyName = "LayerThickness";
                pd7.PropertyType = ((UNIMI.SoilT.Interfaces.StatesVarInfo.LayerThickness)).ValueType.TypeForCurrentValue;
                pd7.PropertyVarInfo = (UNIMI.SoilT.Interfaces.StatesVarInfo.LayerThickness);
                _inputs0_0.Add(pd7);
				mo0_0.Inputs=_inputs0_0;
				//Outputs
				List<PropertyDescription> _outputs0_0 = new List<PropertyDescription>();
                PropertyDescription pd8 = new PropertyDescription();
                pd8.DomainClassType = typeof(UNIMI.SoilT.Interfaces.States);
                pd8.PropertyName = "SoilTemperatureByLayers";
                pd8.PropertyType = ((UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers)).ValueType.TypeForCurrentValue;
                pd8.PropertyVarInfo = (UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers);
                _inputs0_0.Add(pd8);
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
				get { return "Strategy for the calculation of soil temperature with SWAT method. Reference: Neitsch,S.L., Arnold, J.G., Kiniry, J.R., Williams, J.R., King, K.W. Soil and Water Assessment Tool. Theoretical documentation. Version 2000. http://swatmodel.tamu.edu/media/1290/swat2000theory.pdf"; }
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
				get { return "Soil"; }
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

			
			public System.Double LagCoefficient
			{ 
				get {
						VarInfo vi= _modellingOptionsManager.GetParameterByName("LagCoefficient");
						if (vi != null) return (System.Double)vi.CurrentValue ;
						else throw new Exception("Parameter 'LagCoefficient' not found in strategy 'SoilTemperatureSWAT'");
				 } set {
							VarInfo vi = _modellingOptionsManager.GetParameterByName("LagCoefficient");
							if (vi != null)  vi.CurrentValue=value;
						else throw new Exception("Parameter 'LagCoefficient' not found in strategy 'SoilTemperatureSWAT'");
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
                LagCoefficientVarInfo.Name = "LagCoefficient";
				LagCoefficientVarInfo.Description =" Lag coefficient that controls the influence of the previous day's temperature on the current day's temperature";
				LagCoefficientVarInfo.MaxValue = 1;
				LagCoefficientVarInfo.MinValue = 0;
				LagCoefficientVarInfo.DefaultValue = 0.8;
				LagCoefficientVarInfo.Units = "dimensionless";
				LagCoefficientVarInfo.ValueType = CRA.ModelLayer.Core.VarInfoValueTypes.GetInstanceForName("Double");

				
       
			}

			//Parameters static VarInfo list 
			
				private static VarInfo _LagCoefficientVarInfo= new VarInfo();
				/// <summary> 
				///LagCoefficient VarInfo definition
				/// </summary>
				public static VarInfo LagCoefficientVarInfo
				{
					get { return _LagCoefficientVarInfo; }
				}					
			
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
					
					
					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();

                    int numberOfLayer = states.HeatCapacity.Length;

                    for (int i = 0; i < numberOfLayer; i++)
                    {
                        UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers.CurrentValue = states1.SoilTemperatureByLayers;
                        RangeBasedCondition r5 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers);
                        if (r5.ApplicableVarInfoValueTypes.Contains(UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers.ValueType)) { prc.AddCondition(r5); }
                    }

					

					//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section4
					//Code written below will not be overwritten by a future code generation

        

					//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
					//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section4 

					//Get the evaluation of postconditions
					string postConditionsResult =pre.VerifyPostconditions(prc, callID);
					//if we have errors, send it to the configured output 
					if(!string.IsNullOrEmpty(postConditionsResult)) { pre.TestsOut(postConditionsResult, true, "PostConditions errors in component UNIMI.SoilT.Strategies.Soil, strategy " + this.GetType().Name ); }
					return postConditionsResult;
				}
				catch (Exception exception)
				{
					//Uncomment the next line to use the trace
					//TraceStrategies.TraceEvent(System.Diagnostics.TraceEventType.Error, 1001,	"Strategy: " + this.GetType().Name + " - Unhandled exception running post-conditions");

					string msg = "Component UNIMI.SoilT.Strategies.Soil, " + this.GetType().Name + ": Unhandled exception running post-condition test. ";
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
					
					UNIMI.SoilT.Interfaces.StatesVarInfo.SoilProfileDepth.CurrentValue=states.SoilProfileDepth;
					UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature.CurrentValue=states.SurfaceSoilTemperature;
					UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureAnnualAverage.CurrentValue=exogenous.AirTemperatureAnnualAverage;

					//Create the collection of the conditions to test
					ConditionsCollection prc = new ConditionsCollection();
					Preconditions pre = new Preconditions();

                    int numberOfLayer = states.HeatCapacity.Length;

                    for (int i = 0; i < numberOfLayer; i++)
                    {
                        UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers.CurrentValue = states.SoilTemperatureByLayers;
                        UNIMI.SoilT.Interfaces.StatesVarInfo.BulkDensity.CurrentValue = states.BulkDensity;
                        UNIMI.SoilT.Interfaces.StatesVarInfo.VolumetricWaterContent.CurrentValue = states.VolumetricWaterContent;
                        UNIMI.SoilT.Interfaces.StatesVarInfo.LayerThickness.CurrentValue = states.LayerThickness;
                        RangeBasedCondition r6 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers);
                        if (r6.ApplicableVarInfoValueTypes.Contains(UNIMI.SoilT.Interfaces.StatesVarInfo.SoilTemperatureByLayers.ValueType)) { prc.AddCondition(r6); }
                        RangeBasedCondition r4 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.BulkDensity);
                        if (r4.ApplicableVarInfoValueTypes.Contains(UNIMI.SoilT.Interfaces.StatesVarInfo.BulkDensity.ValueType)) { prc.AddCondition(r4); }
                        RangeBasedCondition r5 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.VolumetricWaterContent);
                        if (r5.ApplicableVarInfoValueTypes.Contains(UNIMI.SoilT.Interfaces.StatesVarInfo.VolumetricWaterContent.ValueType)) { prc.AddCondition(r5); }
                        RangeBasedCondition r7 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.LayerThickness);
                        if (r7.ApplicableVarInfoValueTypes.Contains(UNIMI.SoilT.Interfaces.StatesVarInfo.LayerThickness.ValueType)) { prc.AddCondition(r7); }
                    }
					
					RangeBasedCondition r1 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.SoilProfileDepth);
					if(r1.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.StatesVarInfo.SoilProfileDepth.ValueType)){prc.AddCondition(r1);}
					RangeBasedCondition r2 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature);
					if(r2.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.StatesVarInfo.SurfaceSoilTemperature.ValueType)){prc.AddCondition(r2);}
					RangeBasedCondition r3 = new RangeBasedCondition(UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureAnnualAverage);
					if(r3.ApplicableVarInfoValueTypes.Contains( UNIMI.SoilT.Interfaces.ExogenousVarInfo.AirTemperatureAnnualAverage.ValueType)){prc.AddCondition(r3);}
					prc.AddCondition(new RangeBasedCondition(_modellingOptionsManager.GetParameterByName("LagCoefficient")));

					

					//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section3
					//Code written below will not be overwritten by a future code generation

        

					//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
					//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section3 
								
					//Get the evaluation of preconditions;					
					string preConditionsResult =pre.VerifyPreconditions(prc, callID);
					//if we have errors, send it to the configured output 
					if(!string.IsNullOrEmpty(preConditionsResult)) { pre.TestsOut(preConditionsResult, true, "PreConditions errors in component UNIMI.SoilT.Strategies.Soil, strategy " + this.GetType().Name ); }
					return preConditionsResult;
				}
				catch (Exception exception)
				{
					//Uncomment the next line to use the trace
					//	TraceStrategies.TraceEvent(System.Diagnostics.TraceEventType.Error, 1002,"Strategy: " + this.GetType().Name + " - Unhandled exception running pre-conditions");

					string msg = "Component UNIMI.SoilT.Strategies.Soil, " + this.GetType().Name + ": Unhandled exception running pre-condition test. ";
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



			//Conversion to mm
			double _SoilProfileDepthmm = states.SoilProfileDepth * 1000;
			//Conversion to mm
			//Total water content
			double _TotalWaterContentmm = 0;
			for (int i = 0; i < states.LayerThickness.Length; i++)
			{
				_TotalWaterContentmm += states.VolumetricWaterContent[i] * states.LayerThickness[i];
			}
			_TotalWaterContentmm = _TotalWaterContentmm * 1000;
			//internal variables
			double _MaximumDumpingDepth = 0;
			double _DumpingDepth = 0;
			double _ScalingFactor = 0;
			double _DepthBottom = 0;
			double _RatioCenter = 0;
			double _DepthFactor = 0;

			//First layer
			double _DepthCenterLayer = states.LayerThickness[0] * 1000 / 2;

			_MaximumDumpingDepth = 1000 +
								  (2500 * states.BulkDensity[0]) /
								  (states.BulkDensity[0] + 686 * Math.Exp(-5.63 * states.BulkDensity[0]));
			_ScalingFactor = _TotalWaterContentmm / ((0.356 - 0.144 * states.BulkDensity[0]) * _SoilProfileDepthmm);
			_DumpingDepth = _MaximumDumpingDepth * Math.Exp((Math.Log(500 / _MaximumDumpingDepth)) *
							Math.Pow((1 - _ScalingFactor) / (1 + _ScalingFactor), 2));
			_RatioCenter = _DepthCenterLayer / _DumpingDepth;
			_DepthFactor = _RatioCenter / (_RatioCenter + Math.Exp(-0.867 - 2.078 * _RatioCenter));

			states.SoilTemperatureByLayers[0] = LagCoefficient * states.SoilTemperatureByLayers[0] +
												   (1 - LagCoefficient) *
												   (_DepthFactor *
													(exogenous.AirTemperatureAnnualAverage - states.SurfaceSoilTemperature) +
													states.SurfaceSoilTemperature);
			//other layers
			for (int i = 1; i < states.LayerThickness.Length; i++)
			{
				_DepthBottom = _DepthBottom + states.LayerThickness[i - 1] * 1000;
				_DepthCenterLayer = _DepthBottom + states.LayerThickness[i] * 1000 / 2;
				_MaximumDumpingDepth = 1000 +
								 (2500 * states.BulkDensity[i]) /
								 (states.BulkDensity[i] + 686 * Math.Exp(-5.63 * states.BulkDensity[i]));
				_ScalingFactor = _TotalWaterContentmm / ((0.356 - 0.144 * states.BulkDensity[i]) * _SoilProfileDepthmm);
				_DumpingDepth = _MaximumDumpingDepth * Math.Exp((Math.Log(500 / _MaximumDumpingDepth)) *
							Math.Pow((1 - _ScalingFactor) / (1 + _ScalingFactor), 2));
				_RatioCenter = _DepthCenterLayer / _DumpingDepth;
				_DepthFactor = _RatioCenter / (_RatioCenter + Math.Exp(-0.867 - 2.078 * _RatioCenter));

				states.SoilTemperatureByLayers[i] = LagCoefficient * states.SoilTemperatureByLayers[i] +
												  (1 - LagCoefficient) *
												  (_DepthFactor *
												   (exogenous.AirTemperatureAnnualAverage - states.SurfaceSoilTemperature) +
												   states.SurfaceSoilTemperature);
			}




			//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.
			//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section1 
		}

		public void Init(UNIMI.SoilT.Interfaces.Rates rates,UNIMI.SoilT.Interfaces.States states,UNIMI.SoilT.Interfaces.Auxiliary auxiliary,UNIMI.SoilT.Interfaces.States states1,UNIMI.SoilT.Interfaces.Exogenous exogenous,UNIMI.SoilT.Interfaces.StatesExternal statesexternal,CRA.AgroManagement.ActEvents actevents)
		{

			states.SoilTemperatureByLayers = new double[states.LayerThickness.Length];

			for (int i = 0; i < states.LayerThickness.Length; i++)
			{
				states.SoilTemperatureByLayers[i] = 15;
			}

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
     - Name: SoilTemperatureSWAT -Version: 001, -Time step: 1
     - Description:
                 * Title: SoilTemperatureSWAT model
                 * Authors: simone.bregaglio@unimi.it
                 * Reference: http://bioma.jrc.ec.europa.eu/ontology/JRC_MARS_biophysical_domain.owl
                 * Institution: University Of Milan
                 * ExtendedDescription: Strategy for the calculation of soil temperature with SWAT method. Reference: Neitsch,S.L., Arnold, J.G., Kiniry, J.R., Williams, J.R., King, K.W. Soil and Water Assessment Tool. Theoretical documentation. Version 2000. http://swatmodel.tamu.edu/media/1290/swat2000theory.pdf
                 * ShortDescription: Strategy for the calculation of soil temperature with SWAT method
     - inputs:
                 * name: VolumetricWaterContent
                               ** description : Volumetric soil water content
                               ** inputtype : variable
                               ** variablecategory : exogenous
                               ** datatype : DOUBLEARRAY
                               ** len : 
                               ** max : 0.8
                               ** min : 0
                               ** default : 0.25
                               ** unit : m3 m-3
                 * name: SurfaceSoilTemperature
                               ** description : Average surface soil temperature
                               ** inputtype : variable
                               ** variablecategory : auxiliary
                               ** datatype : DOUBLE
                               ** max : 60
                               ** min : -60
                               ** default : 25
                               ** unit : degC
                 * name: LayerThickness
                               ** description : Soil layer thickness
                               ** inputtype : parameter
                               ** parametercategory : constant
                               ** datatype : DOUBLEARRAY
                               ** len : 
                               ** max : 3
                               ** min : 0.005
                               ** default : 0.05
                               ** unit : m
                 * name: LagCoefficient
                               ** description : Lag coefficient that controls the influence of the previous day's temperature on the current day's temperature
                               ** inputtype : parameter
                               ** parametercategory : constant
                               ** datatype : DOUBLE
                               ** max : 1
                               ** min : 0
                               ** default : 0.8
                               ** unit : dimensionless
                 * name: SoilTemperatureByLayers
                               ** description : Soil temperature of each layer
                               ** inputtype : variable
                               ** variablecategory : state
                               ** datatype : DOUBLEARRAY
                               ** len : 
                               ** max : 60
                               ** min : -60
                               ** default : 15
                               ** unit : degC
                 * name: AirTemperatureAnnualAverage
                               ** description : Annual average air temperature
                               ** inputtype : parameter
                               ** parametercategory : constant
                               ** datatype : DOUBLE
                               ** max : 50
                               ** min : -40
                               ** default : 15
                               ** unit : degC
                 * name: BulkDensity
                               ** description : Bulk density
                               ** inputtype : parameter
                               ** parametercategory : constant
                               ** datatype : DOUBLEARRAY
                               ** len : 
                               ** max : 1.8
                               ** min : 0.9
                               ** default : 1.3
                               ** unit : t m-3
                 * name: SoilProfileDepth
                               ** description : Soil profile depth
                               ** inputtype : parameter
                               ** parametercategory : constant
                               ** datatype : DOUBLE
                               ** max : 50
                               ** min : 0
                               ** default : 3
                               ** unit : m
     - outputs:
                 * name: SoilTemperatureByLayers
                               ** description : Soil temperature of each layer
                               ** datatype : DOUBLEARRAY
                               ** variablecategory : state
                               ** len : 
                               ** max : 60
                               ** min : -60
                               ** unit : degC
							   

*/
//%%CyML Description End%%