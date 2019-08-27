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
        public bool E_Spray(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            OCRText = OCRText.Replace("\r\nEmulsion Spray Header\r\n", "\r\n").Replace("\r\n= Fabric Angle\r\n", "\r\n").Replace("\r\n-1 to +1\r\n", "\r\n").Replace("\r\n1 to +1\r\n", "\r\n").Replace("\r\n|", "\r\n");
            Regex to178Regex = new Regex(@"to\s178\s[0-9]");
            if (to178Regex.IsMatch(OCRText))
            {
                OCRText = OCRText.Replace("to 178 ", "to 178\r\n");
            }
            Regex regex = new Regex(@"\r\n(([0-9]*(\.\d{1,3})?)|([0-9]*-[0-9]*\/[0-9]*))\s(([0-9]+(\.\d{1,3}))|([0-9]+-[0-9]*\/[0-9]*))\r\n");
            while (regex.IsMatch(OCRText))
            {
                Match match = regex.Match(OCRText);
                OCRText = OCRText.Replace(match.Value, match.Value.Replace(" ", "\r\n"));
            }
            Hashtable headValues = new Hashtable();

            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 0 && item.DPShortName != null))
            {
                int labelIndex = GetLabelIndex("Emulsion Spray Header Angle");
                if (dataPointDefinition.DataSetDefinitionId == 190)
                {
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Emulsion Spray Header Distance"));
                    int slotNumber = 0;
                    value_Label = "Fabric Angle";
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Fabric Angle TS"))
                    {
                        slotNumber = 2;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Fabric Angle DS"))
                    {
                        slotNumber = 3;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Emulsion Spray Header TS"))
                    {
                        slotNumber = 4;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.Contains("Emulsion Spray Header DS"))
                    {
                        slotNumber = 5;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 191 && dataPointDefinition.Title.Contains("Distance nozzle tip to belt run"))
                {
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("General Comments"));
                    int slotNumber = 0;
                    value_Label = "Distance nozzle tip to belt run";
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.EndsWith("TS"))
                    {
                        slotNumber = 3;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    }
                    else if (dataPointDefinition.Title.EndsWith("DS"))
                    {
                        slotNumber = 4;
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    }
                    //Write Target field for variantId 180
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    // To Handle error of Emulsion Spray Header Distance table
                    OCRText = tempOcrText;
                }
                else
                {
                    continue;
                }
                if (value != ErrorText)
                {
                    headValues.Add(dataPointDefinition.Id, value);
                }
                else
                {
                    headValues.Add(dataPointDefinition.Id, 0);
                }
                // Add to Data List
                cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
            }
            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 1))
            {
                int[] dataPoints = { 2784, 2785 };
                double faTs = Convert.ToDouble(headValues[dataPoints[0]].ToString());
                double faDs = Convert.ToDouble(headValues[dataPoints[1]].ToString());
                value = Math.Round((faTs + faDs) / 2, 3).ToString();
                if (faTs == 0 || faDs == 0)
                {
                    value = ErrorText;
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
