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
        private int GetLabelIndex(string label)
        {
            int index_Of_Field = OCRText.IndexOf(label);
            int countOfField = label.Length;
            while (index_Of_Field == -1)
            {
                label = label.Remove(countOfField - 1, 1);
                countOfField = label.Length;
                index_Of_Field = OCRText.IndexOf(label);
                //Error Handle if label not exist 
                if (countOfField <= 2)
                {
                    return -1;        //If label not found
                }
            }
            return index_Of_Field;
        }
        public bool Wire_Section_Shower(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            OCRText = OCRText.Replace("\r\nIst", "\r\n1st").Replace("\r\nabric Angle", "\r\nFabric Angle").Replace("\r\nISS Needle Shower", "\r\nNSS Needle Shower").Replace("\r\nS Needle Shower", "\r\nSS Needle Shower").Replace("\r\nend KO", "\r\n2nd KO");

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                int labelIndex = -1;
                if (dataPointDefinition.DataSetDefinitionId == 174)
                {
                    labelIndex = GetLabelIndex("Forming Wire Showers");
                }
                else if (dataPointDefinition.DataSetDefinitionId == 175)
                {
                    labelIndex = GetLabelIndex("Backing Wire Showers");
                }
                if (dataPointDefinition.Title.Contains("Fabric Angle Range") || (dataPointDefinition.Title.Contains("into ") && dataPointDefinition.Title.EndsWith("S")) || dataPointDefinition.Title.Contains("History"))
                {
                    continue;
                }
                else if (labelIndex != -1)
                {
                    if (dataPointDefinition.Id >= 2529 && dataPointDefinition.Id <= 2532)  // Due to spelling error in database
                    {
                        dataPointDefinition.Title = dataPointDefinition.Title.Replace("SS", "NSS");
                    }
                    int slotNumber = 0;
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    //To get Only Target field of formingwireshower values
                    else
                    {
                        if (dataPointDefinition.Title.Contains("Target"))
                        {
                            //Define slot number for values to find in Text
                            slotNumber = 1;
                            value_Label = dataPointDefinition.Title.Replace(" Target", "");
                            if (value_Label.EndsWith("Angle"))
                            {
                                value_Label = "Fabric Angle";
                            }
                        }
                        else if (dataPointDefinition.Title.Contains("TS"))
                        {
                            //Define slot number for values to find in Text
                            slotNumber = 3;
                            value_Label = dataPointDefinition.Title.Replace(" TS", "");
                        }
                        else if (dataPointDefinition.Title.Contains("DS"))
                        {
                            //Define slot number for values to find in Text
                            slotNumber = 4;
                            value_Label = dataPointDefinition.Title.Replace(" DS", "");
                        }
                        else
                        {
                            continue;
                        }
                        if (dataPointDefinition.DPShortName.Contains("FABANG-TSV") || dataPointDefinition.DPShortName.Contains("FABANG-DSV"))
                        {
                            slotNumber -= 1;
                        }
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    // Add new Object to list
                    cpeDataList.Add(CreateCpeEntryDataPoint(value, dataPointDefinition.Id, cpeDefinitionId, cpeEntryId));
                }
            }
            //Insert list of cpeentrydatapointvalues in to database using datainsertion class
            bool success = dataInsertion.Insert(cpeDataList);

            return success;
        }
    }
}
