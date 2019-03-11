using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public class IAPController : MonoBehaviour, IStoreListener
{

	private static IAPController _instance;

	public static IAPController Instance {
		get {
			if (_instance == null) {
				_instance = Resources.FindObjectsOfTypeAll<IAPController> () [0];
			}
			return _instance;
		}
	}


	private static IStoreController m_StoreController;
	private static IExtensionProvider m_StoreExtensionProvider;
	System.Action onComplete;
	System.Action onFail;


	void Start ()
	{
		if (m_StoreController == null) {
			InitializePurchasing ();
		}
	}

	public void InitializePurchasing ()
	{
		if (IsInitialized ()) {
			return;
		}
		var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());


		foreach (ProductDataController.ProductData productData in ProductDataController.Instance.listProductData) {
			builder.AddProduct (productData.ProductID, ProductType.Consumable);
		}

		builder.AddProduct (ProductDataController.x2EXP, ProductType.Consumable);
		builder.AddProduct (ProductDataController.x3EXP, ProductType.Consumable);
		builder.AddProduct (ProductDataController.x4EXP, ProductType.Consumable);
		builder.AddProduct (ProductDataController.changeGameSpeedID, ProductType.Consumable);

		UnityPurchasing.Initialize (this, builder);
	}


	private bool IsInitialized ()
	{
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public void PurchaseProduct (string productId, System.Action onComplete = null, System.Action onFail = null)
	{

		this.onComplete = onComplete;
		this.onFail = onFail;

		if (this.onFail == null) {
			this.onFail = () => {
				Master.UI.ShowDialog ("Error");
			};
		}
		#if UNITY_EDITOR
		onComplete ();
		onComplete = null;
		return;
		#endif

		if (IsInitialized ()) {
			Product product = m_StoreController.products.WithID (productId);

			if (product != null && product.availableToPurchase) {
				Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", product.definition.id));
				m_StoreController.InitiatePurchase (product);

				if (this.onComplete == null) {
					Debug.Log ("null");
				} else {
					Debug.Log ("not null");
				}
			} else {
				Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		} else {
			if (this.onFail != null) {
				this.onFail ();
				this.onFail = null;
			}
			Debug.Log ("BuyProductID FAIL. Not initialized.");
		}

	}


	public void RestorePurchases ()
	{
		if (!IsInitialized ()) {
			Debug.Log ("RestorePurchases FAIL. Not initialized.");
			return;
		}

		if (Application.platform == RuntimePlatform.IPhonePlayer ||
		    Application.platform == RuntimePlatform.OSXPlayer) {
			Debug.Log ("RestorePurchases started ...");
		} else {
			Debug.Log ("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		Debug.Log ("OnInitialized: PASS");

		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
	}


	public void OnInitializeFailed (InitializationFailureReason error)
	{
		Debug.Log ("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
	{
		Debug.Log ("Purchase Complete -" + args.purchasedProduct.transactionID);
		if (this.onComplete != null) {
			onComplete ();
			onComplete = null;
		} else {
			Debug.Log ("2");
		}
		return PurchaseProcessingResult.Complete;
	}


	public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
	{
		if (this.onFail != null) {
			onFail ();
			onFail = null;
		}
		Debug.Log (string.Format ("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}


}
