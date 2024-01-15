using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model.GeoModel;

namespace Vidly.Models
{
	public class MapDetail
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string Details { get; set; }

		public string Information { get; set; }

		public string DetailsType { get; set; }

		[ElasticsearchGeoPoint]
		public GeoPoint DetailsCoordinates { get; set; }
	}
}