using DotNetShipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public class PercentageRateAdjuster : IRateAdjuster
    {
        private readonly decimal _amount;

        public PercentageRateAdjuster(decimal amount)
        {
            _amount = amount;
        }

        public Rate AdjustRate(Rate rate)
        {
            rate.TotalCharges = rate.TotalCharges * _amount;
            return rate;
        }
    }
}
