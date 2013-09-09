using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class Valider
    {
		public static bool Do(){
			bool result = true;
			DateTime dat1 = DateTime.Now.Date;
			DateTime dat2 = DateTime.Parse("2013-11-1");
			if (dat1.CompareTo(dat2) > 0) {
				result = false;
			}

			return result;
		} 
    }
}
