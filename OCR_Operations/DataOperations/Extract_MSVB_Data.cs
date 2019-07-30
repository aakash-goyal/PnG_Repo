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
        public bool MSVB(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);
            OCRText = OCRText.Replace("TS\r\n", "").Replace("DS\r\n", "");

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = GetLabelIndex("MSVB Data Sheet");
                if (dataPointDefinition.DataSetDefinitionId == 201)
                {
                    if (dataPointDefinition.Title.Contains("Target") || dataPointDefinition.Title.Contains("Average"))  // For title like Target, Average
                    {     // Slot 6 Width coming after Target for blank form if issue persist with filled, make necessary changes
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                        value_Label = dataPointDefinition.Title.Remove(dataPointDefinition.Title.Length - 7);
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);                      //As value will be like 0.125
                    }
                    else if (dataPointDefinition.Title.Contains("Maximum"))
                    {
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                        value_Label = "Target";
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                        if (value != "")
                        {
                            value = (Convert.ToInt32(value) + 0.010).ToString();
                        }
                    }
                    else if (dataPointDefinition.Title.Contains("Minimum"))
                    {
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                        value_Label = "Target";
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                        if (value != "")
                        {
                            value = (Convert.ToInt32(value) - 0.010).ToString();
                        }
                    }
                    else if (dataPointDefinition.Title.Length < 11)      // To take only slot with row no. Title
                    {
                        int rowNumber = Convert.ToInt32(dataPointDefinition.Title.Remove(dataPointDefinition.Title.Length - 7));
                        int slotNumber = 6 * rowNumber + Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                        value_Label = "Average";
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);                      //As value will be like 0.125
                    }
                    else  //Ignore other placeholder titles
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 202)
                {
                    labelIndex = GetLabelIndex("Deckle Position");
                    if (dataPointDefinition.Title.Contains("TS"))
                    {
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                        value_Label = "Slot 6 Width";
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from TS & DS values Table
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        int slotNumber = 6 + Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                        value_Label = "Slot 6 Width";
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from TS & DS values Table
                    }
                    else  //To handle database discrepancies
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 203)
                {
                    labelIndex = GetLabelIndex("MSVB Box Geometry Measurement");
                    if (dataPointDefinition.Title.Contains("History"))      //To handle History field in database
                    {
                        continue;
                    }
                    if (dataPointDefinition.Title.Contains("TS"))
                    {
                        int slotNumber = 1;
                        value_Label = dataPointDefinition.Title.Replace(" TS", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from MSVB Box Geometry Measurement values Table
                    }
                    else if (dataPointDefinition.Title.Contains("DS"))
                    {
                        int slotNumber = 2;
                        value_Label = dataPointDefinition.Title.Replace(" DS", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from MSVB Box Geometry Measurement values Table
                    }
                    else if (dataPointDefinition.Title.Contains("Target"))
                    {
                        int slotNumber = 3;
                        value_Label = dataPointDefinition.Title.Replace(" Target", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from MSVB Box Geometry Measurement values Table
                    }
                    else if (dataPointDefinition.Title.Contains("Range"))
                    {
                        int slotNumber = 4;
                        value_Label = dataPointDefinition.Title.Replace(" Range", "");
                        value = GetSlotFrontLabelValue_RefinerCurve(value_Label, slotNumber, labelIndex);
                        // Function to remove error from MSVB Box Geometry Measurement values Table
                    }
                    // Add to handle Maximum, Minimum fields
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
