using OCR_Operations.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        public bool Refiner_Mechanical_Inspection(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Refiner Mechanical CPE Data");  // GetMeasurement not efficient use Refiner Plate Methods
                if (dataPointDefinition.DataSetDefinitionId == 9)
                {
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    // Function to remove error from Values of Refiner#
                }
                else if (dataPointDefinition.DataSetDefinitionId == 10)
                {
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    // Function to remove error from Values of Door Closure Surface
                }
                else if (dataPointDefinition.DataSetDefinitionId == 11)
                {
                    if (dataPointDefinition.Title.Equals("Tram Alignment"))    // Target And max are ignored by It now
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Tram Alignment
                    }
                    else if (dataPointDefinition.Title.Contains("Rotating Assembly"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Rotating Assembly: ", "");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Rotating Assembly Time
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 12)
                {
                    if (dataPointDefinition.Title.Equals("Backlash (# of handwheel turns)"))    // Target And max are ignored by It now
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Adjusting Mechanism Table
                    }
                    else if (dataPointDefinition.Title.Equals("Shear Pin Condition"))    // Target And max are ignored by It now
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Adjusting Mechanism Table
                    }
                    else if (dataPointDefinition.Title.Equals("Clutch Coupling"))    // Target And max are ignored by It now
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Function to remove error from Values of Adjusting Mechanism Table
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 13)
                {
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    // Function to remove error from Values of Spline Components Table
                }
                else
                {
                    continue;
                }
                // Add to Data List
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
