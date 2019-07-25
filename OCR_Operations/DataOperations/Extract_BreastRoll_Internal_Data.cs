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
        public bool BreastRoll_Internal(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 300)
                {
                    int labelIndex = GetLabelIndex("Range");
                    int slotNumber = 0;
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        slotNumber = 1;
                        value_Label = dataPointDefinition.Title.Replace(" - TS", "");
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        //Error Function
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        slotNumber = 2;
                        value_Label = dataPointDefinition.Title.Replace(" - DS", "");
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        //Error Function
                    }
                    else if (dataPointDefinition.Title.Contains("Target"))
                    {
                        slotNumber = 3;
                        value_Label = dataPointDefinition.Title.Replace(" - Target", "");
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        //Error Function
                    }
                    else if (dataPointDefinition.Title.Contains("Range"))
                    {
                        slotNumber = 4;
                        value_Label = dataPointDefinition.Title.Replace(" - Range", "");
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        //Error Function
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
