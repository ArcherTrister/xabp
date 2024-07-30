using System;
using System.Linq;

using Namotion.Reflection;

using Xunit;

namespace X.Abp.Cli.Summary
{
    public class Summary_Tests
    {
        [Fact]
        public void Should_Student_Summary()
        {
            var student = new Student();
            string studentSummary = typeof(Student).GetXmlDocsSummary();
            Console.WriteLine(studentSummary);
            var piList = typeof(Student).GetProperties();
            foreach (var pi in piList)
            {
                var summary = pi.GetXmlDocsSummary();
                Console.WriteLine($"{pi.Name}-->{summary}");
            }

            //student.GetSummary();

            //var fields = student.GetSummary();
            //var result = fields.ToList();
            Console.WriteLine();
        }
    }
}
