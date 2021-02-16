using UnityEngine;
using WorldMapStrategyKit;

/// <summary>
/// Configure a provinces pool by creating a special hidden country that takes all provinces using the function CountryCreateProvincesPool
/// Then extract a province from this pool and create a new country using the function ProvinceToCountry
/// Adds new provinces from the pool to the new country
/// </summary>

namespace WorldMapStrategyKit {

	public class ProvincesPool : MonoBehaviour {

		WMSK map;

		void Start() {

			// 1) Get a reference to the WMSK API
			map = WMSK.instance;

			// 2) Create the dummy background country named "Pool" and move all provinces to it
			map.CountryCreateProvincesPool("Pool", true);
											
			// 3) Create a new country from province "Yunnan" in the pool of provinces (previously part of China)
			Province province = map.GetProvince("Yunnan", "Pool");
			int yunnanCountryIndex = map.ProvinceToCountry(province, "Yunnan Country", false);

			// 4) Adds more provinces from the pool to the new country
			province = map.GetProvince("Guangxi", "Pool");
			map.CountryTransferProvinceRegion(yunnanCountryIndex, province.mainRegion, false);
			province = map.GetProvince("Guizhou", "Pool");
			map.CountryTransferProvinceRegion(yunnanCountryIndex, province.mainRegion, false);
			province = map.GetProvince("Sichuan", "Pool");
			map.CountryTransferProvinceRegion(yunnanCountryIndex, province.mainRegion, false);

			// 5) Refresh map and frontiers
			map.drawAllProvinces = true;
			map.Redraw(true);

			// 6) Add province names
			map.DrawProvinceLabels(yunnanCountryIndex);

			// 7) Fly to country and fit zoom
			float zoomLevel = map.GetCountryRegionZoomExtents(yunnanCountryIndex);
			map.FlyToCountry(yunnanCountryIndex, 2f, zoomLevel);

		}


	}

}

