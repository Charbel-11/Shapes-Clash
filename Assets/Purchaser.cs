﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.
    public static string D10 = "d10";
    public static string D25 = "d25";
    public static string D50 = "d50";
    public static string D100 = "d100";
    public static string D200 = "d200";

    private ShopManager SM;

    void Start()
    {
        SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();

        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing, we're done
        if (IsInitialized()) { return; }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(D10, ProductType.Consumable);
        builder.AddProduct(D25, ProductType.Consumable);
        builder.AddProduct(D50, ProductType.Consumable);
        builder.AddProduct(D100, ProductType.Consumable);
        builder.AddProduct(D200, ProductType.Consumable);

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);

        StartCoroutine(SetPrices());
    }


    public bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    IEnumerator SetPrices()
    {
        yield return new WaitUntil(() => IsInitialized());
        SM.setPrices();
    }

    public void Buy10D()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(D10);
    }

    public void Buy25D()
    {
        BuyProductID(D25);
    }

    public void Buy50D()
    {
        BuyProductID(D50);
    }

    public void Buy100D() {
        BuyProductID(D100);
    }
    public void Buy200D() {
        BuyProductID(D200);
    }

    public string GetProducePriceFromStore(string id)
    {
        if (m_StoreController != null && m_StoreController.products != null)
            return m_StoreController.products.WithID(id).metadata.localizedPriceString;
        else
            return "";
    }

    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, D10, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            SM.addDiamonds(10);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, D25, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            SM.addDiamonds(25);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, D50, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            SM.addDiamonds(50);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, D100, StringComparison.Ordinal)) {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            SM.addDiamonds(100);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, D200, StringComparison.Ordinal)) {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            SM.addDiamonds(200);
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        SM.showError();
    }
}