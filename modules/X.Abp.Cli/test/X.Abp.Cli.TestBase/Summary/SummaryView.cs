using System;
using System.Collections.Generic;

using Namotion.Reflection;

namespace X.Abp.Cli.Summary
{
    static internal class SummaryView
    {
        static internal void GetSummary(this object obj)
        {
            var type = obj.GetType();
            var fields = type.GetFields();
            foreach (var item in type.GetFields())
            {
                Console.WriteLine($"{item.Name}=>{item.GetXmlDocsSummary()}");
            }

            //foreach (var item in type.GetFields())
            //{
            //    yield return $"{item.Name}=>{item.GetXmlDocsSummary()}";
            //}
            // return null;
        }
    }
}
