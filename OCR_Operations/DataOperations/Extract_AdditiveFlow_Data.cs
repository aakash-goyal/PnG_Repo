using OCR_Operations.DataAcess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OCR_Operations.DataOperations
{
    partial class ExtractData
    {
        public bool AdditiveFlow(int cpeEntryId, int cpeDefinitionId)
        {
            DataInsertion dataInsertion = new DataInsertion();
            List<CpeEntryDataPointValue> cpeDataList = new List<CpeEntryDataPointValue>();
            string value_Label;
            string value;
            List<DataPointDefinition> dataPointDefinitions = GetDataPointDefinitions(cpeEntryId);
            OCRText = OCRText.Replace("\r\nTow - Dry\r\n", "\r\n").Replace("\r\nPerm. Wet\r\n", "\r\n").Replace("\r\nLayer OM\r\n", "\r\n").Replace("\r\nTissue Dry\r\n", "\r\n").Replace("\r\nTemp. Wet\r\n", "\r\n").Replace("\r\nCPO\r\n", "\r\nCPQ\r\n").Replace("\r\ncPO\r\n", "\r\nCPQ\r\n").Replace("\r\nBRm\r\n", "\r\ngpm\r\n").Replace("\r\nrelease Aid\r\n", "\r\nRelease Aid\r\n").Replace("\r\ngom\r\n", "\r\ngpm\r\n").Replace("\r\ncoaggulant\r\n", "\r\nCoaggulant\r\n");

            string blankField = null;
            bool perWetblank = false;
            bool cpqblank = false;
            bool crepeblank = false;
            Hashtable headValues = new Hashtable();
            Regex regex = new Regex(@"\r\n[0-9\s]*(\.\s*\d{1,3})?\r\n");

            foreach (var dataPointDefinition in dataPointDefinitions.Where(item => item.IsCalculated == 0 && item.DPShortName != null))
            {
                int labelIndex = GetLabelIndex("Additive Flow Check CPE Data");
                if (dataPointDefinition.DataSetDefinitionId == 205)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex_SlotWise("Strength", 4));
                    value_Label = "Strength";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("gal measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 206)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Strength -") + 1);
                    string testOcr = OCRText.Substring(GetLabelIndex_SlotWise("Strength", 4)).Replace(" ", "");
                    Match match = regex.Match(testOcr);
                    if (!match.Success)
                    {
                        value = blankField;                                     // As these fields can be blank sometimes
                        perWetblank = true;
                    }
                    else
                    {
                        value_Label = "Strength";
                        int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1)) + 3;
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        int slotNumber = 0;
                        if (dataPointDefinition.Title.Contains("Controller PV"))
                        {
                            slotNumber = 1;
                        }
                        else if (dataPointDefinition.Title.Contains("Controller OP "))
                        {
                            slotNumber = 3;
                        }
                        else if (dataPointDefinition.Title.Contains("gal measured"))
                        {
                            slotNumber = 4;
                        }
                        else if (dataPointDefinition.Title.Contains("time"))
                        {
                            slotNumber = 5;
                        }
                        else
                        {
                            OCRText = tempOcrText;
                            continue;
                        }
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 220)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Strength - Wire"));
                    value_Label = "Center/Felt";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("gal measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 221)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex_SlotWise("Strength", 13));
                    value_Label = "Strength - Wire";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("gal measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 208)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("CPQ"));
                    value_Label = "Strength";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1)) + 12;
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("gal measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 209)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Absorbency Aid"));
                    string testOcr = OCRText.Substring(GetLabelIndex("CPQ"));
                    Match match = regex.Match(testOcr);
                    if (!match.Success)
                    {
                        value = blankField;                                     // As these fields can be blank sometimes
                        cpqblank = true;
                    }
                    else
                    {
                        value_Label = "CPQ";
                        int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        int slotNumber = 0;
                        if (dataPointDefinition.Title.Contains("Controller PV"))
                        {
                            slotNumber = 1;
                        }
                        else if (dataPointDefinition.Title.Contains("Controller OP "))
                        {
                            OCRText = OCRText.Replace("\r\nCPQ\r\n", "\r\n");
                            value_Label = "pm";
                            rowNumber += 15;
                            labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                            slotNumber = 1;
                        }
                        else if (dataPointDefinition.Title.Contains("gal measured"))
                        {
                            OCRText = OCRText.Replace("\r\nCPQ\r\n", "\r\n");
                            value_Label = "pm";
                            rowNumber += 15;
                            labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                            slotNumber = 2;
                        }
                        else if (dataPointDefinition.Title.Contains("time"))
                        {
                            OCRText = OCRText.Replace("\r\nCPQ\r\n", "\r\n");
                            value_Label = "pm";
                            rowNumber += 15;
                            labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                            slotNumber = 3;
                        }
                        else
                        {
                            OCRText = tempOcrText;
                            continue;
                        }
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 210)
                {
                    if (dataPointDefinition.Title.Contains("Avg. gal measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Coaggulant"));
                    value_Label = "Absorbency Aid";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        OCRText = OCRText.Replace("\r\nAbsorbency Aid\r\n", "\r\n");
                        value_Label = "/min";
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("mL measured"))
                    {
                        OCRText = OCRText.Replace("\r\nAbsorbency Aid\r\n", "\r\n");
                        value_Label = "/min";
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        slotNumber = 2;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        OCRText = OCRText.Replace("\r\nAbsorbency Aid\r\n", "\r\n");
                        value_Label = "/min";
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        slotNumber = 3;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 211)
                {
                    if (dataPointDefinition.Title.Contains("Avg. gal measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Flocculant"));
                    value_Label = "Coaggulant";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        OCRText = OCRText.Replace("\r\nCoaggulant\r\n", "\r\n");
                        value_Label = "/min";
                        rowNumber += 3;
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("mL measured"))
                    {
                        OCRText = OCRText.Replace("\r\nCoaggulant\r\n", "\r\n");
                        value_Label = "/min";
                        rowNumber += 3;
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        slotNumber = 2;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        OCRText = OCRText.Replace("\r\nCoaggulant\r\n", "\r\n");
                        value_Label = "/min";
                        rowNumber += 3;
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        slotNumber = 3;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 212)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Emulsion"));
                    value_Label = "Flocculant";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("gal measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 213)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Glue"));
                    value_Label = "Emulsion";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("gal measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 214)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Water for Glue"));
                    value_Label = "Glue";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("gal measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 215)
                {
                    if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Crepe Aid"));
                    value_Label = "Water for Glue";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("gal measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        OCRText = tempOcrText;
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 216)
                {
                    if (dataPointDefinition.Title.Contains("Avg. gal measured"))
                    {
                        continue;
                    }
                    string tempOcrText = OCRText;
                    OCRText = OCRText.Substring(0, GetLabelIndex("Release Aid"));
                    string testOcr = OCRText.Substring(GetLabelIndex("Crepe Aid"));
                    Match match = regex.Match(testOcr);
                    if (!match.Success)
                    {
                        value = blankField;                                     // As these fields can be blank sometimes
                        crepeblank = true;
                    }
                    else
                    {
                        value_Label = "Crepe Aid";
                        int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                        labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                        int slotNumber = 0;
                        if (dataPointDefinition.Title.Contains("Controller PV"))
                        {
                            slotNumber = 1;
                        }
                        else if (dataPointDefinition.Title.Contains("Controller OP "))
                        {
                            slotNumber = 3;
                        }
                        else if (dataPointDefinition.Title.Contains("mL measured"))
                        {
                            slotNumber = 4;
                        }
                        else if (dataPointDefinition.Title.Contains("time"))
                        {
                            slotNumber = 5;
                        }
                        else
                        {
                            OCRText = tempOcrText;
                            continue;
                        }
                        value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                        value = RemoveGeneralError_SteamHood(value);
                    }
                    OCRText = tempOcrText;
                }
                else if (dataPointDefinition.DataSetDefinitionId == 217)
                {
                    if (dataPointDefinition.Title.Contains("Avg. gal measured"))
                    {
                        continue;
                    }
                    value_Label = "Release Aid";
                    int rowNumber = Convert.ToInt32(dataPointDefinition.DPShortName.Substring(dataPointDefinition.DPShortName.Length - 1));
                    labelIndex = GetLabelIndex_SlotWise(value_Label, rowNumber);
                    int slotNumber = 0;
                    if (dataPointDefinition.Title.Contains("Controller PV"))
                    {
                        slotNumber = 1;
                    }
                    else if (dataPointDefinition.Title.Contains("Controller OP "))
                    {
                        slotNumber = 3;
                    }
                    else if (dataPointDefinition.Title.Contains("mL measured"))
                    {
                        slotNumber = 4;
                    }
                    else if (dataPointDefinition.Title.Contains("time"))
                    {
                        slotNumber = 5;
                    }
                    else
                    {
                        continue;
                    }
                    value = GetSlotLabelValue_RefinerPlate(value_Label, slotNumber, labelIndex);
                    value = RemoveGeneralError_SteamHood(value);
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
                int[] dataPointIds;
                if (dataPointDefinition.DataSetDefinitionId == 205)
                {
                    dataPointIds = new int[] { 3216, 3225, 3234, 3218, 3227, 3236, 3220, 3229, 3238, 3221, 3230, 3239 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 206)
                {
                    dataPointIds = new int[] { 3252, 3261, 3270, 3254, 3263, 3272, 3256, 3265, 3274, 3257, 3266, 3275 };
                    if (perWetblank)             // For now not calculate
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 220)
                {
                    dataPointIds = new int[] { 3700, 3709, 3718, 3702, 3711, 3720, 3704, 3713, 3722, 3705, 3714, 3723 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 221)
                {
                    dataPointIds = new int[] { 3736, 3745, 3754, 3738, 3747, 3756, 3740, 3749, 3758, 3741, 3750, 3759 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 208)
                {
                    dataPointIds = new int[] { 3324, 3333, 3342, 3326, 3335, 3344, 3328, 3337, 3346, 3329, 3338, 3347 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 209)
                {
                    dataPointIds = new int[] { 3360, 3369, 3378, 3362, 3371, 3380, 3364, 3373, 3382, 3365, 3374, 3383 };
                    if (cpqblank)
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 210)
                {
                    dataPointIds = new int[] { 3396, 3405, 3414, 3398, 3407, 3416, 3399, 3408, 3417, 3401, 3410, 3419 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 211)
                {
                    dataPointIds = new int[] { 3432, 3441, 3450, 3434, 3443, 3452, 3435, 3444, 3453, 3437, 3446, 3455 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 212)
                {
                    dataPointIds = new int[] { 3468, 3477, 3486, 3470, 3479, 3488, 3472, 3481, 3490, 3473, 3482, 3491 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 213)
                {
                    dataPointIds = new int[] { 3504, 3513, 3522, 3506, 3515, 3524, 3508, 3517, 3526, 3509, 3518, 3527 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 214)
                {
                    dataPointIds = new int[] { 3540, 3549, 3558, 3542, 3551, 3560, 3544, 3553, 3562, 3545, 3554, 3563 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 215)
                {
                    dataPointIds = new int[] { 3576, 3585, 3594, 3578, 3587, 3596, 3580, 3589, 3598, 3581, 3590, 3599 };
                }
                else if (dataPointDefinition.DataSetDefinitionId == 216)
                {
                    dataPointIds = new int[] { 3612, 3621, 3630, 3614, 3623, 3632, 3615, 3624, 3633, 3617, 3626, 3635 };
                    if (crepeblank)
                    {
                        continue;
                    }
                }
                else if (dataPointDefinition.DataSetDefinitionId == 217)
                {
                    dataPointIds = new int[] { 3648, 3657, 3666, 3650, 3659, 3668, 3651, 3660, 3669, 3653, 3662, 3671 };
                }
                else
                {
                    continue;
                }
                if (dataPointDefinition.Title.Contains("Tissue Dry Strength") && dataPointDefinition.Title.Contains("Avg. Calculated Flow"))
                {
                    continue;
                }
                else if (dataPointDefinition.Title.Contains("Tissue Dry Strength") && dataPointDefinition.Title.Contains("Avg. % Difference"))
                {
                    continue;
                }
                else if (dataPointDefinition.Title.Contains("Tissue Dry Strength") && dataPointDefinition.Title.EndsWith("Avg."))
                {
                    continue;
                }
                else if (dataPointDefinition.Title.Contains("Avg. Controller PV"))
                {
                    double pv1 = Convert.ToDouble(headValues[dataPointIds[0]].ToString());
                    double pv2 = Convert.ToDouble(headValues[dataPointIds[1]].ToString());
                    double pv3 = Convert.ToDouble(headValues[dataPointIds[2]].ToString());
                    value = Math.Round((pv1 + pv2 + pv3) / 3, 2).ToString();
                    if (pv1 == 0 && pv2 == 0 && pv3 == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Avg. Controller OP"))
                {
                    double op1 = Convert.ToDouble(headValues[dataPointIds[3]].ToString());
                    double op2 = Convert.ToDouble(headValues[dataPointIds[4]].ToString());
                    double op3 = Convert.ToDouble(headValues[dataPointIds[5]].ToString());
                    value = Math.Round((op1 + op2 + op3) / 3, 1).ToString();
                    if (op1 == 0 && op2 == 0 && op3 == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Avg. gal measured"))
                {
                    double gal1 = Convert.ToDouble(headValues[dataPointIds[6]].ToString());
                    double gal2 = Convert.ToDouble(headValues[dataPointIds[7]].ToString());
                    double gal3 = Convert.ToDouble(headValues[dataPointIds[8]].ToString());
                    value = Math.Round((gal1 + gal2 + gal3) / 3, 2).ToString();
                    if (gal1 == 0 && gal2 == 0 && gal3 == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Avg. mL measured"))
                {
                    double ml1 = Convert.ToDouble(headValues[dataPointIds[6]].ToString());
                    double ml2 = Convert.ToDouble(headValues[dataPointIds[7]].ToString());
                    double ml3 = Convert.ToDouble(headValues[dataPointIds[8]].ToString());
                    value = Math.Round((ml1 + ml2 + ml3) / 3, 0).ToString();
                    if (ml1 == 0 && ml2 == 0 && ml3 == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Avg. time"))
                {
                    double time1 = Convert.ToDouble(headValues[dataPointIds[9]].ToString());
                    double time2 = Convert.ToDouble(headValues[dataPointIds[10]].ToString());
                    double time3 = Convert.ToDouble(headValues[dataPointIds[11]].ToString());
                    value = Math.Round((time1 + time2 + time3) / 3, 0).ToString();
                    if (time1 == 0 && time2 == 0 && time3 == 0)
                    {
                        value = ErrorText;
                    }
                }
                else if (dataPointDefinition.Title.Contains("Avg. Calculated Flow"))
                {
                    double gal1 = Convert.ToDouble(headValues[dataPointIds[6]].ToString());
                    double gal2 = Convert.ToDouble(headValues[dataPointIds[7]].ToString());
                    double gal3 = Convert.ToDouble(headValues[dataPointIds[8]].ToString());
                    double galAvg = Math.Round((gal1 + gal2 + gal3) / 3, 2);
                    double time1 = Convert.ToDouble(headValues[dataPointIds[9]].ToString());
                    double time2 = Convert.ToDouble(headValues[dataPointIds[10]].ToString());
                    double time3 = Convert.ToDouble(headValues[dataPointIds[11]].ToString());
                    double timeAvg = Math.Round((time1 + time2 + time3) / 3, 0);
                    if ((gal1 == 0 && gal2 == 0 && gal3 == 0) || (time1 == 0 && time2 == 0 && time3 == 0))
                    {
                        value = ErrorText;
                    }
                    else
                    {
                        value = Math.Round((galAvg * 60) / timeAvg, 3).ToString();
                    }
                }
                else if (dataPointDefinition.Title.Contains("Avg. % Difference"))
                {
                    double gal1 = Convert.ToDouble(headValues[dataPointIds[6]].ToString());
                    double gal2 = Convert.ToDouble(headValues[dataPointIds[7]].ToString());
                    double gal3 = Convert.ToDouble(headValues[dataPointIds[8]].ToString());
                    double galAvg = Math.Round((gal1 + gal2 + gal3) / 3, 2);
                    double time1 = Convert.ToDouble(headValues[dataPointIds[9]].ToString());
                    double time2 = Convert.ToDouble(headValues[dataPointIds[10]].ToString());
                    double time3 = Convert.ToDouble(headValues[dataPointIds[11]].ToString());
                    double timeAvg = Math.Round((time1 + time2 + time3) / 3, 0);
                    double flow = Math.Round((galAvg * 60) / timeAvg, 3);
                    double pv1 = Convert.ToDouble(headValues[dataPointIds[0]].ToString());
                    double pv2 = Convert.ToDouble(headValues[dataPointIds[1]].ToString());
                    double pv3 = Convert.ToDouble(headValues[dataPointIds[2]].ToString());
                    double pvAvg = Math.Round((pv1 + pv2 + pv3) / 3, 2);
                    if ((gal1 == 0 && gal2 == 0 && gal3 == 0) || (time1 == 0 && time2 == 0 && time3 == 0) || (pv1 == 0 && pv2 == 0 && pv3 == 0))
                    {
                        value = ErrorText;
                    }
                    else
                    {
                        value = Math.Round(((flow - pvAvg) / flow) * 100, 1).ToString();
                    }
                }
                else if (dataPointDefinition.Title.EndsWith("Avg."))
                {
                    double gal1 = Convert.ToDouble(headValues[dataPointIds[6]].ToString());
                    double gal2 = Convert.ToDouble(headValues[dataPointIds[7]].ToString());
                    double gal3 = Convert.ToDouble(headValues[dataPointIds[8]].ToString());
                    double galAvg = Math.Round((gal1 + gal2 + gal3) / 3, 2);
                    double time1 = Convert.ToDouble(headValues[dataPointIds[9]].ToString());
                    double time2 = Convert.ToDouble(headValues[dataPointIds[10]].ToString());
                    double time3 = Convert.ToDouble(headValues[dataPointIds[11]].ToString());
                    double timeAvg = Math.Round((time1 + time2 + time3) / 3, 0);
                    double flow = Math.Round((galAvg * 60) / timeAvg, 3);
                    double pv1 = Convert.ToDouble(headValues[dataPointIds[0]].ToString());
                    double pv2 = Convert.ToDouble(headValues[dataPointIds[1]].ToString());
                    double pv3 = Convert.ToDouble(headValues[dataPointIds[2]].ToString());
                    double pvAvg = Math.Round((pv1 + pv2 + pv3) / 3, 2);
                    if ((gal1 == 0 && gal2 == 0 && gal3 == 0) || (time1 == 0 && time2 == 0 && time3 == 0) || (pv1 == 0 && pv2 == 0 && pv3 == 0))
                    {
                        value = ErrorText;
                    }
                    else
                    {
                        double pct = Math.Abs(Math.Round(((flow - pvAvg) / flow) * 100, 1));
                        if (pct > 10)
                        {
                            value = "Calibration Needed";
                        }
                        else
                        {
                            value = "At Standard";
                        }
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
