using System;
using System.Collections.Generic;
using JsonFx.Json;
using System.Net;
namespace ScrollsGuidePriceMod
{
	public class SGPrice {
		public readonly int buy;
		public readonly int sell;
		public readonly DateTime lastSeen;
		public readonly string text;
		public SGPrice(int buy, int sell, DateTime lastSeen) {
			this.buy = buy;
			this.sell = sell;
			this.lastSeen = lastSeen;
			if (buy <= 0 && sell <= 0) {
				/* Have no prices */
				text = "B?S?";
			} else if (buy <= 0) {
				/* Only have sell price*/
				text = "B?S" + sell;
			} else if (sell <= 0){
				/* Only have buy price*/
				text = "B" + buy + "S?";
			} else {
				/* Have Both prices*/
				text = "B" + buy + "S" + sell;
			}

		}
		public override string ToString() {
			return text;
		}
	}
	public class ScrollsGuidePrices
	{
		private static readonly SGPrice invalidPrice = new SGPrice(-1,-1,DateTime.MinValue);

		#region Singleton-Pattern
		private static readonly ScrollsGuidePrices instance = new ScrollsGuidePrices();
		public static ScrollsGuidePrices Instance
		{
			get 
			{
				return instance; 
			}
		}
		#endregion

		private ScrollsGuidePrices(){}

		protected Dictionary<long, SGPrice> idToPrice = new Dictionary<long, SGPrice>();
		public void UpdatePrices() {
			//Console.WriteLine ("Fetching Prices from ScrollsGuide...");
			string json = new WebClient ().DownloadString ("http://a.scrollsguide.com/prices");
			JsonReader jsr = new JsonReader ();
			Dictionary<string, object> response = jsr.Read<Dictionary<string, object>> (json);
			if ("success".Equals(response["msg"])) {
				if (response ["data"] is Dictionary<string, object>[]) {
					Dictionary<string, object>[] data = (Dictionary<string, object>[])response ["data"];
					idToPrice.Clear ();
					foreach (Dictionary<string, object> price in data) {
						object id, buy, sell;
						if (price.TryGetValue ("id", out id) && price.TryGetValue ("buy", out buy) && price.TryGetValue ("sell", out sell)) {
							if (id is int && buy is int && sell is int) {
								idToPrice.Add ((long)(int)id, new SGPrice ((int)buy, (int)sell, DateTime.Now));
								//Console.WriteLine ("Price for {0}: {1}/{2}", id, buy, sell);
							} else {
								Console.WriteLine ("Could not parse to int. id: {0} buy: {1} sell: {2}", id, buy, sell);
							}
						} else {
							Console.WriteLine ("Could not read id, buy or sell from Price Data");
						}
					}
				} else {
					throw new Exception ("Could not read Data from successful message :(");
				}
			} else {
				throw new Exception ("Scrollsguide was unsuccessful");
			}
		}

		public SGPrice getPriceForCard(Card c) {
			return getPriceForCardId (c.typeId);
		} 
		public SGPrice getPriceForCardId(long cardid) {
			if (idToPrice != null) {
				SGPrice p;
				if(idToPrice.TryGetValue (cardid, out p)) {
					return p;
				}
			}
			return invalidPrice;
		}
	}
}

