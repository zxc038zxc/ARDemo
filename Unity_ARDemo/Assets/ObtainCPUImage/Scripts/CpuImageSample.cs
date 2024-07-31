using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CpuImageSample : MonoBehaviour
{
	[SerializeField]
	private ARCameraManager _arCameraMgr;
	[SerializeField]
	private RawImage _rawCameraImage;
	[SerializeField]
	private Text _imageInfo;

	private Texture2D _cameraTexture;

	private void OnEnable()
	{
		_arCameraMgr.frameReceived += OnCameraFrameReceived;
	}

	private void OnDisable()
	{
		_arCameraMgr.frameReceived -= OnCameraFrameReceived;
	}

	private void OnCameraFrameReceived(ARCameraFrameEventArgs obj)
	{
		UpdateCameraImage();
	}

	private void UpdateCameraImage()
	{
		if (!_arCameraMgr.TryAcquireLatestCpuImage(out XRCpuImage image))
		{
			return;
		}

		_imageInfo.text = $"width:{image.width}, height:{image.height}, planeCount:{image.planeCount}, timestamp:{image.timestamp}, format:{image.format}";

		var format = TextureFormat.RGBA32;

		if (_cameraTexture == null || _cameraTexture.width != image.width || _cameraTexture.height != image.height)
		{
			_cameraTexture = new Texture2D(image.width,image.height, format,false);
		}

		var conversionParams = new XRCpuImage.ConversionParams(image, format, XRCpuImage.Transformation.MirrorY);

		var rawTextureData = _cameraTexture.GetRawTextureData<byte>();

		try
		{
			unsafe
			{
				image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
			}
		}
		finally
		{
			image.Dispose();
		}

		_cameraTexture.Apply();
		_rawCameraImage.texture = _cameraTexture; 
	}
}
