using System;
using TMPro;
using UnityEngine;

namespace MostlyPureFunctions.DateTime
{
    [Serializable]
    public class DatePicker
    {
        [SerializeField] private TMP_Dropdown dayDropDown;
        [SerializeField] private TMP_Dropdown monthDropDown;
        [SerializeField] private TMP_Dropdown yearDropDown;
        private readonly int _earliestYear = new System.DateTime(System.DateTime.Today.Year - 100, System.DateTime.Today.Month, System.DateTime.Today.Day).Year;
        private readonly int _latestYear = new System.DateTime(System.DateTime.Today.Year - 10, System.DateTime.Today.Month, System.DateTime.Today.Day).Year;
        private int _currentDaySelection = 1;
        private int _currentMonthSelection = 1;
        private int _currentYearSelection;
        public System.DateTime ReturnCurrentDateSelection => new System.DateTime(_currentYearSelection, _currentMonthSelection, _currentDaySelection);
    
        private void Awake()
        {
            AddEvents();
            InitialiseDropDownOptions();
        }
    
        private void AddEvents()
        {
            monthDropDown.onValueChanged.AddListener(OnMonthValueChanged);
            yearDropDown.onValueChanged.AddListener(OnYearValueChanged);
            dayDropDown.onValueChanged.AddListener(OnDayValueChanged);
        }
    
        private void InitialiseDropDownOptions()
        {
            _currentYearSelection = _earliestYear;
            SetDaysDropDownOptions(_currentYearSelection, _currentMonthSelection);
            SetMonthDropDownOptions();
            SetYearDropDownOptions();

            dayDropDown.value = 0;
            monthDropDown.value = 0;
            yearDropDown.value = 0;
        }
    
        private void OnDayValueChanged(int value)
        {
            _currentDaySelection = value + 1;
        }

        private void OnMonthValueChanged(int value)
        {
            _currentMonthSelection = value + 1;
            SetDaysDropDownOptions(_currentYearSelection, _currentMonthSelection);
        }
    
        private void OnYearValueChanged(int value)
        {
            _currentYearSelection = int.Parse(yearDropDown.captionText.text);
            SetDaysDropDownOptions(_currentYearSelection, _currentMonthSelection);
        }

        private void SetDaysDropDownOptions(int year, int month)//DateTime.Months/Days doesnt use 0 based counting.
        {
            var daysInMonth = System.DateTime.DaysInMonth(year, month);
            dayDropDown.ClearOptions();
            for (var currentDay = 1; currentDay <= daysInMonth; currentDay++)
            {
                dayDropDown.options.Add(new TMP_Dropdown.OptionData(DateTimeOrdinals.ConvertToOrdinalString(currentDay)));
            }
            dayDropDown.value = 0;
        }
    
        private void SetMonthDropDownOptions()
        {
            const int monthsInYear = 12;
            monthDropDown.ClearOptions();
            for (var currentMonth = 0; currentMonth < monthsInYear; currentMonth++)
            {
                monthDropDown.options.Add(new TMP_Dropdown.OptionData(((DateTimeOrdinals.Months)currentMonth).ToString()));
            }
        }
    
        private void SetYearDropDownOptions()
        {
            yearDropDown.ClearOptions();
            for (var currentYear = _earliestYear; currentYear < _latestYear; currentYear++)
            {
                yearDropDown.options.Add(new TMP_Dropdown.OptionData(currentYear.ToString()));
            }
        }
    }
}
