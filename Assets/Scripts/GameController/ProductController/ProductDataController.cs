using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class ProductDataController : MonoBehaviour
{
    private static ProductDataController _instance;
    public static ProductDataController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.FindObjectsOfTypeAll<ProductDataController>()[0];
            }
            return _instance;
        }
    }

    //Unit data
    [XmlRoot("ProductsCollection")]
    public class ProductDataCollection
    {
        [XmlArray("Products")]
        [XmlArrayItem("Product")]
        public List<ProductData> ListProductsData = new List<ProductData>();
    }
    public ProductDataCollection productDataCollection;

    [System.Serializable]
    public class ProductData
    {
        public string ProductID = "";
        public string ProductSKU = "";
        public bool IsConsumable;
        public string Type;
        public int Value;
        public float Price;
    }
    public ProductData productData;
    public List<ProductData> listProductData = new List<ProductData>();

    public static string changeGameSpeedID = "x2_gameplayspeed";
    public static string removeAdID = "removead";
    public static string x2EXP = "x2_exp";
    public static string x3EXP = "x3_exp";
    public static string x4EXP = "x4_exp";

    public float buyX2Price = 0.99f;
    public float buyX3Price = 1.99f;
    public float buyX4Price = 2.99f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Master.ProductData == null)
        {
            Master.ProductData = this;
        }
        else
        {
            Destroy(gameObject);
        }
        LoadProductData();
    }

    void Start()
    {
        SetProductDataToUltimateMobile();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadProductData()
    {
        listProductData.Clear();
        TextAsset textAsset = Resources.Load("Data/Products/ProductsData") as TextAsset;
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ProductDataCollection));
        using (var reader = new System.IO.StringReader(textAsset.text))
        {
            this.productDataCollection = (ProductDataCollection)serializer.Deserialize(reader);
        }
        listProductData = productDataCollection.ListProductsData;

    }

    public void SetProductDataToUltimateMobile()
    {
        foreach (ProductData product in listProductData)
        {
            //IAPController.Instance.AddProduct(product.ProductID, product.ProductSKU, product.IsConsumable);
        }
    }

    public ProductData GetProductDataByID(string id)
    {
        foreach (ProductData productData in listProductData)
        {
            if (id == productData.ProductID)
            {
                return productData;
            }
        }
        return null;
    }
}
