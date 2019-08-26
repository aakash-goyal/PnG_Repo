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
        public bool FeltCleaning_Box(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            int ds2Label = GetLabelIndex_SlotWise("\r\nDS", 2);
            OCRText = OCRText.Substring(0, ds2Label + 1) + OCRText.Substring(ds2Label + 4);

            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 0))
            {
                int labelIndex = GetLabelIndex("FCB CPE Data");
                if (dataPointDefinition.DataSetDefinitionId == 157)
                {
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.EndsWith("_1"))
                    {
                        value_Label = "TS";
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.EndsWith("_2"))
                    {
                        labelIndex = GetLabelIndex_SlotWise("TS", 2);
                        value_Label = "TS";
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.EndsWith("_36"))
                    {
                        labelIndex = GetLabelIndex_SlotWise("DS", 2);
                        value_Label = "DS";
                        slotNumber = 1;
                    }
                    else
                    {
                        value_Label = "DS";
                        int columnNum = Convert.ToInt32(dataPointDefinition.Title.Last());
                        slotNumber = 2 * (columnNum - 2) + 1;
                    }
                    if (dataPointDefinition.Title.StartsWith("#2"))
                    {
                        slotNumber += 1;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 158)
                {
                    value_Label = dataPointDefinition.Title.Replace("FCB #1 - ", "");
                    int slotNumber = 0;
                    string tempOCRText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("FCB #2 Box Geometry Measurement") + 1);
                    if (dataPointDefinition.Title.Contains("Lead In Angle") && (!dataPointDefinition.Title.Contains("TS") && !dataPointDefinition.Title.Contains("DS")))
                    {
                        continue;
                    }
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        value_Label = value_Label.Replace(" TS", "");
                        slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        value_Label = value_Label.Replace(" DS", "");
                        slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("TRGT"))
                    {
                        value_Label = value_Label.Replace(" TRGT", "");
                        slotNumber = 3;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("RANGE"))
                    {
                        value_Label = value_Label.Replace(" RANGE", "");
                        slotNumber = 4;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        OCRText = tempOCRText;
                        continue;
                    }
                    OCRText = tempOCRText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 159)
                {
                    value_Label = dataPointDefinition.Title.Replace("FCB #2 - ", "");
                    int slotNumber = 0;
                    string tempOCRText = OCRText;
                    OCRText = OCRText.Substring(GetLabelIndex("FCB #2 Box Geometry Measurement"), GetLabelIndex("1st Support Bar(Towards WE)") - GetLabelIndex("FCB #2 Box Geometry Measurement"));
                    if (dataPointDefinition.Title.Contains("Lead In Angle") && (!dataPointDefinition.Title.Contains("TS") && !dataPointDefinition.Title.Contains("DS")))
                    {
                        continue;
                    }
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        value_Label = value_Label.Replace(" TS", "");
                        slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        value_Label = value_Label.Replace(" DS", "");
                        slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("TRGT"))
                    {
                        value_Label = value_Label.Replace(" TRGT", "");
                        slotNumber = 3;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("RANGE"))
                    {
                        value_Label = value_Label.Replace(" RANGE", "");
                        slotNumber = 4;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        OCRText = tempOCRText;
                        continue;
                    }
                    OCRText = tempOCRText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 160)
                {
                    value_Label = dataPointDefinition.Title.Replace("1st Support Bar - ", "");
                    int slotNumber = 0;
                    string tempOCRText = OCRText;
                    OCRText = OCRText.Substring(GetLabelIndex("1st Support Bar(Towards WE)"), GetLabelIndex("2st Support Bar (Towards DE)") - GetLabelIndex("1st Support Bar(Towards WE)"));

                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        value_Label = value_Label.Replace(" TS", "");
                        slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        value_Label = value_Label.Replace(" DS", "");
                        slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        OCRText = tempOCRText;
                        continue;
                    }
                    OCRText = tempOCRText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 161)
                {
                    value_Label = dataPointDefinition.Title.Replace("1st Support Bar - ", "");
                    int slotNumber = 0;
                    string tempOCRText = OCRText;
                    OCRText = OCRText.Substring(GetLabelIndex("2st Support Bar (Towards DE)"));
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("TS"))
                    {
                        value_Label = value_Label.Replace(" TS", "");
                        slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        value_Label = value_Label.Replace(" DS", "");
                        slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        OCRText = tempOCRText;
                        continue;
                    }
                    OCRText = tempOCRText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 162)
                {
                    labelIndex = GetLabelIndex_SlotWise("TS", 2);
                    value_Label = dataPointDefinition.Title.Substring(0, 2);
                    int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Last());
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
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
