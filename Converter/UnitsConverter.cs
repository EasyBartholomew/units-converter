using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Converter
{
    public static class UnitsConverter
    {
        #region Data Members

        /// <summary>
        /// Map every units his dimensions 
        /// </summary>
        public static Dictionary<string, IEnumerable<string>> UnitsDimensionsMap { get; private set; }

        /// <summary>
        /// Hold map (by name on unit) of static "FromXXX" methods
        /// </summary>
        private static Dictionary<string, IEnumerable<MethodInfo>> UnitsDimensionsMethodsMap;

        #endregion

        #region Init

        static UnitsConverter()
        {
            ReflectUnits();
        }

        /// <summary>
        /// Reflect unitsNet DLL to get all posible units and hits dimension
        /// </summary>
        private static void ReflectUnits()
        {
            UnitsDimensionsMap = new Dictionary<string, IEnumerable<string>>();
            UnitsDimensionsMethodsMap = new Dictionary<string, IEnumerable<MethodInfo>>();

            try
            {
                // get assebly
                var asm = Assembly.Load("UnitsNet");
                var nameSpace = "UnitsNet";

                // get types (include units)
                var UnitsClasses = asm.GetTypes().Where(p =>
                     p.Namespace == nameSpace &&
                     p.IsPublic
                ).OrderBy(p => p.Name).ToList();

                // on every type (posible its a unit)
                foreach (var unit in UnitsClasses)
                {
                    // dimention list of current unit
                    List<string> dimensions = new List<string>();
                    List<MethodInfo> staticFromMethods = new List<MethodInfo>();

                    // On every method in type
                    foreach (var method in unit.GetMethods().OrderBy(m => m.Name))
                    {
                        // Get only method with only double as parameter
                        // And Starts with "FromX"
                        
                         
                        var parameters = method.GetParameters();

                        bool validParams = false;
                        foreach (var param in parameters)
                        {
                            if (param.ParameterType.FullName == "System.Double")
                                validParams = true;
                        }

                        if (method.Name.Contains("From") &&
                            method.Name.Length > 4 &&
                            validParams &&
                            parameters.Count() == 1)
                        {
                            // Add name only of dimention
                            dimensions.Add(method.Name.Substring(4));
                            staticFromMethods.Add(method);
                        }
                    }

                    // if there is dimention its meen it a valied unit type 
                    if (dimensions.Count > 0)
                    {
                        UnitsDimensionsMap.Add(unit.Name, dimensions);
                        UnitsDimensionsMethodsMap.Add(unit.Name, staticFromMethods);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region API 

        /// <summary>
        /// Convert unit dimension
        /// </summary>
        /// <param name="unit">Name of unit (like "Angle")</param>
        /// <param name="fromDimension">Name of current value dimention to convert (like "Degrees")</param>
        /// <param name="toDimension">Name of wanted dimention to get (like "Degrees")</param>
        /// <param name="value">Current dimention value</param>
        /// <param name="isSuccess">true if success</param>
        /// <returns>value of wanted dimention</returns>
        public static double Convert(string unit, string fromDimension, string toDimension, double value, out bool isSuccess)
        {
            double result = 0;
            try
            {
                // Get Units methods  
                var UnitTypeObjects = UnitsDimensionsMethodsMap[unit];

                // Get wanted From method
                MethodInfo fromMethod = (from method in UnitTypeObjects
                                         where method.Name == "From" + fromDimension
                                         select method).FirstOrDefault();
                
                // Create a type of unitsNet by static "FromXXX" method with wanted value as parameter
                var unitsNetObj = fromMethod.Invoke(null, new object[] { value });

                // Get the wanted dimention by reflection 
                result = (double)unitsNetObj.GetType().GetProperty(toDimension).GetValue(unitsNetObj);

                isSuccess = true;

            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return result;
        }

        #endregion

    }
}
