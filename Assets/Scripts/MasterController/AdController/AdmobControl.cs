using System;
using UnityEngine;
using System.Collections;

using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine.Advertisements;

public class AdmobControl : SingletonMono<AdmobControl>
{
	public string bannerAdmobID_Android;
	public string InadsAdmobID_Android;
	public string videoAdmobID_Android;
	public string bannerAdmobID_IOS;
	public string InadsAdmobID_IOS;
	public string videoAdmobID_IOS;
	string bannerAdmobID, InadsAdmobID;
	public InterstitialAd interstitial;
	public bool isTest;
	BannerView bannerView;

	public static AdmobControl mark;

	void Awake ()
	{
		if (mark == null) {
			mark = this;
			DontDestroyOnLoad (gameObject);
		} else
			Destroy (gameObject);
		if (isTest == true) {
			bannerAdmobID = "ca-app-pub-3940256099942544/6300978111";
			InadsAdmobID = "ca-app-pub-3940256099942544/1033173712";
		} else {
			#if UNITY_ANDROID
			bannerAdmobID = bannerAdmobID_Android;
			InadsAdmobID = InadsAdmobID_Android;
			Advertisement.Initialize ("3047976", false);
			#elif UNITY_IOS
      		bannerAdmobID = bannerAdmobID_IOS;
      		InadsAdmobID = InadsAdmobID_IOS;
			Advertisement.Initialize ("3047977", false);
			#endif
		}
	}

	void Start ()
	{
		RequestInterstitial ();
	}

	private FreeRewardDialog _FreeRewardDialog;
	private int type = -1;

	private void ShowRewardedAd ()
	{
		var options = new ShowOptions { resultCallback = HandleShowResult };
		Advertisement.Show (options);
	}

	private void HandleShowResult (ShowResult result)
	{
		switch (result) {
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			if (_FreeRewardDialog != null) {
				if (type == 0)
					_FreeRewardDialog.showVideoCompleted (true);
				else
					_FreeRewardDialog.callback1 (true);
				_FreeRewardDialog = null;
			}
			break;
		case ShowResult.Skipped:
			Debug.Log ("The ad was skipped before reaching the end.");
			if (_FreeRewardDialog != null) {
				if (type == 0)
					_FreeRewardDialog.showVideoCompleted (false);
				else
					_FreeRewardDialog.callback1 (false);
				_FreeRewardDialog = null;
			}
			break;
		case ShowResult.Failed:
			Debug.LogError ("The ad failed to be shown.");
			if (_FreeRewardDialog != null) {
				if (type == 0)
					_FreeRewardDialog.showVideoCompleted (false);
				else
					_FreeRewardDialog.callback1 (false);
				_FreeRewardDialog = null;
			}
			break;
		}
	}

	public void showAdmobVideo (FreeRewardDialog _FreeRewardDialog, int type)
	{
		this.type = type;
		this._FreeRewardDialog = _FreeRewardDialog;
		if (Advertisement.IsReady ()) {
			ShowRewardedAd ();
			Debug.LogError ("showUnityVideo");

		} else if (this._FreeRewardDialog != null) {
			Debug.LogError ("Failed to load video");
			if (type == 0)
				this._FreeRewardDialog.showVideoCompleted (false);
			else
				this._FreeRewardDialog.callback1 (false);
			this._FreeRewardDialog = null;
		}
	}

	public void RequestBannerAdmob ()
	{
		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView (bannerAdmobID, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();
		// Load the banner with the request.
		bannerView.LoadAd (request);
		bannerView.Hide ();
	}

	public bool isHasBanner ()
	{
		if (bannerView != null) {
			return true;
		}
		return false;
	}

	public void ShowBannerAdmob ()
	{
		if (bannerView != null) {
			bannerView.Show ();
		}

	}

	public void HideBannerAdmob ()
	{
		if (bannerView != null) {
			bannerView.Hide ();
		}
	}

	public void ClearBannerAdmob ()
	{
		if (bannerView != null) {
			bannerView.Hide ();
			bannerView.Destroy ();
		}
	}

	public void RequestInterstitial ()
	{
		#if UNITY_ANDROID
		string adUnitId = InadsAdmobID;
		#elif UNITY_IPHONE
    string adUnitId = InadsAdmobID_IOS;
		#else
    string adUnitId = "unexpected_platform";
		#endif
		interstitial = new InterstitialAd (adUnitId);
		AdRequest request = new AdRequest.Builder ().Build ();
		this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
		this.interstitial.OnAdFailedToLoad += this.HandleInterstitialAdFailedToLoad;
		interstitial.LoadAd (request);
	}

	public void ShowInadsAdmob ()
	{
		Debug.LogError ("Show ads interstitial!");
		if (interstitial != null && interstitial.IsLoaded ()) {
			interstitial.Show ();
		} else if (interstitial == null)
			Debug.LogError ("interstitial=null");
		else {
			Debug.LogError ("interstitial.IsLoaded=" + interstitial.IsLoaded ());
		}
//		Debug.LogError ("show intersitital by Unity");
//		if (Advertisement.IsReady ()) {
//			ShowRewardedAd ();
//
//		}
	}

	public void HandleInterstitialClosed (object sender, EventArgs args)
	{
		this.interstitial.OnAdClosed -= this.HandleInterstitialClosed;
		this.interstitial.OnAdFailedToLoad -= this.HandleInterstitialAdFailedToLoad;
		interstitial.Destroy ();
		RequestInterstitial ();
	}

	public void HandleInterstitialAdFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		Debug.LogError ("HandleInterstitialAdFailedToLoad=" + args.Message);
	}
}
