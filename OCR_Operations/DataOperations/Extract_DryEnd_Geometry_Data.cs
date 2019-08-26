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
        private string RemoveError_DryEndGeometry(string value)
        {
            if (value.Length == 1)
            {
                value = value.Replace("O", "0").Replace("S", "5").Replace("Z", "2");
            }
            Regex regexCheck = new Regex("^[a-zA-Z\\s()]+$");
            if (regexCheck.Match(value).Value != "")
            {
                return ErrorText;
            }
            value = value.Replace(" ", "").Replace("O", "0").Replace("S", "5").Replace("Z", "2");
            Regex regex = new Regex(@"(^-?[0-9]*(\.\d{1,3})?$)|(^-?[0-9]*\/[0-9]*$)");
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
        public bool DryEnd_Geometry(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            OCRText = OCRText.Replace("\r\nFlush\r\n", "\r\n").Replace("\r\n0 +/- 1/8\r\n", "\r\n").Replace("\r\nFoil\r\n", "\r\n").Replace("\r\n0 +/-1/8\r\n", "\r\n").Replace("\r\n0+/- 1/8\r\n", "\r\n").Replace("\r\n0+/-1/8\r\n", "\r\n");

            string blankField = null;
            bool isBlank = false;
            Regex regex = new Regex(@"(\r\n-?[0-9\s]*(\.\s*\d{1,3})?\r\n)|\r\n-?[0-9\s]*\/[0-9\s]*\r\n");  // Regex to test row is blank
            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 0 && item.DPShortName != null))
            {
                if (dataPointDefinition.DataSetDefinitionId == 305)
                {
                    int labelIndex = GetLabelIndex("DS Trailing");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else
                    {
                        string tempOcrText = OCRText;
                        int slotNumber = 0;
                        if (dataPointDefinition.Title.Contains("Jumbo Foil"))
                        {
                            OCRText = OCRText.Substring(0, GetLabelIndex("After Calendar")).Substring(GetLabelIndex("Jumbo Foil")).Replace("\r\nO\r\n", "\r\n0\r\n").Replace("\r\nS\r\n", "\r\n5\r\n").Replace("\r\nZ\r\n", "\r\n2\r\n");
                            if (!regex.IsMatch(OCRText))
                            {
                                isBlank = true;
                            }
                        }
                        else if (dataPointDefinition.Title.Contains("After Calendar"))
                        {
                            OCRText = OCRText.Substring(0, GetLabelIndex("After Scanner")).Substring(GetLabelIndex("After Calendar")).Replace("\r\nO\r\n", "\r\n0\r\n").Replace("\r\nS\r\n", "\r\n5\r\n").Replace("\r\nZ\r\n", "\r\n2\r\n");
                            if (!regex.IsMatch(OCRText))
                            {
                                isBlank = true;
                            }
                        }
                        else if (dataPointDefinition.Title.Contains("After Scanner"))
                        {
                            OCRText = OCRText.Substring(0, GetLabelIndex("Other")).Substring(GetLabelIndex("After Scanner")).Replace("\r\nO\r\n", "\r\n0\r\n").Replace("\r\nS\r\n", "\r\n5\r\n").Replace("\r\nZ\r\n", "\r\n2\r\n");
                            if (!regex.IsMatch(OCRText))
                            {
                                isBlank = true;
                            }
                        }
                        else
                        {
                            OCRText = OCRText.Substring(0, GetLabelIndex("Page 2 of 2")).Substring(GetLabelIndex("Other")).Replace("\r\nO\r\n", "\r\n0\r\n").Replace("\r\nS\r\n", "\r\n5\r\n").Replace("\r\nZ\r\n", "\r\n2\r\n");
                            if (!regex.IsMatch(OCRText))
                            {
                                isBlank = true;
                            }
                        }
                        if (!isBlank)
                        {
                            if (dataPointDefinition.Title.Contains("TS Leading Edge"))
                            {
                                slotNumber = 1;
                                value_Label = dataPointDefinition.Title.Replace(" - TS Leading Edge", "");
                                labelIndex = GetLabelIndex(value_Label);
                                value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                                value = RemoveError_DryEndGeometry(value);
                            }
                            else if (dataPointDefinition.Title.Contains("TS Trailing Edge"))
                            {
                                slotNumber = 2;
                                value_Label = dataPointDefinition.Title.Replace(" - TS Trailing Edge", "");
                                labelIndex = GetLabelIndex(value_Label);
                                value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                                value = RemoveError_DryEndGeometry(value);
                            }
                            else if (dataPointDefinition.Title.Contains("DS Leading Edge"))
                            {
                                slotNumber = 3;
                                value_Label = dataPointDefinition.Title.Replace(" - DS Leading Edge", "");
                                labelIndex = GetLabelIndex(value_Label);
                                value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                                value = RemoveError_DryEndGeometry(value);
                            }
                            else if (dataPointDefinition.Title.Contains("DS Trailing Edge"))
                            {
                                slotNumber = 4;
                                value_Label = dataPointDefinition.Title.Replace(" - DS Trailing Edge", "");
                                labelIndex = GetLabelIndex(value_Label);
                                value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                                value = RemoveError_DryEndGeometry(value);
                            }
                            else
                            {
                                OCRText = tempOcrText;
                                continue;
                            }
                        }
                        else           //To set field null if that row is blank
                        {
                            value = blankField;
                            isBlank = false;
                        }
                        OCRText = tempOcrText;
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
