using System;
using UnityEngine;
using UnityEngine.Purchasing;

//public class IAPScript : MonoBehaviour
//{
//    public IAPButton iapBtn;

//    private void Awake()
//    {
//        iapBtn.productId = "p_gold";
//        iapBtn.onPurchaseComplete.AddListener((product) =>
//        {
//            print("인앱성공");
//        });
//        iapBtn.onPurchaseFailed.AddListener((product, error) =>
//        {
//            print("인앱실패");
//        });
//    }
//}

public class IAPManager : MonoBehaviour, IStoreListener
{
    //public IAPButton iapBtn;
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    private ITransactionHistoryExtensions m_TransactionHistoryExtensions;

    public const string productId = "p_gold"; //여러개라면 여러개의 변수선언

    private bool m_PurchaseInProgress;
    private string productIdInProcessing;

    public GooglePlayManager gpm;

    private Web _web;

    private void Awake()
    {
        _web = new Web(this);
        InitializePurchasing();
    }

    private void InitializePurchasing()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(productId, ProductType.Consumable, new IDs
        {
            {productId, GooglePlay.Name},
            {productId, MacAppStore.Name}
        });
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyConsumable()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.


        if (m_PurchaseInProgress == true)
        {
            Debug.Log("Please wait, purchase in progress");
            return;
        }

        if (!IsInitialized())
        {
            Debug.LogError("Purchasing is not initialized");
            return;
        }

        if (m_StoreController.products.WithID(productId) == null)
        {
            Debug.LogError("No product has id " + productId);
            return;
        }

        //m_PurchaseInProgress = true;
        BuyProductID(productId);
    }

    void BuyProductID(string productID)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            m_PurchaseInProgress = true;

            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productID);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: {0}", product.definition.id));
                productIdInProcessing = product.definition.id;
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            InitializePurchasing();
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");
        Debug.Log("Available items:");
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log(string.Join(" - ",
                    new[]
                    {
                        item.transactionID,
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString,
                        item.transactionID,
                        item.receipt
                }));
            }
        }
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
        m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    public void OnPurchaseFailed(Product item, PurchaseFailureReason error)
    {
        Debug.Log("Purchase failed: " + item.definition.id + " / error : " + error);
        Debug.Log("Store specific error code: " + m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());

        if (m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
        {
            Debug.Log("Purchase failure description message: " +
                      m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
        }

        m_PurchaseInProgress = false;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("Purchase OK: " + args.purchasedProduct.definition.id + "===" + productIdInProcessing);
        Debug.Log("Receipt: " + args.purchasedProduct.receipt);

        gpm.SetLog("Receipt: " + args.purchasedProduct.receipt);

        _web.Send(args.purchasedProduct.receipt);

        // A consumable product has been purchased by this user.
        if (string.Equals(args.purchasedProduct.definition.id, productIdInProcessing, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: {0}", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
        }

        m_PurchaseInProgress = false;

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Pending;
    }
}