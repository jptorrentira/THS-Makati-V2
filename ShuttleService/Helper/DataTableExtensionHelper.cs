using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

public static class DataTableExtensionHelper
{
    public static DataTable ToDataTable<T>(this IEnumerable<T> items)
    {
        var dt = new DataTable(typeof(T).Name);
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
            dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

        foreach (var item in items)
        {
            var values = props.Select(p => p.GetValue(item) ?? DBNull.Value).ToArray();
            dt.Rows.Add(values);
        }

        return dt;
    }
}
