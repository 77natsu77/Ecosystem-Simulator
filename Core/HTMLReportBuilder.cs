using System.Text;
using Ecosystem_Simulator.Core;

namespace Core
{
    public class HTMLReportBuilder
    {
        private StringBuilder _htmlBuilder;

        public HTMLReportBuilder()
        {
            _htmlBuilder = new StringBuilder();
            _htmlBuilder.AppendLine("<html><head><title>Simulation Report</title></head><body>");
        }

        public void AddSection(string title, string content)
        {
            _htmlBuilder.AppendLine($"<h2>{title}</h2>");
            _htmlBuilder.AppendLine($"<p>{content}</p>");
        }

        public string GetHTML()
        {
            _htmlBuilder.AppendLine("</body></html>");
            return _htmlBuilder.ToString();
        }

        // This function must be redone to fit this new format, it is currently designed for a single entry, but we need to be able to save multiple entries over time, so it needs to be redesigned to take in a list of stats entries and convert them into html format
         
    }
}