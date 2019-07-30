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
        public bool VSI(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            //If bottle number remain constant remove them from OCRTEXT and update slotnumbers
            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("Action Needed");
                //To get Only Consistency field values
                if (dataPointDefinition.DataSetDefinitionId == 30)
                {
                    if (dataPointDefinition.Title.Contains("Discharge of Machine Broke Supply Chest"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("Chest", 2);
                    }
                    else if (dataPointDefinition.Title.Contains("Discharge of Machine Broke Surge Chest"))
                    {
                        labelIndex = GetFrontLabelIndex_SlotWise("Chest", 1);
                    }
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Consistency"))
                    {
                        int slotNumber = 2;                                                          // Bottle number will vary
                        value_Label = dataPointDefinition.Title.Replace("/Consistency", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("VSI"))
                    {
                        int slotNumber = 4;                                                          // Bottle number will vary
                        value_Label = dataPointDefinition.Title.Replace("/VSI", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
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

                // Add new Object to list
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
