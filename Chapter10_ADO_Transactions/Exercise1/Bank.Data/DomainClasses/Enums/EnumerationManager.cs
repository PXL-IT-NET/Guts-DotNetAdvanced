using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Bank.Data.DomainClasses.Enums
{
    public class EnumerationManager
    {
        public static Array GetValues(Type enumerationType)
        {
            var allValues = Enum.GetValues(enumerationType);
            ArrayList returnValues = new ArrayList();
            foreach (Enum value in allValues)
            {
                FieldInfo fieldInfo = enumerationType.GetField(value.ToString());
                if (fieldInfo != null)
                {
                    BrowsableAttribute[] browsableAttributes = fieldInfo.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                    if (browsableAttributes?.Length > 0)
                    {
                        //  If the Browsable attribute is false
                        if (browsableAttributes[0].Browsable == false)
                        {
                            // Do not add the enumeration to the list.
                            continue;
                        }
                    }

                    DescriptionAttribute[] descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                    if (descriptionAttributes?.Length > 0)
                    {
                        returnValues.Add(descriptionAttributes[0].Description);
                    }
                    else
                        returnValues.Add(value);
                }
            }
            return returnValues.ToArray();
        }
    }
}