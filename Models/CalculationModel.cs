﻿using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Numerics;
using System;

namespace Prefix_List_Compare.Models
{
    public class CalculationModel
    {
        private const string RegexExpression = "\\d{1,3}\\s{0,}?[\\,\\.]\\s{0,}?\\d{1,3}\\s{0,}?[\\,\\.]\\s{0,}?\\d{1,3}\\s{0,}[\\,\\.]\\s{0,}?\\d{1,3}\\s{0,}?\\/\\s{0,}?\\d{1,2}";
        private HashSet<string> _networksCurrentConfiguration= new();
        private HashSet<string> _networksDesiredConfiguration = new();
        private StringBuilder _resultBuilder = new StringBuilder();       
        public async Task<string> CalculateNetworkDifferences(string currentConfiguration, string desiredNetworks)
        {
            return await Task.Run(() =>
            {
                var regex = new Regex(RegexExpression);
                _networksCurrentConfiguration.Clear();
                foreach (Match network in regex.Matches(currentConfiguration))
                {
                    _networksCurrentConfiguration.Add(network.Value.Replace(" ", "").Replace(",","."));
                }
                _networksDesiredConfiguration.Clear();
                foreach (Match network in regex.Matches(desiredNetworks))
                {
                    _networksDesiredConfiguration.Add(network.Value.Replace(" ", "").Replace(",","."));
                }
                _resultBuilder.Clear();
                foreach (string network in _networksDesiredConfiguration)
                {
                    if (!_networksCurrentConfiguration.Contains(network))
                    {
                        _resultBuilder.AppendLine(network);
                    }
                }
                var result= _resultBuilder.ToString();
                return result;
            });
        }

        static (BigInteger, BigInteger) GetIPRange(IPAddress baseAddress, IPAddress prefixLength)
        {
            return (1, 1);
        }

        static BigInteger IPToBigInteger(IPAddress address)
        {
            byte[] bytes = address.GetAddressBytes();
            Array.Reverse(bytes);
            return new BigInteger(bytes);
        }
    }
}