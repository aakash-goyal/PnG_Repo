using OCR_Operations.DataAcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        private string GetSlotLabelValue_RefinerPlate(string label, int slotNumber, int labelIndex)
        {
            int index_Of_Field = OCRText.IndexOf(label, labelIndex);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label, labelIndex);
                //Error Handle if label not exist 
                if (countOfField <= 2)
                {
                    return ErrorText;
                }
            }
            int startIndex = OCRText.IndexOf("\r\n", index_Of_Field + 1) + 2;  //To handle label with \r\n add 1
            if (startIndex == 1 || startIndex >= OCRText.Length)
            {
                return ErrorText;
            }
            for (int i = 1; i < slotNumber; i++)
            {//OCRText.IndexOf("\r\n", (index_Of_Field + countOfField + 2))
                startIndex = OCRText.IndexOf("\r\n", startIndex) + 2; // add +2 for /r/n characters
                if (startIndex == 1 || startIndex >= OCRText.Length)
                {
                    return ErrorText;
                }
            }
            int lastIndex = OCRText.IndexOf("\r\n", startIndex);
            if (startIndex == 1 || startIndex >= OCRText.Length)
            {
                return ErrorText;
            }
            if (lastIndex == -1)
            {
                return OCRText.Substring(startIndex);
            }
            int value_length = lastIndex - startIndex;
            string value = OCRText.Substring(startIndex, value_length);
            return value;
        }
        private string GetLabelValue_RefinerPlate(string label, int labelIndex)
        {
            int index_Of_Field = OCRText.IndexOf(label, labelIndex);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label, labelIndex);
                //Error Handle if label not exist 
                if (countOfField <= 2)
                {
                    return ErrorText;
                }
            }
            int startIndex = OCRText.IndexOf("\r\n", index_Of_Field) + 2;
            int lastIndex = OCRText.IndexOf("\r\n", startIndex);
            if (startIndex == 1 || startIndex >= OCRText.Length)
            {
                return ErrorText;
            }
            if (lastIndex == -1)
            {
                return OCRText.Substring(startIndex);
            }
            int value_Length = lastIndex - startIndex;
            string value = OCRText.Substring(startIndex, value_Length); // add +2 for /r/n characters
            return value;
        }
        private string RemoveGeneralError_RefinerPlate(string value)
        {
            value = value.Replace(" ", "");
            if (value.Contains(","))
            {
                value = value.Replace(",", ".");
            }
            else if (value.Contains("-"))
            {
                value = value.Replace("-", ".");
            }
            //Pattern to match decimal number
            string decimal_Pattern = @"^[0-9]*(\.\d{1,3})?$";
            Regex regex = new Regex(decimal_Pattern);
            Match match = regex.Match(value);
            while (match.Value == "")
            {
                // If value doesn't match then return blank
                if (value.Length == 0)
                {
                    return ErrorText;
                }
                value = value.Remove((value.Length - 1), 1);
                match = regex.Match(value);
            }

            return value;
        }
        private string RemoveYesNoError_RefinerPlate(string value)
        {
            if (value.Equals(ErrorText))
            {
                return ErrorText;
            }
            if (value.ToUpper().Contains("YES"))
            {
                value = "Yes";
                return value;
            }
            else if (value.ToUpper().Contains("NO"))
            {
                value = "No";
                return value;
            }
            else
            {
                return ErrorText;
            }
        }
        public bool Refiner_Plate_Inspection(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 228)
                {
                    int labelIndex = GetLabelIndex("General Refiner Information");  // Form have multiple pages
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    // Function to remove error from Values of Refiner Information
                }
                else if (dataPointDefinition.DataSetDefinitionId == 229 && Convert.ToInt32(dataPointDefinition.Title.Substring(0, 2)) < 13) // As 13th row exist in database not in form
                {
                    int labelIndex = GetLabelIndex("New Plate Depth");  // Form have multiple pages
                    value_Label = "New Plate Depth";
                    if (dataPointDefinition.Title.Contains("Refiner Plate Pattern"))
                    {
                        int slotNumber = (3 * Convert.ToInt32(dataPointDefinition.Title.Replace(" Refiner Plate Pattern", ""))) - 3 + 1;  // 3 for previous row
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    }
                    else if (dataPointDefinition.Title.Contains("Min. Groove Depth"))
                    {
                        int slotNumber = (3 * Convert.ToInt32(dataPointDefinition.Title.Replace(" Min. Groove Depth", ""))) - 3 + 3;  // 3 for previous row
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_RefinerPlate(value);          // Function to remove error from Values of Refiner Information
                    }
                    else
                    {
                        continue;     // For irregularities in database
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 230 || dataPointDefinition.DataSetDefinitionId == 232 || dataPointDefinition.DataSetDefinitionId == 234 || dataPointDefinition.DataSetDefinitionId == 236)
                {
                    int labelIndex = GetLabelIndex(dataPointDefinition.Title.Substring(0, 10)); //Position number index
                    if (dataPointDefinition.Title.Contains("Min. Accept Groove Depth"))
                    {
                        value_Label = "Min. Accept Groove Depth";
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        // Define function to handle common errors in Min. Accept Groove Depth Value 
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Depth"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" - Groove Depth", "").Substring(13);
                        int slotNumber = 1;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Define function to handle common errors in Groove Depth Value 
                    }
                    else if (dataPointDefinition.Title.Contains("Groove Width"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" - Groove Width", "").Substring(13);
                        int slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Define function to handle common errors in Groove Depth Value 
                    }
                    else if (dataPointDefinition.Title.Contains("Bar Width"))
                    {
                        value_Label = dataPointDefinition.Title.Replace(" - Bar Width", "").Substring(13);
                        int slotNumber = 3;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Define function to handle common errors in Bar Width Value 
                    }
                    else if (dataPointDefinition.Title.Contains("Avg"))
                    {
                        value_Label = "Average";
                        int slotNumber = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        // Define function to handle common errors in Average Value
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 231)
                {
                    int labelIndex = GetLabelIndex("Position 1");
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);
                }
                else if (dataPointDefinition.DataSetDefinitionId == 233)
                {
                    int labelIndex = GetLabelIndex("Position 2");
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);
                }
                else if (dataPointDefinition.DataSetDefinitionId == 235)
                {
                    int labelIndex = GetLabelIndex("Position 3");
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);
                }
                else if (dataPointDefinition.DataSetDefinitionId == 237)
                {
                    int labelIndex = GetLabelIndex("Position 4");
                    value_Label = dataPointDefinition.Title;
                    value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                    value = RemoveYesNoError_RefinerPlate(value);
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
