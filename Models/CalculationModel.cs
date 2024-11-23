using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prefix_List_Compare.Models
{
    public class CalculationModel
    {
        public string Greeting {get; set;} = "Hello from the model!";
        private const string RegexExpression = "\\d{1,3}\\s{0,}?[\\,\\.]\\s{0,}?\\d{1,3}\\s{0,}?[\\,\\.]\\s{0,}?\\d{1,3}\\s{0,}[\\,\\.]\\s{0,}?\\d{1,3}\\s{0,}?\\/\\s{0,}?\\d{1,2}";
        private List<string> _networksCurrentConfiguration= new();
        private List<string> _networksDesiredConfiguration = new();
        private string _result = "";        
        public async Task<string> CalculateNetworkDifferences(string currentConfiguration, string desiredNetworks)
        {
            return await Task.Run(() =>
            {
                var regex = new Regex(RegexExpression);
                foreach (Match network in regex.Matches(currentConfiguration))
                {

                    _networksCurrentConfiguration.Add(network.Value.Replace(" ", "").Replace(",", "."));
                }

                foreach (Match network in regex.Matches(desiredNetworks))
                {

                    _networksDesiredConfiguration.Add(network.Value.Replace(" ", "").Replace(",", "."));
                }
                foreach (string network in _networksDesiredConfiguration)
                {
                    if (!_networksCurrentConfiguration.Contains(network))
                    {
                        _result = _result + network + '\n';
                    }
                }
                _result.TrimEnd('\n');
                return _result;
            });
        }
    }
}