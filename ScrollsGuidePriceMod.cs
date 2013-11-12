using System;
using ScrollsModLoader.Interfaces;
using Mono.Cecil;
using UnityEngine;
namespace ScrollsGuidePriceMod
{
	public class ScrollsGuidePriceMod : BaseMod
	{

		public ScrollsGuidePriceMod () {
			ScrollsGuidePrices.Instance.UpdatePrices ();
		}

		public static MethodDefinition[] GetHooks(TypeDefinitionCollection scrollsTypes, int version) {
		MethodDefinition[] methods = scrollsTypes ["CardListPopup"].Methods.GetMethod ("RenderCost");
			if (methods.Length == 1) 
				return methods;
			Console.WriteLine ("Can not find methodDefinitin for CardListPopup.RenderCost");
			return new MethodDefinition[] {};
		}
		public static string GetName() {
			return "SGPriceMod";
		}
		public static int GetVersion() {
			return 1;
		}

		public override void BeforeInvoke (InvocationInfo info) {
		}

		public override void AfterInvoke (InvocationInfo info, ref object returnValue) {
			Rect rect = (Rect)info.arguments [0];
			Card card = (Card)info.arguments [1];
			string text = ScrollsGuidePrices.Instance.getPriceForCard (card).ToString();
			Vector2 maxlabelSize = GUI.skin.label.CalcSize(new GUIContent(text));
			//Rect myRect = new Rect (rect.x - rect.width, rect.yMax, rect.width * 2, rect.height / 4);
			//Center above Resource-Cost
			Rect myRect = new Rect (rect.x - maxlabelSize.x / 2 + rect.width / 2, rect.y - 3 * maxlabelSize.y / 4, maxlabelSize.x, maxlabelSize.y);
			/*Rect myRect = new Rect ();
			myRect.x = rect.xMax - maxlabelSize.x;
			myRect.y = rect.yMax - maxlabelSize.y;
			myRect.xMax = rect.x;
			myRect.yMax = rect.yMax;
			myRect.height = maxlabelSize.y;
			myRect.width = maxlabelSize.x;*/
			//GUI.Label (myRect, ScrollsGuidePrices.Instance.getPriceForCard(card).ToString());
			GUI.Label (myRect, text);
		}
	}
}