using UnityEngine;
using System.Collections;

public class ProductController : MonoBehaviour
{

	// Use this for initialization
	public ProductDataController.ProductData productData;

	private UITexture iconTypeTexture;
	private UITexture iconTexture;
	private UILabel valueLabel;
	private UILabel priceLabel;



	void Awake ()
	{
		iconTypeTexture = Master.GetChildByName (gameObject, "IconType").GetComponent<UITexture> ();
		iconTexture = Master.GetChildByName (gameObject, "Icon").GetComponent<UITexture> ();
		valueLabel = Master.GetChildByName (gameObject, "ValueLabel").GetComponent<UILabel> ();
		priceLabel = Master.GetChildByName (gameObject, "PriceLabel").GetComponent<UILabel> ();
	}

	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void SetInfo (ProductDataController.ProductData productData)
	{
		if (productData == null ||
		    productData.Type == null) {
			Destroy (gameObject);
			return;
		}
		this.productData = productData;
		if (productData.ProductID.Equals ("gem_2.49")) {
			iconTexture.mainTexture = Resources.Load<Texture2D> ("gem_1");
		} else if (productData.ProductID.Equals ("gem_4.9")) {
			iconTexture.mainTexture = Resources.Load<Texture2D> ("gem_2");
		} else if (productData.ProductID.Equals ("gem_9.9")) {
			iconTexture.mainTexture = Resources.Load<Texture2D> ("gem_3");
		} else if (productData.ProductID.Equals ("gem_29.9")) {
			iconTexture.mainTexture = Resources.Load<Texture2D> ("gem_4");
		} else if (productData.ProductID.Equals ("star_2.49")) {
			iconTexture.mainTexture = Resources.Load<Texture2D> ("star_1");
		} else if (productData.ProductID.Equals ("star_4.9")) {
			iconTexture.mainTexture = Resources.Load<Texture2D> ("star_2");
		} else if (productData.ProductID.Equals ("star_9.9")) {
			iconTexture.mainTexture = Resources.Load<Texture2D> ("star_3");
		} else if (productData.ProductID.Equals ("star_29.9")) {
			iconTexture.mainTexture = Resources.Load<Texture2D> ("star_4");
		}
		if (productData.Type.Equals ("Gem"))
			iconTypeTexture.mainTexture = Resources.Load<Texture2D> ("gem_icon");
		else if (productData.Type.Equals ("Star"))
			iconTypeTexture.mainTexture = Resources.Load<Texture2D> ("star_icon");
		valueLabel.text = productData.Value.ToString ();
		priceLabel.text = "$" + productData.Price.ToString ();
	}

	public void OnTouchIn ()
	{
		IAPController.Instance.PurchaseProduct (productData.ProductID, () => {
			Master.Audio.PlaySound ("snd_buy");
			if (productData.Type == "Gem") {
				Master.Stats.Gem += productData.Value;
			}
			if (productData.Type == "Star") {
				Master.Stats.Star += productData.Value;
			}
		});
	}

}
