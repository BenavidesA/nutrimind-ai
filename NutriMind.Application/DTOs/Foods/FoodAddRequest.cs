using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriMind.Application.DTOs.Foods
{
    public class FoodAddRequest
    {
        public string Name { get; set; } = string.Empty;
        public double Calories { get; set; }
        public double Carbs { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double ServingSizeG { get; set; } = 100;
    }
}
