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
        public bool DuoCleaner(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            OCRText = OCRText.Replace("\r\nTS Distance\r\n", "\r\n").Replace("\r\n1.000\r\n", "\r\n").Replace("\r\n0.125\r\n", "\r\n").Replace("\r\nDS Distance\r\n", "\r\n").Replace("\r\nTS\r\n", "\r\n").Replace("\r\nDS\r\n", "\r\n");

            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 0 && item.DPShortName != null))
            {
                if (dataPointDefinition.DataSetDefinitionId == 486)
                {
                    int labelIndex = GetLabelIndex("Duocleaner Head Distance");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        string tempOcrText = OCRText;
                        OCRText = OCRText.Substring(0, GetLabelIndex("Verify Nozzle runs just to deckle area of roll"));
                        value_Label = "Range";
                        int slotNumber = 0;
                        if (dataPointDefinition.Title.Contains("TS"))
                        {
                            slotNumber = 1;
                        }
                        else
                        {
                            slotNumber = 2;
                        }
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                        OCRText = tempOcrText;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 487)
                {
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Verify nozzle setup as shown below"));
                    int labelIndex = GetLabelIndex("Verify Nozzle runs just to deckle area of roll");
                    value_Label = "Yes or No";
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("TS"))
                    {
                        slotNumber = 1;
                    }
                    else
                    {
                        slotNumber = 2;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);                          //As value only yes or no;
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 488)
                {
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("General Comments"));
                    int labelIndex = GetLabelIndex("Yes or No");
                    value_Label = "Verify nozzle setup as shown below";
                    value = GetFrontLabelValue_PulperCurve(value_Label, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);                             //As value only yes or no;
                    OCRText = tempOcrText;
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
