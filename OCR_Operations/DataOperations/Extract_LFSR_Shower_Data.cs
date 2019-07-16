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
        public bool LFSR_Shower(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 63)
                {
                    int slotNumber;
                    int labelIndex = GetFrontLabelIndex_SlotWise("TS", 2);
                    if (dataPointDefinition.Title.Contains("Average"))
                    {
                        if (dataPointDefinition.Title.Contains("Needle Shower Distance"))
                        {
                            slotNumber = 1;
                        }
                        else
                        {
                            slotNumber = 2;
                        }
                        string[] labelArray = new string[] { "O", "55", "110", "165", "220" };
                        double localvalue = 0;
                        foreach (string label in labelArray)
                        {
                            value = GetSlotLabelValue_RefinerPlate(label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);
                            if (value != "")
                            {
                                localvalue += Convert.ToDouble(value);
                            }
                        }
                        value = (localvalue / 5).ToString();                   // To Calculate average
                    }
                    else if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        if (dataPointDefinition.Title.Contains("Needle Shower Distance"))
                        {
                            value_Label = dataPointDefinition.Title.Replace("Needle Shower Distance/", "");
                            slotNumber = 1;
                        }
                        else
                        {
                            value_Label = dataPointDefinition.Title.Replace("Fan Shower Distance/", "");
                            slotNumber = 2;
                        }
                        if (value_Label == "0")
                        {
                            value_Label = "O";
                        }
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);                   // To handle Values of type 2.356
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 64)
                {
                    int slotNumber;
                    int labelIndex = GetFrontLabelIndex_SlotWise("TS", 1);

                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("History"))
                    {
                        continue;
                    }
                    else
                    {
                        if (dataPointDefinition.Title.Contains("Needle Shower Angle"))
                        {
                            value_Label = dataPointDefinition.Title.Replace("Needle Shower Angle/", "");
                            slotNumber = 1;
                        }
                        else
                        {
                            value_Label = dataPointDefinition.Title.Replace("Fan Shower Angle/", "");
                            slotNumber = 2;
                        }
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 65)
                {
                    if (dataPointDefinition.IsConstantValue == 1 && dataPointDefinition.ConstantValue != null)
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
