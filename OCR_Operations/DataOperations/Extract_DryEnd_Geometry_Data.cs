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
        public bool DryEnd_Geometry(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 305)
                {
                    int labelIndex = GetLabelIndex("DS Trailing");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        int slotNumber = 0;
                        if (dataPointDefinition.Title.Contains("TS Leading Edge"))
                        {
                            slotNumber = 2;
                            value_Label = dataPointDefinition.Title.Replace(" - TS Leading Edge", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            //Error function
                        }
                        else if (dataPointDefinition.Title.Contains("TS Trailing Edge"))
                        {
                            slotNumber = 3;
                            value_Label = dataPointDefinition.Title.Replace(" - TS Trailing Edge", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            //Error function
                        }
                        else if (dataPointDefinition.Title.Contains("DS Leading Edge"))
                        {
                            slotNumber = 4;
                            value_Label = dataPointDefinition.Title.Replace(" - DS Leading Edge", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            //Error function
                        }
                        else if (dataPointDefinition.Title.Contains("DS Trailing Edge"))
                        {
                            slotNumber = 5;
                            value_Label = dataPointDefinition.Title.Replace(" - DS Trailing Edge", "");
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            //Error function
                        }
                        else
                        {
                            continue;
                        }
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
