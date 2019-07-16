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
        public bool Hydrofoil(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 391)
                {
                    int slotNumber = 0;
                    int labelIndex = GetFrontLabelIndex_SlotWise("LE Box to Cloth", 1);
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        if (dataPointDefinition.Title.Contains("LE Box to cloth"))
                        {
                            slotNumber += 1;
                        }
                        else if (dataPointDefinition.Title.Contains("TE Box to cloth"))
                        {
                            slotNumber += 2;
                        }
                        if (dataPointDefinition.Title.Contains("TS"))
                        {
                            slotNumber += 2;
                        }
                        else if (dataPointDefinition.Title.Contains("DS"))
                        {
                            slotNumber += 4;
                        }
                        value_Label = "Minimum";
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Remove Error for LE information
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 392)
                {
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
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
