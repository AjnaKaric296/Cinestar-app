using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinestar_app.Models
{
    public class Reward
    {
        public string Name { get; set; }           // Naziv nagrade
        public string Image { get; set; }          // Putanja slike u Resources/Images
        public int StarsRequired { get; set; }     // Bodovi potrebni za nagradu
    }

}
