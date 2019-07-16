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
        public bool LFSR_SoftDoctor(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 59)
                {
                    int slotNumber;
                    int labelIndex = GetFrontLabelIndex_SlotWise("TS", 2);
                    dataPointDefinition.Title = dataPointDefinition.Title.Replace("/0", "/TS");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Average"))
                    {
                        if (dataPointDefinition.Title.Contains("Doctor Free Height - in"))
                        {
                            slotNumber = 1;
                        }
                        else
                        {
                            slotNumber = 2;
                        }
                        string[] labelArray = new string[] { "TS", "55", "110", "165", "220" };
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
                    else if (dataPointDefinition.Title.Contains("Doctor Free Height - in"))
                    {
                        if (dataPointDefinition.Title.Contains("Minimum"))
                        {
                            value = "1.313";
                        }
                        else if (dataPointDefinition.Title.Contains("Maximum"))
                        {
                            value = "1.438";
                        }
                        else
                        {
                            value_Label = dataPointDefinition.Title.Replace("Doctor Free Height - in/", "");
                            slotNumber = 1;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);                   // To handle Values of type 2.356
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("Holder to Roll (loaded) - in"))
                    {
                        if (dataPointDefinition.Title.Contains("Minimum"))
                        {
                            value = "0.813";
                        }
                        else if (dataPointDefinition.Title.Contains("Maximum"))
                        {
                            value = "0.938";
                        }
                        else
                        {
                            value_Label = dataPointDefinition.Title.Replace("Holder to Roll (loaded) - in/", "");
                            slotNumber = 2;
                            value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                            value = RemoveGeneralError_SteamHood(value);                   // To handle Values of type 2.356
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 60)
                {
                    int labelIndex = GetFrontLabelIndex_SlotWise("TS", 1);

                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Minimum"))
                    {
                        value = "34.5";
                    }
                    else if (dataPointDefinition.Title.Contains("Maximum"))
                    {
                        value = "38.5";
                    }
                    else if (dataPointDefinition.Title.Contains("Doctor Holder Angle (loaded)"))
                    {
                        value_Label = dataPointDefinition.Title.Replace("Doctor Holder Angle (loaded)/", "");
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
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
                // Add to Data List
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
