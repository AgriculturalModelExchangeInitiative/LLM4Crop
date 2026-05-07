
using System;
using System.Collections.Generic;
using System.Text;

namespace UNIMI.SoilT.Interfaces
{
	/// <summary>
	/// Tags for Xml parsing.
	/// </summary>
	internal static class XmlTags
	{
		public const string E_VALUES = "Values";
		public const string E_VALUES_E_KEYVALUE = "KeyValue";
		public const string E_VALUES_E_KEYVALUE_A_NAME = "name";
		public const string E_VALUES_E_KEYVALUE_E_PARAMETER = "Parameter";
		public const string E_VALUES_E_KEYVALUE_E_PARAMETER_A_NAME = "name";
		public const string E_VALUES_E_KEYVALUE_E_PARAMETER_E_VALUE = "Value";
	}
}