﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vidly.Models
{
	public class MapModel
	{
		public string MapData { get; set; }
		public double CenterLongitude { get; set; }
		public double CenterLatitude { get; set; }
		public uint MaxDistanceInMeter { get; set; }
	}
}