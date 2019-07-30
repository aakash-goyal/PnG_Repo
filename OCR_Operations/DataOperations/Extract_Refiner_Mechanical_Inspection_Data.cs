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
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Refiner Mechanical CPE Data");

                if (dataPointDefinition.DataSetDefinitionId == 10)            // Check on cpe site if it's can have only limited values
                {
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveError_String_WireEdge(value);
                }
                else if (dataPointDefinition.DataSetDefinitionId == 11)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Rotating Assembly"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Rotating Assembly: ", "");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 12)
                {
                    if (dataPointDefinition.Title.Contains("Blank"))
                    {
                        continue;
                    }
                    else if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Equals("Backlash (# of handwheel turns)"))    // Target And max are ignored by It now
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Equals("Shear Pin Condition"))
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveError_String_WireEdge(value);
                    }
                    else if (dataPointDefinition.Title.Equals("Clutch Coupling"))    // Target And max are ignored by It now
                    {
                        value_Label = dataPointDefinition.Title;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveError_String_WireEdge(value);
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
                    value = RemoveError_String_WireEdge(value);
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
