﻿namespace PRGReaderLibrary
{
    using System;
    using System.Collections.Generic;

    public static class UnitsNamesConstants
    {
        private static Dictionary<Units, UnitsNames> BaseDictionary { get; } = GetFilledDictionary();
        private static Dictionary<Units, UnitsNames> BaseAnalogDictionary { get; } = GetFilledAnalogDictionary();
        private static Dictionary<Units, UnitsNames> BaseDigitalDictionary { get; } = GetFilledDigitalDictionary();

        private static UnitsNames GetCustomName(int index, List<UnitsElement> customUnits = null, string defaultOnOff = "")
        {
            if (customUnits == null ||
                index < 0 ||
                index >= customUnits.Count)
            {
                return new UnitsNames(defaultOnOff);
            }

            var unit = customUnits[index];
            return unit.IsEmpty
                ? new UnitsNames(defaultOnOff, bool.FalseString, bool.TrueString)
                : new UnitsNames($"{unit.DigitalUnitsOff}/{unit.DigitalUnitsOn}", 
                unit.DigitalUnitsOff, unit.DigitalUnitsOn);
        }

        private static UnitsNames GetNameCollection(Units units, List<UnitsElement> customUnits = null)
        {
            switch (units)
            {
                //Analog part
                case Units.DegreesC:
                    return new UnitsNames("°C");

                case Units.DegreesF:
                    return new UnitsNames("°F");

                case Units.Percents:
                    return new UnitsNames("%");

                case Units.Cfh:
                    return new UnitsNames("Kg");

                //Digital part
                case Units.DigitalUnused:
                    return new UnitsNames("DigitalUnused", bool.FalseString, bool.TrueString);

                case Units.OffOn:
                    return new UnitsNames("Off/On", "Off", "On");

                case Units.ClosedOpen:
                    return new UnitsNames("Closed/Open", "Closed", "Open");

                case Units.StopStart:
                    return new UnitsNames("Stop/Start", "Stop", "Start");

                case Units.DisabledEnabled:
                    return new UnitsNames("Disabled/Enabled", "Disabled", "Enabled");

                case Units.NormalAlarm:
                    return new UnitsNames("Normal/Alarm", "Normal", "Alarm");

                case Units.NormalHigh:
                    return new UnitsNames("Normal/High", "Normal", "High");

                case Units.NormalLow:
                    return new UnitsNames("Normal/Low", "Normal", "Low");

                case Units.NoYes:
                    return new UnitsNames("No/Yes", "No", "Yes");

                case Units.CoolHeat:
                    return new UnitsNames("Cool/Heat", "Cool", "Heat");

                case Units.UnoccupiedOccupied:
                    return new UnitsNames("Unoccupied/Occupied", "Unoccupied", "Occupied");

                case Units.OnOff:
                    return new UnitsNames("On/Off", "On", "Off");

                case Units.OpenClosed:
                    return new UnitsNames("Open/Closed", "Open", "Closed");

                case Units.StartStop:
                    return new UnitsNames("Start/Stop", "Start", "Stop");

                case Units.EnabledDisabled:
                    return new UnitsNames("Enabled/Disabled", "Enabled", "Disabled");

                case Units.AlarmNormal:
                    return new UnitsNames("Alarm/Normal", "Alarm", "Normal");

                case Units.HighNormal:
                    return new UnitsNames("High/Normal", "High", "Normal");

                case Units.LowHigh:
                    return new UnitsNames("Low/High", "Low", "High");

                case Units.LowNormal:
                    return new UnitsNames("Low/Normal", "Low", "Normal");

                case Units.YesNo:
                    return new UnitsNames("Yes/No", "Yes", "No");

                case Units.HeatCool:
                    return new UnitsNames("Heat/Cool", "Heat", "Cool");

                case Units.OccupiedUnoccupied:
                    return new UnitsNames("Occupied/Unoccupied", "Occupied", "Unoccupied");

                case Units.HighLow:
                    return new UnitsNames("High/Low", "High", "Low");

                case Units.CustomDigital1:
                case Units.CustomDigital2:
                case Units.CustomDigital3:
                case Units.CustomDigital4:
                case Units.CustomDigital5:
                case Units.CustomDigital6:
                case Units.CustomDigital7:
                case Units.CustomDigital8:
                    return GetCustomName(units - Units.CustomDigital1, customUnits, units.ToString());

                default:
                    return new UnitsNames(units.ToString());
            }
        }

        private static Dictionary<Units, UnitsNames> GetFilledDictionary(List<UnitsElement> customUnits = null, Func<Units, bool> predicate = null)
        {
            var names = new Dictionary<Units, UnitsNames>();
            foreach (Units units in Enum.GetValues(typeof(Units)))
            {
                if (predicate != null && !predicate(units))
                {
                    continue;
                }

                names.Add(units, GetNameCollection(units, customUnits));
            }

            return names;
        }

        private static Dictionary<Units, UnitsNames> GetFilledAnalogDictionary(List<UnitsElement> customUnits = null) =>
            GetFilledDictionary(customUnits, units => units.IsAnalog());

        private static Dictionary<Units, UnitsNames> GetFilledDigitalDictionary(List<UnitsElement> customUnits = null) =>
            GetFilledDictionary(customUnits, units => units.IsDigital());

        public static Dictionary<Units, UnitsNames> GetNames(List<UnitsElement> customUnits = null) =>
            customUnits == null ? BaseDictionary : GetFilledDictionary(customUnits);

        public static Dictionary<Units, UnitsNames> GetAnalogNames(List<UnitsElement> customUnits = null) =>
            customUnits == null ? BaseAnalogDictionary : GetFilledAnalogDictionary(customUnits);

        public static Dictionary<Units, UnitsNames> GetDigitalNames(List<UnitsElement> customUnits = null) =>
            customUnits == null ? BaseDigitalDictionary : GetFilledDigitalDictionary(customUnits);

        public static Units UnitsFromName(string name, List<UnitsElement> customUnits = null)
        {
            var names = GetNames(customUnits);
            foreach (var pair in names)
            {
                if (name.Equals(pair.Value.OffOnName, StringComparison.OrdinalIgnoreCase))
                {
                    return pair.Key;
                }
            }

            throw new NotImplementedException($@"This name not implemented.
Name: {name}
CustomUnits.Count: {customUnits?.Count}");
        }
    }
}