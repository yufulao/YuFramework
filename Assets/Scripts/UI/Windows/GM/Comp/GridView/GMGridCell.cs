using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yu;

public class GMGridCell : GridCell<GMMethodData, GMGridContext>
{
    [SerializeField] private Button btnExecute;
    [SerializeField] private TextMeshProUGUI textMethodName;
    [SerializeField] private List<TextMeshProUGUI> textParameterDescList = new List<TextMeshProUGUI>();
    [SerializeField] private List<TMP_InputField> inputFieldParameterList = new List<TMP_InputField>();
    [SerializeField] private List<TMP_Dropdown_Event> dropdownParameterList = new List<TMP_Dropdown_Event>();

    private GMMethodData _cacheMethodData;


    public override void Initialize()
    {
        btnExecute.onClick.AddListener(OnBtnClickExecute);
    }

    public override void UpdateContent(GMMethodData methodData)
    {
        _cacheMethodData = methodData;
        textMethodName.text = methodData.CommandDesc;
        var parameterCount = methodData.ParametersData.Length;
        if (parameterCount > inputFieldParameterList.Count || parameterCount > dropdownParameterList.Count)
        {
            Debug.LogError("参数数量过多" + methodData.CommandDesc);
            return;
        }

        for (var i = 0; i < parameterCount; i++)
        {
            UpdateParameter(methodData.ParametersData[i], i);
        }

        HideUselessParameterInputUI(parameterCount);
    }

    /// <summary>
    /// 刷新单个gm指令的参数
    /// </summary>
    private void UpdateParameter(GMParameterData parameterData, int indexUI)
    {
        var textDesc = textParameterDescList[indexUI];
        var inputField = inputFieldParameterList[indexUI];
        var dropdown = dropdownParameterList[indexUI];

        textDesc.gameObject.SetActive(true);
        textDesc.text = parameterData.ParameterDesc;

        if (parameterData.ParameterOptionArray == null)
        {
            inputField.gameObject.SetActive(true);
            dropdown.gameObject.SetActive(false);
            UpdateInputField(parameterData, inputField);
            return;
        }

        inputField.gameObject.SetActive(false);
        dropdown.gameObject.SetActive(true);
        UpdateDropdown(parameterData, dropdown);
    }

    /// <summary>
    /// 刷新单个下拉列表
    /// </summary>
    private void UpdateDropdown(GMParameterData parameterData, TMP_Dropdown_Event dropdown)
    {
        //空间复杂度可优化
        var optionList = new List<string>();
        foreach (var optionObj in parameterData.ParameterOptionArray)
        {
            optionList.Add(optionObj.ToString());
        }

        dropdown.UpdateOptions(optionList);
    }

    /// <summary>
    /// 刷新单个输入框
    /// </summary>
    private void UpdateInputField(GMParameterData parameterData, TMP_InputField inputField)
    {
        inputField.text = "";
    }

    /// <summary>
    /// 隐藏无用的参数输入ui
    /// </summary>
    private void HideUselessParameterInputUI(int parameterCount)
    {
        for (var i = parameterCount; i < textParameterDescList.Count; i++)
        {
            textParameterDescList[i].gameObject.SetActive(false);
        }

        for (var i = parameterCount; i < inputFieldParameterList.Count; i++)
        {
            inputFieldParameterList[i].gameObject.SetActive(false);
        }

        for (var i = parameterCount; i < dropdownParameterList.Count; i++)
        {
            dropdownParameterList[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// execute按钮点击时
    /// </summary>
    private void OnBtnClickExecute()
    {
        var (valid, parameterArray) = GetCurrentParameters();
        if (!valid)
        {
            return;
        }

        var gmGridData = new GMGridData
        {
            Index = Index,
            ParameterArray = parameterArray,
        };
        Context.OnBtnClickExecute?.Invoke(gmGridData);
    }

    /// <summary>
    /// 获取当前的参数
    /// </summary>
    /// <returns></returns>
    private (bool, object[]) GetCurrentParameters()
    {
        var parameterNeedCount = _cacheMethodData.ParametersData.Length;
        var result = new object[parameterNeedCount];
        for (var i = 0; i < parameterNeedCount; i++)
        {
            var parameterData = _cacheMethodData.ParametersData[i];
            if (parameterData.ParameterOptionArray == null)
            {
                var parameterInputField = inputFieldParameterList[i].text;
                //string转参，非代理参数类型，预期内可以转换
                GMCommandParse.ParseArgument(parameterInputField, parameterData.ParameterType, out var parameter);
                if (string.IsNullOrEmpty(parameterInputField))
                {
                    return (false, null);
                }

                result[i] = parameter;
                continue;
            }

            var dropdownValue = dropdownParameterList[i].value;
            if (dropdownValue < 0)
            {
                return (false, null);
            }

            var parameterDropdown = parameterData.ParameterOptionArray[dropdownValue];
            result[i] = parameterDropdown;
        }

        return (true, result);
    }
}