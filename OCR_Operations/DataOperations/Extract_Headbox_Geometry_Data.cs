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
        public bool Headbox_Geometry(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeDefinitionId);

            foreach (var dataPointDefinition in dataPointDefinitions)
            {
                if (dataPointDefinition.DataSetDefinitionId == 350)
                {
                    int labelIndex = GetLabelIndex("Average");
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Blank"))
                    {
                        continue;
                    }
                    if (dataPointDefinition.IsConstantValue == 1)
                    {
                        value = dataPointDefinition.ConstantValue;
                    }
                    else if (dataPointDefinition.Title.Contains("Average"))
                    {
                        double localValue = 0;
                        value_Label = "\r\n" + dataPointDefinition.Title.Remove(dataPointDefinition.Title.IndexOf("(") - 1).Replace("TS - ", "");
                        for (int slot = 1; slot < 4; slot++)
                        {
                            localValue += GetPreProcessValue_HeaderboxIntercept(value_Label, slot, labelIndex);
                        }
                        value = Math.Round((localValue / 3), 3).ToString();
                    }
                    else
                    {
                        value_Label = "\r\n" + dataPointDefinition.Title.Remove(dataPointDefinition.Title.IndexOf("(") - 1).Replace("TS - ", "");
                        slotNumber = Convert.ToInt32(dataPointDefinition.Title.Substring(dataPointDefinition.Title.Length - 1));
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);                     //As value like 0.25
                    }
                }
                // Add for DS table
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
