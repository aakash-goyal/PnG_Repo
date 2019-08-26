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
        public bool Blowoff(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            { // All error function is taking value as 0.250
                if (dataPointDefinition.DataSetDefinitionId == 50)
                {
                    int labelIndex = GetLabelIndex("Couch Roll");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Result"))
                    {
                        string sampleNum = dataPointDefinition.Title.Replace(" Result", "").Last().ToString();
                        value_Label = "Sample #" + sampleNum;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.EndsWith("Average"))                                       // Check why Average target is blank while form has not.
                    {
                        double localvalue = 0;
                        for (int i = 1; i < 4; i++)
                        {
                            value_Label = "Sample #" + i;
                            localvalue = GetPreProcessValue_HeaderboxIntercept(value_Label, 1, labelIndex);
                        }
                        value = Math.Round((localvalue / 3), 3).ToString();
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 51)
                {
                    int labelIndex = GetLabelIndex("Lead Roll");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Result"))
                    {
                        string sampleNum = dataPointDefinition.Title.Replace(" Result", "").Last().ToString();
                        value_Label = "Sample #" + sampleNum;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.EndsWith("Average"))                                       // Check why Average target is blank while form has not.
                    {
                        double localvalue = 0;
                        for (int i = 1; i < 4; i++)
                        {
                            value_Label = "Sample #" + i;
                            localvalue = GetPreProcessValue_HeaderboxIntercept(value_Label, 1, labelIndex);
                        }
                        value = Math.Round((localvalue / 3), 3).ToString();
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 52)
                {
                    int labelIndex = GetLabelIndex("After LFSR");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Result"))
                    {
                        string sampleNum = dataPointDefinition.Title.Replace(" Result", "").Last().ToString();
                        value_Label = "Sample #" + sampleNum;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.EndsWith("Average"))                                       // Check why Average target is blank while form has not.
                    {
                        double localvalue = 0;
                        for (int i = 1; i < 4; i++)
                        {
                            value_Label = "Sample #" + i;
                            localvalue = GetPreProcessValue_HeaderboxIntercept(value_Label, 1, labelIndex);
                        }
                        value = Math.Round((localvalue / 3), 3).ToString();
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 53)
                {
                    int labelIndex = GetLabelIndex("APD Roll");
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Result"))
                    {
                        string sampleNum = dataPointDefinition.Title.Replace(" Result", "").Last().ToString();
                        value_Label = "Sample #" + sampleNum;
                        value = GetLabelValue_RefinerPlate(value_Label, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    else if (dataPointDefinition.Title.EndsWith("Average"))                                       // Check why Average target is blank while form has not.
                    {
                        double localvalue = 0;
                        for (int i = 1; i < 4; i++)
                        {
                            value_Label = "Sample #" + i;
                            localvalue = GetPreProcessValue_HeaderboxIntercept(value_Label, 1, labelIndex);
                        }
                        value = Math.Round((localvalue / 3), 3).ToString();
                    }
                    else if (dataPointDefinition.Title.EndsWith("Moisture"))                                    // Check why Average target is blank while form has not.
                    {
                        double localvalue = 0;
                        for (int i = 1; i < 4; i++)
                        {
                            value_Label = "Sample #" + i;
                            localvalue = GetPreProcessValue_HeaderboxIntercept(value_Label, 1, labelIndex);
                        }
                        value = (100 - Math.Round((localvalue / 3), 3)).ToString();
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
