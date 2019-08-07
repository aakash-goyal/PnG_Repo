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
        private int GetLabelIndex_SlotWise(string label, int slotNumber)
        {
            int index_Of_Field = 0, countOfField = 0, index = 0;
            for (int i = 1; i <= slotNumber; i++)
            {
                index_Of_Field = OCRText.IndexOf(label, index_Of_Field);
                countOfField = label.Length;
                while (index_Of_Field == -1)
                {
                    label = label.Remove(label.Length - 1, 1);
                    countOfField = label.Length;
                    index_Of_Field = OCRText.IndexOf(label, index);
                    //Error Handle if label not exist 
                    if (countOfField <= 4)
                    {
                        return -1;        //If label not found
                    }
                }
                index_Of_Field += countOfField;
                index = index_Of_Field;
            }

            return index_Of_Field - countOfField;
        }
        public bool Papilion_RefinerPlate(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                //if (dataPointDefinition.DataSetDefinitionId == 401)        // Check with CPE site   // Check if values will be 0.025 kind only
                //{

                //}
                if (dataPointDefinition.DataSetDefinitionId == 402 || dataPointDefinition.DataSetDefinitionId == 404)
                {
                    int labelIndex = GetLabelIndex("North Section of Fixed Plates Plates");
                    string label = "North Fixed";
                    if (dataPointDefinition.DataSetDefinitionId == 403)
                    {
                        labelIndex = GetLabelIndex("North Section of Rotating Plates Plates");
                        label = "North Rotating";
                    }
                    if (dataPointDefinition.Title.Contains("Serial"))
                    {
                        value_Label = "Serial #";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Last());
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Depth Plate"))
                    {
                        value_Label = "Groove Depth";
                        int plateNum = Convert.ToInt32(dataPointDefinition.Title.Last());
                        int[] slotNumbers = new int[] { 0, 6, 12, 18 };
                        double localValue = 0;
                        foreach (int slotNumber in slotNumbers)
                        {
                            localValue += GetPreProcessValue_FrontLabel_PulperCurve(value_Label, (slotNumber + plateNum), labelIndex);
                        }
                        value = Math.Round((localValue / 4), 3).ToString();
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Depth"))
                    {
                        value_Label = "Groove Depth";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace(label + " Groove Depth", ""));
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Width Plate"))
                    {
                        value_Label = "Groove Width";
                        int plateNum = Convert.ToInt32(dataPointDefinition.Title.Last());
                        int[] slotNumbers = new int[] { 0, 6, 12, 18 };
                        double localValue = 0;
                        foreach (int slotNumber in slotNumbers)
                        {
                            localValue += GetPreProcessValue_FrontLabel_PulperCurve(value_Label, (slotNumber + plateNum), labelIndex);
                        }
                        value = Math.Round((localValue / 4), 3).ToString();
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Width"))
                    {
                        value_Label = "Groove Width";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace(label + " Groove Width", ""));
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Bar Width Plate"))
                    {
                        value_Label = "Bar Width";
                        int plateNum = Convert.ToInt32(dataPointDefinition.Title.Last());
                        int[] slotNumbers = new int[] { 0, 6, 12, 18 };
                        double localValue = 0;
                        foreach (int slotNumber in slotNumbers)
                        {
                            localValue += GetPreProcessValue_FrontLabel_PulperCurve(value_Label, (slotNumber + plateNum), labelIndex);
                        }
                        value = Math.Round((localValue / 4), 3).ToString();
                    }
                    else if (dataPointDefinition.Title.Contains("Bar Width"))
                    {
                        value_Label = "Bar Width";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace(label + " Bar Width", ""));
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else  //To handle database discrepancies only
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 403 || dataPointDefinition.DataSetDefinitionId == 405)
                {
                    int labelIndex = GetLabelIndex("South Section of Fixed Plates Plates");
                    string label = "South Fixed";
                    if (dataPointDefinition.DataSetDefinitionId == 405)
                    {
                        labelIndex = GetLabelIndex("South Section of Fixed Rotating Plates");
                        label = "South Rotating";
                    }
                    if (dataPointDefinition.Title.Contains("Serial"))
                    {
                        value_Label = "Plate #6";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Last());
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Depth Plate"))
                    {
                        value_Label = "Plate #6";
                        int plateNum = Convert.ToInt32(dataPointDefinition.Title.Last());
                        int[] slotNumbers = new int[] { 0, 6, 12, 18 };
                        double localValue = 0;
                        foreach (int slotNumber in slotNumbers)
                        {
                            localValue += GetPreProcessValue_FrontLabel_PulperCurve(value_Label, (slotNumber + plateNum + 6), labelIndex);
                        }
                        value = Math.Round((localValue / 4), 3).ToString();
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Depth"))
                    {
                        value_Label = "Plate #6";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace(label + " Groove Depth", "")) + 6;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Width Plate"))
                    {
                        labelIndex = GetLabelIndex_SlotWise("#DIV", 6);
                        value_Label = "#DIV";
                        int plateNum = Convert.ToInt32(dataPointDefinition.Title.Last());
                        int[] slotNumbers = new int[] { 0, 6, 12, 18 };
                        double localValue = 0;
                        foreach (int slotNumber in slotNumbers)
                        {
                            localValue += GetPreProcessValue_FrontLabel_PulperCurve(value_Label, (slotNumber + plateNum), labelIndex);
                        }
                        value = Math.Round((localValue / 4), 3).ToString();
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Width"))
                    {
                        labelIndex = GetLabelIndex_SlotWise("#DIV", 6);
                        value_Label = "#DIV";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace(label + " Groove Width", ""));
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Bar Width Plate"))
                    {
                        labelIndex = GetLabelIndex_SlotWise("#DIV", 12);
                        value_Label = "#DIV";
                        int plateNum = Convert.ToInt32(dataPointDefinition.Title.Last());
                        int[] slotNumbers = new int[] { 0, 6, 12, 18 };
                        double localValue = 0;
                        foreach (int slotNumber in slotNumbers)
                        {
                            localValue += GetPreProcessValue_FrontLabel_PulperCurve(value_Label, (slotNumber + plateNum), labelIndex);
                        }
                        value = Math.Round((localValue / 4), 3).ToString();
                    }
                    else if (dataPointDefinition.Title.Contains("Bar Width"))
                    {
                        labelIndex = GetLabelIndex_SlotWise("#DIV", 12);
                        value_Label = "#DIV";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Replace(label + " Bar Width", ""));
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else  //To handle database discrepancies only
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
