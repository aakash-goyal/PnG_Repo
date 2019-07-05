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
        public bool Pressure_Screen(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Thin Stock Screen CPE");  // GetMeasurement not efficient use Refiner Plate Methods

                if (dataPointDefinition.DataSetDefinitionId == 101)
                {
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    // Function to remove error from Values of Type Of Screen
                }
                else if (dataPointDefinition.DataSetDefinitionId >= 102 && dataPointDefinition.DataSetDefinitionId <= 107)
                {
                    int labelSlotNum = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.IndexOf("/") - 1, 1));
                    labelIndex = GetFrontLabelIndex_SlotWise("Internal Paddle Bottom Clearance", labelSlotNum); //Screen Table Number need to considered
                    value_Label = "Internal Paddle Bottom Clearance";
                    if (dataPointDefinition.Title.Contains("External Paddle Top Clearance"))
                    {
                        int slotNumber = (6 * Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1, 1))) - 6 + 2;  // 6 for previous row   //Also check rownumber come in OCRTEXT or not
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of External top Clearance
                    }
                    else if (dataPointDefinition.Title.Contains("Internal Paddle Top Clearance"))
                    {
                        int slotNumber = (6 * Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1, 1))) - 6 + 3;  // 6 for previous row
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of Internal top Clearance
                    }
                    else if (dataPointDefinition.Title.Contains("External Paddle Bottom Clearance"))
                    {
                        int slotNumber = (6 * Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1, 1))) - 6 + 5;  // 6 for previous row
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of External bottom Clearance
                    }
                    else if (dataPointDefinition.Title.Contains("Internal Paddle Bottom Clearance"))
                    {
                        int slotNumber = (6 * Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1, 1))) - 6 + 6;  // 6 for previous row
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from Values of External bottom Clearance
                    }
                    else
                    {
                        continue;
                    }
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
